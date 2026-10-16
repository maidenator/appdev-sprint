# DESIGN.md — Cebu Dental Tourism Hub

Source of truth. If CSS / razor / components drift, this wins.

## 1. Discovery

- **Artifact:** marketplace / clinic finder + quote funnel (not marketing-only, not dashboard). Regions: Home map-led finder, Clinics directory, Detail pricing table, Compare side-by-side, Request form, Admin inbox.
- **Audience:** AU/US/UK dental tourists + local Cebu patients. Low trust tolerance, price-sensitive, scanning on mobile.
- **Primary job:** pick a verified clinic with transparent PHP price → request written quote.
- **Adjectives:** clinical, calm, transparent, capable, warm.
- **Essence:** quiet confidence.

## 2. Aesthetic + signature move

**Evolved clinical.** Keep clean trust feel, kill Bootstrap-template tells. Left-aligned editorial rhythm, generous whitespace, one deep-marine ink, one teal, one earned coral accent (selection / best-price only).

**Signature move:** price-ledger strip — tabular-nums PHP price rows with hairline ledger rules + visit-schedule chips. Appears on Home, Clinics cards, Detail table, Compare cards. Nothing else decorates.

## 3. Typography

- Display: `Fraunces`, serif, 600-700, —0.01em, line-height 1.1. Page titles + hero only.
- Body/UI: `Public Sans`, 400/500/600/700, line-height 1.55, max 68ch.
- Scale (1.25 major-third, base 16px): 12.8 / 16 / 20 / 25 / 31.2 / 39.
- Never Inter/Roboto/Helvetica/Arial as primary. Monospace only for `CEB-INQ-0001` refs.
- Numbers: `font-variant-numeric: tabular-nums` on all prices, tables, badges.

## 4. Color (OKLCH, 60-30-10)

| role | hex | oklch | use |
|---|---|---|---|
| bg | #F6F8FA | oklch(0.96 0.005 240) | page |
| surface | #FFFFFF | oklch(1 0 0) | cards |
| fg | #0F1E2E | oklch(0.26 0.04 250) | text |
| muted | #5B6B7F | oklch(0.58 0.04 240) | secondary |
| border | #DFE6EC | oklch(0.90 0.01 240) | hairlines |
| primary-deep | #0A2540 | oklch(0.28 0.06 250) | headings, footer ink |
| primary | #00627D | oklch(0.51 0.08 212) | links, primary buttons |
| accent | #00A896 | oklch(0.68 0.12 175) | focus, selected edge, map active |
| accent-coral | #E76F51 | oklch(0.67 0.14 38) | earned only: selected card ring, best-price |
| gold | #B7791F | oklch(0.63 0.11 75) | warning text on light (AA) |
| success | #047857 | oklch(0.52 0.10 165) | success text |
| warning-bg | #FFFBEB | oklch(0.98 0.02 90) | warning surface |
| error | #B42318 | oklch(0.50 0.13 28) | error text |
| error-bg | #FEF2F2 | oklch(0.97 0.01 20) | error surface |

Rules: accent-coral never as large background. No purple/blue gradients. No glassmorphism. No radial glows.

## 5. Spacing / radius / shadow

- Base 4px. Within-group 8-12px, between-sections 32-48px desktop / 24px mobile.
- Radii (2 max): `--r-card: 10px`, `--r-pill: 999px`. No blob >16px on small cards.
- Shadow (one approach — soft elevation only, never border+shadow stack): `--shadow-1: 0 1px 2px oklch(0.25 0.03 250 / 0.06)`, `--shadow-2: 0 8px 24px oklch(0.25 0.03 250 / 0.08)`. Defined edge via 1px border OR shadow, never both.
- Motion: ease-out, 120ms micro / 220ms panel, transform+opacity only. `prefers-reduced-motion: reduce` disables.

## 6. Craft decisions

- Layout: top header (no sidebar on public), left-aligned content max 1140px, map+ledger split 7/5. Break centered-column reflex.
- Buttons ranked by importance: primary deep-teal solid → secondary outline → tertiary link. Never color-by-meaning.
- Forms: real `<label>`, correct Input types, inline `ValidationMessage` keeps input, error = border + message + icon, not color alone.
- Tables: left text, right tabular-nums prices, light row rules, no center-numeric.
- Icons: single 1.5px stroke set (Bootstrap Icons), no emoji as UI icon. Emoji only in prose, never in badge/chip.
- Imagery: real clinic/metro-Cebu cues, no stock pointing-at-laptop, no gradient blobs.
- Dark mode: out of scope v1. Tokens reserve roles.

## 7. Slop-audit (2026-10-16)

- No Inter/Helvetica display, no purple gradient, no hero+3-equal-cards, no icon-tile grid, no `→` on every CTA (ledger uses text verbs), no eyebrow-above-everything.
- Bare-structure check: ledger + map split reads as clinic finder even unstyled. Pass.
- A11y gate: focus-visible 2px accent ring, 44px targets on CTA, contrast AA (gold/error darkened for AA), keyboard map list mirrors pins.

Changelog: v1 init — evolved clinical, Fraunces+Public Sans, OKLCH ledger system.
- v2 seed: 46 clinics (6 hub archetypes + 40 real directory names/areas, Cebu City → Danao/Carcar). Names/areas real; prices, accreditations, coords (area-approx), contacts (blank → hub form) are demo. Footer carries the disclaimer.
