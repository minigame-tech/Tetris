namespace Tetris.Models;

/// <summary>
/// Represents the four standard Super Rotation System (SRS) rotation states.
/// </summary>
public enum RotationState
{
    Spawn = 0, // 0: Default spawn orientation
    Right = 1, // R: Rotated 90 degrees clockwise
    Two = 2,   // 2: Rotated 180 degrees
    Left = 3   // L: Rotated 270 degrees clockwise (90 degrees counter-clockwise)
}

public static class RotationStateExtensions
{
    public static RotationState RotateClockwise(this RotationState state) =>
        (RotationState)(((int)state + 1) % 4);

    public static RotationState RotateCounterClockwise(this RotationState state) =>
        (RotationState)(((int)state + 3) % 4);
}

