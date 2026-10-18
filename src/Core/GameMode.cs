namespace Tetris.Core;

/// <summary>
/// Defines available game modes.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Classic endless marathon: start at level 1, speed increases with every 10 lines.
    /// </summary>
    Marathon,

    /// <summary>
    /// Clear 40 lines as fast as possible.
    /// </summary>
    Sprint40,

    /// <summary>
    /// Score as many points as possible within 2 minutes (120 seconds).
    /// </summary>
    Ultra2Min
}

