# Cebu Dental Hub — appdev-sprint

Blazor Server (.NET 8) marketplace for finding dentists across Cebu province:
browse 46 seeded clinics on a Leaflet map, compare transparent PHP pricing,
request an itemized treatment plan, and manage inquiries from the clinic-admin inbox.

> **Demo data.** Clinic *names and areas* are real directory listings. Prices,
> availability, accreditations, coordinates (area-approximate), and contacts are
> **illustrative demo anchors** — always confirm a written quote with the clinic
> in Philippine Pesos (PHP). See `DESIGN.md` for the design system.

## Run

```powershell
dotnet run --urls http://localhost:5000
```

- First start applies EF migrations and seeds the SQLite database (`appdev.db`,
  git-ignored; each checkout keeps its own file).
- Open `http://localhost:5000`. Map tiles load keyless from OpenStreetMap.

## Demo login (dev only)

Seeded by `Data/IdentitySeed.cs` on startup:

| Role        | Email                      | Password   |
|-------------|----------------------------|------------|
| ClinicAdmin | `admin@cebudental.example` | `Admin123!`|

Visit `/admin` while logged in as the demo admin to see the inquiry inbox
and clinic catalog. Do not use these credentials in production.

## What is where

| Path | Owner (per `TASKS.md`) | Contents |
|---|---|---|
| `Components/Pages/`, `Components/Layout/`, `Components/Shared/` | UI | Home, Clinics, Detail, Compare, Request, Admin inbox, map component |
| `Data/`, `Models/`, `Services/`, `Migrations/` | DB | EF Core + SQLite, 46-clinic seed with backfill, catalog + inquiry services |
| `wwwroot/css/tokens.css`, `DESIGN.md` | shared | OKLCH design tokens + design source of truth |
| `tests/appdev-sprint.Tests/` | shared | xUnit suite (seed, catalog, compare math, inquiry flow) |
| `.opencode/skills/` | shared | Agent skills: `frontend-design`, `frontend-design-deslop`, `web-design-guidelines`, `blazor-ui` |

Shared contracts are frozen — `Clinic` / `Procedure` / `Inquiry` shapes and
`InquiryService.SubmitAsync(...)`. Need a contract change? Land it as its own
task; never redefine a contract locally.

## Key flows

- **Finder:** `Home` / `Clinics` — filter by procedure, budget, accreditation,
  city; pin click highlights + auto-scrolls to the clinic card without resetting zoom.
- **Compare:** pick up to 3 clinics, side-by-side price ledger; CTA carries
  `clinicId` + `procedureId` into the quote form.
- **Request:** validated inquiry form (contact, future visit date, consent),
  double-submit guard, procedure preserved across clinic switches.
- **Admin inbox:** metrics, status filter, quote/accept/decline transitions
  behind the `ClinicAdmin` role.

## Verify

```powershell
dotnet build -c Release
dotnet test tests/appdev-sprint.Tests/appdev-sprint.Tests.csproj -c Release
```

13 tests, all green on `master`. Note: stop the running app before a Debug
build — the running exe locks `bin/Debug`.
