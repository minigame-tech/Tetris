using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Frosted glass overlay rendered when gameplay is paused.
/// </summary>
public sealed class PauseOverlay
{
    private readonly List<Button> _buttons = new();

    public PauseOverlay(Action onResume, Action onRestart, Action onSettings, Action onMainMenu)
    {
        int modalW = 320;
        int modalH = 360;
        int modalX = (1280 - modalW) / 2;
        int modalY = (720 - modalH) / 2;

        int btnW = 240;
        int btnH = 44;
        int startY = modalY + 90;
        int gap = 16;
        int btnX = modalX + (modalW - btnW) / 2;

        _buttons.Add(new Button(new Rectangle(btnX, startY, btnW, btnH), "RESUME", onResume, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (btnH + gap), btnW, btnH), "RESTART", onRestart, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (btnH + gap) * 2, btnW, btnH), "SETTINGS", onSettings, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (btnH + gap) * 3, btnW, btnH), "MAIN MENU", onMainMenu, 18));
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        foreach (var b in _buttons)
        {
            b.Update(mousePos, mousePressed, audio);
        }
    }

    public void Draw()
    {
        // Dark translucent scrim
        Raylib.DrawRectangle(0, 0, 1280, 720, new Color(10, 14, 25, 200));

        int modalW = 320;
        int modalH = 360;
        int modalX = (1280 - modalW) / 2;
        int modalY = (720 - modalH) / 2;

        // Glass modal container
        Raylib.DrawRectangle(modalX, modalY, modalW, modalH, ColorPalette.CardBackground);
        Raylib.DrawRectangleLinesEx(new Rectangle(modalX, modalY, modalW, modalH), 2f, ColorPalette.BoardGlow);

        string title = "PAUSED";
        int tSize = 28;
        int tWidth = Raylib.MeasureText(title, tSize);
        Raylib.DrawText(title, modalX + (modalW - tWidth) / 2, modalY + 30, tSize, ColorPalette.TextPrimary);

        foreach (var b in _buttons)
        {
            b.Draw();
        }
    }
}

