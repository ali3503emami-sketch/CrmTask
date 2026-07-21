# Coding Standards (React / TypeScript)

The same principle as the backend: follow established, widely-adopted conventions rather than inventing our own. See [coding-standards.md](./coding-standards.md) for the backend equivalent — this file is its frontend counterpart, held to the same rigor.

For visual/component conventions (which library, density, RTL, theme tokens), see [frontend-design-system.md](./frontend-design-system.md) — this file covers code style, that one covers UI decisions.

## Primary references

- **[Airbnb JavaScript Style Guide](https://github.com/airbnb/javascript)** (incl. its [React/JSX section](https://github.com/airbnb/javascript/tree/master/react)) — one of the most widely adopted style guides in the JS ecosystem, enforced via `eslint-config-airbnb` (4,400+ dependent projects). Our default for formatting, component structure, and JSX conventions.
- **[React docs — "Thinking in React"](https://react.dev/learn/thinking-in-react)** and the official React docs generally — for component decomposition and data-flow conventions.
- **typescript-eslint recommended rules** — for type-safety-related lint rules Airbnb's config doesn't cover.

## Non-negotiable rules

1. **Names say what things are.** A component is named after what the user sees/does (`ContractExpiryBanner`, not `Comp3` or `DataWrapper`). A hook is named `useXxx` and says what it returns or does (`useContractReminders`, not `useData`).
2. **Components do one thing.** If a component mixes data-fetching, business rules, and heavy layout, split it: a container/hook for data + logic, a presentational component for rendering. This is the same "thin controller" idea as the backend — see [architecture-principles.md](./architecture-principles.md).
3. **Comments explain *why*, never *what*.** Same rule as backend. Use TSDoc (`/** ... */`) on exported hooks/utilities in shared code (`src/shared`), not on every local component.
4. **No dead code, no commented-out code, no `console.log` left in.**
5. **TypeScript strict mode is on** (`strict: true` in `tsconfig.json`). No `any` without a one-line comment justifying it; prefer `unknown` + narrowing.
6. **Format/lint is enforced by tooling, not memory** — see below.

## Automated enforcement

- **ESLint** with `eslint-config-airbnb`, `eslint-config-airbnb/hooks` (Rules of Hooks), `eslint-plugin-jsx-a11y` (accessibility — not optional, this is a form real staff will use daily), and `@typescript-eslint`.
- **Prettier** for formatting, wired so ESLint defers formatting concerns to it (`eslint-config-prettier`).
- Both run in a pre-commit hook and must also pass before `/code-review` — a PR with lint errors doesn't get to the review step.

## File/folder conventions

- Feature-first, not type-first: `src/features/contracts/`, `src/features/tasks/`, not one giant `src/components/` bucket. Each feature folder holds its own components, hooks, and API calls for that feature.
- `src/shared/` (mirrors the backend's shared layer) holds cross-feature hooks, UI primitives, and utilities — check here, then check for an existing package (see [library-usage-policy.md](./library-usage-policy.md)), before writing a new one.
- One component per file; file name matches the component name.
