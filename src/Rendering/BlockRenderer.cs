using Raylib_cs;
using Tetris.Models;

namespace Tetris.Rendering;

/// <summary>
/// Renders high-end 3D-beveled, embossed, and glowing arcade blocks.
/// </summary>
public static class BlockRenderer
{
    public static void DrawBlock(int x, int y, int size, TetrominoType type, float alpha = 1.0f, bool isFlashing = false)
    {
        if (type == TetrominoType.None) return;

        int pad = 1;
        int bx = x + pad;
        int by = y + pad;
        int bsize = size - (pad * 2);

        if (isFlashing)
        {
            // Flash pure bright white during line clear animation
            Raylib.DrawRectangle(bx, by, bsize, bsize, new Color(255, 255, 255, (int)(255 * alpha)));
            return;
        }

        var baseColor = ColorPalette.GetColor(type);
        byte a = (byte)(255 * Math.Clamp(alpha, 0f, 1f));

        // Base background
        var color = new Color(baseColor.R, baseColor.G, baseColor.B, a);
        Raylib.DrawRectangle(bx, by, bsize, bsize, color);

        // Top & Left highlight bevel
        int bevel = Math.Max(2, bsize / 7);
        var highlight = new Color(255, 255, 255, (int)(110 * alpha));
        Raylib.DrawRectangle(bx, by, bsize, bevel, highlight);
        Raylib.DrawRectangle(bx, by, bevel, bsize, highlight);

        // Bottom & Right shadow bevel
        var shadow = new Color(0, 0, 0, (int)(110 * alpha));
        Raylib.DrawRectangle(bx, by + bsize - bevel, bsize, bevel, shadow);
        Raylib.DrawRectangle(bx + bsize - bevel, by, bevel, bsize, shadow);

        // Inner jewel / glossy core
        int innerInset = bevel + 2;
        int innerSize = bsize - (innerInset * 2);
        if (innerSize > 2)
        {
            var innerColor = new Color(
                (int)Math.Min(255, baseColor.R + 45),
                (int)Math.Min(255, baseColor.G + 45),
                (int)Math.Min(255, baseColor.B + 45),
                (int)(180 * alpha)
            );
            Raylib.DrawRectangle(bx + innerInset, by + innerInset, innerSize, innerSize, innerColor);
        }

        // Crisp dark outer border
        var outline = new Color(15, 20, 30, (int)(180 * alpha));
        Raylib.DrawRectangleLines(bx, by, bsize, bsize, outline);
    }

    /// <summary>
    /// Renders a ghost projection block with futuristic neon wireframes and faint translucent fill.
    /// </summary>
    public static void DrawGhostBlock(int x, int y, int size, TetrominoType type)
    {
        if (type == TetrominoType.None) return;

        int pad = 2;
        int bx = x + pad;
        int by = y + pad;
        int bsize = size - (pad * 2);

        var baseCol = ColorPalette.GetColor(type);

        // Soft translucent body
        var fillCol = new Color(baseCol.R, baseCol.G, baseCol.B, (byte)35);
        Raylib.DrawRectangle(bx, by, bsize, bsize, fillCol);

        // Outer glowing wireframe
        var wireCol = new Color(baseCol.R, baseCol.G, baseCol.B, (byte)160);
        Raylib.DrawRectangleLines(bx, by, bsize, bsize, wireCol);

        // Accent corner dots for high-tech HUD look
        int dotSize = Math.Max(2, bsize / 6);
        Raylib.DrawRectangle(bx, by, dotSize, dotSize, wireCol);
        Raylib.DrawRectangle(bx + bsize - dotSize, by, dotSize, dotSize, wireCol);
        Raylib.DrawRectangle(bx, by + bsize - dotSize, dotSize, dotSize, wireCol);
        Raylib.DrawRectangle(bx + bsize - dotSize, by + bsize - dotSize, dotSize, dotSize, wireCol);
    }
}
