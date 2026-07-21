# Architecture Principles

## Layering

```
Controllers (Api)        → HTTP concerns only: model binding, status codes, routing.
   ↓
Application (Services)   → Use-cases / business rules. This is where logic lives.
   ↓
Domain (Core/Models)     → Entities, value objects, domain rules that don't depend on infrastructure.
   ↓
Infrastructure           → EF Core, SQL Server, SMS gateway, OCR service, file storage.
```

A controller action should read like a short paragraph: validate the request shape, call one application service method, map the result to a response. If a controller method is doing `if/else` business logic, loops over business rules, or touching `DbContext` directly, that logic belongs one layer down.

## The rule that matters most: no duplicated logic across controllers

If a piece of logic (a calculation, a validation rule, a mapping) is needed by **two or more controllers**, it does not live in either controller. It moves down into a shared **service** (Application layer) or, if it's pure data shape/validation with no side effects, a shared **model/DTO** (Domain or Contracts layer) that both controllers depend on.

Concretely, for this CRM: contract-expiry logic, checklist-template validation, task-assignment rules, and OCR-field-extraction post-processing are exactly the kind of logic that must live in `Application/Services`, never copy-pasted into `CustomersController` and `TasksController` separately.

## Shared code lives in one discoverable place

- **`src/Shared`** (or `lib/` if you prefer that name — pick one and stay consistent) holds cross-cutting helpers that don't belong to a single feature: date/time helpers, Persian-calendar conversion, phone-number normalization, result/error wrapper types.
- Before writing a new helper, **search `src/Shared` first** — and search NuGet second (see [library-usage-policy.md](./library-usage-policy.md)) before writing it yourself. Don't create a second date-formatting helper because the first one wasn't easy to find.

## No redundant code

- Don't write a new method that's 90% identical to an existing one — extract the common part and parameterize the difference.
- Don't hand-roll something the framework or a well-tested package already does well (mapping, validation, retry policies, pagination) — see the library policy doc.
- Three near-identical lines inline are fine. A third near-identical *method* is a signal to extract a shared one.

## The same layering applies to the frontend

```
Components (presentational)  → JSX/markup and layout only. No fetching, no business rules.
   ↓
Hooks (feature logic)        → useContractExpiry(), useChecklistValidation(), etc. This is where logic lives.
   ↓
API layer (src/shared/api)   → Typed request functions (via TanStack Query) wrapping the backend endpoints.
```

A component should read like a template: given this data and these callbacks, render this. If a component has `if/else` business rules, direct `fetch`/`axios` calls, or duplicate data-shaping logic, that belongs in a hook instead — this is the frontend's version of "no logic in controllers."

**No duplicated logic across components**, same as the backend rule: if two feature components both need to compute "is this contract expiring soon," that check lives in one shared hook or utility in `src/shared`, not copy-pasted into both components. See [frontend-coding-standards.md](./frontend-coding-standards.md) for file/folder conventions.

## As the project grows

This file describes principles, not the current folder tree. Once the solution is scaffolded, add a `docs/solution-structure.md` documenting the actual projects/folders, and link it from `CLAUDE.md`. Keep this principles file about *rules*, and the structure file about *what exists*.
