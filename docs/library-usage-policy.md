# Library Usage Policy — don't reinvent the wheel

## Rule

Before writing any non-trivial piece of infrastructure code (parsing, retry/backoff, validation, mapping, scheduling, PDF/Excel generation, OCR, HTTP resilience), check whether a well-established, actively-maintained package already does it. Only write it by hand if no suitable package exists or the need is truly one line of logic.

"Well-established" means: widely used in the community, actively maintained, and ideally listed in a curated collection — **[quozd/awesome-dotnet](https://github.com/quozd/awesome-dotnet)** for backend/.NET, **[brillout/awesome-react-components](https://github.com/brillout/awesome-react-components)** for React — check there first when unsure what the community-standard choice is for a given concern.

## Pre-approved defaults for this project

Use these unless there's a specific reason not to — don't re-litigate the choice per feature:

| Concern | Library | Notes |
|---|---|---|
| ORM | **EF Core** (SQL Server provider) | Already decided for this project. |
| Background jobs / reminders | **Hangfire** (SQL Server storage) | Follow-up reminders, contract-expiry reminders, task due-date reminders. |
| Validation | **FluentValidation** | Keep validation rules out of controllers and out of entities; one validator class per command/DTO. |
| Object mapping | **Mapster** (or AutoMapper if the team prefers it) | Don't hand-write DTO↔entity mapping boilerplate. |
| Logging | **Serilog** | Structured logging, sink to file + console at minimum. |
| API documentation | **Swashbuckle (Swagger/OpenAPI)** | Auto-generated from controllers/DTOs — don't hand-write API docs that will drift. |
| HTTP resilience (OCR service, SMS gateway calls) | **Polly** | Retry/circuit-breaker around any external HTTP call (SMS gateway, OCR microservice). |
| Testing | xUnit, Moq, FluentAssertions | See [testing-strategy.md](./testing-strategy.md). |
| Frontend UI components | **Ant Design (antd)** | Chosen specifically for dense, RTL, enterprise-admin-style screens. See [frontend-design-system.md](./frontend-design-system.md) — this is a firm decision, not an open question. |
| Frontend data fetching | **TanStack Query** | Don't hand-roll fetch/cache/loading-state logic in components. |
| Frontend forms | **React Hook Form** + a schema validator (Zod) | Needed for the dynamic checklist builder's varied field types. |
| Frontend API mocking (tests) | **MSW (Mock Service Worker)** | See [testing-strategy.md](./testing-strategy.md) — the 2026-standard way to test components against a fake network layer instead of hand-mocking `fetch`. |
| Mobile packaging | **Capacitor** | Wraps the same responsive React app for iOS/Android instead of a separate codebase. See [mobile-strategy.md](./mobile-strategy.md) — this is a firm decision, not an open question. |
| Linting (frontend) | **eslint-config-airbnb** + `eslint-plugin-jsx-a11y` + `typescript-eslint` | See [frontend-coding-standards.md](./frontend-coding-standards.md). |

## Adding a new dependency

1. Confirm it solves a real, current need (not a hypothetical future one).
2. Check download count / maintenance activity (last release date, open issues trend) — avoid abandoned packages.
3. Add it via the package manager (`dotnet add package`, `npm install`) — never vendor a copy of a library's source into the repo.
4. Note *why* in the PR description if the choice isn't obvious from the table above.
