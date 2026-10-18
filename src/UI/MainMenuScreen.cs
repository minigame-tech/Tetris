using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Core;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Modern title screen featuring animated neon branding and navigation buttons.
/// </summary>
public sealed class MainMenuScreen
{
    private readonly List<Button> _buttons = new();
    private float _glowTimer = 0f;

    public MainMenuScreen(Action onPlay, Action onModes, Action onHowToPlay, Action onHighScores, Action onSettings, Action onQuit)
    {
        int startY = 240;
        int btnWidth = 280;
        int btnHeight = 48;
        int gap = 16;
        int centerX = (1280 - btnWidth) / 2;

        _buttons.Add(new Button(new Rectangle(centerX, startY, btnWidth, btnHeight), "PLAY NOW", onPlay, 22));
        _buttons.Add(new Button(new Rectangle(centerX, startY + (btnHeight + gap), btnWidth, btnHeight), "GAME MODES", onModes, 20));
        _buttons.Add(new Button(new Rectangle(centerX, startY + (btnHeight + gap) * 2, btnWidth, btnHeight), "HOW TO PLAY", onHowToPlay, 20));
        _buttons.Add(new Button(new Rectangle(centerX, startY + (btnHeight + gap) * 3, btnWidth, btnHeight), "HIGH SCORES", onHighScores, 20));
        _buttons.Add(new Button(new Rectangle(centerX, startY + (btnHeight + gap) * 4, btnWidth, btnHeight), "SETTINGS", onSettings, 20));
        _buttons.Add(new Button(new Rectangle(centerX, startY + (btnHeight + gap) * 5, btnWidth, btnHeight), "EXIT", onQuit, 20));
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        _glowTimer += dt;
        foreach (var btn in _buttons)
        {
            btn.Update(mousePos, mousePressed, audio);
        }
    }

    public void Draw(TextureManager textures)
    {
        // Background from assets/background/BackGroundMainMenu.jpg
        TextureManager.DrawBackground(textures.MainMenuBackground, 1280, 720, Color.White);

        // Ambient dark overlay for title and button readability
        Raylib.DrawRectangle(0, 0, 1280, 720, new Color(10, 15, 28, 120));

        // Animated neon Title
        float pulse = MathF.Sin(_glowTimer * 2.5f) * 0.15f + 0.85f;
        string title = "TETRIS  NEO";
        int titleSize = 64;
        int titleWidth = Raylib.MeasureText(title, titleSize);
        int titleX = (1280 - titleWidth) / 2;
        int titleY = 90;

        // Title glow dropshadow
        var glowCol = new Color(0, 220, 255, (int)(100 * pulse));
        Raylib.DrawText(title, titleX - 2, titleY - 2, titleSize, glowCol);
        Raylib.DrawText(title, titleX + 2, titleY + 2, titleSize, glowCol);
        Raylib.DrawText(title, titleX, titleY, titleSize, ColorPalette.TextPrimary);

        string subtitle = "THE MODERN HIGH-PERFORMANCE ARCADE EDITION";
        int subSize = 16;
        int subWidth = Raylib.MeasureText(subtitle, subSize);
        Raylib.DrawText(subtitle, (1280 - subWidth) / 2, titleY + 70, subSize, ColorPalette.TextAccent);

        // Buttons
        foreach (var btn in _buttons)
        {
            btn.Draw();
        }

        // Version & Credit footer
        string footer = "minigame-tech  |  Raylib-cs  |  v1.0.0";
        int fWidth = Raylib.MeasureText(footer, 14);
        Raylib.DrawText(footer, (1280 - fWidth) / 2, 685, 14, ColorPalette.TextSecondary);
    }
}
