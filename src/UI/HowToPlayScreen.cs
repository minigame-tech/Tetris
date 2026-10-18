using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Displays comprehensive gameplay instructions, keyboard mappings,
/// SRS rotation rules, and advanced scoring mechanics using the How_Games backdrop.
/// </summary>
public sealed class HowToPlayScreen
{
    private readonly Button _backButton;

    public HowToPlayScreen(Action onBack)
    {
        _backButton = new Button(new Rectangle((1280 - 220) / 2, 630, 220, 46), "BACK TO MENU", onBack, 18);
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        _backButton.Update(mousePos, mousePressed, audio);
    }

    public void Draw(TextureManager textures)
    {
        // Background from How_Games.jpg
        TextureManager.DrawBackground(textures.HowToPlayBackground, 1280, 720, new Color(200, 215, 255, 255));
        Raylib.DrawRectangle(0, 0, 1280, 720, new Color(12, 16, 28, 215));

        // Title
        string title = "HOW TO PLAY";
        int tSize = 34;
        int tWidth = Raylib.MeasureText(title, tSize);
        Raylib.DrawText(title, (1280 - tWidth) / 2, 40, tSize, ColorPalette.TextPrimary);

        // Main glass modal
        int cardX = 140;
        int cardY = 95;
        int cardW = 1000;
        int cardH = 510;

        Raylib.DrawRectangle(cardX, cardY, cardW, cardH, ColorPalette.CardBackground);
        Raylib.DrawRectangleLines(cardX, cardY, cardW, cardH, ColorPalette.CardBorder);

        // Section 1: CONTROLS (Left column)
        int col1X = cardX + 40;
        int curY = cardY + 30;
        Raylib.DrawText("CORE CONTROLS", col1X, curY, 20, ColorPalette.TextAccent);
        curY += 32;

        DrawKeyMapping(col1X, curY, "Left / Right (or A / D)", "Move Tetromino horizontally (DAS/ARR auto-repeat)");
        curY += 36;
        DrawKeyMapping(col1X, curY, "Down (or S)", "Soft Drop (accelerate gravity, +1 pt/cell)");
        curY += 36;
        DrawKeyMapping(col1X, curY, "Spacebar", "Hard Drop (instant placement & lock, +2 pt/cell)");
        curY += 36;
        DrawKeyMapping(col1X, curY, "Up / W / X", "Rotate 90 degrees Clockwise (SRS kicks)");
        curY += 36;
        DrawKeyMapping(col1X, curY, "Z / Left Control", "Rotate 90 degrees Counter-Clockwise");
        curY += 36;
        DrawKeyMapping(col1X, curY, "C / Left Shift", "Hold Piece (store piece, 1 swap per drop)");
        curY += 36;
        DrawKeyMapping(col1X, curY, "Escape / P", "Pause / Resume Game");
        curY += 36;
        DrawKeyMapping(col1X, curY, "R", "Quick Restart Game");

        // Section 2: ADVANCED MECHANICS (Right column)
        int col2X = cardX + 530;
        curY = cardY + 30;
        Raylib.DrawText("RULES & SCORING", col2X, curY, 20, ColorPalette.TextAccent);
        curY += 32;

        DrawInfoRow(col2X, curY, "Super Rotation System (SRS):", "Pieces kick off walls and blocks smoothly when rotating near obstacles.");
        curY += 46;
        DrawInfoRow(col2X, curY, "7-Bag Randomizer:", "Guarantees all 7 pieces appear per cycle. No long droughts!");
        curY += 46;
        DrawInfoRow(col2X, curY, "Lock Delay:", "0.5s grace period when touching a surface before locking.");
        curY += 46;
        DrawInfoRow(col2X, curY, "Back-to-Back Bonus (B2B):", "Consecutive Tetris (4-line) or T-Spin clears award a +50% score bonus!");
        curY += 46;
        DrawInfoRow(col2X, curY, "Combos:", "Successive line clears increment combos for compounding bonus points.");
        curY += 46;
        DrawInfoRow(col2X, curY, "T-Spins:", "Rotate a T piece into tight 3-corner pockets for massive points!");

        // Draw back button
        _backButton.Draw();
    }

    private static void DrawKeyMapping(int x, int y, string key, string desc)
    {
        Raylib.DrawRectangle(x, y, 175, 26, new Color(30, 42, 68, 220));
        Raylib.DrawRectangleLines(x, y, 175, 26, ColorPalette.CardBorder);
        Raylib.DrawText(key, x + 8, y + 6, 12, ColorPalette.TextGold);
        Raylib.DrawText(desc, x + 190, y + 6, 13, ColorPalette.TextSecondary);
    }

    private static void DrawInfoRow(int x, int y, string header, string body)
    {
        Raylib.DrawText(header, x, y, 15, ColorPalette.TextPrimary);
        Raylib.DrawText(body, x, y + 20, 13, ColorPalette.TextSecondary);
    }
}

