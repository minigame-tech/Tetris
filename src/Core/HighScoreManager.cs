using System.Text.Json;

namespace Tetris.Core;

/// <summary>
/// Records and persists high scores, lines, and speedrun records across game modes.
/// </summary>
public sealed class HighScoreManager
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "highscore.json");

    public long MarathonHighScore { get; set; } = 0;
    public int MarathonBestLines { get; set; } = 0;
    public int MarathonBestLevel { get; set; } = 1;

    public float Sprint40BestTime { get; set; } = 0f; // 0 means uncompleted
    public long Ultra2MinHighScore { get; set; } = 0;

    public static HighScoreManager Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                var loaded = JsonSerializer.Deserialize<HighScoreManager>(json);
                if (loaded != null) return loaded;
            }
        }
        catch
        {
            // Silently recover with defaults
        }

        var instance = new HighScoreManager();
        instance.Save();
        return instance;
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch
        {
            // Suppress IO errors
        }
    }

    public bool UpdateMarathon(long score, int lines, int level)
    {
        bool isNew = false;
        if (score > MarathonHighScore)
        {
            MarathonHighScore = score;
            isNew = true;
        }
        if (lines > MarathonBestLines) MarathonBestLines = lines;
        if (level > MarathonBestLevel) MarathonBestLevel = level;

        if (isNew) Save();
        return isNew;
    }

    public bool UpdateSprint(float timeSeconds)
    {
        bool isNew = false;
        if (Sprint40BestTime <= 0f || timeSeconds < Sprint40BestTime)
        {
            Sprint40BestTime = timeSeconds;
            isNew = true;
            Save();
        }
        return isNew;
    }

    public bool UpdateUltra(long score)
    {
        bool isNew = false;
        if (score > Ultra2MinHighScore)
        {
            Ultra2MinHighScore = score;
            isNew = true;
            Save();
        }
        return isNew;
    }
}

