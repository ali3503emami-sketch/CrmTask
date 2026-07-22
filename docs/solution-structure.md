# Solution Structure

## Backend (`backend/`)

Clean/layered architecture — see [architecture-principles.md](./architecture-principles.md) for the rules behind this shape, this file just documents what exists.

```
backend/
  CrmTask.sln
  Directory.Build.props      # Nullable, StyleCop.Analyzers, shared analyzer config for every project
  .editorconfig               # backend-specific rule overrides (layered on the repo-root one)
  stylecop.json
  src/
    CrmTask.Domain/           # Entities, no dependencies on anything else. Currently: Customers/
    CrmTask.Application/      # Use-case services, DTOs, FluentValidation validators. Depends on Domain only.
    CrmTask.Infrastructure/    # EF Core DbContext, entity configs, repository implementations, migrations.
    CrmTask.Api/               # ASP.NET Core controllers + Program.cs composition root. Depends on all three.
  tests/
    CrmTask.Domain.Tests/          # xUnit + FluentAssertions, no mocking needed (pure entity logic).
    CrmTask.Application.Tests/     # xUnit + Moq + FluentAssertions, mocks repository interfaces.
    CrmTask.Api.IntegrationTests/  # xUnit + WebApplicationFactory, hits a real (uniquely-named, disposable) LocalDB database.
```

### Per-feature layout inside each layer

Each layer groups its own files by feature, not by technical type — e.g. `CrmTask.Domain/Customers/`, `CrmTask.Application/Customers/`, `CrmTask.Infrastructure/Customers/` (and the same for `Contacts/`, `Contracts/`). When Tasks, Correspondence, etc. are built, they get their own sibling folders the same way — this mirrors the frontend's `src/features/` convention.

`Contract` is a good reference for **derived, not stored, status**: `ContractStatus` (Active/ExpiringSoon/Ended) is computed from `EndDate` against "today" every time, via `Contract.GetStatus(DateOnly today)` — never persisted, so it can't go stale. "Today" is injected as a `TimeProvider` (registered as `TimeProvider.System` in `Program.cs`), not read from `DateTime.Now` directly, specifically so tests can fix it (see `CrmTask.Application.Tests/TestSupport/FakeTimeProvider.cs`) instead of being flaky around whatever day they happen to run on. Any other module with a similar "is this thing due/expiring" concept (task due dates, contract-adjacent reminders) should follow the same pattern.

Cross-feature references go through the *parent's* service, not the repository directly — e.g. `ContactsController` checks `ICustomerService.GetByIdAsync` (not `ICustomerRepository`) to confirm the parent customer exists before creating/listing a contact. Keeps the dependency direction feature → feature's own service, not feature → another feature's persistence details. `TasksController` follows the same rule with *two* parents: `IStaffService` (always — a task must be assigned) and `ICustomerService` (only when `CustomerId` is provided, since a task can be purely internal).

### `TaskItem`'s dynamic checklist — the reference for "owned collection with a private backing field"

`ChecklistItem` (child of `TaskItem`) models the scenario's "چک‌باکس، کشو، لیست‌باکس، تکست‌باکس" requirement: one `FieldType` enum plus a single `Value` string, so a task's checklist can mix arbitrary field types without a schema change per type. Two things about how it's built are worth knowing before extending it:

- **`ChecklistItem.Options` and `TaskItem.ChecklistItems` are read-only properties** (`IReadOnlyList<T>`) backed by private `List<T>` fields (`_options`, `_checklistItems`) — the domain model never exposes a way to mutate the collection from outside except through the entity's own methods (`SetValue`, `SetChecklistItemValue`). EF Core finds these backing fields automatically by naming convention (`_options` for `Options`, etc.); see `TaskItemEntityConfiguration` in Infrastructure for the `OwnsMany` + field-based `Options` column (stored as a single JSON string column via a value converter + explicit `ValueComparer`, not a further child table — simpler than a 4th table for what's just a handful of strings).
- **Validation of `Value` lives in `ChecklistItem.SetValue`**, keyed off `FieldType` (checkbox must be "true"/"false"; dropdown/listbox must be one of `Options`; textbox accepts anything) — this is the same "invariants belong to the entity, not the caller" rule as everywhere else, just applied to a field that changes shape based on another field's value.
- **`TaskService`'s mutation methods (`MarkAsDoneAsync`, `ReassignAsync`, `SetChecklistItemValueAsync`) all follow load → mutate → `SaveChangesAsync`**, unlike the create-only repositories elsewhere — `ITaskRepository.GetByIdAsync` deliberately returns a *change-tracked* entity (not `AsNoTracking()`) for this reason. If a future module needs similar in-place mutation (not just create+read), follow this same repository shape rather than inventing a new one.

### Commands

```
dotnet build                                    # whole solution
dotnet test                                     # whole solution (Domain + Application + integration)
dotnet run --project src/CrmTask.Api             # run the API (Swagger UI at /swagger in Development)
dotnet ef migrations add <Name> --project src/CrmTask.Infrastructure --startup-project src/CrmTask.Api --output-dir Migrations
```

Local dev connection string is set via `dotnet user-secrets` (see [configuration-and-secrets.md](./configuration-and-secrets.md)), not in `appsettings.json` — pointing at the real local SQL Server 2019 instance (`localhost`), browsable in SSMS, not LocalDB. Migrations apply automatically on startup in the Development environment only.

Automated integration tests (`CrmTask.Api.IntegrationTests`) still target `(localdb)\MSSQLLocalDB` with a uniquely-named, auto-deleted database per run (see `CustomApiFactory`) — that's a deliberate, separate choice from the dev database above: tests want a disposable throwaway instance, not the one you're browsing in SSMS.

## Frontend (`frontend/`)

```
frontend/
  src/
    main.tsx              # QueryClientProvider + antd ConfigProvider (RTL/theme/locale) wiring
    App.tsx                # Thin shell: header + routed/composed feature pages
    theme.ts               # Ant Design theme tokens (see frontend-design-system.md)
    shared/
      api/httpClient.ts    # Thin fetch wrapper (base URL from VITE_API_BASE_URL)
    features/
      customers/
        types.ts            # Types mirroring the API's DTOs
        customersApi.ts      # Typed request functions
        useCustomers.ts       # TanStack Query hook (read)
        useCreateCustomer.ts  # TanStack Query mutation hook (write)
        CustomersPage.tsx     # The actual page — table + create-customer modal
        CustomersPage.test.tsx
      contacts/
        # Same shape as customers/. ContactsPanel is a Drawer opened from a
        # row action on CustomersPage — the pattern for any "sub-feature tied
        # to a parent record".
      contracts/
        # Same pattern as contacts/ — a second row-action Drawer (ContractsPanel)
        # on CustomersPage. Date fields are plain "YYYY-MM-DD" text inputs, not
        # antd's DatePicker — deliberate, to avoid Jalali/Gregorian locale
        # parsing ambiguity now that ConfigProvider is set to fa_IR. Revisit
        # once a real Jalali-aware date picker is chosen (not decided yet).
      staff/
        # Minimal: list + create only, no detail page. Exists to populate the
        # assignee dropdown on TasksPage — not a login/auth system. See
        # docs/configuration-and-secrets.md and the auth discussion in
        # CLAUDE.md's history for why real authentication is a separate,
        # deliberately-not-yet-built piece of work.
      tasks/
        # A top-level page (its own tab in App.tsx), not nested under Customers
        # — a task can be internal (customerId: null) or tied to a customer.
        # TasksPage.tsx builds the checklist template at creation time via
        # antd's Form.List (a dynamic array of {label, fieldType, options});
        # ChecklistPanel.tsx is the separate Drawer for *filling in* an
        # existing task's checklist values, one input per FieldType (Checkbox,
        # Select for Dropdown, Radio.Group for ListBox, Input for TextBox).
    test/
      setup.ts             # jest-dom, matchMedia mock, RTL cleanup, MSW server lifecycle
      mocks/
        handlers.ts          # MSW request handlers (mutable in-memory data per docs/testing-strategy.md)
        server.ts
```

New modules (Contracts, Tasks, Correspondence, ...) get their own folder under `src/features/`, following the same internal shape (`types.ts`, `xApi.ts`, `useX.ts` hooks, `XPage.tsx` + its test) as Customers.

### Commands

Unchanged from [frontend-design-system.md](./frontend-design-system.md)/root `CLAUDE.md`: `npm run dev`, `npm test`, `npm run lint`, `npm run build`.

### Talking to the backend

`VITE_API_BASE_URL` (in `.env.development`, gitignored — copy `.env.development.example`) points at the API's URL. CORS on the API side (`Program.cs`) is configured from `Cors:AllowedOrigins` in `appsettings.json` to allow the Vite dev server's origin.
