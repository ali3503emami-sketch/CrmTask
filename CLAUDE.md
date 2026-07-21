# CLAUDE.md

Guidance for Claude Code (and any other agent) working in this repository.

## Project

A web-based CRM + task-management system (customers, contacts/follow-ups, contracts, internal/customer tasks with dynamic checklists, sales proforma invoices, correspondence registry with OCR, service-request-to-task conversion, per-user-configurable login). Stack: **ASP.NET Core (C#) API + React/TypeScript frontend + SQL Server**. No application code has been written yet — this file and `docs/` define the rules the codebase will be built under from day one.

## Documentation map

This file stays short on purpose (see [Keeping this file lean](#keeping-this-file-lean)). Everything with real detail lives in `docs/` — read the relevant one before touching that area:

| Doc | Read it when you're... |
|---|---|
| [docs/coding-standards.md](./docs/coding-standards.md) | writing any C# — naming, comments, formatting, analyzer setup |
| [docs/testing-strategy.md](./docs/testing-strategy.md) | writing **any** code — TDD is mandatory, this is the workflow and toolchain |
| [docs/architecture-principles.md](./docs/architecture-principles.md) | deciding where a piece of logic belongs (controller vs service vs shared) |
| [docs/library-usage-policy.md](./docs/library-usage-policy.md) | about to write infrastructure-y code — check this before reinventing something |
| [docs/configuration-and-secrets.md](./docs/configuration-and-secrets.md) | adding any config value, connection string, or API key |
| [docs/git-and-github-setup.md](./docs/git-and-github-setup.md) | committing, pushing, or setting up the GitHub remote |

## The rules that always apply

1. **TDD, no exceptions.** Write the failing test before the production code. Details: [docs/testing-strategy.md](./docs/testing-strategy.md).
2. **Thin controllers.** Business logic lives in the Application/service layer, never in a controller action. If two controllers need the same logic, extract it to a shared service or model — don't copy-paste. Details: [docs/architecture-principles.md](./docs/architecture-principles.md).
3. **Don't reinvent the wheel.** Check [docs/library-usage-policy.md](./docs/library-usage-policy.md)'s pre-approved list, or search for an established package, before hand-writing infrastructure code.
4. **No secrets in source, ever.** `dotnet user-secrets` locally, environment variables in deployment. Details: [docs/configuration-and-secrets.md](./docs/configuration-and-secrets.md).
5. **Formatting/style is the tooling's job** (`.editorconfig` + StyleCop.Analyzers), not a matter of personal preference. Run `dotnet format` before committing.
6. **Before every commit or push**, tests must be green and the **`/code-review`** skill must be run against the change. Don't push unreviewed or untested code.
7. **No redundant code.** If you're about to write something 90% identical to existing code, extract and parameterize instead of duplicating.

## Keeping this file lean

This file is a map and a short list of standing rules — not a place for detailed explanations. When you (Claude) make an architectural decision, add a convention, or the project reaches a new milestone worth documenting:

- Put the detail in a **new or existing file under `./docs/`**, not here.
- Add **one line** to both the table above and to `docs/README.md` pointing at it.
- Do this proactively, as part of the work that prompted it — don't wait to be asked, and don't let this file grow past a quick skim.

## Repo structure & commands

Not filled in yet — there's no solution scaffolded. Once it is, add a `docs/solution-structure.md` (folder layout, project names) and a `docs/commands.md` or a section here with the actual `dotnet build` / `dotnet test` / `npm run dev` commands, and link it from the table above.
