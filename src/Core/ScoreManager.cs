namespace Tetris.Core;

/// <summary>
/// Manages gameplay statistics, points, level advancement, combo streaks,
/// back-to-back multipliers, and score event notifications.
/// </summary>
public sealed class ScoreManager
{
    public long Score { get; private set; }
    public int Lines { get; private set; }
    public int Level { get; private set; } = 1;
    public int Combo { get; private set; } = -1; // -1 means no combo active
    public bool BackToBack { get; private set; } = false;

    public string LastActionText { get; private set; } = string.Empty;
    public int LastActionScore { get; private set; } = 0;

    public void Reset(int startingLevel = 1)
    {
        Score = 0;
        Lines = 0;
        Level = Math.Max(1, startingLevel);
        Combo = -1;
        BackToBack = false;
        LastActionText = string.Empty;
        LastActionScore = 0;
    }

    public void AddSoftDropPoints(int cells)
    {
        Score += cells * 1;
    }

    public void AddHardDropPoints(int cells)
    {
        Score += cells * 2;
    }

    /// <summary>
    /// Processes a line clear or T-spin lock and updates score, level, and combos according to Tetris Guidelines.
    /// </summary>
    public void RegisterClear(int linesCleared, bool isTSpin, bool isTSpinMini, bool isPerfectClear)
    {
        int basePoints = 0;
        string actionName = string.Empty;
        bool isDifficultClear = false;

        if (isTSpin)
        {
            if (isTSpinMini)
            {
                switch (linesCleared)
                {
                    case 0:
                        basePoints = 100 * Level;
                        actionName = "T-SPIN MINI";
                        break;
                    case 1:
                        basePoints = 200 * Level;
                        actionName = "T-SPIN MINI SINGLE";
                        isDifficultClear = true;
                        break;
                    case 2:
                        basePoints = 400 * Level;
                        actionName = "T-SPIN MINI DOUBLE";
                        isDifficultClear = true;
                        break;
                }
            }
            else
            {
                switch (linesCleared)
                {
                    case 0:
                        basePoints = 400 * Level;
                        actionName = "T-SPIN";
                        break;
                    case 1:
                        basePoints = 800 * Level;
                        actionName = "T-SPIN SINGLE";
                        isDifficultClear = true;
                        break;
                    case 2:
                        basePoints = 1200 * Level;
                        actionName = "T-SPIN DOUBLE";
                        isDifficultClear = true;
                        break;
                    case 3:
                        basePoints = 1600 * Level;
                        actionName = "T-SPIN TRIPLE";
                        isDifficultClear = true;
                        break;
                }
            }
        }
        else
        {
            switch (linesCleared)
            {
                case 1:
                    basePoints = 100 * Level;
                    actionName = "SINGLE";
                    break;
                case 2:
                    basePoints = 300 * Level;
                    actionName = "DOUBLE";
                    break;
                case 3:
                    basePoints = 500 * Level;
                    actionName = "TRIPLE";
                    break;
                case 4:
                    basePoints = 800 * Level;
                    actionName = "TETRIS!";
                    isDifficultClear = true;
                    break;
            }
        }

        // Apply Back-to-Back bonus if applicable
        if (isDifficultClear)
        {
            if (BackToBack)
            {
                basePoints = (int)(basePoints * 1.5f);
                actionName = "B2B " + actionName;
            }
            BackToBack = true;
        }
        else if (linesCleared > 0)
        {
            BackToBack = false;
        }

        // Combo calculations
        if (linesCleared > 0)
        {
            Combo++;
            if (Combo > 0)
            {
                int comboBonus = 50 * Combo * Level;
                basePoints += comboBonus;
            }
        }
        else if (!isTSpin)
        {
            Combo = -1;
        }

        // Perfect Clear bonus
        if (isPerfectClear && linesCleared > 0)
        {
            int pcBonus = linesCleared switch
            {
                1 => 800 * Level,
                2 => 1200 * Level,
                3 => 1800 * Level,
                4 => BackToBack ? 3200 * Level : 2000 * Level,
                _ => 1000 * Level
            };
            basePoints += pcBonus;
            actionName = "PERFECT CLEAR! " + actionName;
        }

        Score += basePoints;
        Lines += linesCleared;
        LastActionScore = basePoints;
        LastActionText = actionName;

        // Advance level every 10 lines
        int newLevel = 1 + (Lines / 10);
        if (newLevel > Level)
        {
            Level = newLevel;
        }
    }

    /// <summary>
    /// Gets the gravity fall delay in seconds based on current level.
    /// Uses the modern guideline curve.
    /// </summary>
    public float GetGravityInterval()
    {
        // Standard guideline formula: (0.8 - ((level - 1) * 0.007))^(level - 1)
        double baseVal = 0.8 - ((Level - 1) * 0.007);
        if (baseVal < 0.1) baseVal = 0.1;
        double seconds = Math.Pow(baseVal, Level - 1);
        return Math.Max(0.025f, (float)seconds);
    }
}

