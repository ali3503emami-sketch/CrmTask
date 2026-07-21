# Configuration & Secrets

## Rule

**No password, connection string, API key, or token is ever committed to source control** — not in `appsettings.json`, not in a comment, not "temporarily" during development.

## Where configuration lives, by environment

| Environment | Mechanism | Committed to git? |
|---|---|---|
| Local development | `dotnet user-secrets` (via `Microsoft.Extensions.Configuration.UserSecrets`) | No — stored outside the repo, in the user profile directory, keyed by a `UserSecretsId` in the `.csproj`. |
| CI / shared dev / production | Environment variables | No — set on the host/pipeline, never in a file that's committed. |
| Non-secret defaults (timeouts, feature flags, log levels) | `appsettings.json` / `appsettings.{Environment}.json` | Yes — this file holds no secrets, so it's safe to commit. |

Configuration precedence in ASP.NET Core: environment variables override `appsettings.{Environment}.json`, which overrides `appsettings.json`, which is overridden by user-secrets in Development. This lets the same code run correctly in every environment without branching logic.

Environment variable naming: since `:` isn't valid in env var names on all platforms, use double underscore for nested keys — e.g. the config key `Sms:Kavenegar:ApiKey` becomes the environment variable `Sms__Kavenegar__ApiKey`.

## Setup checklist for a new project/service

1. `dotnet user-secrets init` in the API project — adds a `UserSecretsId` GUID to the `.csproj` (safe to commit, it's just a pointer, not a secret).
2. `dotnet user-secrets set "ConnectionStrings:Default" "..."` for local SQL Server connection strings, SMS gateway API keys, etc.
3. In production/staging, set the equivalent environment variables on the host (or, once budget allows, move to a proper secret store).
4. Double-check `.gitignore` covers `appsettings.Development.json` **if** it ever ends up holding anything sensitive — by default it shouldn't need to, since user-secrets exists precisely so this file doesn't have to.

## Frontend (React/Vite) config — a different set of rules

**Anything in a Vite frontend `.env` file gets baked into the public JS bundle at build time.** There is no such thing as a "frontend secret" — if it's reachable from `import.meta.env.VITE_*`, an end user can read it by opening dev tools. So:

- Frontend `.env` files (`.env`, `.env.production`, prefixed `VITE_*`) are for **non-secret, environment-specific config only**: the API base URL, a public feature-flag value, the Google OAuth *client ID* (client IDs are public by design, unlike client secrets).
- Anything actually sensitive (Google OAuth client **secret**, SMS gateway keys, database credentials) stays server-side only, behind a backend endpoint the frontend calls — it never has a `VITE_` counterpart.
- Frontend `.env*` files are still gitignored (see `.gitignore`) as a matter of hygiene and to keep per-environment URLs out of source, even though their contents aren't secret — this avoids a committed `.env` silently pointing every developer at the wrong API URL.

## What this means for this CRM specifically

The SQL Server connection string, the SMS gateway API key (Kavenegar/Melipayamak/IPPanel), and the Google OAuth client secret are exactly the kind of values that go through user-secrets locally and environment variables in deployment — never into `appsettings.json`.
