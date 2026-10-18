using System.Numerics;
using Raylib_cs;

namespace Tetris.Rendering;

/// <summary>
/// High-performance particle engine for impacts, line-clear detonations,
/// level-up celebrations, and ambient starfield drift.
/// </summary>
public sealed class ParticleSystem
{
    private struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Size;
        public float Life;
        public float MaxLife;
        public float Gravity;
    }

    private readonly List<Particle> _particles = new(512);
    private readonly Random _random = new();

    // Ambient floating dust particles
    private readonly List<Vector2> _ambientDust = new(60);
    private readonly List<float> _ambientSpeeds = new(60);

    public ParticleSystem(int screenWidth = 1280, int screenHeight = 720)
    {
        for (int i = 0; i < 60; i++)
        {
            _ambientDust.Add(new Vector2(_random.Next(screenWidth), _random.Next(screenHeight)));
            _ambientSpeeds.Add(15f + (float)_random.NextDouble() * 30f);
        }
    }

    public void Update(float dt, int screenWidth, int screenHeight)
    {
        // Update active particles
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var p = _particles[i];
            p.Life -= dt;
            if (p.Life <= 0f)
            {
                _particles.RemoveAt(i);
                continue;
            }

            p.Position += p.Velocity * dt;
            p.Velocity.Y += p.Gravity * dt;
            p.Velocity.X *= MathF.Pow(0.96f, dt * 60f); // Air resistance

            _particles[i] = p;
        }

        // Update ambient dust particles
        for (int i = 0; i < _ambientDust.Count; i++)
        {
            var pos = _ambientDust[i];
            pos.Y += _ambientSpeeds[i] * dt;
            pos.X += MathF.Sin(pos.Y * 0.02f) * 10f * dt;
            if (pos.Y > screenHeight)
            {
                pos.Y = -5;
                pos.X = _random.Next(screenWidth);
            }
            _ambientDust[i] = pos;
        }
    }

    public void Draw()
    {
        // Draw ambient dust
        for (int i = 0; i < _ambientDust.Count; i++)
        {
            var pos = _ambientDust[i];
            Raylib.DrawCircle((int)pos.X, (int)pos.Y, 1.5f, new Color(120, 180, 255, 45));
        }

        // Draw active dynamic particles
        for (int i = 0; i < _particles.Count; i++)
        {
            var p = _particles[i];
            float frac = p.Life / p.MaxLife;
            byte alpha = (byte)(p.Color.A * frac);
            var col = new Color(p.Color.R, p.Color.G, p.Color.B, alpha);
            float curSize = Math.Max(1f, p.Size * frac);

            Raylib.DrawRectangle((int)(p.Position.X - curSize / 2), (int)(p.Position.Y - curSize / 2),
                (int)curSize, (int)curSize, col);
        }
    }

    public void SpawnHardDropSparks(float startX, float endX, float y, Color color)
    {
        int count = 25;
        for (int i = 0; i < count; i++)
        {
            float px = startX + (float)_random.NextDouble() * (endX - startX);
            float angle = (float)(_random.NextDouble() * Math.PI); // Upward spray
            float speed = 80f + (float)_random.NextDouble() * 220f;

            _particles.Add(new Particle
            {
                Position = new Vector2(px, y),
                Velocity = new Vector2(MathF.Cos(angle) * speed, -MathF.Sin(angle) * speed),
                Color = color,
                Size = 3f + (float)_random.NextDouble() * 4f,
                Life = 0.35f + (float)_random.NextDouble() * 0.25f,
                MaxLife = 0.6f,
                Gravity = 300f
            });
        }
    }

    public void SpawnLineClearExplosion(float x, float y, float width, Color color)
    {
        int count = 40;
        for (int i = 0; i < count; i++)
        {
            float px = x + (float)_random.NextDouble() * width;
            float angle = (float)(_random.NextDouble() * Math.PI * 2);
            float speed = 60f + (float)_random.NextDouble() * 320f;

            _particles.Add(new Particle
            {
                Position = new Vector2(px, y),
                Velocity = new Vector2(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed),
                Color = color,
                Size = 4f + (float)_random.NextDouble() * 5f,
                Life = 0.45f + (float)_random.NextDouble() * 0.4f,
                MaxLife = 0.85f,
                Gravity = 200f
            });
        }
    }

    public void SpawnCelebration(float screenWidth, float screenHeight)
    {
        int bursts = 5;
        Color[] festiveColors = { ColorPalette.ColorI, ColorPalette.ColorJ, ColorPalette.ColorL,
                                  ColorPalette.ColorO, ColorPalette.ColorS, ColorPalette.ColorT };

        for (int b = 0; b < bursts; b++)
        {
            float cx = screenWidth * 0.2f + (float)_random.NextDouble() * (screenWidth * 0.6f);
            float cy = screenHeight * 0.15f + (float)_random.NextDouble() * (screenHeight * 0.5f);
            var col = festiveColors[_random.Next(festiveColors.Length)];

            for (int i = 0; i < 35; i++)
            {
                float angle = (float)(_random.NextDouble() * Math.PI * 2);
                float speed = 100f + (float)_random.NextDouble() * 350f;
                _particles.Add(new Particle
                {
                    Position = new Vector2(cx, cy),
                    Velocity = new Vector2(MathF.Cos(angle) * speed, MathF.Sin(angle) * speed),
                    Color = col,
                    Size = 5f + (float)_random.NextDouble() * 6f,
                    Life = 0.8f + (float)_random.NextDouble() * 0.6f,
                    MaxLife = 1.4f,
                    Gravity = 180f
                });
            }
        }
    }

    public void Clear()
    {
        _particles.Clear();
    }
}

