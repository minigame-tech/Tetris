using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Core;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Allows players to select between Marathon, 40-Line Sprint, and 2-Minute Ultra modes.
/// </summary>
public sealed class ModeSelectScreen
{
    private readonly List<Button> _buttons = new();
    private readonly Action<GameMode> _onSelectMode;
    private readonly Action _onBack;

    public ModeSelectScreen(Action<GameMode> onSelectMode, Action onBack)
    {
        _onSelectMode = onSelectMode;
        _onBack = onBack;

        int cardW = 300;
        int startY = 190;
        int gap = 30;
        int totalW = (cardW * 3) + (gap * 2);
        int startX = (1280 - totalW) / 2;

        _buttons.Add(new Button(new Rectangle(startX + 20, startY + 260, cardW - 40, 44), "PLAY MARATHON", () => _onSelectMode(GameMode.Marathon), 18));
        _buttons.Add(new Button(new Rectangle(startX + cardW + gap + 20, startY + 260, cardW - 40, 44), "PLAY SPRINT", () => _onSelectMode(GameMode.Sprint40), 18));
        _buttons.Add(new Button(new Rectangle(startX + (cardW + gap) * 2 + 20, startY + 260, cardW - 40, 44), "PLAY ULTRA", () => _onSelectMode(GameMode.Ultra2Min), 18));

        _buttons.Add(new Button(new Rectangle((1280 - 200) / 2, 600, 200, 44), "BACK TO MENU", _onBack, 18));
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        foreach (var b in _buttons)
        {
            b.Update(mousePos, mousePressed, audio);
        }
    }

    public void Draw(TextureManager textures)
    {
        TextureManager.DrawBackground(textures.MainMenuBackground, 1280, 720, Color.White);
        Raylib.DrawRectangle(0, 0, 1280, 720, new Color(12, 16, 28, 160));

        string title = "SELECT GAME MODE";
        int tSize = 36;
        int tWidth = Raylib.MeasureText(title, tSize);
        Raylib.DrawText(title, (1280 - tWidth) / 2, 70, tSize, ColorPalette.TextPrimary);

        string subtitle = "CHOOSE YOUR CHALLENGE";
        int sSize = 16;
        int sWidth = Raylib.MeasureText(subtitle, sSize);
        Raylib.DrawText(subtitle, (1280 - sWidth) / 2, 120, sSize, ColorPalette.TextAccent);

        // Draw mode cards
        int cardW = 300;
        int cardH = 340;
        int startY = 190;
        int gap = 30;
        int totalW = (cardW * 3) + (gap * 2);
        int startX = (1280 - totalW) / 2;

        DrawModeCard(startX, startY, cardW, cardH, "MARATHON", "ENDLESS SURVIVAL",
            "The classic Tetris experience.\nGravity increases every 10 lines.\nSurvive as long as possible\nand chase the highest score!",
            ColorPalette.ColorI);

        DrawModeCard(startX + cardW + gap, startY, cardW, cardH, "SPRINT 40", "SPEEDRUN ATTACK",
            "Clear 40 lines as fast as you can!\nEvery millisecond counts.\nPrecision, clean stacking,\nand instant hard drops win.",
            ColorPalette.ColorO);

        DrawModeCard(startX + (cardW + gap) * 2, startY, cardW, cardH, "ULTRA 2-MIN", "SCORE TIME TRIAL",
            "Race against the clock!\nYou have 120 seconds to\nrack up maximum points with\nTetris clears and T-Spins.",
            ColorPalette.ColorT);

        // Draw buttons
        foreach (var b in _buttons)
        {
            b.Draw();
        }
    }

    private static void DrawModeCard(int x, int y, int w, int h, string title, string tag, string desc, Color accent)
    {
        Raylib.DrawRectangle(x, y, w, h, ColorPalette.CardBackground);
        Raylib.DrawRectangleLines(x, y, w, h, ColorPalette.CardBorder);

        // Top accent stripe
        Raylib.DrawRectangle(x, y, w, 6, accent);

        // Header
        Raylib.DrawText(title, x + 24, y + 26, 24, ColorPalette.TextPrimary);
        Raylib.DrawText(tag, x + 24, y + 56, 13, accent);

        // Description lines
        string[] lines = desc.Split('\n');
        int lineY = y + 105;
        foreach (var line in lines)
        {
            Raylib.DrawText(line, x + 24, lineY, 15, ColorPalette.TextSecondary);
            lineY += 24;
        }
    }
}
