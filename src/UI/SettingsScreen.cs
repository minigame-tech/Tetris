using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Core;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Allows players to customize audio levels, toggles, visual effects, and display preferences.
/// </summary>
public sealed class SettingsScreen
{
    private readonly GameSettings _settings;
    private readonly AudioManager _audio;
    private readonly List<Button> _buttons = new();
    private readonly Button _backButton;

    public SettingsScreen(GameSettings settings, AudioManager audio, Action onBack)
    {
        _settings = settings;
        _audio = audio;

        int cardX = 340;
        int cardY = 120;
        int cardW = 600;
        int btnW = 140;
        int btnH = 38;

        int rowY = cardY + 50;
        int rowGap = 50;

        // Master Volume
        _buttons.Add(new Button(new Rectangle(cardX + cardW - 170, rowY, btnW, btnH), GetVolumeText(_settings.MasterVolume), () =>
        {
            _settings.MasterVolume = CycleVolume(_settings.MasterVolume);
            _audio.ApplyVolumes();
            _settings.Save();
        }, 16));

        // Music Volume
        rowY += rowGap;
        _buttons.Add(new Button(new Rectangle(cardX + cardW - 170, rowY, btnW, btnH), GetVolumeText(_settings.MusicVolume), () =>
        {
            _settings.MusicVolume = CycleVolume(_settings.MusicVolume);
            _audio.ApplyVolumes();
            _settings.Save();
        }, 16));

        // SFX Volume
        rowY += rowGap;
        _buttons.Add(new Button(new Rectangle(cardX + cardW - 170, rowY, btnW, btnH), GetVolumeText(_settings.SfxVolume), () =>
        {
            _settings.SfxVolume = CycleVolume(_settings.SfxVolume);
            _audio.ApplyVolumes();
            _settings.Save();
        }, 16));

        // Mute
        rowY += rowGap;
        _buttons.Add(new Button(new Rectangle(cardX + cardW - 170, rowY, btnW, btnH), _settings.IsMuted ? "MUTED" : "UNMUTED", () =>
        {
            _audio.ToggleMute();
        }, 16));

        // Ghost Piece
        rowY += rowGap;
        _buttons.Add(new Button(new Rectangle(cardX + cardW - 170, rowY, btnW, btnH), _settings.GhostPieceEnabled ? "ENABLED" : "DISABLED", () =>
        {
            _settings.GhostPieceEnabled = !_settings.GhostPieceEnabled;
            _settings.Save();
        }, 16));

        // Screen Shake
        rowY += rowGap;
        _buttons.Add(new Button(new Rectangle(cardX + cardW - 170, rowY, btnW, btnH), _settings.ScreenShakeEnabled ? "ENABLED" : "DISABLED", () =>
        {
            _settings.ScreenShakeEnabled = !_settings.ScreenShakeEnabled;
            _settings.Save();
        }, 16));

        // Particles
        rowY += rowGap;
        _buttons.Add(new Button(new Rectangle(cardX + cardW - 170, rowY, btnW, btnH), _settings.ParticlesEnabled ? "ENABLED" : "DISABLED", () =>
        {
            _settings.ParticlesEnabled = !_settings.ParticlesEnabled;
            _settings.Save();
        }, 16));

        _backButton = new Button(new Rectangle((1280 - 220) / 2, 620, 220, 46), "SAVE & BACK", onBack, 18);
    }

    private static float CycleVolume(float current)
    {
        int pct = (int)Math.Round(current * 10);
        pct = (pct + 2) % 12; // 0, 2, 4, 6, 8, 10
        return pct / 10f;
    }

    private static string GetVolumeText(float vol)
    {
        return $"{(int)(vol * 100)}%";
    }

    public void Update(float dt, Vector2 mousePos, bool mousePressed, AudioManager audio)
    {
        // Update button texts to reflect current state
        _buttons[0].Text = GetVolumeText(_settings.MasterVolume);
        _buttons[1].Text = GetVolumeText(_settings.MusicVolume);
        _buttons[2].Text = GetVolumeText(_settings.SfxVolume);
        _buttons[3].Text = _settings.IsMuted ? "MUTED" : "UNMUTED";
        _buttons[4].Text = _settings.GhostPieceEnabled ? "ENABLED" : "DISABLED";
        _buttons[5].Text = _settings.ScreenShakeEnabled ? "ENABLED" : "DISABLED";
        _buttons[6].Text = _settings.ParticlesEnabled ? "ENABLED" : "DISABLED";

        foreach (var b in _buttons)
        {
            b.Update(mousePos, mousePressed, audio);
        }

        _backButton.Update(mousePos, mousePressed, audio);
    }

    public void Draw(TextureManager textures)
    {
        TextureManager.DrawBackground(textures.MainMenuBackground, 1280, 720, Color.White);
        Raylib.DrawRectangle(0, 0, 1280, 720, new Color(12, 16, 28, 170));

        string title = "SETTINGS & AUDIO";
        int tSize = 34;
        int tWidth = Raylib.MeasureText(title, tSize);
        Raylib.DrawText(title, (1280 - tWidth) / 2, 50, tSize, ColorPalette.TextPrimary);

        int cardX = 340;
        int cardY = 110;
        int cardW = 600;
        int cardH = 480;

        Raylib.DrawRectangle(cardX, cardY, cardW, cardH, ColorPalette.CardBackground);
        Raylib.DrawRectangleLines(cardX, cardY, cardW, cardH, ColorPalette.CardBorder);

        string[] labels = {
            "Master Volume",
            "Music Volume",
            "Sound FX Volume",
            "Mute All Audio",
            "Ghost Piece Projection",
            "Screen Shake FX",
            "Dynamic Particles"
        };

        int rowY = cardY + 58;
        int rowGap = 50;

        for (int i = 0; i < labels.Length; i++)
        {
            Raylib.DrawText(labels[i], cardX + 35, rowY + 8, 18, ColorPalette.TextPrimary);
            _buttons[i].Draw();
            rowY += rowGap;
        }

        _backButton.Draw();
    }
}

