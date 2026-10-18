# Tetris Neo 🕹️

[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4.svg?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![Raylib-cs](https://img.shields.io/badge/Raylib--cs-v8.1.0-red.svg?style=flat&logo=raylib)](https://github.com/ChrisDill/Raylib-cs)
[![SRS Compliant](https://img.shields.io/badge/SRS-Tetris%20Guideline%20Compliant-00F0F0.svg?style=flat)](#super-rotation-system-srs)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Linux%20%7C%20Windows%20%7C%20macOS-brightgreen.svg)](#cross-platform-support)

**Tetris Neo** is an arcade-quality, modern Tetris clone built in **C# 12** and **.NET 8** leveraging the high-performance [Raylib-cs](https://github.com/ChrisDill/Raylib-cs) binding. Engineered to adhere strictly to the official **Tetris Guideline Standard**, it features tournament-grade **Super Rotation System (SRS)** wall kicks, the **7-bag randomizer**, high-precision **DAS/ARR** input polling, procedural real-time 16-bit audio synthesis, and modern visual polish including screen trauma physics, dynamic particle explosions, and glassmorphic UI panels.

---

## 🌟 Key Features

### 🎮 Tournament-Grade Mechanics
- **Authentic Super Rotation System (SRS)**: Complete implementation of official wall kick tables for both standard tetrominoes ($J, L, S, T, Z$) and the specialized 4×4 bounding box kick table for the $I$-piece.
- **7-Bag Randomizer**: Fair, drought-free piece generation using the Fisher-Yates shuffle algorithm.
- **Extended Placement Lock Delay**: $0.5\,\text{s}$ grace period with up to 15 reset maneuvers (move/rotate) before locking.
- **T-Spin Recognition**: Mathematical 3-corner evaluation algorithm detecting both standard **T-Spins** and **T-Spin Minis**.
- **Hold Queue**: Dedicated 1-swap buffer per drop cycle.
- **Ghost Projection**: Translucent neon projection guide pinpointing exact hard drop coordinates.
- **Next Queue**: Multi-piece foresight displaying upcoming queue tetrominoes.

### 🏆 Diverse Game Modes
1. **Marathon Mode**: Classic endless endurance survival. Gravity speeds up dynamically every 10 lines cleared.
2. **Sprint (40 Lines) Mode**: High-octane speedrun attack. Clear 40 lines as fast as humanly possible with millisecond-precision timing.
3. **Ultra (2 Minutes) Mode**: Timed score trial. Maximize point accumulation within 120 seconds through Back-to-Back Tetris clears and T-Spins.

### 🎨 Visuals & Aesthetics
- **Beveled 3D Jewel Blocks**: Specular highlight emboss, ambient occlusion bevels, and vibrant neon jewel centers.
- **Dynamic Particle Engine**: Procedural landing dust bursts, line clear explosions matching block colors, celebration fireworks, and ambient starfield drift.
- **Camera Trauma / Screen Shake**: Directional non-linear screen shake ($\text{Offset} \propto \text{Trauma}^2$) on hard drops and multi-line clears.
- **Glassmorphic UI**: Semi-transparent dark carbon panels with glowing cyan accents.
- **Interactive Menu Suite**: Main Menu, Mode Select, How to Play, High Scores Hall of Fame, In-Game Pause, and Game Over modals.

### 🔊 Pure Procedural Audio Architecture
- **Zero Asset Dependencies for Audio**: Generates crystal-clear 16-bit 44.1 kHz PCM audio directly in memory via mathematical waveform synthesis (Sine, Triangle, Square, White Noise, and ADSR envelopes).
- **Sound FX**: Crisp movement blips, rotational chirps, deep sub-bass hard drop thuds, harmonized line clear chords, and triumphant Tetris fanfares.
- **Procedural Background Soundtrack**: Integrated chiptune/synthwave arpeggiated melodic loop running seamlessly in the background.
- **Audio Controls**: Master, Music, SFX volume sliders, and instant mute toggle with persistence.

---

## 🏗️ Architecture & Project Structure

The project follows a clean, decoupled domain-driven design:

```
Tetris/
├── assets/
│   └── backgrounds/            # HD background images (Main, Menu, HowToPlay)
├── docs/
│   ├── ARCHITECTURE.md         # Technical architectural design document
│   ├── SRS_SPECIFICATION.md    # Super Rotation System & Wall Kicks specification
│   └── CONTROLS.md             # Keybinds & DAS/ARR input tuning
├── src/
│   ├── Audio/
│   │   ├── AudioManager.cs     # Audio lifecycle, playback, and volume control
│   │   ├── SoundGenerator.cs   # In-memory 16-bit PCM waveform synthesis
│   │   └── SoundEffectType.cs  # Audio event enumeration
│   ├── Core/
│   │   ├── Game.cs             # Main engine coordinator & game loop
│   │   ├── GameState.cs        # High-level application state machine
│   │   ├── GameMode.cs         # Marathon, Sprint40, Ultra2Min definitions
│   │   ├── GameSettings.cs     # Configuration persistence (settings.json)
│   │   ├── ScoreManager.cs     # Guideline scoring, B2B streaks, level curves
│   │   ├── HighScoreManager.cs # Hall of Fame persistence (highscore.json)
│   │   ├── InputManager.cs     # DAS / ARR precision input polling
│   │   └── TestRunner.cs       # Built-in headless verification test suite
│   ├── Models/
│   │   ├── Board.cs            # 10x22 matrix, collision tests, line clearing
│   │   ├── Tetromino.cs        # SRS shape offsets and rotation states
│   │   ├── TetrominoType.cs    # I, J, L, O, S, T, Z pieces enum
│   │   ├── RotationState.cs    # 0, R, 2, L rotation cycle enum
│   │   ├── Position.cs         # 2D discrete grid coordinates (Row, Col)
│   │   ├── WallKicks.cs        # Standard JLSTZ and I-piece kick tables
│   │   └── BagRandomizer.cs    # 7-bag randomizer generator
│   ├── Rendering/
│   │   ├── BoardRenderer.cs    # In-game matrix and HUD card layout renderer
│   │   ├── BlockRenderer.cs    # 3D beveled block & ghost projection drawing
│   │   ├── ColorPalette.cs     # Neon color definitions & UI palettes
│   │   ├── ParticleSystem.cs   # Particle emission and lifecycle engine
│   │   ├── ScreenShake.cs      # Quadratic camera trauma simulator
│   │   ├── FloatingTextSystem.cs# Animated score and combo popups
│   │   └── TextureManager.cs   # Asset loader with procedural fallbacks
│   ├── UI/
│   │   ├── Button.cs           # Interactive UI button with audio & glow
│   │   ├── MainMenuScreen.cs   # Title screen with neon branding
│   │   ├── ModeSelectScreen.cs # Mode selection cards
│   │   ├── HowToPlayScreen.cs  # Illustrated rules and controls screen
│   │   ├── HighScoresScreen.cs # Records breakdown display
│   │   ├── SettingsScreen.cs   # Visual and audio configuration screen
│   │   ├── PauseOverlay.cs     # Translucent pause menu
│   │   └── GameOverOverlay.cs  # Game summary & record notification modal
│   └── Program.cs              # Entry point with CLI flags
├── .gitignore
├── CONTRIBUTING.md
├── LICENSE
├── nuget.config
└── Tetris.csproj
```

---

## 🕹️ Controls

| Action | Primary Key | Secondary Key |
| :--- | :--- | :--- |
| **Move Left** | `Left Arrow` | `A` |
| **Move Right** | `Right Arrow` | `D` |
| **Soft Drop** | `Down Arrow` | `S` |
| **Hard Drop** | `Spacebar` | — |
| **Rotate Clockwise** | `Up Arrow` | `W` / `X` |
| **Rotate Counter-Clockwise** | `Z` | `Left Control` |
| **Hold Piece** | `C` | `Left Shift` |
| **Pause / Resume** | `Escape` | `P` |
| **Quick Restart** | `R` | — |

> **Note on DAS & ARR**: Delayed Auto Shift (DAS) activates after holding a horizontal direction for $133\,\text{ms}$, after which Auto Repeat Rate (ARR) shifts the piece every $33\,\text{ms}$ for fluid, responsive movement.

---

## 📊 Guideline Scoring Table

Points scale directly with the current **Level**:

| Action | Base Score | Back-to-Back (B2B) |
| :--- | :--- | :--- |
| **Single** | $100 \times \text{Level}$ | — |
| **Double** | $300 \times \text{Level}$ | — |
| **Triple** | $500 \times \text{Level}$ | — |
| **Tetris (4 Lines)** | $800 \times \text{Level}$ | $1,200 \times \text{Level}$ (1.5×) |
| **T-Spin Mini No Lines** | $100 \times \text{Level}$ | — |
| **T-Spin Mini Single** | $200 \times \text{Level}$ | $300 \times \text{Level}$ |
| **T-Spin No Lines** | $400 \times \text{Level}$ | — |
| **T-Spin Single** | $800 \times \text{Level}$ | $1,200 \times \text{Level}$ |
| **T-Spin Double** | $1,200 \times \text{Level}$ | $1,800 \times \text{Level}$ |
| **T-Spin Triple** | $1,600 \times \text{Level}$ | $2,400 \times \text{Level}$ |
| **Combo Bonus** | $+ (50 \times \text{Combo} \times \text{Level})$ | Added to line clear |
| **Perfect Clear (All Clear)** | $+ (800 \text{ to } 3,200) \times \text{Level}$ | Varies by line count |
| **Soft Drop** | $+1$ per cell | — |
| **Hard Drop** | $+2$ per cell | — |

---

## 🚀 Building & Running

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.
- Raylib-compatible graphics driver (OpenGL 3.3+ / Mesa / Wayland / X11).

### Compilation
Clone the repository and build:
```bash
git clone https://github.com/minigame-tech/Tetris.git
cd Tetris
dotnet build
```

### Launching the Game
```bash
dotnet run
```

### Running the Headless Self-Test Suite
Tetris Neo includes an integrated verification test suite that validates randomizer distribution, SRS wall kick matrices, matrix collision detection, and procedural audio synthesis without needing a display server:
```bash
dotnet run -- --test
```

Sample output:
```text
==================================================
        TETRIS NEO - INTEGRATION SUITE            
==================================================
  [PASS] 7-Bag Randomizer guarantees uniform distribution across 10 bags
  [PASS] RotationState clockwise cycle (0 -> R -> 2 -> L -> 0)
  [PASS] SRS J kick table contains 5 test offsets
  [PASS] SRS I kick table contains 5 specialized offsets
  [PASS] SRS O piece uses 1 zero offset (no kicks)
  [PASS] Empty board permits standard spawn placement
  [PASS] Out of bounds left rejected
  [PASS] Out of bounds right rejected
  [PASS] Out of bounds bottom rejected
  [PASS] Ghost drop distance lands above bottom floor
  [PASS] Full row detected at row 21
  [PASS] Line cleared completely leaves board empty
  [PASS] Tetris clear awards 800 base points at Level 1
  [PASS] Back-to-Back Tetris awards 1.5x bonus (1200 points + 50 combo)
  [PASS] Clearing 10 lines advances game to Level 2
  [PASS] Procedural move sound generates valid WAV header (>= 44 bytes)
  [PASS] Procedural Tetris fanfare generates multi-note PCM buffer
  [PASS] Procedural music loop generates multi-bar melody buffer
--------------------------------------------------
Tests executed: 18 | Passed: 18 | Failed: 0
==================================================
```

---

## 📄 Documentation & Policies

For deep technical insights and community standards, review:
- [Architecture Overview](docs/ARCHITECTURE.md) - System diagrams, game loop, audio synthesis, and particle physics.
- [SRS Specification](docs/SRS_SPECIFICATION.md) - Super Rotation System kick tables and 3-corner T-Spin geometry.
- [Controls & DAS/ARR Guide](docs/CONTROLS.md) - Detailed input timings and competitive maneuver tips.
- [Security Policy](SECURITY.md) - Vulnerability disclosure and threat mitigation.
- [Code of Conduct](CODE_OF_CONDUCT.md) - Contributor standards and enforcement guidelines.

---

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for code style guidelines, testing rules, and pull request procedures.

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).
Built with passion by the **minigame-tech** team.

