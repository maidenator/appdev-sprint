---
name: blazor-ui
description: Blazor Server adapter for non-generic UI. Maps design tokens to wwwroot CSS and Razor CSS isolation, overrides Bootstrap defaults and Blazor template chrome in appdev-sprint.
---

# Blazor UI adapter — net8.0 Razor Components, InteractiveServer

Stack is Blazor Server only. No React / Next.js / Tailwind. This skill adapts `frontend-design` + `frontend-design-deslop` output to this repo.

Project: `C:\Users\L23X17W21\appdev-sprint` — `Components/`, `Pages/`, `wwwroot/app.css` + `bootstrap/`.

## Token mapping (BEFORE any markup)

1. Read `DESIGN.md` at project root if present, else read `.opencode/skills/frontend-design-deslop/references/token-core.css`.
2. Emit tokens to `wwwroot/css/tokens.css` (create dir if missing), import first in `Components/App.razor` `<head>` before `app.css` + `bootstrap`:
   - `--bg, --surface, --fg, --muted, --border, --accent, --accent-fg, --success, --warning, --error`
   - `--font-display, --font-body`, type scale ratio + 6 steps, `--space-base`, max 2 radii, ONE shadow approach
   - Colors in OKLCH. Near-black not pure black, off-white not pure white.
3. `wwwroot/app.css` and all `*.razor.css` reference only `var(--*)`. No scattered hex/px. Existing `--cebu-*` vars stay — map new tokens onto them, don't duplicate systems.

## Blazor overrides

- Kill template chrome: no sidebar `NavMenu` with Home/Counter/Weather + `top-row` + centered `max-width` column as the layout. `Components/Layout/MainLayout.razor` gets one intentional grid + one brief-specific layout move.
- Bootstrap is reset, not design: override `.btn-primary, .form-control, .card, .alert, .navbar` from tokens. No default `#1b6ec2 / #258cfb` blue, no Helvetica/Arial as primary face, no identical rounded cards with `rgba(0,0,0,.1)` shadow everywhere.
- Components are `.razor` + `.razor.css` isolation. Every interactive component ships full state matrix: default, hover, active, focus-visible, disabled, loading, error, empty. Forms use real `<label>`, correct `Input*` types, inline validation that keeps input — no placeholder-as-label.
- Motion: CSS only, `transform` + `opacity`, ease-out <300ms, `prefers-reduced-motion` respected. No animate-everything, no layout-property animation.
- Copy: plain verbs, sentence case, CTA says what happens ("Save changes" → toast "Saved"). Errors explain fix, never vague apology.

## Workflow

1. Load `frontend-design` for direction, `frontend-design-deslop` Phases 0-1 for DESIGN.md/tokens.
2. Apply this adapter for file placement.
3. Load `web-design-guidelines` to audit changed `.razor`/`.css` — fix `file:line` findings before done.
4. If no live screenshot available, run bare-structure check: would this layout read as generic with all styling removed? If yes, change skeleton, not just colors.
