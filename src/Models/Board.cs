namespace Tetris.Models;

/// <summary>
/// Represents the 10x22 Tetris matrix (2 hidden buffer rows at top, 20 visible play rows).
/// Handles block storage, collision detection, ghost piece projection, line clearing,
/// T-Spin evaluation, and Perfect Clear checks.
/// </summary>
public sealed class Board
{
    public const int Width = 10;
    public const int TotalRows = 22;
    public const int VisibleRows = 20;
    public const int HiddenRows = 2; // Rows 0 and 1 are hidden buffer rows

    private readonly TetrominoType[,] _grid = new TetrominoType[TotalRows, Width];

    public TetrominoType this[int row, int col]
    {
        get => _grid[row, col];
        set => _grid[row, col] = value;
    }

    public void Clear()
    {
        for (int r = 0; r < TotalRows; r++)
        {
            for (int c = 0; c < Width; c++)
            {
                _grid[r, c] = TetrominoType.None;
            }
        }
    }

    public bool IsCellValid(int row, int col)
    {
        return row >= 0 && row < TotalRows && col >= 0 && col < Width;
    }

    public bool IsCellOccupied(int row, int col)
    {
        // Out-of-bounds horizontally or beneath the floor counts as occupied
        if (col < 0 || col >= Width || row >= TotalRows)
        {
            return true;
        }

        // Above the ceiling is empty space
        if (row < 0)
        {
            return false;
        }

        return _grid[row, col] != TetrominoType.None;
    }

    /// <summary>
    /// Validates if a tetromino can occupy the specified position and rotation without collisions.
    /// </summary>
    public bool IsValidPosition(Tetromino piece, Position position, RotationState rotation)
    {
        var blocks = piece.GetBlockPositions(position, rotation);
        foreach (var b in blocks)
        {
            if (b.Col < 0 || b.Col >= Width || b.Row >= TotalRows)
            {
                return false;
            }

            if (b.Row >= 0 && _grid[b.Row, b.Col] != TetrominoType.None)
            {
                return false;
            }
        }
        return true;
    }

    public bool CanPlace(Tetromino piece)
    {
        return IsValidPosition(piece, piece.Position, piece.Rotation);
    }

    /// <summary>
    /// Calculates the number of rows a piece can drop downwards until hitting the floor or locked blocks.
    /// </summary>
    public int CalculateGhostDropDistance(Tetromino piece)
    {
        int distance = 0;
        while (IsValidPosition(piece, piece.Position with { Row = piece.Position.Row + distance + 1 }, piece.Rotation))
        {
            distance++;
        }
        return distance;
    }

    /// <summary>
    /// Calculates the landing position of the ghost piece.
    /// </summary>
    public Position GetGhostPosition(Tetromino piece)
    {
        int dropDistance = CalculateGhostDropDistance(piece);
        return piece.Position with { Row = piece.Position.Row + dropDistance };
    }

    /// <summary>
    /// Commits the falling tetromino to the matrix grid.
    /// </summary>
    public void LockTetromino(Tetromino piece)
    {
        var blocks = piece.GetCurrentBlockPositions();
        foreach (var b in blocks)
        {
            if (b.Row >= 0 && b.Row < TotalRows && b.Col >= 0 && b.Col < Width)
            {
                _grid[b.Row, b.Col] = piece.Type;
            }
        }
    }

    /// <summary>
    /// Finds all currently full rows across the matrix.
    /// </summary>
    public List<int> FindFullLines()
    {
        var full = new List<int>();
        for (int r = 0; r < TotalRows; r++)
        {
            bool isFull = true;
            for (int c = 0; c < Width; c++)
            {
                if (_grid[r, c] == TetrominoType.None)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull)
            {
                full.Add(r);
            }
        }
        return full;
    }

    /// <summary>
    /// Removes specified rows and shifts down all upper blocks.
    /// </summary>
    public void ClearLines(IReadOnlyList<int> lineIndices)
    {
        if (lineIndices.Count == 0) return;

        // Process from bottom to top
        var sorted = lineIndices.OrderBy(r => r).ToList();

        foreach (var rowToClear in sorted)
        {
            for (int r = rowToClear; r > 0; r--)
            {
                for (int c = 0; c < Width; c++)
                {
                    _grid[r, c] = _grid[r - 1, c];
                }
            }
            // Clear top row
            for (int c = 0; c < Width; c++)
            {
                _grid[0, c] = TetrominoType.None;
            }
        }
    }

    /// <summary>
    /// Returns true if every cell in the matrix is empty (Perfect Clear / All Clear).
    /// </summary>
    public bool IsBoardEmpty()
    {
        for (int r = 0; r < TotalRows; r++)
        {
            for (int c = 0; c < Width; c++)
            {
                if (_grid[r, c] != TetrominoType.None)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Performs official Tetris Guideline 3-corner T-Spin evaluation.
    /// </summary>
    public bool CheckTSpin(Tetromino piece, out bool isMini)
    {
        isMini = false;
        if (piece.Type != TetrominoType.T || !piece.LastMoveWasRotation)
        {
            return false;
        }

        // T-piece 3x3 bounding box corners relative to piece.Position
        int r = piece.Position.Row;
        int c = piece.Position.Col;

        bool tl = IsCellOccupied(r, c);
        bool tr = IsCellOccupied(r, c + 2);
        bool bl = IsCellOccupied(r + 2, c);
        bool br = IsCellOccupied(r + 2, c + 2);

        int occupiedCorners = (tl ? 1 : 0) + (tr ? 1 : 0) + (bl ? 1 : 0) + (br ? 1 : 0);

        if (occupiedCorners < 3)
        {
            return false;
        }

        // Identify front corners pointing toward T's protrusion
        bool front1, front2;
        switch (piece.Rotation)
        {
            case RotationState.Spawn: // Pointing Up
                front1 = tl;
                front2 = tr;
                break;
            case RotationState.Right: // Pointing Right
                front1 = tr;
                front2 = br;
                break;
            case RotationState.Two:   // Pointing Down
                front1 = bl;
                front2 = br;
                break;
            case RotationState.Left:  // Pointing Left
                front1 = tl;
                front2 = bl;
                break;
            default:
                front1 = false;
                front2 = false;
                break;
        }

        // Guideline: If both front corners are occupied OR kick test 5 was used, it's a full T-Spin
        if ((front1 && front2) || piece.LastRotationUsedSpecialKick)
        {
            isMini = false;
        }
        else
        {
            isMini = true;
        }

        return true;
    }
}

