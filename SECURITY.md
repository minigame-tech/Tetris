# Security Policy

## 1. Supported Versions

We take the security and integrity of **Tetris Neo** seriously. The table below outlines the support status for project versions:

| Version | Supported | Security Maintenance |
| :--- | :--- | :--- |
| **1.0.x** | :white_check_mark: Yes | Active security patches and bug fixes |
| **< 1.0.0** | :x: No | Unsupported |

---

## 2. Reporting a Vulnerability

If you discover a security vulnerability, security flaw, or potential exploit within Tetris Neo, please follow our responsible disclosure procedure:

1. **Do Not File Public Issues**: Never disclose vulnerabilities in public GitHub issues, discussions, or pull requests before they have been reviewed and addressed.
2. **Contact Email**: Submit a detailed security advisory via email to:
   - **`programmazione.moderna@gmail.com`** (or repository security team)
3. **Include Key Details**:
   - Component / Module affected (e.g. `HighScoreManager`, `TextureManager`, `Raylib-cs` native interop).
   - Detailed description of the vulnerability and its potential attack vector.
   - Minimal reproducible proof-of-concept (PoC) or step-by-step reproduction steps.
   - Potential impact assessment.
4. **Response Timeline**:
   - **Acknowledgment**: Within **48 hours** of receiving the report.
   - **Triage & Assessment**: Within **5 business days**.
   - **Resolution & Patch**: Coordinated disclosure and security release deployed as soon as a fix is verified.

---

## 3. Threat Model & Security Posture

### 3.1 Network Isolation
Tetris Neo is designed as a standalone, zero-network desktop application:
- **No In-Game Sockets**: The game does not establish outgoing TCP/UDP connections or telemetry pipelines during gameplay.
- **Offline Reliability**: All audio waveforms, fonts, and logic execute strictly in local process memory.

### 3.2 File I/O & Path Traversal Mitigations
- **Relative Path Anchoring**: Persistence files (`settings.json`, `highscore.json`) and asset search paths are strictly anchored to `AppContext.BaseDirectory` and validated before access.
- **Exception Quarantine**: All JSON serialization and file read/write routines are wrapped in robust exception handlers to prevent denial-of-service or unhandled process crashes due to corrupted files.

### 3.3 Native Interop & Memory Safety
- **Raylib-cs Interop**: Native calls to GLFW and OpenGL through Raylib-cs are strictly typed.
- **Unsafe Code Boundaries**: `AllowUnsafeBlocks` in `Tetris.csproj` is restricted exclusively to low-level byte buffers required for in-memory WAV audio synthesis. No unmanaged memory leaks or unbounded pointer arithmetic are permitted.

### 3.4 Dependency Integrity
- NuGet packages are resolved with strict package hash validation.
- All dependencies are regularly monitored for upstream vulnerabilities using automated dependency scanners (`dotnet list package --vulnerable`).

