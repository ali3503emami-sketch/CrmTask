# Documentation Index

Detailed rules and decisions live here as focused, single-topic files. `CLAUDE.md` at the repo root stays short and just points here — see it for the quick-reference version of these rules.

| Doc | Covers |
|---|---|
| [coding-standards.md](./coding-standards.md) | C# naming, formatting, comment discipline, StyleCop/.editorconfig enforcement |
| [testing-strategy.md](./testing-strategy.md) | TDD workflow, xUnit/Moq/FluentAssertions, frontend test tooling |
| [architecture-principles.md](./architecture-principles.md) | Layering, thin controllers, where shared logic goes |
| [library-usage-policy.md](./library-usage-policy.md) | Don't reinvent the wheel — pre-approved libraries per concern |
| [configuration-and-secrets.md](./configuration-and-secrets.md) | user-secrets, environment variables, what never gets committed |
| [git-and-github-setup.md](./git-and-github-setup.md) | `.gitignore` rationale, GitHub remote setup steps |

As the project grows, new topics (solution structure, deployment, API conventions, domain glossary, etc.) get their own file here, added to this table and to `CLAUDE.md`'s doc map — not folded into an existing file until it stops being a good fit.
