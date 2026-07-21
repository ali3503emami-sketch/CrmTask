# Testing Strategy — TDD is mandatory

## The rule

**No production code is written before a failing test exists for it.** Red → Green → Refactor, in that order, every time:

1. Write a test that expresses the behavior you're about to build. Watch it fail (red) — this proves the test actually tests something.
2. Write the minimum code to make it pass (green).
3. Refactor with the safety net of the passing test, re-run to confirm still green.

This applies to bug fixes too: reproduce the bug as a failing test first, then fix it. The test is what proves the bug is actually gone and stays gone.

Claude/any agent working in this repo must follow this loop — do not write a controller/service/component method and then "add tests after" as a separate step. If a task arrives without an existing test for the behavior being changed, write the test first.

## Toolchain

### Backend (.NET)

| Concern | Tool | Why |
|---|---|---|
| Test framework | **xUnit** | The de-facto standard for modern .NET Core; constructor-based DI for test fixtures; parallel by default. |
| Mocking | **Moq** | Most widely used .NET mocking library; isolates the unit under test from its dependencies. |
| Assertions | **FluentAssertions** | Readable, intention-revealing assertions (`result.Should().Be(...)`) over raw `Assert.Equal`. |
| Integration/API tests | **Microsoft.AspNetCore.Mvc.Testing** (`WebApplicationFactory`) | Spins up the real pipeline in-memory against a test database. |

Structure: one test project per source project (`MyApp.Domain.Tests`, `MyApp.Application.Tests`, `MyApp.Api.IntegrationTests`, ...), mirroring the folder/namespace layout of what it tests.

Naming: `MethodUnderTest_Scenario_ExpectedResult` (e.g., `CreateContract_WhenEndDateBeforeStartDate_ThrowsValidationException`). Follow **Arrange-Act-Assert**, with the three sections visually separated (blank line or comment) in every test.

### Frontend (React/TypeScript)

| Concern | Tool | Why |
|---|---|---|
| Test runner | **Vitest** | Fast, native ESM, drop-in Jest-compatible API for a Vite-based React app. |
| Component testing | **React Testing Library** | Tests behavior from the user's perspective (queries by role/text), not implementation details. |

## What must have tests

- Every service/use-case class in the Application layer (business rules — contracts expiring, checklist validation, task assignment, etc.).
- Every controller action's contract (status code, shape of response) via integration tests — not necessarily every branch, that's the service layer's job.
- Every non-trivial React component with logic (forms, checklist builder, dynamic list rendering) — not pure presentational components with no branching.

## What doesn't need a dedicated test

- Simple DTOs/records with no logic.
- Framework wiring (`Program.cs` startup) — covered indirectly by integration tests actually running.

## Before every commit or push

Tests must be green locally. See the repo's git workflow in [git-and-github-setup.md](./git-and-github-setup.md) — `/code-review` runs *after* tests pass, not instead of them.
