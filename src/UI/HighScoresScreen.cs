using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Core;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Displays persistent records, high scores, lines, and speedrun times across game modes.
/// </summary>
public sealed class HighScoresScreen
{
    private readonly Button _backButton;

    public HighScoresScreen(Action onBack)
    {
        _backButton = new Button(new Rectangle((1280 - 220) / 2, 620, 220, 46), "BACK TO MENU", onBack, 18);
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        _backButton.Update(mousePos, mousePressed, audio);
    }

    public void Draw(TextureManager textures, HighScoreManager highScores)
    {
        TextureManager.DrawBackground(textures.MainMenuBackground, 1280, 720, Color.White);
        Raylib.DrawRectangle(0, 0, 1280, 720, new Color(12, 16, 28, 170));

        string title = "HALL OF FAME & RECORDS";
        int tSize = 34;
        int tWidth = Raylib.MeasureText(title, tSize);
        Raylib.DrawText(title, (1280 - tWidth) / 2, 60, tSize, ColorPalette.TextPrimary);

        int cardW = 320;
        int cardH = 420;
        int startY = 140;
        int gap = 30;
        int totalW = (cardW * 3) + (gap * 2);
        int startX = (1280 - totalW) / 2;

        // Marathon Card
        DrawRecordCard(startX, startY, cardW, cardH, "MARATHON", ColorPalette.ColorI, new[]
        {
            ("HIGH SCORE", highScores.MarathonHighScore.ToString("N0")),
            ("MAX LINES", highScores.MarathonBestLines.ToString()),
            ("PEAK LEVEL", highScores.MarathonBestLevel.ToString())
        });

        // Sprint Card
        string sprintTime = highScores.Sprint40BestTime > 0f ? $"{highScores.Sprint40BestTime:0.00}s" : "--";
        DrawRecordCard(startX + cardW + gap, startY, cardW, cardH, "SPRINT 40L", ColorPalette.ColorO, new[]
        {
            ("BEST TIME", sprintTime),
            ("TARGET", "40 Lines Cleared"),
            ("GOAL", "Sub 60.00s Speedrun")
        });

        // Ultra Card
        DrawRecordCard(startX + (cardW + gap) * 2, startY, cardW, cardH, "ULTRA 2-MIN", ColorPalette.ColorT, new[]
        {
            ("HIGH SCORE", highScores.Ultra2MinHighScore.ToString("N0")),
            ("TIME LIMIT", "120 Seconds"),
            ("STRATEGY", "B2B Tetris & T-Spins")
        });

        _backButton.Draw();
    }

    private static void DrawRecordCard(int x, int y, int w, int h, string title, Color accent, (string Label, string Value)[] stats)
    {
        Raylib.DrawRectangle(x, y, w, h, ColorPalette.CardBackground);
        Raylib.DrawRectangleLines(x, y, w, h, ColorPalette.CardBorder);
        Raylib.DrawRectangle(x, y, w, 6, accent);

        Raylib.DrawText(title, x + 24, y + 30, 24, ColorPalette.TextPrimary);

        int curY = y + 90;
        foreach (var (lbl, val) in stats)
        {
            Raylib.DrawText(lbl, x + 24, curY, 14, ColorPalette.TextSecondary);
            curY += 24;
            Raylib.DrawText(val, x + 24, curY, 26, ColorPalette.TextGold);
            curY += 60;
        }
    }
}

