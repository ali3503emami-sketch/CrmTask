# Git & GitHub Setup

## What's already done locally

- `git init` has been run at the repo root.
- `.gitignore` covers: .NET build output (`bin/`, `obj/`, `.vs/`), React/Node build output (`node_modules/`, `dist/`, `build/`, coverage reports), IDE/OS clutter (`.idea/`, `Thumbs.db`, `.DS_Store`), local SQL Server data files (`*.mdf`, `*.ldf`, `*.ndf`), logs, and anything secret-shaped (`*.env`, `appsettings.*.Local.json`). See the `.gitignore` file itself for the full list — it's based on GitHub's official [VisualStudio.gitignore](https://github.com/github/gitignore/blob/main/VisualStudio.gitignore) template plus a Node/React section.
- An initial commit has been made containing only the project scaffolding (`CLAUDE.md`, `docs/`, `.gitignore`) — no application code yet, so there was nothing to review before this first commit.

## Before every future commit/push

Run the **`/code-review`** skill against the pending changes and make sure tests are green (see [testing-strategy.md](./testing-strategy.md)) *before* committing. This is a standing rule for this repo, not a one-time step.

## Setting up the GitHub remote (do this part yourself)

This step touches your GitHub account, so it's left for you to run rather than done automatically:

1. **Authenticate the GitHub CLI** (already installed on this machine, not yet logged in):
   ```
   gh auth login
   ```
   Follow the prompts (choose GitHub.com, HTTPS, and authenticate via browser).

2. **Create the remote repository** — from the project root:
   ```
   gh repo create <your-repo-name> --private --source=. --remote=origin
   ```
   Use `--public` instead of `--private` if you want it public. This creates the repo on your GitHub account and wires it as the `origin` remote in one step.

3. **Push the current branch:**
   ```
   git push -u origin main
   ```
   (If your default branch is named `master` instead of `main`, substitute accordingly, or rename it first with `git branch -M main`.)

### If you'd rather use the website instead of `gh`

1. Go to github.com → **New repository** → give it a name, choose Public/Private, **do not** initialize with a README/.gitignore/license (this repo already has those, and it avoids a merge conflict on first push).
2. Copy the remote URL it gives you, then:
   ```
   git remote add origin <the-url-it-gave-you>
   git branch -M main
   git push -u origin main
   ```

### Recommended repo settings once created

- Branch protection on `main`: require a pull request before merging, once you're not the only contributor.
- Add a `CODEOWNERS` file if/when a second person joins, so reviews are routed automatically.
