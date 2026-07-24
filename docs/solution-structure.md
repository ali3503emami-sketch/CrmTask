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
- **`TaskItem` is also editable until `Done`** (`Update`, `Reassign`, `SetChecklistItemValue` all call a private `EnsureEditable()` guard that throws `InvalidOperationException` once `Status == Done`) — a completed task is immutable by design. `TasksController` translates that exception into a `409 Conflict`, not a `400` (it's not a validation error — the request would be perfectly valid on an open task).

### A real EF Core gotcha: adding a *new* owned entity to an *already-persisted* parent

`CustomerPersonnel` (owned collection on `Customer`, same pattern as `ChecklistItem`) hit a genuine bug the first time a customer with an empty personnel list was updated to add personnel: EF Core threw `DbUpdateConcurrencyException: ... affected 0 row(s)`, because it generated an `UPDATE` for the new row instead of an `INSERT`.

**Why:** `CustomerPersonnel.Create()` sets `Id = Guid.NewGuid()` client-side (same as every other entity in this codebase). EF Core's default "is this Added or Modified" heuristic, for an entity that appears in an already-tracked navigation collection, is: *if the primary key already has a non-default value, assume the row already exists in the database.* This works fine for `TaskItem.ChecklistItems` because there, the checklist items are always created in the *same* `SaveChanges` call as their brand-new parent `TaskItem` (a pure insert-only graph) — the bug only shows up when new owned children are added to a *pre-existing* parent in a later, separate update, which is exactly what `Customer.ReplacePersonnel` does.

**Fix** (in `CustomerRepository.SaveChangesAsync`, not in the domain — this is an EF-specific concern): before saving, for every tracked `CustomerPersonnel` in `Modified` state, call `entry.GetDatabaseValuesAsync()` (EF's own built-in "does a row with this key actually exist" check); if it returns `null`, the entry is re-marked `Added`. A few extra per-row existence checks only on the (small, infrequent) personnel-edit path — correct beats clever here.

**If this bites again**: any owned collection where new items can be added to an *already-saved* parent (not just at initial creation) needs this same fix. `ChecklistItem` doesn't need it *today* only because nothing currently adds new checklist items to an existing task after creation — if that changes, apply the identical fix to `TaskRepository`.

### `ReferenceData` — one shared implementation for three identical lookup lists

Positions (سمت‌ها), Customer Categories (دسته‌بندی مشتریان), and Activity Fields (زمینه فعالیت) are three admin-managed lists that are 100% identical in shape — just "Id + Title", list + create, no update/delete. Rather than tripling the Domain/Application/Infrastructure/Api boilerplate, there's **one** `ReferenceListItem` entity (`CrmTask.Domain/ReferenceData/`) with a `ReferenceListKind` enum discriminator, **one** `ReferenceListItemEntityConfiguration` (single `ReferenceListItems` table, unique index on `(Kind, Title)` so the same title can't be added twice within a kind), **one** `IReferenceListService`/`ReferenceListRepository` pair, and **three thin Api controllers** (`PositionsController`, `CustomerCategoriesController`, `ActivityFieldsController`) that each just inherit `ReferenceListControllerBase` with their own route and fixed `Kind` — giving three clean REST resources (`/api/positions`, `/api/customer-categories`, `/api/activity-fields`) without three copies of the actual logic. The frontend mirrors this with one generic `ReferenceListPage` component (`frontend/src/shared/referenceData/`) and three one-line page wrappers.

Fields elsewhere that read from these lists (`StaffMember.Position`, `CustomerPersonnel.Position`, `Customer.CategoryTitle`, `Customer.ActivityField`) store the chosen **title string directly**, not a foreign key — deliberately simple for now (no join, no cascade-on-rename concern to handle yet); revisit only if a real referential-integrity need shows up later.

### `TaskItem.CreatedByStaffId` — a second, distinct staff reference

`AssignedToStaffId` (who the task is for) and `CreatedByStaffId` (who registered it) are separate fields — a task can be created by one staff member and assigned to another. This exists specifically to drive the Dashboard's "کارهای من" tab (tasks assigned to me OR created by me). Both are required, non-nullable `Guid` FKs to `StaffMembers`, configured as two independent `HasOne<StaffMember>()` relationships in `TaskItemEntityConfiguration` — EF Core handles multiple FKs to the same principal type fine as long as each has its own `HasForeignKey` and no shared navigation property is implied.

### A real migration gotcha: adding a non-nullable FK column to a table with existing rows

Adding `CreatedByStaffId` (non-nullable `Guid`) to the already-populated `Tasks` table crashed on `dotnet run` against the real dev database: `dotnet ef migrations add` generates `ADD COLUMN ... DEFAULT '00000000-0000-0000-0000-000000000000'` for existing rows (the CLR default for a non-nullable value type) — then the very next migration step, `ALTER TABLE ADD CONSTRAINT FK_Tasks_StaffMembers_CreatedByStaffId`, fails immediately because no `StaffMembers` row has that all-zero id.

**Fix**: added a backfill `migrationBuilder.Sql("UPDATE [Tasks] SET [CreatedByStaffId] = [AssignedToStaffId];")` between the `AddColumn` and `AddForeignKey` steps in the generated migration — existing tasks' already-valid assignee is the most sensible available stand-in for an unknown historical creator. **This class of bug only shows up against a database that already has rows** (an empty/fresh dev or CI database never hits it) — caught here specifically because dev points at a real, populated SQL Server instance rather than a throwaway one. Any future non-nullable FK column added to a table that might already have data needs the same treatment: backfill before adding the constraint, not after.

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
    App.tsx                # Thin shell: CurrentUserProvider + header (title + CurrentUserPicker)
    # + a sidebar Ant Design Menu (mode="inline"). One top-level "کاربر" (User)
    # menu group for now — future role-based menus would be sibling top-level
    # groups, not nested under it — with: داشبورد (Dashboard, the default/landing
    # page), اطلاعات پایه (Basic Info: Staff, Positions, Customer Categories,
    # Activity Fields), امور مشتریان (Customer Affairs: Customers), and انجام کار
    # (Do Task: the full TasksPage). `pageComponents: Record<PageKey, () => JSX.Element>`
    # maps menu item keys straight to page components — add a new page by adding
    # one menu item + one map entry, no routing library involved.
    theme.ts               # Ant Design theme tokens (see frontend-design-system.md)
    shared/
      api/httpClient.ts    # Thin fetch wrapper (base URL from VITE_API_BASE_URL)
      currentUser/
        # A temporary "شما کیستید؟" (who are you?) simulator — CurrentUserContext.tsx
        # (React context + useCurrentUser() hook, sessionStorage-backed so it
        # resets per browser tab/session, NOT localStorage) and CurrentUserPicker.tsx
        # (the header dropdown, populated from useStaff()). Stands in for real
        # login (deliberately not built yet, see CLAUDE.md) so the Dashboard can
        # filter "my tasks" and task creation can record a creator. Any page that
        # needs "who is using the app right now" reads `useCurrentUser().currentStaffId`
        # — don't invent a second mechanism.
      referenceData/
        # Generic list+create UI for the three admin-managed lookup lists
        # (mirrors the backend's ReferenceListControllerBase consolidation):
        # types.ts, referenceListApi.ts (factory: createReferenceListApi(basePath)),
        # useReferenceList.ts (useReferenceList(queryKey, basePath) / useCreateReferenceListItem(...)),
        # ReferenceListPage.tsx (the actual table+modal page, parameterized by
        # labels/route). features/positions, features/customerCategories, and
        # features/activityFields are each just a ~15-line wrapper passing their
        # own labels/basePath/queryKey into ReferenceListPage — add a fourth list
        # the same way, no new page logic needed.
      date/
        # The shared Jalali date components — every date input/display in the
        # app goes through one of these, not a raw antd DatePicker or dayjs
        # formatting. All three wrap react-multi-date-picker + react-date-object.
        PersianDateField.tsx      # Date-only picker, value in/out = Gregorian "YYYY-MM-DD"
        PersianDateTimeField.tsx  # Date+time picker (adds the time_picker plugin), value in/out = full ISO datetime
        PersianCalendar.tsx       # Always-visible (non-popup) day picker, used for TasksPage's calendar
        # Shared gotcha across all three: a plain incoming string must be parsed
        # with the `gregorian` calendar explicitly (`new DateObject({ date, calendar: gregorian })`)
        # — otherwise DatePicker parses it using its own display calendar
        # (persian) and silently misreads e.g. "2024-03-20" as a Jalali date.
        # Outgoing values must convert back with `.convert(gregorian, gregorian_en)`
        # — `gregorian_en` (not the default locale) is what keeps the emitted
        # digits ASCII, matching the backend's own `PersianDateConverter.ToShamsi`
        # format exactly (e.g. "1403/01/01", not "۱۴۰۳/۰۱/۰۱"). The *display*
        # widgets themselves correctly render Persian digits (persian_fa locale)
        # for the calendar UI — only the round-tripped string values need to
        # stay ASCII. Don't re-derive a Shamsi string on the frontend for
        # anything the backend already returns (`xxxShamsi` DTO fields) — display
        # those directly; only use these components for *input*.
    features/
      customers/
        types.ts            # Types mirroring the API's DTOs, including Personnel + all profile fields
        customersApi.ts      # Typed request functions (create + update)
        useCustomers.ts       # TanStack Query hook (read)
        useCreateCustomer.ts  # TanStack Query mutation hook (create: name/category/phone only)
        useUpdateCustomer.ts  # TanStack Query mutation hook (update: full profile + personnel)
        CustomersPage.tsx     # Table (with an all-fields client-side search box) + create-customer modal
        CustomerProfilePanel.tsx # Drawer opened via the row's "ویرایش" action — the
        # *only* place profile fields (manager name/birthdate, address, fax,
        # notes, national ID) and the embedded Personnel Form.List are edited;
        # Create stays deliberately minimal (name/category/phone) per the
        # backend split between CreateCustomerRequest and UpdateCustomerRequest.
      contacts/
        # Same shape as customers/. ContactsPanel is a Drawer opened from a
        # row action on CustomersPage — the pattern for any "sub-feature tied
        # to a parent record". The next-follow-up field uses PersianDateField
        # with `minDate` set to today; displayed dates use the backend's own
        # `contactedAtShamsi`/`nextFollowUpAtShamsi` fields, not a re-derived one.
      contracts/
        # Same pattern as contacts/ — a second row-action Drawer (ContractsPanel)
        # on CustomersPage. Start/end date fields use PersianDateField; the list
        # displays the backend's `startDateShamsi`/`endDateShamsi` directly.
      positions/, customerCategories/, activityFields/
        # Each just a thin page wrapping shared/referenceData/ReferenceListPage
        # with its own labels/basePath/queryKey — see the shared/referenceData/
        # entry above. Live under "اطلاعات پایه" in App.tsx's menu.
      staff/
        # StaffPage.tsx: list + create only (no edit/deactivate — matches the
        # backend's endpoints), plus an optional Position field (a Select
        # populated from useReferenceList('positions', '/api/positions'), same
        # list Customer Personnel's position reads from). Also still populates
        # the assignee dropdown on TasksPage — not a login/auth system. See
        # docs/configuration-and-secrets.md and the auth discussion in
        # CLAUDE.md's history for why real authentication is a separate,
        # deliberately-not-yet-built piece of work.
      dashboard/
        # DashboardPage.tsx — the menu's default/landing page. Two tabs: "کارهای
        # جاری" (read-only — no add/edit, just TaskListTable's مشاهده/اتمام‌کار
        # actions — filtered to `assignedToStaffId === currentStaffId ||
        # createdByStaffId === currentStaffId`; shows an Empty prompt to pick a
        # current user first if none is set) and "قراردادهای خاتمه‌یافته" (all
        # contracts across every customer, via useAllContracts, filtered
        # client-side to `status === 'Ended'`). Reuses tasks/TaskListTable.tsx
        # and tasks/TaskDetailPanel.tsx rather than duplicating the row/detail
        # UI — see the tasks/ entry below for why that extraction happened.
      tasks/
        # A top-level page (reached via "انجام کار" in App.tsx's menu), not
        # nested under Customers — a task can be internal (customerId: null)
        # or tied to a customer. TaskListTable.tsx (table + مشاهده/اتمام‌کار
        # row actions) was extracted out of TasksPage.tsx specifically so
        # DashboardPage could reuse it without copying the column/action
        # definitions — if a third place ever needs to list tasks, extend this
        # component's props rather than forking it again. Creating a task
        # requires a current user to be set (`useCurrentUser().currentStaffId`
        # becomes `CreatedByStaffId`); TasksPage warns and refuses to open the
        # create modal otherwise. TasksPage.tsx has an always-visible PersianCalendar at the top for
        # day-based filtering: selectedDate === null shows every task
        # (unfiltered, the default state); picking a day filters to that day's
        # Open tasks, with a "نمایش همه" toggle to also reveal that day's Done
        # tasks (it does NOT clear the day filter — see the user-facing spec in
        # CLAUDE.md's history). Opening the create-task modal while a day is
        # selected pre-fills `dueAt` to that day. TasksPage.tsx builds the
        # checklist template at creation time via antd's Form.List (a dynamic
        # array of {label, fieldType, options}); TaskDetailPanel.tsx (opened via
        # the row's "مشاهده" button) is the Drawer that replaced the old
        # ChecklistPanel.tsx — it shows the whole task (title, description,
        # due date, assignee, customer, status) plus the checklist-filling UI
        # in one place, with an inline "ویرایش" toggle for title/description/
        # dueAt/customer that only appears while the task is Open (checklist
        # inputs are also disabled once Done, mirroring the backend's lock).
        # selectedTaskId (not the task object) is kept in TasksPage's state and
        # the live task is looked up from the `tasks` query result each render
        # — storing the object itself would go stale after any mutation
        # (edit, mark-done, checklist change) until the panel was reopened.
    test/
      setup.ts             # jest-dom, matchMedia mock, RTL cleanup, MSW server lifecycle
      mocks/
        handlers.ts          # MSW request handlers (mutable in-memory data per docs/testing-strategy.md)
        toShamsi.ts           # Mirrors the backend's PersianDateConverter.ToShamsi for mock
        # responses (via react-date-object, converting to `persian` calendar +
        # `gregorian_en` locale for ASCII digits) — keeps mocked Shamsi fields
        # realistic without duplicating the .NET conversion logic by hand.
        server.ts
```

New modules (Correspondence, ...) get their own folder under `src/features/`, following the same internal shape (`types.ts`, `xApi.ts`, `useX.ts` hooks, `XPage.tsx` + its test) as Customers.

### Testing rmdp-based components (PersianDateField/PersianDateTimeField/PersianCalendar)

A few non-obvious things came up writing tests against these:

- **Digit style differs by purpose, not by bug**: the calendar *display* renders Persian digits (locale `persian_fa`) — assert against those (e.g. `screen.getByDisplayValue('۱۴۰۳/۰۱/۰۱')`). The *round-tripped value* (what `onChange` emits, and what the backend's `xxxShamsi` fields contain) is ASCII (e.g. `'1403/01/01'`) — don't assert Persian digits there.
- **A page with more than one rmdp calendar on it is ambiguous by day-number text alone.** TasksPage has both an always-visible `PersianCalendar` and, inside the create-task Modal, a `PersianDateTimeField` popup — both render `role="dialog"` and the same day markup. Disambiguate the Modal itself with `screen.findByRole('dialog', { name: '<modal title>' })`, and disambiguate the *popup* calendar (vs. the inline one) by scoping to the element wrapped in react-multi-date-picker's floating positioner (an element with inline `style="position: absolute..."` as an ancestor) — see `withinDueDatePopup()` in `TasksPage.test.tsx` for the pattern.
- **rmdp's nav arrows and day cells have no useful accessible name** (`aria-roledescription`, not `aria-label`) — query them via `document.querySelector('.rmdp-arrow-container.rmdp-right')` / `.rmdp-day` rather than `getByRole('button', { name: ... })`.
- **`eslint-plugin-testing-library` is not installed in this project** — don't add `eslint-disable-next-line testing-library/...` comments for the node-access queries above; there's no rule to disable, and the disable comment itself becomes a lint error ("Definition for rule ... was not found").

### Selecting an antd `Select` option in tests: click the inner content div, not the outer `role="option"` one

Each rendered option is actually two nested elements — `<div role="option" aria-label="...">` wrapping a child `<div class="ant-select-item-option-content">` with the same text. `screen.findAllByText(label)` returns both (in that order), and clicking `[0]` (the outer, role-bearing one) **visually looks like it worked** — the closed Select shows the picked label — but doesn't actually register the value on the Form field; submission sends `null`. Only clicking `[1]` (the inner content div, i.e. `options[options.length - 1]`) fires antd's real `onChange`. This was found the hard way (a "passes visually, fails on submit" bug) wiring the Position/Category/Activity-field selects — don't use `getByRole('option', { name })` for antd Select in this codebase; use `screen.findAllByText(label)` and click the last match.

### Don't assert an exact date/month in a test that navigates a calendar "relative to today"

`ContactsPanel`'s next-follow-up test clicks the calendar's "next month" arrow and picks a day — the first version of this test then asserted the exact resulting Shamsi string (`1405/05/\d{2}`), which was only true on the day it was written. It failed the very next day the suite ran, once "today" rolled into a new month. There's no injectable clock for the frontend calendar components (unlike the backend's `TimeProvider` pattern) — so tests built around "whatever day/month it happens to be" should assert the *shape* of the result (e.g. `/\d{4}\/\d{2}\/\d{2}/`), not a hardcoded value.

### `@ant-design/icons` is broken under Node/Vitest out of the box — patched via `patch-package`

`@ant-design/icons@6.3.2`'s CJS build (`lib/colorUtils.js`) does `require("@ant-design/colors/es/generate")` — a hardcoded ES-module subpath, and `@ant-design/colors` ships no `exports` map to redirect it to a CJS twin. Any import from `@ant-design/icons` (even a single outline icon like `MenuOutlined`) eagerly loads this file and throws `SyntaxError: Cannot use import statement outside a module` under plain Node — which is exactly the module loader Vitest uses for externalized `node_modules` packages, so this broke as soon as the sidebar's mobile hamburger button (see nav section above) imported its first icon. Neither `vite.config.ts`'s `resolve.alias` nor Vitest's `server.deps.inline`/`deps.optimizer` config can fix it: the failing `require()` runs via Node's own module system, not through Vite's resolver, regardless of inlining settings. The package does ship a working CJS equivalent (`lib/generate.js`) right next to the broken import, so the fix is a one-line patch (`patches/@ant-design+icons+6.3.2.patch`, applied automatically via the `postinstall` script — **run `npm install` once after pulling, not just `npm ci --ignore-scripts`,** or the patch won't apply and this resurfaces).

### antd `Drawer`'s close animation never settles under jsdom — don't assert on DOM removal

The mobile nav Drawer's exit transition (`rc-motion`, `motionDeadline: 500`) relies on jsdom `getComputedStyle`/animation-frame behavior that never actually completes in the test environment, so `waitForElementToBeRemoved` on the Drawer's content hangs until the test times out — even with a generous timeout. `rc-drawer` does synchronously toggle an `ant-drawer-open` class on the root wrapper off the `open` prop, independent of the motion state — assert on that class instead (see the "closes the mobile menu drawer..." test in `App.test.tsx`).

### Commands

Unchanged from [frontend-design-system.md](./frontend-design-system.md)/root `CLAUDE.md`: `npm run dev`, `npm test`, `npm run lint`, `npm run build`.

### Talking to the backend

`VITE_API_BASE_URL` (in `.env.development`, gitignored — copy `.env.development.example`) points at the API's URL. CORS on the API side (`Program.cs`) is configured from `Cors:AllowedOrigins` in `appsettings.json` to allow the Vite dev server's origin.
