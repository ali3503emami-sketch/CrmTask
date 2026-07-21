# CLAUDE.md

Guidance for Claude Code (and any other agent) working in this repository.

## Project

A web-based CRM + task-management system (customers, contacts/follow-ups, contracts, internal/customer tasks with dynamic checklists, sales proforma invoices, correspondence registry with OCR, service-request-to-task conversion, per-user-configurable login). Stack: **ASP.NET Core (C#) API + React/TypeScript frontend + SQL Server**, shipped to **iOS and Android via Capacitor** — a native mobile release is a firm requirement, not an optional stretch goal. No application code has been written yet — this file and `docs/` define the rules the codebase will be built under from day one.

## Documentation map

This file stays short on purpose (see [Keeping this file lean](#keeping-this-file-lean)). Everything with real detail lives in `docs/` — read the relevant one before touching that area:

| Doc | Read it when you're... |
|---|---|
| [docs/coding-standards.md](./docs/coding-standards.md) | writing any C# — naming, comments, formatting, analyzer setup |
| [docs/frontend-coding-standards.md](./docs/frontend-coding-standards.md) | writing any React/TypeScript — naming, component structure, ESLint/Prettier setup |
| [docs/testing-strategy.md](./docs/testing-strategy.md) | writing **any** code — TDD is mandatory, this is the workflow and toolchain (backend and frontend) |
| [docs/architecture-principles.md](./docs/architecture-principles.md) | deciding where a piece of logic belongs (controller/component vs service/hook vs shared) |
| [docs/library-usage-policy.md](./docs/library-usage-policy.md) | about to write infrastructure-y code — check this before reinventing something |
| [docs/mobile-strategy.md](./docs/mobile-strategy.md) | touching anything platform-related — why Capacitor, what plugins, how the native boundary is kept thin |
| [docs/configuration-and-secrets.md](./docs/configuration-and-secrets.md) | adding any config value, connection string, or API key (backend or frontend) |
| [docs/git-and-github-setup.md](./docs/git-and-github-setup.md) | committing, pushing, or setting up the GitHub remote |

## The rules that always apply

1. **TDD, no exceptions — backend and frontend alike.** Write the failing test (xUnit or RTL) before the production code. Details: [docs/testing-strategy.md](./docs/testing-strategy.md).
2. **Thin controllers, thin components.** Business logic lives in the Application/service layer (backend) or in hooks (frontend), never in a controller action or a component body. If two controllers — or two components — need the same logic, extract it to a shared service/hook or model — don't copy-paste. Details: [docs/architecture-principles.md](./docs/architecture-principles.md).
3. **Don't reinvent the wheel.** Check [docs/library-usage-policy.md](./docs/library-usage-policy.md)'s pre-approved list, or search for an established package, before hand-writing infrastructure code — backend or frontend.
4. **No secrets in source, ever.** `dotnet user-secrets` locally, environment variables in deployment; on the frontend, `VITE_*` vars are public by nature, so real secrets never go there either. Details: [docs/configuration-and-secrets.md](./docs/configuration-and-secrets.md).
5. **Formatting/style is the tooling's job** — `.editorconfig` + StyleCop.Analyzers for C#, ESLint (Airbnb config) + Prettier for React — not a matter of personal preference. Run `dotnet format` / `eslint --fix` before committing.
6. **Before every commit or push**, tests must be green and the **`/code-review`** skill must be run against the change. Don't push unreviewed or untested code.
7. **No redundant code.** If you're about to write something 90% identical to existing code, extract and parameterize instead of duplicating.
8. **Mobile is mandatory, not optional.** The React app must ship as a real iOS/Android app via Capacitor — see [docs/mobile-strategy.md](./docs/mobile-strategy.md). Keep platform-specific access (camera, push, storage) behind the thin abstraction described there so features stay platform-agnostic.

## Keeping this file lean

This file is a map and a short list of standing rules — not a place for detailed explanations. When you (Claude) make an architectural decision, add a convention, or the project reaches a new milestone worth documenting:

- Put the detail in a **new or existing file under `./docs/`**, not here.
- Add **one line** to both the table above and to `docs/README.md` pointing at it.
- Do this proactively, as part of the work that prompted it — don't wait to be asked, and don't let this file grow past a quick skim.

## Repo structure & commands

Not filled in yet — there's no solution scaffolded. Once it is, add a `docs/solution-structure.md` (folder layout, project names) and a `docs/commands.md` or a section here with the actual `dotnet build` / `dotnet test` / `npm run dev` commands, and link it from the table above.
