# Coding Standards (C# / .NET)

This project follows established, widely-adopted community conventions rather than inventing our own. When in doubt, prefer the source with the most community consensus (Microsoft's own docs, then the most-starred/most-maintained community guide).

## Primary references

- **[Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)** — the official baseline for naming, layout, and language usage.
- **[dotnet/runtime coding-style.md](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md)** — how the .NET team itself writes C#: Allman braces, 4-space indent, `_camelCase` for private/internal fields, `var` when the type is obvious, no `this.` unless required.
- **[thangchung/clean-code-dotnet](https://github.com/thangchung/clean-code-dotnet)** (7.7k★) — Robert C. Martin's *Clean Code* principles adapted for .NET. This is our primary reference for naming, function size, SOLID application, and comment discipline.

## Non-negotiable rules

1. **Names say what things are.** No `Manager`, `Helper`, `Data`, `Info` suffixes as a substitute for a real name. A class name should make its one responsibility obvious.
2. **Functions do one thing.** If a method needs a comment to explain "step 1 / step 2" sections, split it into named private methods instead.
3. **Comments explain *why*, never *what*.** Well-named code doesn't need narration. Write a comment only for a non-obvious constraint, a workaround, or a decision that would otherwise be re-litigated. XML doc comments (`///`) are required on all public APIs (controllers, public service interfaces, shared library methods) — these are documentation, not the "why" comments above.
4. **No dead code, no commented-out code.** Delete it; git history remembers it.
5. **Nullable reference types are enabled** (`<Nullable>enable</Nullable>`) project-wide. Don't suppress warnings with `!` unless you've actually proven non-null; add a one-line comment when you do.
6. **Format is enforced by tooling, not memory.** See [Automated enforcement](#automated-enforcement) below — if the formatter/analyzer disagrees with a hand-written style choice, the tooling wins.

## Automated enforcement

- **`.editorconfig`** at the repo root defines formatting rules (indentation, brace style, naming rules) so every editor and `dotnet format` agree.
- **StyleCop.Analyzers** (NuGet, build-time Roslyn analyzer) enforces the rules above automatically and fails the build on violation. StyleCop uses its own `stylecop.json` for documentation-header rules, but rule *severity* is still tunable from `.editorconfig`.
- Run `dotnet format` before committing; CI (once set up) should run `dotnet format --verify-no-changes` plus the analyzer build.

## Frontend (React/TypeScript)

- ESLint + Prettier, enforced the same way: formatting is tooling's job, not a matter of taste.
- Components are named after what the user sees/does, not their implementation detail.
