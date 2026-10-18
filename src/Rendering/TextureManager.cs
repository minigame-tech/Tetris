using Raylib_cs;

namespace Tetris.Rendering;

/// <summary>
/// Loads, caches, and manages background images and fallback procedural visuals.
/// </summary>
public sealed class TextureManager : IDisposable
{
    public Texture2D? MainMenuBackground { get; private set; }
    public Texture2D? InGameBackground { get; private set; }
    public Texture2D? HowToPlayBackground { get; private set; }

    private bool _isDisposed = false;

    public void LoadTextures()
    {
        MainMenuBackground = TryLoadTexture("BackGroundMainMenu.jpg");
        InGameBackground = TryLoadTexture("BackGroundMain.jpg");
        HowToPlayBackground = TryLoadTexture("How_Games.jpg");
    }

    private static Texture2D? TryLoadTexture(string fileName)
    {
        string[] searchPaths = {
            Path.Combine("assets", "background", fileName),
            Path.Combine("assets", "backgrounds", fileName),
            Path.Combine(AppContext.BaseDirectory, "assets", "background", fileName),
            Path.Combine(AppContext.BaseDirectory, "assets", "backgrounds", fileName),
            Path.Combine("src", "background", fileName),
            Path.Combine(AppContext.BaseDirectory, "src", "background", fileName),
            fileName
        };

        foreach (var p in searchPaths)
        {
            if (File.Exists(p))
            {
                try
                {
                    var tex = Raylib.LoadTexture(p);
                    if (tex.Id > 0)
                    {
                        return tex;
                    }
                }
                catch
                {
                    // Continue searching
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Renders a background texture scaled to fill the entire window, or draws a sleek fallback cyber gradient.
    /// </summary>
    public static void DrawBackground(Texture2D? texture, int screenWidth, int screenHeight, Color tint)
    {
        if (texture.HasValue && texture.Value.Id > 0)
        {
            var src = new Rectangle(0, 0, texture.Value.Width, texture.Value.Height);
            var dst = new Rectangle(0, 0, screenWidth, screenHeight);
            Raylib.DrawTexturePro(texture.Value, src, dst, System.Numerics.Vector2.Zero, 0f, tint);
        }
        else
        {
            // Fallback gradient: Deep space navy to dark violet
            Raylib.DrawRectangleGradientV(0, 0, screenWidth, screenHeight,
                new Color(10, 14, 26, 255),
                new Color(22, 16, 40, 255));

            // Grid lines overlay
            for (int y = 0; y < screenHeight; y += 40)
            {
                Raylib.DrawLine(0, y, screenWidth, y, new Color(40, 60, 100, 30));
            }
            for (int x = 0; x < screenWidth; x += 40)
            {
                Raylib.DrawLine(x, 0, x, screenHeight, new Color(40, 60, 100, 30));
            }
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        if (MainMenuBackground.HasValue && MainMenuBackground.Value.Id > 0)
        {
            Raylib.UnloadTexture(MainMenuBackground.Value);
            MainMenuBackground = null;
        }

        if (InGameBackground.HasValue && InGameBackground.Value.Id > 0)
        {
            Raylib.UnloadTexture(InGameBackground.Value);
            InGameBackground = null;
        }

        if (HowToPlayBackground.HasValue && HowToPlayBackground.Value.Id > 0)
        {
            Raylib.UnloadTexture(HowToPlayBackground.Value);
            HowToPlayBackground = null;
        }
    }
}

