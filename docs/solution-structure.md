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

Cross-feature references go through the *parent's* service, not the repository directly — e.g. `ContactsController` checks `ICustomerService.GetByIdAsync` (not `ICustomerRepository`) to confirm the parent customer exists before creating/listing a contact. Keeps the dependency direction feature → feature's own service, not feature → another feature's persistence details.

### Commands

```
dotnet build                                    # whole solution
dotnet test                                     # whole solution (Domain + Application + integration)
dotnet run --project src/CrmTask.Api             # run the API (Swagger UI at /swagger in Development)
dotnet ef migrations add <Name> --project src/CrmTask.Infrastructure --startup-project src/CrmTask.Api --output-dir Migrations
```

Local dev connection string is set via `dotnet user-secrets` (see [configuration-and-secrets.md](./configuration-and-secrets.md)), not in `appsettings.json` — pointing at `(localdb)\MSSQLLocalDB`. Migrations apply automatically on startup in the Development environment only.

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
