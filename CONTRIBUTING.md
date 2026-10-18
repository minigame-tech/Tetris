# Contributing to Tetris Neo

Thank you for your interest in contributing to **Tetris Neo**! We strive to maintain high-performance, tournament-grade gameplay adhering strictly to the official Tetris Guideline standards.

---

## 🛠️ Development Setup

1. **Prerequisites**:
   - [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Git
   - An IDE of choice: JetBrains Rider, Visual Studio 2022, or VS Code with C# Dev Kit.

2. **Clone & Build**:
   ```bash
   git clone https://github.com/minigame-tech/Tetris.git
   cd Tetris
   dotnet build
   ```

3. **Run Verification Tests**:
   Before submitting any changes, verify that all core mechanics tests pass:
   ```bash
   dotnet run -- --test
   ```

---

## 📐 Code Style & Conventions

- **Language Standards**: C# 12, .NET 8 LTS.
- **Null Safety**: Nullable reference types are enabled (`<Nullable>enable</Nullable>`). Avoid null-suppression operators (`!`) unless accompanied by an explanatory comment.
- **Naming Conventions**:
  - `PascalCase` for Types, Methods, Properties, and Public Members.
  - `camelCase` with leading underscore (`_fieldName`) for private fields.
  - Clear, domain-centric nomenclature matching official Tetris Guideline terminology (e.g. *Tetromino*, *SRS*, *Lock Delay*, *T-Spin*, *Back-to-Back*).
- **Comments & Docstrings**: Maintain XML documentation comments on all public classes, methods, and enum values.

---

## 🚀 Pull Request Workflow

1. **Fork & Branch**:
   Create a descriptive feature branch from `master`:
   ```bash
   git checkout -b feature/srs-enhancement
   ```
2. **Commit Early & Clearly**:
   Write conventional commit messages (e.g., `feat: implement DAS tuning slider in settings`, `fix: correct I-piece 3rd kick offset`).
3. **Verify Locally**:
   Ensure zero compiler warnings and 100% test pass rate:
   ```bash
   dotnet build && dotnet run -- --test
   ```
4. **Open a PR**:
   Submit your pull request with a detailed description of the problem solved, testing methodology, and screenshots if UI changes were made.

