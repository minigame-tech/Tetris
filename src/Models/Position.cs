namespace Tetris.Models;

/// <summary>
/// Represents a discrete 2D grid position on the Tetris matrix (Row, Column).
/// Row increases downwards (0 = top buffer row, 21 = bottom).
/// Column increases rightwards (0 = leftmost, 9 = rightmost).
/// </summary>
public readonly record struct Position(int Row, int Col)
{
    public static Position operator +(Position a, Position b) => new(a.Row + b.Row, a.Col + b.Col);
    public static Position operator -(Position a, Position b) => new(a.Row - b.Row, a.Col - b.Col);

    public static readonly Position Zero = new(0, 0);
}

