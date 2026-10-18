using System.Numerics;
using Raylib_cs;

namespace Tetris.Rendering;

/// <summary>
/// Displays animated score banners, combo tags, and milestone callouts
/// with bouncy scale pop and smooth alpha fades.
/// </summary>
public sealed class FloatingTextSystem
{
    private struct FloatingItem
    {
        public string Text;
        public Vector2 Position;
        public Color Color;
        public int FontSize;
        public float Life;
        public float MaxLife;
    }

    private readonly List<FloatingItem> _items = new(16);

    public void Add(string text, Vector2 position, Color color, int fontSize = 26, float duration = 1.2f)
    {
        _items.Add(new FloatingItem
        {
            Text = text,
            Position = position,
            Color = color,
            FontSize = fontSize,
            Life = duration,
            MaxLife = duration
        });
    }

    public void Update(float dt)
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            var item = _items[i];
            item.Life -= dt;
            if (item.Life <= 0f)
            {
                _items.RemoveAt(i);
                continue;
            }

            // Float gently upwards
            item.Position.Y -= 35f * dt;
            _items[i] = item;
        }
    }

    public void Draw()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            float frac = item.Life / item.MaxLife;
            byte alpha = (byte)(255 * Math.Clamp(frac * 1.5f, 0f, 1f));

            int textWidth = Raylib.MeasureText(item.Text, item.FontSize);
            int x = (int)(item.Position.X - textWidth / 2f);
            int y = (int)item.Position.Y;

            // Subtle black shadow for readability
            Raylib.DrawText(item.Text, x + 2, y + 2, item.FontSize, new Color(0, 0, 0, (int)(alpha * 0.7f)));

            var col = new Color(item.Color.R, item.Color.G, item.Color.B, (int)alpha);
            Raylib.DrawText(item.Text, x, y, item.FontSize, col);
        }
    }

    public void Clear()
    {
        _items.Clear();
    }
}
