# Task Claims — Cebu Dental Tourism Hub

One owner per row. Read this file before editing. Never touch another owner's files.
Update your row's Status when you start (OPEN → IN PROGRESS) and when merged (→ DONE).

## Worktrees & branches (all at C:\Users\L23X17W21\)

| Agent      | Worktree      | Branch              | Port | SQLite file (own, never shared) |
|------------|---------------|---------------------|------|---------------------------------|
| opencode   | `appdev-db`   | `agent/opencode-db` | 5252 | `appdev.db` in its own worktree |
| antigravity| `appdev-ui`   | `agent/antigravity-ui` | 5253 | `appdev.db` in its own worktree |
| (integrate)| `appdev-sprint` | `master`          | —    | agents never touch              |

Run with: `dotnet run --urls http://localhost:<port>` from your own worktree.

## Hot files (single owner at a time — coordinate explicitly before touching)

`*.csproj`, `Program.cs`, `*/Migrations/*`, `appsettings*.json` → owner: **opencode-db**

## Folder ownership (do not cross)

- `Pages/` (classic Razor Pages: `_LoginPartial`, Identity support) → **opencode-db**
- `Components/Pages/` + `Components/Layout/` + `Components/Shared/` (Blazor UI) → **antigravity-ui**
- `wwwroot/js/map.js` → **antigravity-ui**

## Claims

| # | Task | Owner | Branch | Files owned | Status |
|---|------|-------|--------|-------------|--------|
| 1 | EF Core + SQLite + entities + seed (6 Cebu clinics, PHP prices) | opencode-db | agent/opencode-db | `Models/`, `Data/`, migrations | IN PROGRESS |
| 2 | Identity + ClinicAdmin role + /admin gate | opencode-db | agent/opencode-db | `Areas/Identity/`, `Program.cs` (auth section), `Pages/`, `Data/IdentitySeed.cs` | IN PROGRESS |
| 3 | Inquiry model + service + home-price anchors | opencode-db | agent/opencode-db | `Services/`, `Models/Inquiry.cs`, `Models/HomePriceAnchor.cs` | OPEN |
| 4 | Leaflet map component + Cebu pins + filters | antigravity-ui | agent/antigravity-ui | `Components/Shared/Map*`, `wwwroot/js/map.js` | OPEN |
| 5 | Public pages: Home, Clinics, Detail, Compare | antigravity-ui | agent/antigravity-ui | `Components/Pages/`, `Components/Layout/` (nav links only) | OPEN |
| 6 | Inquiry form UI (`Request.razor`) | antigravity-ui | agent/antigravity-ui | `Components/Pages/Request.razor` — calls backend service, no own persistence | BLOCKED on #3 |
| 7 | Admin inbox + CRUD UI (`Admin/`) | antigravity-ui | agent/antigravity-ui | `Components/Pages/Admin/` — reads backend models, sits behind backend's `[Authorize]` | BLOCKED on #1 |

## Shared contracts (land first — change only as its own task, never as a side effect)

- `Clinic` / `Procedure` / `Package` / `Inquiry` entity shapes — defined in #1/#3, consumed by #5/#7.
- Inquiry submit signature — defined in #3, consumed by #6.
- Rule: need a contract change? Report it; opencode-db merges it; UI rebases. Never redefine a contract locally.

## Protocol

- Claim a row (Status → IN PROGRESS) before editing; release when merged.
- Rebase on `master` + `dotnet build` green before declaring done.
- Merge one branch at a time into `master`; run build + smoke test between merges.
- Predict conflicts without touching disk: `git merge-tree --write-tree master <branch>`.
- Blocked rows: build UI with a clearly-marked `TODO(contract #N)` + stub, report what's blocked.
- Demo credentials + run instructions go in README (backend adds backend part, UI adds UI part — coordinate).
