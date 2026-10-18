using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Rendering;

namespace Tetris.UI;

/// <summary>
/// Reusable interactive UI button with hover animations, glowing borders,
/// sound effects, and click callbacks.
/// </summary>
public sealed class Button
{
    public Rectangle Bounds { get; set; }
    public string Text { get; set; }
    public int FontSize { get; set; }
    public Action? OnClick { get; set; }

    public bool IsHovered { get; private set; }
    private bool _wasHovered = false;

    public Button(Rectangle bounds, string text, Action? onClick = null, int fontSize = 20)
    {
        Bounds = bounds;
        Text = text;
        OnClick = onClick;
        FontSize = fontSize;
    }

    public void Update(Vector2 mousePos, bool mousePressed, AudioManager? audio = null)
    {
        IsHovered = Raylib.CheckCollisionPointRec(mousePos, Bounds);

        if (IsHovered && !_wasHovered)
        {
            audio?.PlaySound(SoundEffectType.ButtonHover);
        }
        _wasHovered = IsHovered;

        if (IsHovered && mousePressed)
        {
            audio?.PlaySound(SoundEffectType.ButtonClick);
            OnClick?.Invoke();
        }
    }

    public void Draw()
    {
        var col = IsHovered ? ColorPalette.ButtonHover : ColorPalette.ButtonNormal;
        var borderCol = IsHovered ? ColorPalette.TextAccent : ColorPalette.CardBorder;

        // Button background
        Raylib.DrawRectangleRec(Bounds, col);

        // Hover outer glow
        if (IsHovered)
        {
            Raylib.DrawRectangleLinesEx(new Rectangle(Bounds.X - 2, Bounds.Y - 2, Bounds.Width + 4, Bounds.Height + 4), 2f, ColorPalette.BoardGlow);
        }

        // Button border
        Raylib.DrawRectangleLinesEx(Bounds, IsHovered ? 2f : 1f, borderCol);

        // Button text centered
        int textWidth = Raylib.MeasureText(Text, FontSize);
        int tx = (int)(Bounds.X + (Bounds.Width - textWidth) / 2);
        int ty = (int)(Bounds.Y + (Bounds.Height - FontSize) / 2);

        var textCol = IsHovered ? ColorPalette.TextAccent : ColorPalette.TextPrimary;
        Raylib.DrawText(Text, tx, ty, FontSize, textCol);
    }
}

