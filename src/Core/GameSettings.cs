using System.Text.Json;

namespace Tetris.Core;

/// <summary>
/// User configuration settings persisted across game sessions.
/// </summary>
public sealed class GameSettings
{
    private static readonly string SettingsFilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");

    public float MasterVolume { get; set; } = 0.8f;
    public float MusicVolume { get; set; } = 0.6f;
    public float SfxVolume { get; set; } = 0.8f;
    public bool IsMuted { get; set; } = false;

    public bool GhostPieceEnabled { get; set; } = true;
    public bool ScreenShakeEnabled { get; set; } = true;
    public bool ParticlesEnabled { get; set; } = true;

    /// <summary>
    /// Delayed Auto Shift delay (milliseconds before auto-repeat begins).
    /// </summary>
    public float DasMs { get; set; } = 133f;

    /// <summary>
    /// Auto Repeat Rate (milliseconds between repeats once DAS activates).
    /// </summary>
    public float ArrMs { get; set; } = 33f;

    public static GameSettings Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var loaded = JsonSerializer.Deserialize<GameSettings>(json);
                if (loaded != null)
                {
                    return loaded;
                }
            }
        }
        catch
        {
            // Fallback to defaults on error
        }

        var defaultSettings = new GameSettings();
        defaultSettings.Save();
        return defaultSettings;
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
            // Suppress persistence errors during sandboxing
        }
    }
}

