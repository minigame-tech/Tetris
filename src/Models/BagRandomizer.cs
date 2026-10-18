namespace Tetris.Models;

/// <summary>
/// Implements the official Tetris Guideline 7-Bag Random Generator.
/// Generates sequences containing one of each of the 7 tetrominoes in randomized order.
/// Prevents piece starvation (e.g. infinite droughts of I-pieces).
/// </summary>
public sealed class BagRandomizer
{
    private readonly Random _random;
    private readonly List<TetrominoType> _queue = new();
    private static readonly TetrominoType[] AllTypes =
    {
        TetrominoType.I, TetrominoType.J, TetrominoType.L,
        TetrominoType.O, TetrominoType.S, TetrominoType.T, TetrominoType.Z
    };

    public BagRandomizer(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
        ReplenishBag();
        ReplenishBag(); // Keep at least 14 pieces buffered
    }

    private void ReplenishBag()
    {
        var bag = (TetrominoType[])AllTypes.Clone();
        // Modern Fisher-Yates shuffle
        for (int i = bag.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }
        _queue.AddRange(bag);
    }

    /// <summary>
    /// Pops the next Tetromino from the queue and ensures future bags are generated.
    /// </summary>
    public TetrominoType PopNext()
    {
        if (_queue.Count < 7)
        {
            ReplenishBag();
        }

        var next = _queue[0];
        _queue.RemoveAt(0);
        return next;
    }

    /// <summary>
    /// Peeks at upcoming pieces in the queue without consuming them (for HUD previews).
    /// </summary>
    public IReadOnlyList<TetrominoType> PeekUpcoming(int count)
    {
        while (_queue.Count < count)
        {
            ReplenishBag();
        }

        return _queue.Take(count).ToList();
    }

    /// <summary>
    /// Clears and resets the randomizer.
    /// </summary>
    public void Reset()
    {
        _queue.Clear();
        ReplenishBag();
        ReplenishBag();
    }
}

