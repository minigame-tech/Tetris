using System.Numerics;
using Raylib_cs;
using Tetris.Core;
using Tetris.Models;

namespace Tetris.Rendering;

/// <summary>
/// Orchestrates the in-game scene: the 10x20 matrix, HUD cards, hold box,
/// next queue, ghost piece projection, animations, and overlays.
/// </summary>
public sealed class BoardRenderer
{
    public const int CellSize = 30;
    public const int BoardLeftX = 490;
    public const int BoardTopY = 60;
    public const int BoardWidth = Board.Width * CellSize;          // 300 px
    public const int BoardHeight = Board.VisibleRows * CellSize;    // 600 px

    private const int SideCardWidth = 200;
    private const int LeftCardX = BoardLeftX - SideCardWidth - 30; // 260 px
    private const int RightCardX = BoardLeftX + BoardWidth + 30;   // 820 px
    private const int CardY = 60;
    private const int CardHeight = 600;

    public void RenderGame(
        Board board,
        Tetromino? activePiece,
        TetrominoType holdPiece,
        bool canHold,
        IReadOnlyList<TetrominoType> nextQueue,
        ScoreManager score,
        HighScoreManager highScores,
        GameMode mode,
        float gameTime,
        GameSettings settings,
        ParticleSystem particles,
        FloatingTextSystem floatingText,
        Vector2 screenShakeOffset,
        IReadOnlyList<int>? clearingLines,
        float lineClearAnimTimer)
    {
        // 1. Board Background card & glow
        int bx = BoardLeftX + (int)screenShakeOffset.X;
        int by = BoardTopY + (int)screenShakeOffset.Y;

        // Subtle outer neon glow
        Raylib.DrawRectangle(bx - 6, by - 6, BoardWidth + 12, BoardHeight + 12, ColorPalette.BoardGlow);
        Raylib.DrawRectangle(bx, by, BoardWidth, BoardHeight, ColorPalette.BoardBackground);

        // Grid lines
        for (int r = 1; r < Board.VisibleRows; r++)
        {
            Raylib.DrawLine(bx, by + r * CellSize, bx + BoardWidth, by + r * CellSize, ColorPalette.BoardGridLines);
        }
        for (int c = 1; c < Board.Width; c++)
        {
            Raylib.DrawLine(bx + c * CellSize, by, bx + c * CellSize, by + BoardHeight, ColorPalette.BoardGridLines);
        }

        // 2. Render locked cells
        for (int r = Board.HiddenRows; r < Board.TotalRows; r++)
        {
            int drawY = by + (r - Board.HiddenRows) * CellSize;
            bool isClearing = clearingLines != null && clearingLines.Contains(r);

            for (int c = 0; c < Board.Width; c++)
            {
                var type = board[r, c];
                if (type != TetrominoType.None)
                {
                    int drawX = bx + c * CellSize;
                    if (isClearing)
                    {
                        // Strobe white flash
                        float flash = MathF.Sin(lineClearAnimTimer * 30f) * 0.5f + 0.5f;
                        BlockRenderer.DrawBlock(drawX, drawY, CellSize, type, 1.0f, flash > 0.3f);
                    }
                    else
                    {
                        BlockRenderer.DrawBlock(drawX, drawY, CellSize, type);
                    }
                }
            }
        }

        // 3. Render Ghost piece (if enabled and piece active)
        if (activePiece != null && settings.GhostPieceEnabled && (clearingLines == null || clearingLines.Count == 0))
        {
            var ghostPos = board.GetGhostPosition(activePiece);
            var ghostBlocks = activePiece.GetBlockPositions(ghostPos, activePiece.Rotation);
            foreach (var b in ghostBlocks)
            {
                if (b.Row >= Board.HiddenRows && b.Row < Board.TotalRows)
                {
                    int gx = bx + b.Col * CellSize;
                    int gy = by + (b.Row - Board.HiddenRows) * CellSize;
                    BlockRenderer.DrawGhostBlock(gx, gy, CellSize, activePiece.Type);
                }
            }
        }

        // 4. Render Active falling piece
        if (activePiece != null && (clearingLines == null || clearingLines.Count == 0))
        {
            var blocks = activePiece.GetCurrentBlockPositions();
            foreach (var b in blocks)
            {
                if (b.Row >= Board.HiddenRows && b.Row < Board.TotalRows)
                {
                    int px = bx + b.Col * CellSize;
                    int py = by + (b.Row - Board.HiddenRows) * CellSize;
                    BlockRenderer.DrawBlock(px, py, CellSize, activePiece.Type);
                }
            }
        }

        // Crisp neon matrix frame border
        Raylib.DrawRectangleLinesEx(new Rectangle(bx - 2, by - 2, BoardWidth + 4, BoardHeight + 4), 2f, ColorPalette.BoardBorder);

        // 5. Left Card (Hold & Stats)
        RenderLeftCard(LeftCardX, CardY, SideCardWidth, CardHeight, holdPiece, canHold, score, mode, gameTime);

        // 6. Right Card (Next Queue & Scores)
        RenderRightCard(RightCardX, CardY, SideCardWidth, CardHeight, nextQueue, score, highScores, mode);

        // 7. Dynamic Overlays (Particles & Floating Text)
        particles.Draw();
        floatingText.Draw();
    }

    private void RenderLeftCard(int x, int y, int width, int height, TetrominoType hold, bool canHold, ScoreManager score, GameMode mode, float gameTime)
    {
        // Glass panel card
        Raylib.DrawRectangle(x, y, width, height, ColorPalette.CardBackground);
        Raylib.DrawRectangleLines(x, y, width, height, ColorPalette.CardBorder);

        // Header: HOLD
        Raylib.DrawRectangle(x, y, width, 36, ColorPalette.CardHeader);
        Raylib.DrawText("HOLD", x + 72, y + 9, 18, ColorPalette.TextPrimary);

        // Hold Box
        int holdBoxX = x + 20;
        int holdBoxY = y + 50;
        int holdBoxSize = 160;
        Raylib.DrawRectangle(holdBoxX, holdBoxY, holdBoxSize, 110, new Color(10, 14, 26, 180));
        Raylib.DrawRectangleLines(holdBoxX, holdBoxY, holdBoxSize, 110, canHold ? ColorPalette.CardBorder : new Color(120, 60, 60, 180));

        if (hold != TetrominoType.None)
        {
            DrawTetrominoPreview(hold, holdBoxX + holdBoxSize / 2, holdBoxY + 55, 24, canHold ? 1.0f : 0.45f);
        }

        // Mode Badge
        int curY = y + 185;
        Raylib.DrawText("GAME MODE", x + 20, curY, 14, ColorPalette.TextSecondary);
        curY += 20;
        string modeStr = mode switch
        {
            GameMode.Sprint40 => "SPRINT 40L",
            GameMode.Ultra2Min => "ULTRA 2 MIN",
            _ => "MARATHON"
        };
        Raylib.DrawText(modeStr, x + 20, curY, 18, ColorPalette.TextAccent);

        // Level
        curY += 45;
        Raylib.DrawText("LEVEL", x + 20, curY, 14, ColorPalette.TextSecondary);
        curY += 20;
        Raylib.DrawText(score.Level.ToString(), x + 20, curY, 26, ColorPalette.TextPrimary);

        // Lines Cleared
        curY += 50;
        Raylib.DrawText("LINES", x + 20, curY, 14, ColorPalette.TextSecondary);
        curY += 20;
        string lineStr = mode == GameMode.Sprint40 ? $"{score.Lines} / 40" : score.Lines.ToString();
        Raylib.DrawText(lineStr, x + 20, curY, 24, ColorPalette.TextPrimary);

        // Time
        curY += 50;
        Raylib.DrawText("TIME", x + 20, curY, 14, ColorPalette.TextSecondary);
        curY += 20;
        float displayTime = mode == GameMode.Ultra2Min ? Math.Max(0f, 120f - gameTime) : gameTime;
        int minutes = (int)displayTime / 60;
        int seconds = (int)displayTime % 60;
        int hundredths = (int)((displayTime - (int)displayTime) * 100);
        string timeStr = $"{minutes:00}:{seconds:00}.{hundredths:00}";
        Raylib.DrawText(timeStr, x + 20, curY, 20, ColorPalette.TextAccent);

        // Controls reminder
        curY = y + height - 100;
        Raylib.DrawText("CONTROLS:", x + 20, curY, 12, ColorPalette.TextSecondary);
        Raylib.DrawText("Arrows / WASD : Move/Rot", x + 20, curY + 18, 11, ColorPalette.TextSecondary);
        Raylib.DrawText("Space : Hard Drop", x + 20, curY + 34, 11, ColorPalette.TextSecondary);
        Raylib.DrawText("C / Shift : Hold", x + 20, curY + 50, 11, ColorPalette.TextSecondary);
        Raylib.DrawText("Esc / P : Pause", x + 20, curY + 66, 11, ColorPalette.TextSecondary);
    }

    private void RenderRightCard(int x, int y, int width, int height, IReadOnlyList<TetrominoType> nextQueue, ScoreManager score, HighScoreManager highScores, GameMode mode)
    {
        // Glass panel card
        Raylib.DrawRectangle(x, y, width, height, ColorPalette.CardBackground);
        Raylib.DrawRectangleLines(x, y, width, height, ColorPalette.CardBorder);

        // Header: NEXT
        Raylib.DrawRectangle(x, y, width, 36, ColorPalette.CardHeader);
        Raylib.DrawText("NEXT", x + 75, y + 9, 18, ColorPalette.TextPrimary);

        // Next Queue Display (show next 4 pieces)
        int previewY = y + 50;
        int previewCount = Math.Min(4, nextQueue.Count);
        for (int i = 0; i < previewCount; i++)
        {
            var type = nextQueue[i];
            int boxY = previewY + (i * 72);
            DrawTetrominoPreview(type, x + width / 2, boxY + 32, 20, 1.0f);
        }

        // Score Section
        int curY = y + 360;
        Raylib.DrawText("SCORE", x + 20, curY, 14, ColorPalette.TextSecondary);
        curY += 22;
        Raylib.DrawText(score.Score.ToString("N0"), x + 20, curY, 24, ColorPalette.TextPrimary);

        // High Score
        curY += 45;
        Raylib.DrawText("BEST RECORD", x + 20, curY, 14, ColorPalette.TextSecondary);
        curY += 22;
        string recordStr = mode switch
        {
            GameMode.Sprint40 => highScores.Sprint40BestTime > 0 ? $"{highScores.Sprint40BestTime:0.00}s" : "--",
            GameMode.Ultra2Min => highScores.Ultra2MinHighScore.ToString("N0"),
            _ => highScores.MarathonHighScore.ToString("N0")
        };
        Raylib.DrawText(recordStr, x + 20, curY, 20, ColorPalette.TextGold);

        // Combo & B2B indicators
        curY += 50;
        if (score.Combo > 0)
        {
            Raylib.DrawText($"COMBO x{score.Combo}!", x + 20, curY, 18, ColorPalette.TextAccent);
            curY += 26;
        }

        if (score.BackToBack)
        {
            Raylib.DrawRectangle(x + 20, curY, 120, 24, new Color(255, 140, 0, 160));
            Raylib.DrawText("B2B ACTIVE", x + 28, curY + 5, 13, ColorPalette.TextPrimary);
        }
    }

    private static void DrawTetrominoPreview(TetrominoType type, int centerX, int centerY, int blockSize, float alpha)
    {
        var dummy = new Tetromino(type);
        var localShape = dummy.GetLocalShape(RotationState.Spawn);

        // Calculate bounding box center
        int minR = int.MaxValue, maxR = int.MinValue;
        int minC = int.MaxValue, maxC = int.MinValue;
        foreach (var p in localShape)
        {
            if (p.Row < minR) minR = p.Row;
            if (p.Row > maxR) maxR = p.Row;
            if (p.Col < minC) minC = p.Col;
            if (p.Col > maxC) maxC = p.Col;
        }

        float shapeWidth = (maxC - minC + 1) * blockSize;
        float shapeHeight = (maxR - minR + 1) * blockSize;

        float originX = centerX - (shapeWidth / 2f) - (minC * blockSize);
        float originY = centerY - (shapeHeight / 2f) - (minR * blockSize);

        foreach (var p in localShape)
        {
            int bx = (int)(originX + p.Col * blockSize);
            int by = (int)(originY + p.Row * blockSize);
            BlockRenderer.DrawBlock(bx, by, blockSize, type, alpha);
        }
    }
}

