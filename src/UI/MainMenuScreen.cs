using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Core;
using Tetris.Models;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Modern title screen featuring a dynamic cyber-arcade background with animated
/// floating tetrominoes, neon grid perspective, and interactive buttons.
/// </summary>
public sealed class MainMenuScreen
{
    private struct FloatingPiece
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float RotationAngle;
        public float AngularVelocity;
        public TetrominoType Type;
        public float Scale;
        public float Alpha;
    }

    private readonly List<Button> _buttons = new();
    private readonly List<FloatingPiece> _floatingPieces = new();
    private readonly Random _random = new(1337);
    private float _glowTimer = 0f;
    private const int CardWidth = 350;
    private const int CardHeight = 410;
    private const int CardX = (1280 - CardWidth) / 2;
    private const int CardY = 210;

    private const int ButtonWidth = 280;
    private const int ButtonHeight = 46;
    private const int ButtonGap = 13;

    public MainMenuScreen(Action onPlay, Action onModes, Action onHowToPlay, Action onHighScores, Action onSettings, Action onQuit)
    {
        int totalButtonHeight = (6 * ButtonHeight) + (5 * ButtonGap);
        int startY = CardY + (CardHeight - totalButtonHeight) / 2;
        int btnX = CardX + (CardWidth - ButtonWidth) / 2;

        _buttons.Add(new Button(new Rectangle(btnX, startY, ButtonWidth, ButtonHeight), "PLAY NOW", onPlay, 20));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (ButtonHeight + ButtonGap), ButtonWidth, ButtonHeight), "GAME MODES", onModes, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (ButtonHeight + ButtonGap) * 2, ButtonWidth, ButtonHeight), "HOW TO PLAY", onHowToPlay, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (ButtonHeight + ButtonGap) * 3, ButtonWidth, ButtonHeight), "HIGH SCORES", onHighScores, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (ButtonHeight + ButtonGap) * 4, ButtonWidth, ButtonHeight), "SETTINGS", onSettings, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (ButtonHeight + ButtonGap) * 5, ButtonWidth, ButtonHeight), "EXIT GAME", onQuit, 18));

        // Initialize ambient floating tetrominoes
        TetrominoType[] types = { TetrominoType.I, TetrominoType.J, TetrominoType.L,
                                  TetrominoType.O, TetrominoType.S, TetrominoType.T, TetrominoType.Z };

        for (int i = 0; i < 14; i++)
        {
            _floatingPieces.Add(new FloatingPiece
            {
                Position = new Vector2(_random.Next(1280), _random.Next(720)),
                Velocity = new Vector2(
                    (float)(_random.NextDouble() * 30.0 - 15.0),
                    (float)(15.0 + _random.NextDouble() * 35.0)
                ),
                RotationAngle = (float)(_random.NextDouble() * Math.PI * 2),
                AngularVelocity = (float)(_random.NextDouble() * 0.8 - 0.4),
                Type = types[i % types.Length],
                Scale = (float)(16.0 + _random.NextDouble() * 14.0),
                Alpha = (float)(0.20 + _random.NextDouble() * 0.25)
            });
        }
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        _glowTimer += dt;

        // Update floating background tetrominoes
        for (int i = 0; i < _floatingPieces.Count; i++)
        {
            var p = _floatingPieces[i];
            p.Position += p.Velocity * dt;
            p.RotationAngle += p.AngularVelocity * dt;

            // Screen wrap
            if (p.Position.Y > 750)
            {
                p.Position.Y = -50;
                p.Position.X = _random.Next(1280);
            }
            if (p.Position.X < -60) p.Position.X = 1320;
            if (p.Position.X > 1340) p.Position.X = -40;

            _floatingPieces[i] = p;
        }

        foreach (var btn in _buttons)
        {
            btn.Update(mousePos, mousePressed, audio);
        }
    }

    public void Draw(TextureManager textures)
    {
        // 1. Draw base high-resolution cyber arcade background
        TextureManager.DrawBackground(textures.MainMenuBackground, 1280, 720, Color.White);

        // 2. Animated perspective cyber grid on bottom horizon
        DrawCyberGrid();

        // 3. Draw ambient floating neon tetrominoes in background
        DrawFloatingPieces();

        // 4. Subtle ambient vignette to focus on center menu
        Raylib.DrawRectangleGradientV(0, 0, 1280, 720,
            new Color(10, 14, 26, 90),
            new Color(15, 12, 35, 170));

        // 5. Title & Branding
        float pulse = MathF.Sin(_glowTimer * 2.5f) * 0.15f + 0.85f;
        string title = "TETRIS  NEO";
        int titleSize = 64;
        int titleWidth = Raylib.MeasureText(title, titleSize);
        int titleX = (1280 - titleWidth) / 2;
        int titleY = 65;

        // Title glow dropshadow
        var glowCol = new Color(0, 220, 255, (int)(110 * pulse));
        Raylib.DrawText(title, titleX - 2, titleY - 2, titleSize, glowCol);
        Raylib.DrawText(title, titleX + 2, titleY + 2, titleSize, glowCol);
        Raylib.DrawText(title, titleX, titleY, titleSize, ColorPalette.TextPrimary);

        string subtitle = "THE MODERN HIGH-PERFORMANCE ARCADE EDITION";
        int subSize = 15;
        int subWidth = Raylib.MeasureText(subtitle, subSize);
        Raylib.DrawText(subtitle, (1280 - subWidth) / 2, titleY + 70, subSize, ColorPalette.TextAccent);

        // 6. Menu Glass Card Container (perfectly framing the centered buttons)
        Raylib.DrawRectangle(CardX, CardY, CardWidth, CardHeight, ColorPalette.CardBackground);
        Raylib.DrawRectangleLinesEx(new Rectangle(CardX, CardY, CardWidth, CardHeight), 2f, ColorPalette.CardBorder);

        // Buttons
        foreach (var btn in _buttons)
        {
            btn.Draw();
        }

        // Version & Credit footer
        string footer = "minigame-tech  |  Raylib-cs  |  v1.0.0";
        int fWidth = Raylib.MeasureText(footer, 14);
        Raylib.DrawText(footer, (1280 - fWidth) / 2, 690, 14, ColorPalette.TextSecondary);
    }

    private void DrawCyberGrid()
    {
        int horizonY = 520;
        int screenWidth = 1280;
        int screenHeight = 720;

        // Horizontal perspective lines
        for (int y = horizonY; y < screenHeight; y += (int)Math.Max(6, (y - horizonY) * 0.4f + 8))
        {
            float alphaFrac = (float)(y - horizonY) / (screenHeight - horizonY);
            var col = new Color(0, 200, 255, (int)(45 * alphaFrac));
            Raylib.DrawLine(0, y, screenWidth, y, col);
        }

        // Vertical radiating lines from center vanishing point
        int cx = screenWidth / 2;
        for (int x = -400; x <= screenWidth + 400; x += 90)
        {
            var col = new Color(0, 180, 255, 30);
            Raylib.DrawLine(cx, horizonY, x, screenHeight, col);
        }
    }

    private void DrawFloatingPieces()
    {
        foreach (var p in _floatingPieces)
        {
            var dummy = new Tetromino(p.Type);
            var localShape = dummy.GetLocalShape(RotationState.Spawn);
            var baseCol = ColorPalette.GetColor(p.Type);

            int blockSize = (int)p.Scale;
            float cos = MathF.Cos(p.RotationAngle);
            float sin = MathF.Sin(p.RotationAngle);

            foreach (var pos in localShape)
            {
                // Local relative offset from shape center
                float lx = (pos.Col - 1.5f) * blockSize;
                float ly = (pos.Row - 1.0f) * blockSize;

                // Rotate offset
                float rx = lx * cos - ly * sin;
                float ry = lx * sin + ly * cos;

                int drawX = (int)(p.Position.X + rx);
                int drawY = (int)(p.Position.Y + ry);

                // Draw translucent glowing block
                var fill = new Color(baseCol.R, baseCol.G, baseCol.B, (int)(255 * p.Alpha * 0.45f));
                var wire = new Color(baseCol.R, baseCol.G, baseCol.B, (int)(255 * p.Alpha));

                Raylib.DrawRectangle(drawX, drawY, blockSize - 1, blockSize - 1, fill);
                Raylib.DrawRectangleLines(drawX, drawY, blockSize - 1, blockSize - 1, wire);
            }
        }
    }
}
