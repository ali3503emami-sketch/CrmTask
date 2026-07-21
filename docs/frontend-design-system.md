# Frontend Design System

## The requirement driving this

Every module in this CRM (customer records, contracts, tasks/checklists, correspondence log) shows a lot of information per screen. The design has to stay **refined and legible under that density** — not cramped, not cluttered, and not achieved by shrinking things arbitrarily. This doc is the concrete decision for how that's done, so it isn't re-litigated per feature.

## Decision: Ant Design, in compact mode, RTL, with Vazirmatn

| Piece | Choice | Why |
|---|---|---|
| Component library | **Ant Design (antd)** | Purpose-built for data-dense enterprise admin UIs — rich `Table` (sorting/filtering/pagination built in), `Descriptions`, `ProForm`-style patterns, and first-class RTL + locale support out of the box. Nothing else in the React ecosystem matches its depth for this specific kind of app (see the research summary this decision is based on — Ant Design was the clear, repeatedly-recommended choice for dense enterprise/CRM interfaces with RTL needs). |
| Density mode | **`theme.compactAlgorithm`** (antd's built-in compact theme) | This is the mechanism, not manual padding tweaks — it systematically tightens spacing/sizing across every component so density stays consistent instead of ad-hoc per screen. |
| Direction | **RTL via `<ConfigProvider direction="rtl" locale={faIR}>`** wrapping the app in `main.tsx` | The whole product is Persian; RTL is set once, globally, not per component. |
| Font | **Vazirmatn** (variable font, self-hosted via `@fontsource-variable/vazirmatn`, no external font CDN call) | The standard modern open-source Persian typeface — clean at small sizes, which matters directly for a compact/dense layout. |
| Theme tokens | `src/theme.ts` | `colorPrimary: '#2f6f6b'` (a restrained, desaturated teal — refined rather than loud), `borderRadius: 6` (soft but not overly rounded), `fontSize: 13` (one step below antd's 14px default, consistent with compact mode). Table header background and cell padding are tuned slightly beyond the compact defaults for the specific case of long data tables. |

## What "refined despite density" means concretely, day to day

- **Use the component built for the data shape, don't build a denser one by hand.** A read-only detail view is `<Descriptions size="small">`, not a hand-rolled grid of labels and values. A summary number is `<Statistic>`, not a styled `<div>`.
- **Whitespace is a token, not a guess.** Spacing comes from the compact theme + antd's `Space`/`Row`/`Col` gutter props — never a one-off inline `margin`.
- **Tables carry status as color, not just text.** Use `<Tag color="success|warning|default">` (or antd's semantic status colors) so state reads at a glance in a dense list — this was already applied in the first screen built (contract status column).
- **Don't fight the compact theme with per-component overrides.** If a specific screen needs more or less density than the global compact setting gives it, that's a signal to reconsider the layout, not to hand-override padding on that one component.
- **Test note:** Ant Design's `Table` reads `window.matchMedia` (responsive breakpoints) and briefly touches `getComputedStyle` while measuring scrollbar width. The former is mocked in `src/test/setup.ts` (real requirement, tests fail without it); the latter logs a harmless "not implemented" jsdom warning during tests — known jsdom/antd interaction, not a real failure, safe to ignore in test output.

## What's already scaffolded (`frontend/`)

- Vite + React 19 + TypeScript, Ant Design + `@ant-design/icons` + Vazirmatn installed and wired in `src/main.tsx`.
- `src/App.tsx` is a working proof of the density/RTL/theme setup (a customers table + stat tiles) — replace it with real routing/features as they're built, it's a starting reference, not a page to keep.
- ESLint (typescript-eslint + `eslint-plugin-jsx-a11y`, flat config) + Prettier, Vitest + React Testing Library + `@testing-library/jest-dom`, following [testing-strategy.md](./testing-strategy.md) and [frontend-coding-standards.md](./frontend-coding-standards.md).

Note on `eslint-config-airbnb` specifically: it doesn't support ESLint's modern flat-config format well as of this setup, so it isn't installed as a mechanical dependency — Airbnb's guide is still the *prose* reference for naming/JSX conventions in [frontend-coding-standards.md](./frontend-coding-standards.md), mechanically enforced instead via `typescript-eslint` + `jsx-a11y` + Prettier. If `eslint-config-airbnb` gains solid flat-config support later, revisit this.

## A note on the dev machine

The Node.js installed here is **v20.6.1**, which is old enough that several current packages (jsdom's latest major, `create-vite`'s latest major) refused to run and had to be pinned to slightly older, still-current versions to get a clean install. Everything works as configured, but upgrading Node to a current LTS (22.x or newer) would remove the need for those pins going forward. Not urgent, just flagged.

## As the UI grows

Once real feature pages (customers, contracts, tasks, correspondence) exist, patterns worth extracting — a standard list-page layout, a standard detail-drawer, a checklist-field-type renderer — get their own short doc here (e.g. `docs/frontend-components.md`), linked from this file and `docs/README.md`, rather than growing this file past a decision record.
