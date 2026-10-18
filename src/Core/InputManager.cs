using Raylib_cs;

namespace Tetris.Core;

/// <summary>
/// Provides high-precision input handling featuring DAS (Delayed Auto Shift)
/// and ARR (Auto Repeat Rate) for tournament-grade tetromino control.
/// </summary>
public sealed class InputManager
{
    private readonly GameSettings _settings;

    private float _leftHoldTimer = 0f;
    private float _leftRepeatTimer = 0f;
    private float _rightHoldTimer = 0f;
    private float _rightRepeatTimer = 0f;
    private float _downHoldTimer = 0f;
    private float _downRepeatTimer = 0f;

    public InputManager(GameSettings settings)
    {
        _settings = settings;
    }

    public void Update(float deltaTime)
    {
        // Update horizontal DAS/ARR timers
        bool leftDown = Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A);
        bool rightDown = Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D);
        bool downDown = Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S);

        float dasSeconds = _settings.DasMs / 1000f;
        float arrSeconds = _settings.ArrMs / 1000f;

        if (leftDown)
        {
            _leftHoldTimer += deltaTime;
            if (_leftHoldTimer >= dasSeconds)
            {
                _leftRepeatTimer += deltaTime;
            }
        }
        else
        {
            _leftHoldTimer = 0f;
            _leftRepeatTimer = 0f;
        }

        if (rightDown)
        {
            _rightHoldTimer += deltaTime;
            if (_rightHoldTimer >= dasSeconds)
            {
                _rightRepeatTimer += deltaTime;
            }
        }
        else
        {
            _rightHoldTimer = 0f;
            _rightRepeatTimer = 0f;
        }

        if (downDown)
        {
            _downHoldTimer += deltaTime;
            _downRepeatTimer += deltaTime;
        }
        else
        {
            _downHoldTimer = 0f;
            _downRepeatTimer = 0f;
        }
    }

    public bool ShouldMoveLeft()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A))
        {
            return true;
        }

        float dasSeconds = _settings.DasMs / 1000f;
        float arrSeconds = _settings.ArrMs / 1000f;

        if (_leftHoldTimer >= dasSeconds && _leftRepeatTimer >= arrSeconds)
        {
            _leftRepeatTimer = 0f;
            return true;
        }

        return false;
    }

    public bool ShouldMoveRight()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D))
        {
            return true;
        }

        float dasSeconds = _settings.DasMs / 1000f;
        float arrSeconds = _settings.ArrMs / 1000f;

        if (_rightHoldTimer >= dasSeconds && _rightRepeatTimer >= arrSeconds)
        {
            _rightRepeatTimer = 0f;
            return true;
        }

        return false;
    }

    public bool ShouldSoftDrop()
    {
        return Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S);
    }

    public bool ShouldHardDrop()
    {
        return Raylib.IsKeyPressed(KeyboardKey.Space);
    }

    public bool ShouldRotateClockwise()
    {
        return Raylib.IsKeyPressed(KeyboardKey.Up) ||
               Raylib.IsKeyPressed(KeyboardKey.W) ||
               Raylib.IsKeyPressed(KeyboardKey.X);
    }

    public bool ShouldRotateCounterClockwise()
    {
        return Raylib.IsKeyPressed(KeyboardKey.Z) ||
               Raylib.IsKeyPressed(KeyboardKey.LeftControl);
    }

    public bool ShouldHold()
    {
        return Raylib.IsKeyPressed(KeyboardKey.C) ||
               Raylib.IsKeyPressed(KeyboardKey.LeftShift);
    }

    public bool ShouldPause()
    {
        return Raylib.IsKeyPressed(KeyboardKey.Escape) ||
               Raylib.IsKeyPressed(KeyboardKey.P);
    }

    public bool ShouldRestart()
    {
        return Raylib.IsKeyPressed(KeyboardKey.R);
    }
}

