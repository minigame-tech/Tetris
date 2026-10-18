namespace Tetris.Models;

/// <summary>
/// Implements the official Tetris Guideline Super Rotation System (SRS) Wall Kick tables.
/// When a piece attempts to rotate and collides with the wall or locked blocks,
/// the engine tests a predefined sequence of offset tests (kicks) to find a valid position.
/// </summary>
public static class WallKicks
{
    // Standard kick table for J, L, S, T, Z tetrominoes (5 tests per rotation)
    // Key format: (fromState, toState)
    private static readonly Dictionary<(RotationState, RotationState), Position[]> StandardKicks = new()
    {
        // 0 -> R
        { (RotationState.Spawn, RotationState.Right), new Position[]
            { new(0, 0), new(0, -1), new(-1, -1), new(2, 0), new(2, -1) } },
        // R -> 0
        { (RotationState.Right, RotationState.Spawn), new Position[]
            { new(0, 0), new(0, 1), new(1, 1), new(-2, 0), new(-2, 1) } },
        // R -> 2
        { (RotationState.Right, RotationState.Two), new Position[]
            { new(0, 0), new(0, 1), new(1, 1), new(-2, 0), new(-2, 1) } },
        // 2 -> R
        { (RotationState.Two, RotationState.Right), new Position[]
            { new(0, 0), new(0, -1), new(-1, -1), new(2, 0), new(2, -1) } },
        // 2 -> L
        { (RotationState.Two, RotationState.Left), new Position[]
            { new(0, 0), new(0, 1), new(-1, 1), new(2, 0), new(2, 1) } },
        // L -> 2
        { (RotationState.Left, RotationState.Two), new Position[]
            { new(0, 0), new(0, -1), new(1, -1), new(-2, 0), new(-2, -1) } },
        // L -> 0
        { (RotationState.Left, RotationState.Spawn), new Position[]
            { new(0, 0), new(0, -1), new(1, -1), new(-2, 0), new(-2, -1) } },
        // 0 -> L
        { (RotationState.Spawn, RotationState.Left), new Position[]
            { new(0, 0), new(0, 1), new(-1, 1), new(2, 0), new(2, 1) } }
    };

    // Specialized kick table for the I-tetromino (4x4 bounding box)
    private static readonly Dictionary<(RotationState, RotationState), Position[]> IKicks = new()
    {
        // 0 -> R
        { (RotationState.Spawn, RotationState.Right), new Position[]
            { new(0, 0), new(0, -2), new(0, 1), new(1, -2), new(-2, 1) } },
        // R -> 0
        { (RotationState.Right, RotationState.Spawn), new Position[]
            { new(0, 0), new(0, 2), new(0, -1), new(-1, 2), new(2, -1) } },
        // R -> 2
        { (RotationState.Right, RotationState.Two), new Position[]
            { new(0, 0), new(0, -1), new(0, 2), new(-2, -1), new(1, 2) } },
        // 2 -> R
        { (RotationState.Two, RotationState.Right), new Position[]
            { new(0, 0), new(0, 1), new(0, -2), new(2, 1), new(-1, -2) } },
        // 2 -> L
        { (RotationState.Two, RotationState.Left), new Position[]
            { new(0, 0), new(0, 2), new(0, -1), new(-1, 2), new(2, -1) } },
        // L -> 2
        { (RotationState.Left, RotationState.Two), new Position[]
            { new(0, 0), new(0, -2), new(0, 1), new(1, -2), new(-2, 1) } },
        // L -> 0
        { (RotationState.Left, RotationState.Spawn), new Position[]
            { new(0, 0), new(0, 1), new(0, -2), new(2, 1), new(-1, -2) } },
        // 0 -> L
        { (RotationState.Spawn, RotationState.Left), new Position[]
            { new(0, 0), new(0, -1), new(0, 2), new(-2, -1), new(1, 2) } }
    };

    private static readonly Position[] NoKicks = { Position.Zero };

    /// <summary>
    /// Gets the list of candidate translation offsets to test when transitioning
    /// from current rotation state to next rotation state for the given tetromino type.
    /// </summary>
    public static ReadOnlySpan<Position> GetKickOffsets(TetrominoType type, RotationState from, RotationState to)
    {
        if (type == TetrominoType.O)
        {
            return NoKicks;
        }

        var table = type == TetrominoType.I ? IKicks : StandardKicks;
        if (table.TryGetValue((from, to), out var offsets))
        {
            return offsets;
        }

        return NoKicks;
    }
}

