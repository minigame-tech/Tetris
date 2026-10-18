using System.Numerics;

namespace Tetris.Rendering;

/// <summary>
/// Non-linear camera trauma system producing crisp, visceral screen shakes.
/// </summary>
public sealed class ScreenShake
{
    private float _trauma = 0f;
    private readonly Random _random = new();

    public float MaxOffset { get; set; } = 12f;
    public float DecayRate { get; set; } = 2.2f;

    public void AddTrauma(float amount)
    {
        _trauma = Math.Clamp(_trauma + amount, 0f, 1f);
    }

    public void Update(float dt)
    {
        if (_trauma > 0f)
        {
            _trauma = Math.Max(0f, _trauma - DecayRate * dt);
        }
    }

    public Vector2 GetOffset()
    {
        if (_trauma <= 0.001f) return Vector2.Zero;

        // Quadratic response for punchy hits
        float shake = _trauma * _trauma;
        float offsetX = (float)(_random.NextDouble() * 2.0 - 1.0) * MaxOffset * shake;
        float offsetY = (float)(_random.NextDouble() * 2.0 - 1.0) * MaxOffset * shake;

        return new Vector2(offsetX, offsetY);
    }
}

