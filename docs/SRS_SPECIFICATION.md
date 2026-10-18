# Super Rotation System (SRS) Technical Specification

## 1. Overview

The **Super Rotation System (SRS)** is the standard rotation engine defined by the official **Tetris Guideline**. SRS dictates:
- Exact pivot centers for each tetromino within its bounding box.
- The 4 discrete rotation states: `0` (Spawn), `R` (Right / 90° CW), `2` (180°), and `L` (Left / 90° CCW).
- Predefined sequences of translation kick offsets tested sequentially whenever an attempted rotation collides with matrix boundaries or locked blocks.

---

## 2. Coordinate Conventions

In Tetris Neo:
- **Grid Coordinates**: $(Row, Col)$ where $Row$ increases downwards ($0 = \text{top hidden buffer}$, $21 = \text{bottom floor}$) and $Col$ increases rightwards ($0 = \text{left wall}$, $9 = \text{right wall}$).
- **SRS Cartesian Offsets $(x, y)$**: Where $+x$ is Right and $+y$ is Up.
- **Conversion to Grid Offset $(\Delta Row, \Delta Col)$**:
  $$\Delta Col = x$$
  $$\Delta Row = -y$$

---

## 3. JLSTZ Wall Kick Offset Table

When rotating pieces $J, L, S, T, Z$, the engine attempts up to 5 discrete offset tests in order:

| Rotation Transition | Test 1 | Test 2 | Test 3 | Test 4 | Test 5 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **$0 \rightarrow R$** | $(0, 0)$ | $(-1, 0)$ | $(-1, +1)$ | $(0, -2)$ | $(-1, -2)$ |
| **$R \rightarrow 0$** | $(0, 0)$ | $(+1, 0)$ | $(+1, -1)$ | $(0, +2)$ | $(+1, +2)$ |
| **$R \rightarrow 2$** | $(0, 0)$ | $(+1, 0)$ | $(+1, -1)$ | $(0, +2)$ | $(+1, +2)$ |
| **$2 \rightarrow R$** | $(0, 0)$ | $(-1, 0)$ | $(-1, +1)$ | $(0, -2)$ | $(-1, -2)$ |
| **$2 \rightarrow L$** | $(0, 0)$ | $(+1, 0)$ | $(+1, +1)$ | $(0, -2)$ | $(+1, -2)$ |
| **$L \rightarrow 2$** | $(0, 0)$ | $(-1, 0)$ | $(-1, -1)$ | $(0, +2)$ | $(-1, +2)$ |
| **$L \rightarrow 0$** | $(0, 0)$ | $(-1, 0)$ | $(-1, -1)$ | $(0, +2)$ | $(-1, +2)$ |
| **$0 \rightarrow L$** | $(0, 0)$ | $(+1, 0)$ | $(+1, +1)$ | $(0, -2)$ | $(+1, -2)$ |

*Translated to grid offsets $(\Delta Row, \Delta Col)$, Test 1 is always $(0, 0)$ (basic rotation without kick).*

---

## 4. I-Tetromino Wall Kick Offset Table

Because the $I$-tetromino occupies a $4 \times 4$ bounding box and rotates about an off-grid center, it utilizes a dedicated kick table:

| Rotation Transition | Test 1 | Test 2 | Test 3 | Test 4 | Test 5 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **$0 \rightarrow R$** | $(0, 0)$ | $(-2, 0)$ | $(+1, 0)$ | $(-2, -1)$ | $(+1, +2)$ |
| **$R \rightarrow 0$** | $(0, 0)$ | $(+2, 0)$ | $(-1, 0)$ | $(+2, +1)$ | $(-1, -2)$ |
| **$R \rightarrow 2$** | $(0, 0)$ | $(-1, 0)$ | $(+2, 0)$ | $(-1, +2)$ | $(+2, -1)$ |
| **$2 \rightarrow R$** | $(0, 0)$ | $(+1, 0)$ | $(-2, 0)$ | $(+1, -2)$ | $(-2, +1)$ |
| **$2 \rightarrow L$** | $(0, 0)$ | $(+2, 0)$ | $(-1, 0)$ | $(+2, +1)$ | $(-1, -2)$ |
| **$L \rightarrow 2$** | $(0, 0)$ | $(-2, 0)$ | $(+1, 0)$ | $(-2, -1)$ | $(+1, +2)$ |
| **$L \rightarrow 0$** | $(0, 0)$ | $(+1, 0)$ | $(-2, 0)$ | $(+1, -2)$ | $(-2, +1)$ |
| **$0 \rightarrow L$** | $(0, 0)$ | $(-1, 0)$ | $(+2, 0)$ | $(-1, +2)$ | $(+2, -1)$ |

---

## 5. T-Spin Evaluation Algorithm

A **T-Spin** occurs when a player maneuvers a $T$-tetromino into a constricted space and locks it. Tetris Neo evaluates T-Spins using the official **3-Corner Rule**:

### Evaluation Steps
1. The last successful maneuver before locking must be a **rotation**.
2. Examine the 4 diagonal corners of the $T$-piece's $3 \times 3$ bounding box:
   - **Top-Left**: $(Row, Col)$
   - **Top-Right**: $(Row, Col + 2)$
   - **Bottom-Left**: $(Row + 2, Col)$
   - **Bottom-Right**: $(Row + 2, Col + 2)$
   *(Any corner outside matrix boundaries or occupied by a locked block counts as occupied).*
3. **Corner Threshold**: At least **3 of the 4 corners** must be occupied.
4. **Mini vs. Full T-Spin Classification**:
   - Identify the two **front corners** facing the protruding nose of the $T$:
     - State `0` (Up): Top-Left and Top-Right
     - State `R` (Right): Top-Right and Bottom-Right
     - State `2` (Down): Bottom-Left and Bottom-Right
     - State `L` (Left): Top-Left and Bottom-Left
   - If both front corners are occupied **OR** the rotation used the 5th SRS kick test, it is graded as a **Full T-Spin**.
   - If only one front corner is occupied, it is graded as a **T-Spin Mini**.

---

## 6. Extended Placement Lock Delay

To prevent frustrating involuntary lockouts during active maneuvering:
- **Lock Delay**: When a piece contacts the floor or resting blocks, a $0.5\,\text{s}$ countdown begins.
- **Reset Trigger**: Any valid horizontal shift or rotation resets the lock timer to $0.0\,\text{s}$.
- **Limit**: To prevent infinite stalling, players are allotted a maximum of **15 reset maneuvers** per piece. Once exhausted, the piece locks immediately upon surface contact.

