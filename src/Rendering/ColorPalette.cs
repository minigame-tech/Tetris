using Raylib_cs;
using Tetris.Models;

namespace Tetris.Rendering;

/// <summary>
/// Defines the visual color scheme, block neon palettes, and UI accents.
/// </summary>
public static class ColorPalette
{
    // Tetromino vibrant neon colors
    public static readonly Color ColorI = new(0, 240, 245, 255);    // Bright Cyan
    public static readonly Color ColorJ = new(35, 100, 255, 255);   // Electric Blue
    public static readonly Color ColorL = new(255, 140, 10, 255);   // Vivid Orange
    public static readonly Color ColorO = new(255, 225, 25, 255);   // Sunburst Yellow
    public static readonly Color ColorS = new(25, 235, 70, 255);    // Neon Green
    public static readonly Color ColorT = new(180, 45, 255, 255);   // Cyber Magenta
    public static readonly Color ColorZ = new(255, 40, 70, 255);    // Crimson Red

    // Ghost piece colors (with transparency)
    public const byte GhostAlpha = 70;

    // UI & Board colors
    public static readonly Color BoardBackground = new(10, 14, 24, 230);
    public static readonly Color BoardGridLines = new(45, 58, 85, 90);
    public static readonly Color BoardBorder = new(80, 140, 240, 220);
    public static readonly Color BoardGlow = new(0, 200, 255, 60);

    public static readonly Color CardBackground = new(16, 22, 38, 220);
    public static readonly Color CardBorder = new(60, 95, 155, 180);
    public static readonly Color CardHeader = new(35, 50, 85, 230);

    public static readonly Color TextPrimary = new(245, 250, 255, 255);
    public static readonly Color TextSecondary = new(150, 175, 210, 255);
    public static readonly Color TextAccent = new(0, 230, 255, 255);
    public static readonly Color TextGold = new(255, 215, 0, 255);

    public static readonly Color ButtonNormal = new(28, 38, 64, 230);
    public static readonly Color ButtonHover = new(45, 70, 125, 245);
    public static readonly Color ButtonActive = new(65, 100, 180, 255);

    public static Color GetColor(TetrominoType type)
    {
        return type switch
        {
            TetrominoType.I => ColorI,
            TetrominoType.J => ColorJ,
            TetrominoType.L => ColorL,
            TetrominoType.O => ColorO,
            TetrominoType.S => ColorS,
            TetrominoType.T => ColorT,
            TetrominoType.Z => ColorZ,
            _ => new Color(0, 0, 0, 0)
        };
    }

    public static Color GetGhostColor(TetrominoType type)
    {
        var baseCol = GetColor(type);
        return new Color(baseCol.R, baseCol.G, baseCol.B, GhostAlpha);
    }
}

