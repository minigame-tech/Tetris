using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Frosted glass overlay rendered when gameplay is paused.
/// Provides immediate options to resume, restart, adjust settings, or return to the main menu.
/// </summary>
public sealed class PauseOverlay
{
    private readonly List<Button> _buttons = new();
    private readonly Action _onResume;
    private readonly Action _onMainMenu;

    public PauseOverlay(Action onResume, Action onRestart, Action onSettings, Action onMainMenu)
    {
        _onResume = onResume;
        _onMainMenu = onMainMenu;

        int modalW = 380;
        int modalH = 400;
        int modalX = (1280 - modalW) / 2;
        int modalY = (720 - modalH) / 2;

        int btnW = 280;
        int btnH = 46;
        int startY = modalY + 95;
        int gap = 14;
        int btnX = modalX + (modalW - btnW) / 2;

        _buttons.Add(new Button(new Rectangle(btnX, startY, btnW, btnH), "RESUME GAME", onResume, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (btnH + gap), btnW, btnH), "RESTART SESSION", onRestart, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (btnH + gap) * 2, btnW, btnH), "SETTINGS", onSettings, 18));
        _buttons.Add(new Button(new Rectangle(btnX, startY + (btnH + gap) * 3, btnW, btnH), "RETURN TO MAIN MENU", onMainMenu, 18));
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        // Keyboard shortcuts while paused:
        if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.P))
        {
            audio.PlaySound(SoundEffectType.ButtonClick);
            _onResume.Invoke();
            return;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.M))
        {
            audio.PlaySound(SoundEffectType.ButtonClick);
            _onMainMenu.Invoke();
            return;
        }

        foreach (var b in _buttons)
        {
            b.Update(mousePos, mousePressed, audio);
        }
    }

    public void Draw()
    {
        // Dark translucent scrim
        Raylib.DrawRectangle(0, 0, 1280, 720, new Color(10, 14, 25, 215));

        int modalW = 380;
        int modalH = 400;
        int modalX = (1280 - modalW) / 2;
        int modalY = (720 - modalH) / 2;

        // Glass modal container
        Raylib.DrawRectangle(modalX, modalY, modalW, modalH, ColorPalette.CardBackground);
        Raylib.DrawRectangleLinesEx(new Rectangle(modalX, modalY, modalW, modalH), 2f, ColorPalette.BoardGlow);

        // Header Title
        string title = "GAME PAUSED";
        int tSize = 28;
        int tWidth = Raylib.MeasureText(title, tSize);
        Raylib.DrawText(title, modalX + (modalW - tWidth) / 2, modalY + 28, tSize, ColorPalette.TextPrimary);

        string subtitle = "SESSION PROGRESS FROZEN";
        int sSize = 13;
        int sWidth = Raylib.MeasureText(subtitle, sSize);
        Raylib.DrawText(subtitle, modalX + (modalW - sWidth) / 2, modalY + 62, sSize, ColorPalette.TextAccent);

        foreach (var b in _buttons)
        {
            b.Draw();
        }

        // Keyboard hints
        string hint = "Esc / P : Resume   |   M : Main Menu";
        int hSize = 12;
        int hWidth = Raylib.MeasureText(hint, hSize);
        Raylib.DrawText(hint, modalX + (modalW - hWidth) / 2, modalY + modalH - 26, hSize, ColorPalette.TextSecondary);
    }
}
