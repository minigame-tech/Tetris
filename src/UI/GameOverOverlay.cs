using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Core;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Modal dialog presented upon game completion or matrix overflow,
/// displaying game summary, record badges, and replay options.
/// </summary>
public sealed class GameOverOverlay
{
    private readonly List<Button> _buttons = new();

    public GameOverOverlay(Action onRetry, Action onModes, Action onMainMenu)
    {
        int modalW = 420;
        int modalH = 480;
        int modalX = (1280 - modalW) / 2;
        int modalY = (720 - modalH) / 2;

        int btnW = 320;
        int btnH = 44;
        int startY = modalY + 310;
        int gap = 14;
        int btnX = modalX + (modalW - btnW) / 2;

        _buttons.Add(new Button(new Rectangle(btnX, startY, btnW, btnH), "PLAY AGAIN", onRetry, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (btnH + gap), btnW, btnH), "CHANGE MODE", onModes, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (btnH + gap) * 2, btnW, btnH), "MAIN MENU", onMainMenu, 18));
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        foreach (var b in _buttons)
        {
            b.Update(mousePos, mousePressed, audio);
        }
    }

    public void Draw(ScoreManager score, GameMode mode, float gameTime, bool isNewRecord, bool completedSuccessfully)
    {
        Raylib.DrawRectangle(0, 0, 1280, 720, new Color(10, 14, 25, 215));

        int modalW = 420;
        int modalH = 480;
        int modalX = (1280 - modalW) / 2;
        int modalY = (720 - modalH) / 2;

        Raylib.DrawRectangle(modalX, modalY, modalW, modalH, ColorPalette.CardBackground);
        var borderCol = completedSuccessfully ? ColorPalette.ColorO : new Color(240, 60, 80, 255);
        Raylib.DrawRectangleLinesEx(new Rectangle(modalX, modalY, modalW, modalH), 2f, borderCol);

        // Header Title
        string title = completedSuccessfully ? "SPRINT COMPLETE!" : "GAME OVER";
        int tSize = 30;
        int tWidth = Raylib.MeasureText(title, tSize);
        var titleCol = completedSuccessfully ? ColorPalette.TextGold : new Color(255, 70, 90, 255);
        Raylib.DrawText(title, modalX + (modalW - tWidth) / 2, modalY + 30, tSize, titleCol);

        // New Record Banner
        int curY = modalY + 75;
        if (isNewRecord)
        {
            string recStr = "★  NEW PERSONAL BEST RECORD!  ★";
            int rWidth = Raylib.MeasureText(recStr, 14);
            Raylib.DrawText(recStr, modalX + (modalW - rWidth) / 2, curY, 14, ColorPalette.TextGold);
        }
        else
        {
            curY -= 15;
        }

        // Stats summary
        curY += 35;
        DrawStatRow(modalX + 50, curY, "Final Score:", score.Score.ToString("N0"), ColorPalette.TextPrimary);
        curY += 35;
        DrawStatRow(modalX + 50, curY, "Lines Cleared:", score.Lines.ToString(), ColorPalette.TextPrimary);
        curY += 35;
        DrawStatRow(modalX + 50, curY, "Level Reached:", score.Level.ToString(), ColorPalette.TextPrimary);
        curY += 35;

        int mins = (int)gameTime / 60;
        int secs = (int)gameTime % 60;
        int hun = (int)((gameTime - (int)gameTime) * 100);
        string timeStr = $"{mins:00}:{secs:00}.{hun:00}";
        DrawStatRow(modalX + 50, curY, "Time Elapsed:", timeStr, ColorPalette.TextAccent);

        foreach (var b in _buttons)
        {
            b.Draw();
        }
    }

    private static void DrawStatRow(int x, int y, string label, string value, Color valCol)
    {
        Raylib.DrawText(label, x, y, 16, ColorPalette.TextSecondary);
        int vWidth = Raylib.MeasureText(value, 18);
        Raylib.DrawText(value, x + 320 - vWidth, y, 18, valCol);
    }
}

