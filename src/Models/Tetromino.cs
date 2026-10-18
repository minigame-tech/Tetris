namespace Tetris.Models;

/// <summary>
/// Represents an active or preview Tetromino piece, managing its shape,
/// rotation states according to SRS (Super Rotation System), position, and bounding box.
/// </summary>
public sealed class Tetromino
{
    public TetrominoType Type { get; }
    public Position Position { get; set; }
    public RotationState Rotation { get; set; }

    /// <summary>
    /// Tracks whether the last successful maneuver performed on this piece was a rotation (crucial for T-spin detection).
    /// </summary>
    public bool LastMoveWasRotation { get; set; }

    /// <summary>
    /// Tracks if the last rotation used the 5th kick in SRS (used to distinguish full T-spin vs mini).
    /// </summary>
    public bool LastRotationUsedSpecialKick { get; set; }

    private readonly Position[][] _shapes;

    public Tetromino(TetrominoType type)
    {
        Type = type;
        Rotation = RotationState.Spawn;
        _shapes = GetShapesForType(type);
        Position = GetDefaultSpawnPosition(type);
    }

    private Tetromino(TetrominoType type, Position pos, RotationState rot, Position[][] shapes)
    {
        Type = type;
        Position = pos;
        Rotation = rot;
        _shapes = shapes;
    }

    public Tetromino Clone()
    {
        return new Tetromino(Type, Position, Rotation, _shapes)
        {
            LastMoveWasRotation = LastMoveWasRotation,
            LastRotationUsedSpecialKick = LastRotationUsedSpecialKick
        };
    }

    public static Position GetDefaultSpawnPosition(TetrominoType type)
    {
        // Spawns centered horizontally in top buffer rows (0 and 1)
        return type switch
        {
            TetrominoType.O => new Position(0, 4),
            TetrominoType.I => new Position(0, 3),
            _ => new Position(0, 3)
        };
    }

    /// <summary>
    /// Returns the 4 absolute grid positions occupied by this tetromino at its current position and rotation.
    /// </summary>
    public Position[] GetCurrentBlockPositions()
    {
        return GetBlockPositions(Position, Rotation);
    }

    /// <summary>
    /// Returns the 4 absolute grid positions for arbitrary position and rotation.
    /// </summary>
    public Position[] GetBlockPositions(Position offset, RotationState rotation)
    {
        var shape = _shapes[(int)rotation];
        var result = new Position[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = offset + shape[i];
        }
        return result;
    }

    /// <summary>
    /// Returns the local shape coordinates (0..3) for a given rotation, useful for HUD preview rendering.
    /// </summary>
    public Position[] GetLocalShape(RotationState rotation = RotationState.Spawn)
    {
        return _shapes[(int)rotation];
    }

    private static Position[][] GetShapesForType(TetrominoType type)
    {
        return type switch
        {
            TetrominoType.I => new Position[][]
            {
                new Position[] { new(1, 0), new(1, 1), new(1, 2), new(1, 3) },
                new Position[] { new(0, 2), new(1, 2), new(2, 2), new(3, 2) },
                new Position[] { new(2, 0), new(2, 1), new(2, 2), new(2, 3) },
                new Position[] { new(0, 1), new(1, 1), new(2, 1), new(3, 1) }
            },
            TetrominoType.J => new Position[][]
            {
                new Position[] { new(0, 0), new(1, 0), new(1, 1), new(1, 2) },
                new Position[] { new(0, 1), new(0, 2), new(1, 1), new(2, 1) },
                new Position[] { new(1, 0), new(1, 1), new(1, 2), new(2, 2) },
                new Position[] { new(0, 1), new(1, 1), new(2, 0), new(2, 1) }
            },
            TetrominoType.L => new Position[][]
            {
                new Position[] { new(0, 2), new(1, 0), new(1, 1), new(1, 2) },
                new Position[] { new(0, 1), new(1, 1), new(2, 1), new(2, 2) },
                new Position[] { new(1, 0), new(1, 1), new(1, 2), new(2, 0) },
                new Position[] { new(0, 0), new(0, 1), new(1, 1), new(2, 1) }
            },
            TetrominoType.O => new Position[][]
            {
                new Position[] { new(0, 1), new(0, 2), new(1, 1), new(1, 2) },
                new Position[] { new(0, 1), new(0, 2), new(1, 1), new(1, 2) },
                new Position[] { new(0, 1), new(0, 2), new(1, 1), new(1, 2) },
                new Position[] { new(0, 1), new(0, 2), new(1, 1), new(1, 2) }
            },
            TetrominoType.S => new Position[][]
            {
                new Position[] { new(0, 1), new(0, 2), new(1, 0), new(1, 1) },
                new Position[] { new(0, 1), new(1, 1), new(1, 2), new(2, 2) },
                new Position[] { new(1, 1), new(1, 2), new(2, 0), new(2, 1) },
                new Position[] { new(0, 0), new(1, 0), new(1, 1), new(2, 1) }
            },
            TetrominoType.T => new Position[][]
            {
                new Position[] { new(0, 1), new(1, 0), new(1, 1), new(1, 2) },
                new Position[] { new(0, 1), new(1, 1), new(1, 2), new(2, 1) },
                new Position[] { new(1, 0), new(1, 1), new(1, 2), new(2, 1) },
                new Position[] { new(0, 1), new(1, 0), new(1, 1), new(2, 1) }
            },
            TetrominoType.Z => new Position[][]
            {
                new Position[] { new(0, 0), new(0, 1), new(1, 1), new(1, 2) },
                new Position[] { new(0, 2), new(1, 1), new(1, 2), new(2, 1) },
                new Position[] { new(1, 0), new(1, 1), new(2, 1), new(2, 2) },
                new Position[] { new(0, 1), new(1, 0), new(1, 1), new(2, 0) }
            },
            _ => new Position[][]
            {
                new Position[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0) },
                new Position[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0) },
                new Position[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0) },
                new Position[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0) }
            }
        };
    }
}

