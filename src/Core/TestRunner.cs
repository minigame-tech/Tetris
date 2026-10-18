using Tetris.Audio;
using Tetris.Models;

namespace Tetris.Core;

/// <summary>
/// Built-in self-test verification suite validating game mechanics, SRS kicks,
/// bag randomizer fairness, board collisions, scoring, and procedural audio synthesis.
/// Can be invoked with the '--test' CLI argument in CI or headless environments.
/// </summary>
public static class TestRunner
{
    public static int RunAllTests()
    {
        Console.WriteLine("==================================================");
        Console.WriteLine("        TETRIS NEO - INTEGRATION SUITE            ");
        Console.WriteLine("==================================================");

        int passed = 0;
        int failed = 0;

        void Assert(string testName, bool condition)
        {
            if (condition)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("  [PASS] ");
                Console.ResetColor();
                Console.WriteLine(testName);
                passed++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("  [FAIL] ");
                Console.ResetColor();
                Console.WriteLine(testName);
                failed++;
            }
        }

        // Test 1: 7-Bag Randomizer Fairness
        var randomizer = new BagRandomizer(42);
        bool bagsValid = true;
        for (int b = 0; b < 10; b++)
        {
            var bag = new HashSet<TetrominoType>();
            for (int i = 0; i < 7; i++)
            {
                bag.Add(randomizer.PopNext());
            }
            if (bag.Count != 7 || bag.Contains(TetrominoType.None))
            {
                bagsValid = false;
                break;
            }
        }
        Assert("7-Bag Randomizer guarantees uniform distribution across 10 bags", bagsValid);

        // Test 2: Rotation Cycles
        var r = RotationState.Spawn;
        r = r.RotateClockwise();
        bool rotCwValid = r == RotationState.Right;
        r = r.RotateClockwise();
        rotCwValid &= r == RotationState.Two;
        r = r.RotateClockwise();
        rotCwValid &= r == RotationState.Left;
        r = r.RotateClockwise();
        rotCwValid &= r == RotationState.Spawn;
        Assert("RotationState clockwise cycle (0 -> R -> 2 -> L -> 0)", rotCwValid);

        // Test 3: Wall Kicks SRS Tables
        var jKicks = WallKicks.GetKickOffsets(TetrominoType.J, RotationState.Spawn, RotationState.Right);
        var iKicks = WallKicks.GetKickOffsets(TetrominoType.I, RotationState.Spawn, RotationState.Right);
        var oKicks = WallKicks.GetKickOffsets(TetrominoType.O, RotationState.Spawn, RotationState.Right);
        Assert("SRS J kick table contains 5 test offsets", jKicks.Length == 5);
        Assert("SRS I kick table contains 5 specialized offsets", iKicks.Length == 5);
        Assert("SRS O piece uses 1 zero offset (no kicks)", oKicks.Length == 1 && oKicks[0] == Position.Zero);

        // Test 4: Board Collision and Bounds
        var board = new Board();
        board.Clear();
        var tPiece = new Tetromino(TetrominoType.T);
        Assert("Empty board permits standard spawn placement", board.IsValidPosition(tPiece, new Position(0, 3), RotationState.Spawn));
        Assert("Out of bounds left rejected", !board.IsValidPosition(tPiece, new Position(0, -2), RotationState.Spawn));
        Assert("Out of bounds right rejected", !board.IsValidPosition(tPiece, new Position(0, 9), RotationState.Spawn));
        Assert("Out of bounds bottom rejected", !board.IsValidPosition(tPiece, new Position(22, 3), RotationState.Spawn));

        // Test 5: Ghost Drop Distance
        tPiece.Position = new Position(0, 3);
        int ghostDist = board.CalculateGhostDropDistance(tPiece);
        Assert("Ghost drop distance lands above bottom floor", ghostDist == 20);

        // Test 6: Line Clearing Mechanics
        board.Clear();
        for (int c = 0; c < Board.Width; c++)
        {
            board[21, c] = TetrominoType.I;
        }
        var fullLines = board.FindFullLines();
        Assert("Full row detected at row 21", fullLines.Count == 1 && fullLines[0] == 21);

        board.ClearLines(fullLines);
        Assert("Line cleared completely leaves board empty", board.IsBoardEmpty());

        // Test 7: Scoring System & Back-to-Back
        var score = new ScoreManager();
        score.Reset();
        score.RegisterClear(4, false, false, false); // First Tetris
        Assert("Tetris clear awards 800 base points at Level 1", score.Score == 800 && score.BackToBack);

        score.RegisterClear(4, false, false, false); // B2B Tetris (+1200 B2B points + 50 combo bonus)
        Assert("Back-to-Back Tetris awards 1.5x bonus (1200 points + 50 combo)", score.Score == 2050);

        // Test 8: Level Progression
        score.Reset();
        score.RegisterClear(10, false, false, false);
        Assert("Clearing 10 lines advances game to Level 2", score.Level == 2);

        // Test 9: Audio Waveform Generation
        byte[] moveSound = SoundGenerator.GenerateSound(SoundEffectType.Move);
        byte[] tetrisFanfare = SoundGenerator.GenerateSound(SoundEffectType.TetrisClear);
        byte[] musicLoop = SoundGenerator.GenerateBackgroundMusicLoop();
        Assert("Procedural move sound generates valid WAV header (>= 44 bytes)", moveSound.Length >= 44);
        Assert("Procedural Tetris fanfare generates multi-note PCM buffer", tetrisFanfare.Length > 1000);
        Assert("Procedural music loop generates multi-bar melody buffer", musicLoop.Length > 10000);

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine($"Tests executed: {passed + failed} | Passed: {passed} | Failed: {failed}");
        Console.WriteLine("==================================================");

        return failed == 0 ? 0 : 1;
    }
}
