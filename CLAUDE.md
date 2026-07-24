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
| [docs/frontend-design-system.md](./docs/frontend-design-system.md) | building any UI — Ant Design, compact/RTL theme, density conventions (this is a firm decision) |
| [docs/testing-strategy.md](./docs/testing-strategy.md) | writing **any** code — TDD is mandatory, this is the workflow and toolchain (backend and frontend) |
| [docs/architecture-principles.md](./docs/architecture-principles.md) | deciding where a piece of logic belongs (controller/component vs service/hook vs shared) |
| [docs/library-usage-policy.md](./docs/library-usage-policy.md) | about to write infrastructure-y code — check this before reinventing something |
| [docs/mobile-strategy.md](./docs/mobile-strategy.md) | touching anything platform-related — why Capacitor, what plugins, how the native boundary is kept thin |
| [docs/solution-structure.md](./docs/solution-structure.md) | looking for where something lives — backend/frontend folder layout, per-feature conventions, commands |
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

- `frontend/` — React/TypeScript app (Vite). `npm run dev` · `npm test` · `npm run lint` · `npm run build`
- `backend/` — ASP.NET Core solution (Domain/Application/Infrastructure/Api + matching test projects). `dotnet build` · `dotnet test` · `dotnet run --project src/CrmTask.Api`

Vertical slices built end-to-end so far (backend + frontend, TDD throughout), with real SQL Server persistence (dev now points at a real local SQL Server 2019 instance, not LocalDB — see [docs/configuration-and-secrets.md](./docs/configuration-and-secrets.md)): **Customers** (full CRUD — create/list/get/update, plus a profile — manager name/birthdate, address, fax, notes, national ID, a business category and activity field each chosen from a manageable reference list — and an embedded, fully-replaced-on-save `Personnel` list, editable via a dedicated frontend panel with an all-fields search box on the list), **Contacts** (per-customer contact log with an optional follow-up reminder date), **Contracts** (derived, not stored, Active/ExpiringSoon/Ended status — see [docs/solution-structure.md](./docs/solution-structure.md) for the `TimeProvider` pattern; also exposed cross-customer via `GET /api/contracts` for the Dashboard's ended-contracts tab), **Staff** (full CRUD — list/create/update, an optional position chosen from the same reference list as customers, own frontend tab/page with a ویرایش row action — populates task assignment, *not* a login system), **Tasks** (internal or customer-linked, assigned to staff *and* recording who created it (`CreatedByStaffId`, distinct from the assignee), with a dynamic per-task checklist, full CRUD, locked from editing once marked Done, and a frontend page built around an always-visible Jalali calendar for day-based filtering plus a full task-detail/view panel), and **ReferenceLists** (Positions / Customer Categories / Activity Fields — three identical "Id+Title" admin-managed lookup lists, full CRUD including update, sharing one backend implementation and one generic frontend page with a ویرایش row action, see solution-structure.md). Use these as the template for the next module. **All of "اطلاعات پایه" (Basic Info) is editable, not just create-once** — a firm product decision; any new admin-managed list added under that menu should ship an update path from day one, not list/create-only.

**Every date field also stores a Jalali (Shamsi) mirror string** (`XShamsi`, e.g. `CreatedAtShamsi`) alongside the canonical Gregorian value, via `PersianDateConverter` (Domain/Shared, wraps .NET's built-in `PersianCalendar` — no external package) — a firm product decision, applies to every module with a date. On the frontend, all date/time input goes through the shared `PersianDateField`/`PersianDateTimeField`/`PersianCalendar` components (`frontend/src/shared/date/`, built on `react-multi-date-picker` + `react-date-object`); displayed dates use the backend's own `xxxShamsi` fields directly rather than re-deriving them client-side. See solution-structure.md for the real EF Core gotcha this surfaced (owned-collection children with client-generated keys added to an already-saved parent) and how it was fixed, plus the digit-style (Persian vs. ASCII) and multi-calendar testing gotchas on the frontend side.

Real authentication (login, sessions, the password/OTP/Google-per-user design discussed earlier) is **deliberately not built yet** — Staff is just a data table, not an auth system. Don't assume any endpoint is protected. In the meantime the frontend has a **temporary "who are you?" current-user simulator** (`frontend/src/shared/currentUser/`, sessionStorage-backed, a header dropdown over the Staff list) so the Dashboard can filter "my tasks" and task creation can record a creator — this is explicitly a stand-in, not a security boundary, and should be replaced wholesale once real auth lands.

The frontend's navigation is a single top-level "کاربر" (User) menu (Ant Design `Menu`, `frontend/src/shared/navigation/`, rendered by `App.tsx`) with: **داشبورد** (Dashboard — the default/landing page: a read-only "my tasks" tab (assigned to me OR created by me, no add/edit) plus an "ended contracts" tab), **اطلاعات پایه** (Basic Info — Staff, Positions, Customer Categories, Activity Fields), **امور مشتریان** (Customer Affairs — Customers), and **انجام کار** (Do Task — the full-featured Tasks page). Future top-level menus for other roles would be siblings of "کاربر", not nested under it. The nav is responsive per the standard mobile pattern — a persistent sidebar (`Sider`) on desktop, collapsing below 768px (CSS media query, not JS breakpoint detection) into a hamburger button that opens the same menu in an overlay `Drawer`, which auto-closes on selection — since mobile is a firm requirement (see below), not an afterthought.
