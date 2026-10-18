# Tetris Neo - Controls & Maneuver Guide

## 1. Input Keybindings Reference

Tetris Neo supports standard arcade arrow controls as well as ergonomic alternate WASD bindings:

| Command | Primary Binding | Secondary Binding | Description |
| :--- | :--- | :--- | :--- |
| **Move Left** | `Left Arrow` | `A` | Shifts active piece left 1 column. Holds activate DAS/ARR. |
| **Move Right** | `Right Arrow` | `D` | Shifts active piece right 1 column. Holds activate DAS/ARR. |
| **Soft Drop** | `Down Arrow` | `S` | Accelerates gravity downwards 20×. Awards $+1$ pt per cell. |
| **Hard Drop** | `Spacebar` | — | Instantly drops and locks piece. Awards $+2$ pt per cell. |
| **Rotate Clockwise** | `Up Arrow` | `W` / `X` | Rotates piece 90° clockwise using SRS kick tests. |
| **Rotate Counter-Clockwise** | `Z` | `Left Control` | Rotates piece 90° counter-clockwise using SRS kick tests. |
| **Hold Piece** | `C` | `Left Shift` | Swaps active piece into the Hold queue (1 swap per drop). |
| **Pause / Resume** | `Escape` | `P` | Opens the in-game frosted pause overlay. |
| **Quick Restart** | `R` | — | Immediately restarts current game mode session. |

---

## 2. DAS & ARR Input Tuning

To achieve competitive, tournament-level responsiveness, Tetris Neo bypasses default operating system key-repeat delays using a custom input engine:

### Delayed Auto Shift (DAS)
- **Default Delay**: $133\,\text{ms}$
- When an arrow key is pressed, the tetromino shifts exactly 1 cell immediately. If the key is held continuously beyond the DAS threshold, automatic rapid shifting activates.

### Auto Repeat Rate (ARR)
- **Default Interval**: $33\,\text{ms}$ (approximately 30 shifts per second)
- Once DAS has engaged, the piece continues to shift across the matrix every $33\,\text{ms}$ until released or blocked by an obstacle.

---

## 3. Maneuver Strategy & Tips

1. **Ghost Piece Alignment**: Keep an eye on the translucent ghost outline at the bottom of the matrix before executing a Hard Drop (`Spacebar`).
2. **Back-to-Back (B2B) Chains**: Only **Tetris (4-line clears)** and **T-Spins** sustain a Back-to-Back streak. Clearing a single, double, or triple line without a T-Spin breaks your B2B chain.
3. **Hold Conservation**: The Hold buffer allows you to store an awkward piece (like an early $S$ or $Z$) or save an $I$-tetromino for an upcoming 4-line Tetris clear.
4. **Wall Kick Maneuvers**: When pieces are squeezed against walls or narrow wells, rotating into the wall pushes the piece outward into valid open space according to SRS tables.

