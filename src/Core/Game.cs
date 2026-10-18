using System.Numerics;
using Raylib_cs;
using Tetris.Audio;
using Tetris.Models;
using Tetris.Rendering;
using Tetris.UI;

namespace Tetris.Core;

/// <summary>
/// Main game engine orchestrating game loop, state transitions, physics,
/// scoring, SRS rotation, input polling, audio, and visual rendering.
/// </summary>
public sealed class Game : IDisposable
{
    public const int WindowWidth = 1280;
    public const int WindowHeight = 720;
    public const string WindowTitle = "Tetris Neo";

    private readonly GameSettings _settings;
    private readonly HighScoreManager _highScores;
    private readonly ScoreManager _score = new();
    private readonly Board _board = new();
    private readonly BagRandomizer _randomizer = new();
    private readonly InputManager _input;
    private readonly AudioManager _audio;
    private readonly TextureManager _textures = new();
    private readonly BoardRenderer _boardRenderer = new();
    private readonly ParticleSystem _particles;
    private readonly ScreenShake _screenShake = new();
    private readonly FloatingTextSystem _floatingText = new();

    private GameState _state = GameState.MainMenu;
    private GameMode _mode = GameMode.Marathon;

    private Tetromino? _activePiece;
    private TetrominoType _holdPiece = TetrominoType.None;
    private bool _canHold = true;

    private float _gravityTimer = 0f;
    private float _lockTimer = 0f;
    private const float MaxLockDelay = 0.5f;
    private int _lockResetsRemaining = 15;

    private float _gameTime = 0f;
    private bool _isNewRecord = false;
    private bool _completedSuccessfully = false;

    // Line clear animation state
    private List<int>? _clearingLines = null;
    private float _lineClearTimer = 0f;
    private const float LineClearDuration = 0.25f;

    // Screens & Overlays
    private readonly MainMenuScreen _mainMenuScreen;
    private readonly ModeSelectScreen _modeSelectScreen;
    private readonly HowToPlayScreen _howToPlayScreen;
    private readonly HighScoresScreen _highScoresScreen;
    private readonly SettingsScreen _settingsScreen;
    private readonly PauseOverlay _pauseOverlay;
    private readonly GameOverOverlay _gameOverOverlay;

    private bool _shouldQuit = false;

    public Game()
    {
        _settings = GameSettings.Load();
        _highScores = HighScoreManager.Load();
        _input = new InputManager(_settings);
        _audio = new AudioManager(_settings);
        _particles = new ParticleSystem(WindowWidth, WindowHeight);

        // Initialize screens
        _mainMenuScreen = new MainMenuScreen(
            onPlay: () => StartGame(GameMode.Marathon),
            onModes: () => _state = GameState.ModeSelect,
            onHowToPlay: () => _state = GameState.HowToPlay,
            onHighScores: () => _state = GameState.HighScores,
            onSettings: () => _state = GameState.Settings,
            onQuit: () => _shouldQuit = true
        );

        _modeSelectScreen = new ModeSelectScreen(
            onSelectMode: StartGame,
            onBack: () => _state = GameState.MainMenu
        );

        _howToPlayScreen = new HowToPlayScreen(onBack: () => _state = GameState.MainMenu);
        _highScoresScreen = new HighScoresScreen(onBack: () => _state = GameState.MainMenu);
        _settingsScreen = new SettingsScreen(_settings, _audio, onBack: () => _state = GameState.MainMenu);

        _pauseOverlay = new PauseOverlay(
            onResume: () => _state = GameState.Playing,
            onRestart: () => StartGame(_mode),
            onSettings: () => _state = GameState.Settings,
            onMainMenu: () => _state = GameState.MainMenu
        );

        _gameOverOverlay = new GameOverOverlay(
            onRetry: () => StartGame(_mode),
            onModes: () => _state = GameState.ModeSelect,
            onMainMenu: () => _state = GameState.MainMenu
        );
    }

    public void Run()
    {
        Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint);
        Raylib.InitWindow(WindowWidth, WindowHeight, WindowTitle);
        Raylib.SetTargetFPS(60);

        _textures.LoadTextures();

        while (!Raylib.WindowShouldClose() && !_shouldQuit)
        {
            float dt = Raylib.GetFrameTime();
            if (dt > 0.1f) dt = 0.1f; // Clamp delta spikes

            Update(dt);
            Draw();
        }

        Close();
    }

    private void StartGame(GameMode mode)
    {
        _mode = mode;
        _score.Reset();
        _board.Clear();
        _randomizer.Reset();
        _holdPiece = TetrominoType.None;
        _canHold = true;
        _gameTime = 0f;
        _gravityTimer = 0f;
        _lockTimer = 0f;
        _lockResetsRemaining = 15;
        _clearingLines = null;
        _particles.Clear();
        _floatingText.Clear();
        _isNewRecord = false;
        _completedSuccessfully = false;

        SpawnNextPiece();
        _state = GameState.Playing;
    }

    private void Update(float dt)
    {
        _audio.Update();
        var mousePos = Raylib.GetMousePosition();
        bool mousePressed = Raylib.IsMouseButtonPressed(MouseButton.Left);

        switch (_state)
        {
            case GameState.MainMenu:
                _mainMenuScreen.Update(dt, mousePos, mousePressed, _audio);
                break;

            case GameState.ModeSelect:
                _modeSelectScreen.Update(dt, mousePos, mousePressed, _audio);
                break;

            case GameState.HowToPlay:
                _howToPlayScreen.Update(dt, mousePos, mousePressed, _audio);
                break;

            case GameState.HighScores:
                _highScoresScreen.Update(dt, mousePos, mousePressed, _audio);
                break;

            case GameState.Settings:
                _settingsScreen.Update(dt, mousePos, mousePressed, _audio);
                break;

            case GameState.Playing:
                UpdatePlaying(dt);
                break;

            case GameState.Paused:
                _pauseOverlay.Update(dt, mousePos, mousePressed, _audio);
                if (_input.ShouldPause())
                {
                    _state = GameState.Playing;
                }
                break;

            case GameState.GameOver:
                _gameOverOverlay.Update(dt, mousePos, mousePressed, _audio);
                if (_input.ShouldRestart())
                {
                    StartGame(_mode);
                }
                break;
        }

        if (_settings.ParticlesEnabled)
        {
            _particles.Update(dt, WindowWidth, WindowHeight);
        }
        _screenShake.Update(dt);
        _floatingText.Update(dt);
    }

    private void UpdatePlaying(float dt)
    {
        if (_input.ShouldPause())
        {
            _state = GameState.Paused;
            return;
        }

        if (_input.ShouldRestart())
        {
            StartGame(_mode);
            return;
        }

        _gameTime += dt;

        // Check Ultra 2-Minute time limit
        if (_mode == GameMode.Ultra2Min && _gameTime >= 120f)
        {
            TriggerGameOver(true);
            return;
        }

        // Handle line clear animation delay
        if (_clearingLines != null)
        {
            _lineClearTimer -= dt;
            if (_lineClearTimer <= 0f)
            {
                _board.ClearLines(_clearingLines);
                _clearingLines = null;
                SpawnNextPiece();
            }
            return;
        }

        if (_activePiece == null) return;

        _input.Update(dt);

        // 1. Hold piece
        if (_input.ShouldHold() && _canHold)
        {
            _canHold = false;
            var currentType = _activePiece.Type;
            if (_holdPiece == TetrominoType.None)
            {
                _holdPiece = currentType;
                SpawnNextPiece();
            }
            else
            {
                var nextType = _holdPiece;
                _holdPiece = currentType;
                _activePiece = new Tetromino(nextType);
            }
            _audio.PlaySound(SoundEffectType.Hold);
            _lockTimer = 0f;
            _lockResetsRemaining = 15;
            return;
        }

        // 2. Horizontal movement
        if (_input.ShouldMoveLeft())
        {
            TryMove(new Position(0, -1));
        }
        else if (_input.ShouldMoveRight())
        {
            TryMove(new Position(0, 1));
        }

        // 3. Rotation (Super Rotation System)
        if (_input.ShouldRotateClockwise())
        {
            TryRotate(true);
        }
        else if (_input.ShouldRotateCounterClockwise())
        {
            TryRotate(false);
        }

        // 4. Hard Drop
        if (_input.ShouldHardDrop())
        {
            PerformHardDrop();
            return;
        }

        // 5. Soft Drop & Gravity
        bool isSoftDropping = _input.ShouldSoftDrop();
        float gravityInterval = _score.GetGravityInterval();
        if (isSoftDropping)
        {
            gravityInterval = Math.Min(0.04f, gravityInterval / 20f);
        }

        _gravityTimer += dt;
        if (_gravityTimer >= gravityInterval)
        {
            _gravityTimer = 0f;
            if (_board.IsValidPosition(_activePiece, _activePiece.Position with { Row = _activePiece.Position.Row + 1 }, _activePiece.Rotation))
            {
                _activePiece.Position = _activePiece.Position with { Row = _activePiece.Position.Row + 1 };
                _activePiece.LastMoveWasRotation = false;
                if (isSoftDropping)
                {
                    _score.AddSoftDropPoints(1);
                    _audio.PlaySound(SoundEffectType.SoftDrop);
                }
                _lockTimer = 0f;
            }
            else
            {
                // Landed on surface
            }
        }

        // 6. Lock Delay
        bool isOnFloor = !_board.IsValidPosition(_activePiece, _activePiece.Position with { Row = _activePiece.Position.Row + 1 }, _activePiece.Rotation);
        if (isOnFloor)
        {
            _lockTimer += dt;
            if (_lockTimer >= MaxLockDelay)
            {
                LockActivePiece();
            }
        }
        else
        {
            _lockTimer = 0f;
        }
    }

    private void TryMove(Position offset)
    {
        if (_activePiece == null) return;

        var targetPos = _activePiece.Position + offset;
        if (_board.IsValidPosition(_activePiece, targetPos, _activePiece.Rotation))
        {
            _activePiece.Position = targetPos;
            _activePiece.LastMoveWasRotation = false;
            _audio.PlaySound(SoundEffectType.Move);

            ResetLockTimerOnManeuver();
        }
    }

    private void TryRotate(bool clockwise)
    {
        if (_activePiece == null) return;

        var fromRot = _activePiece.Rotation;
        var toRot = clockwise ? fromRot.RotateClockwise() : fromRot.RotateCounterClockwise();

        var kickOffsets = WallKicks.GetKickOffsets(_activePiece.Type, fromRot, toRot);

        for (int i = 0; i < kickOffsets.Length; i++)
        {
            var testPos = _activePiece.Position + kickOffsets[i];
            if (_board.IsValidPosition(_activePiece, testPos, toRot))
            {
                _activePiece.Position = testPos;
                _activePiece.Rotation = toRot;
                _activePiece.LastMoveWasRotation = true;
                _activePiece.LastRotationUsedSpecialKick = (i == 4);

                _audio.PlaySound(SoundEffectType.Rotate);
                ResetLockTimerOnManeuver();
                return;
            }
        }
    }

    private void ResetLockTimerOnManeuver()
    {
        if (_lockResetsRemaining > 0)
        {
            _lockTimer = 0f;
            _lockResetsRemaining--;
        }
    }

    private void PerformHardDrop()
    {
        if (_activePiece == null) return;

        int dropDist = _board.CalculateGhostDropDistance(_activePiece);
        _activePiece.Position = _activePiece.Position with { Row = _activePiece.Position.Row + dropDist };
        _score.AddHardDropPoints(dropDist);

        if (_settings.ScreenShakeEnabled)
        {
            _screenShake.AddTrauma(0.24f);
        }

        // Particle sparks at the bottom of the piece
        if (_settings.ParticlesEnabled)
        {
            var blocks = _activePiece.GetCurrentBlockPositions();
            int maxR = blocks.Max(b => b.Row);
            int minC = blocks.Min(b => b.Col);
            int maxC = blocks.Max(b => b.Col);

            float startX = BoardRenderer.BoardLeftX + minC * BoardRenderer.CellSize;
            float endX = BoardRenderer.BoardLeftX + (maxC + 1) * BoardRenderer.CellSize;
            float y = BoardRenderer.BoardTopY + (maxR - Board.HiddenRows + 1) * BoardRenderer.CellSize;

            _particles.SpawnHardDropSparks(startX, endX, y, ColorPalette.GetColor(_activePiece.Type));
        }

        _audio.PlaySound(SoundEffectType.HardDrop);
        LockActivePiece();
    }

    private void LockActivePiece()
    {
        if (_activePiece == null) return;

        bool isTSpin = _board.CheckTSpin(_activePiece, out bool isMini);
        _board.LockTetromino(_activePiece);

        var fullLines = _board.FindFullLines();
        int linesCount = fullLines.Count;
        int previousLevel = _score.Level;

        if (linesCount > 0)
        {
            bool isPc = _board.IsBoardEmpty();
            _score.RegisterClear(linesCount, isTSpin, isMini, isPc);

            // Trigger animations
            _clearingLines = fullLines;
            _lineClearTimer = LineClearDuration;

            if (_settings.ScreenShakeEnabled)
            {
                float shakeTrauma = 0.25f + (linesCount * 0.15f);
                if (isTSpin) shakeTrauma += 0.2f;
                _screenShake.AddTrauma(shakeTrauma);
            }

            if (_settings.ParticlesEnabled)
            {
                foreach (int row in fullLines)
                {
                    float y = BoardRenderer.BoardTopY + (row - Board.HiddenRows + 0.5f) * BoardRenderer.CellSize;
                    _particles.SpawnLineClearExplosion(BoardRenderer.BoardLeftX, y, BoardRenderer.BoardWidth, ColorPalette.GetColor(_activePiece.Type));
                }
            }

            // Floating banner
            if (!string.IsNullOrEmpty(_score.LastActionText))
            {
                var bannerCol = linesCount == 4 ? ColorPalette.TextGold : ColorPalette.TextAccent;
                _floatingText.Add($"{_score.LastActionText} +{_score.LastActionScore}",
                    new Vector2(BoardRenderer.BoardLeftX + BoardRenderer.BoardWidth / 2f, BoardRenderer.BoardTopY + 260),
                    bannerCol, 26);
            }

            // Sound effects
            if (linesCount == 4)
            {
                _audio.PlaySound(SoundEffectType.TetrisClear);
            }
            else if (isTSpin)
            {
                _audio.PlaySound(SoundEffectType.TSpin);
            }
            else
            {
                var snd = linesCount switch
                {
                    1 => SoundEffectType.LineClear1,
                    2 => SoundEffectType.LineClear2,
                    _ => SoundEffectType.LineClear3
                };
                _audio.PlaySound(snd);
            }

            // Check level up celebration
            if (_score.Level > previousLevel)
            {
                _particles.SpawnCelebration(WindowWidth, WindowHeight);
                _floatingText.Add($"LEVEL {_score.Level}!",
                    new Vector2(BoardRenderer.BoardLeftX + BoardRenderer.BoardWidth / 2f, BoardRenderer.BoardTopY + 180),
                    ColorPalette.ColorO, 32);
                _audio.PlaySound(SoundEffectType.LevelUp);
            }

            // Check Sprint 40 goal
            if (_mode == GameMode.Sprint40 && _score.Lines >= 40)
            {
                TriggerGameOver(true);
                return;
            }
        }
        else
        {
            if (isTSpin)
            {
                _score.RegisterClear(0, true, isMini, false);
                _audio.PlaySound(SoundEffectType.TSpin);
            }
            else
            {
                _score.RegisterClear(0, false, false, false);
                _audio.PlaySound(SoundEffectType.Lock);
            }
            SpawnNextPiece();
        }

        _canHold = true;
    }

    private void SpawnNextPiece()
    {
        var nextType = _randomizer.PopNext();
        var piece = new Tetromino(nextType);

        if (!_board.CanPlace(piece))
        {
            // Block out / Lock out Game Over
            TriggerGameOver(false);
            return;
        }

        _activePiece = piece;
        _gravityTimer = 0f;
        _lockTimer = 0f;
        _lockResetsRemaining = 15;
    }

    private void TriggerGameOver(bool success)
    {
        _state = GameState.GameOver;
        _completedSuccessfully = success;

        switch (_mode)
        {
            case GameMode.Sprint40:
                if (success)
                {
                    _isNewRecord = _highScores.UpdateSprint(_gameTime);
                    _audio.PlaySound(SoundEffectType.ModeComplete);
                }
                else
                {
                    _audio.PlaySound(SoundEffectType.GameOver);
                }
                break;

            case GameMode.Ultra2Min:
                _isNewRecord = _highScores.UpdateUltra(_score.Score);
                _audio.PlaySound(success ? SoundEffectType.ModeComplete : SoundEffectType.GameOver);
                break;

            default: // Marathon
                _isNewRecord = _highScores.UpdateMarathon(_score.Score, _score.Lines, _score.Level);
                _audio.PlaySound(SoundEffectType.GameOver);
                break;
        }
    }

    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(new Color(10, 14, 25, 255));

        switch (_state)
        {
            case GameState.MainMenu:
                _mainMenuScreen.Draw(_textures);
                break;

            case GameState.ModeSelect:
                _modeSelectScreen.Draw(_textures);
                break;

            case GameState.HowToPlay:
                _howToPlayScreen.Draw(_textures);
                break;

            case GameState.HighScores:
                _highScoresScreen.Draw(_textures, _highScores);
                break;

            case GameState.Settings:
                _settingsScreen.Draw(_textures);
                break;

            case GameState.Playing:
            case GameState.Paused:
            case GameState.GameOver:
                // Draw in-game backdrop from BackGroundMain.jpg
                TextureManager.DrawBackground(_textures.InGameBackground, WindowWidth, WindowHeight, new Color(210, 220, 245, 255));
                Raylib.DrawRectangle(0, 0, WindowWidth, WindowHeight, new Color(12, 16, 28, 160));

                var shakeOffset = _settings.ScreenShakeEnabled ? _screenShake.GetOffset() : Vector2.Zero;
                _boardRenderer.RenderGame(
                    _board,
                    _activePiece,
                    _holdPiece,
                    _canHold,
                    _randomizer.PeekUpcoming(4),
                    _score,
                    _highScores,
                    _mode,
                    _gameTime,
                    _settings,
                    _particles,
                    _floatingText,
                    shakeOffset,
                    _clearingLines,
                    _lineClearTimer
                );

                if (_state == GameState.Paused)
                {
                    _pauseOverlay.Draw();
                }
                else if (_state == GameState.GameOver)
                {
                    _gameOverOverlay.Draw(_score, _mode, _gameTime, _isNewRecord, _completedSuccessfully);
                }
                break;
        }

        Raylib.EndDrawing();
    }

    private void Close()
    {
        _textures.Dispose();
        _audio.Dispose();
        Raylib.CloseWindow();
    }

    public void Dispose()
    {
        Close();
    }
}
