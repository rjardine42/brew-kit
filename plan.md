# Wii SD Setup — Project Plan

A cross-platform TUI tool (C# / Spectre.Console) that prepares an SD card for Wii homebrew: validates/formats the card, fetches the LetterBomb exploit, installs selected homebrew apps, and (eventually) safely builds custom system menu themes.

**Why this exists:** The manual process (wii.hacks.guide) works but is error-prone — bad FAT32 formatting, wrong region selection, and manual file copying are the most common failure points. Prior art (ModMii) is Windows-only and dated. Nothing modern, cross-platform, or terminal-native exists.

---

## Background: the setup flow this tool automates

Canonical order per wii.hacks.guide:

1. **SD card prep** — FAT32, ≤32GB. Formatting issues are the #1 cause of the exploit not appearing.
2. **LetterBomb exploit** (System Menu 4.3) — requires the console's region letter and full MAC address. These are submitted to hackmii.com (or a self-hosted LetterBomb service) to generate a per-console ZIP. Contents go to the SD root; existing `private` folder must be renamed first; HackMii Installer goes on root as `boot.elf`.
3. **On-console steps** (user does these; tool can only guide):
   - Trigger the letter in the Wii Message Board → HackMii Installer
   - Install Homebrew Channel + BootMii (required)
   - NAND backup via BootMii (required)
   - Install Priiloader (required)
   - Install Open Shop Channel (recommended)
   - Install cIOS / d2x (recommended)
4. **Homebrew apps** — live in `SD:/apps/<appname>/`. Essentials: Priiloader installer, d2x cIOS installer, OSC client (LibreShop / Homebrew Browser), USB Loader GX, Nintendont.
5. **Themes** (danger zone) — wrong region/version theme = brick. Safe flow: obtain base theme → build `.csm` from `.mym` via ThemeMii → place in `themes/` folder → install with csm-installer on console.

---

## v1 — Core card setup (MVP)

Goal: from blank card to "insert into Wii and trigger LetterBomb" in one guided session.

- [ ] Detect removable drives; let user pick target card
- [ ] Validate card: size ≤32GB, filesystem is FAT32
- [ ] Offer to format as FAT32 (with loud confirmation — destructive)
- [ ] Prompt for console region (U/E/J/K) with help text on where to find it (Wii Settings, top-right letter)
- [ ] Prompt for MAC address with format validation + help text (Settings → Internet → Console Information)
- [ ] Fetch LetterBomb ZIP (hackmii.com endpoint or self-hosted letterbomb service — decide during spike)
- [ ] Handle existing `private` folder (rename to `privateold`)
- [ ] Extract exploit files + HackMii Installer to card root
- [ ] Copy a small hardcoded set of essentials into `/apps` (Priiloader installer, d2x cIOS installer, OSC installer)
- [ ] Final summary screen: what was written, what to do next on the console

Non-goals for v1: theme support, app browsing, non-4.3 exploits (str2hax, Wilbrand, etc.).

### Technical spikes

- [ ] How to programmatically detect + format FAT32 cross-platform (Windows vs macOS) — likely shelling out to `diskutil` / `format`
- [ ] LetterBomb generation: scrape/POST to please.hackmii.com vs. self-hosting the open-source LetterBomb service vs. bundling the generation logic
- [ ] Where to source pinned versions of the essential apps (GitHub releases, OSC API)

---

## v2 — App picker via Open Shop Channel

Goal: replace the hardcoded essentials with a live, browsable catalog.

- [ ] Integrate the OSC public API (oscwii.org) to fetch the app catalog
- [ ] Multi-select TUI browser: categories, search, descriptions
- [ ] "Recommended" preset (the essentials) vs. "browse everything"
- [ ] Download + extract selected apps into `SD:/apps/<name>/` with correct structure
- [ ] Update/refresh mode: detect apps already on card, offer updates

---

## v3 — Guided checklist + themes

**Guided mode:**

- [ ] Interactive on-console checklist for the steps the tool can't do (Message Board → HackMii → HBC/BootMii → NAND backup → Priiloader → cIOS), with per-step troubleshooting notes (e.g., letter not appearing → check card format; freeze on click → wrong region)

**Themes (system menu):**

- [ ] Build `.csm` from `.mym` theme files (port/replicate ThemeMii logic)
- [ ] Base theme acquisition flow (original menu `.app` for user's exact version/region)
- [ ] **Brick-guard:** hard validation that theme version + region match the console before the file is allowed onto the card
- [ ] Place output in `themes/` + ship csm-installer to `/apps`
- [ ] Prominent warnings; require Priiloader/BootMii confirmation before proceeding

---

## Design principles

- **Forgiving input.** MAC/region entry is where non-technical users stall — validate, explain, never fail silently.
- **Destructive ops are loud.** Formatting and NAND-adjacent steps get explicit confirmations.
- **Safety over convenience.** The tool should refuse to do known-brick-risk things (wrong-region themes) rather than warn and allow.
- **Everything works offline where possible.** Cache downloads; the card-writing steps shouldn't require re-fetching.

## References

- wii.hacks.guide — canonical setup guide (LetterBomb, Priiloader, cIOS, OSC, themes)
- oscwii.org — Open Shop Channel + public API
- ModMii — prior art (Windows)
- Open-source LetterBomb web service implementations on GitHub
