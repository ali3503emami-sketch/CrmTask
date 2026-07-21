# Mobile Strategy

## This is a firm requirement, not a "maybe later"

A real, installable mobile app (iOS + Android) **must** be produced for this system. This isn't just "make the web app responsive" — responsiveness is necessary but not sufficient; the CRM has to actually ship to both app stores.

## Decision: Capacitor, wrapping the same React app

We build **one** responsive React/TypeScript web app, and package it for mobile with **[Capacitor](https://capacitorjs.com/)** rather than maintaining a separate React Native codebase.

### Why this over React Native

| | Capacitor (chosen) | React Native |
|---|---|---|
| Codebase | One — the same web app, wrapped | Two — a separate native UI layer |
| Time to first app-store release | ~3–4 weeks on top of an existing web app | ~8–12 weeks (UI is rebuilt even if some logic is shared) |
| Native device access | Via Capacitor plugins (camera, push, filesystem, etc.) — covers everything this CRM needs | Full native, better for very high-end native feel |
| Fit for this project | A form-and-data-heavy internal business tool — exactly Capacitor's sweet spot | Better suited to consumer apps where native feel/performance is the product itself |

Given this is an internal line-of-business CRM (forms, lists, checklists, reminders — not a graphics- or gesture-heavy consumer app), Capacitor gets us to "live in both app stores" fastest, from a single, already-planned React codebase, with no duplicated frontend effort.

### What this CRM specifically needs from Capacitor plugins

- **`@capacitor/camera`** — the دبیرخانه (correspondence) module's "photo of a letter → OCR" feature needs real native camera access, not just an HTML `<input type="file">`. Capacitor's Camera plugin gives full native camera/gallery access even though the UI is a web view.
- **`@capacitor/push-notifications`** (and/or **`@capacitor/local-notifications`** for on-device scheduled reminders) — for follow-up, contract-expiry, and task-due reminders when the app isn't open.
- **`@capacitor/preferences`** — lightweight native local storage (e.g., remembering the last-used login method).

## Architectural consequence: keep native-dependent code behind a thin boundary

Business logic and hooks must not directly assume "we're in a browser." Native-only calls (camera, push, preferences) go through a small `src/shared/platform/` wrapper with a web fallback (e.g., regular `<input type="file">` when running as a plain website) and a Capacitor-backed implementation when running in the wrapped app. This keeps feature code (the OCR capture screen, the reminders feature) unaware of *which* platform it's running on — same principle as [architecture-principles.md](./architecture-principles.md)'s "no logic duplicated across call sites," just applied to platform APIs instead of business rules.

## Build/release, once the app is scaffolded

To be filled in with the actual commands once the solution exists: `npx cap add ios`, `npx cap add android`, `npx cap sync`, and the app-store submission checklist (icons, splash screens, signing). Add a `docs/mobile-release-checklist.md` at that point and link it here rather than growing this file indefinitely.
