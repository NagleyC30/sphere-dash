# Sphere Dash — Next Steps (Private Dev Notes)

Personal roadmap / scratchpad for where to take the game next. Not player-facing.
Grouped by theme, roughly ordered from "easiest wins" to "bigger systems."

Current state (baseline):
- 2 levels, 3 difficulties (Easy/Medium/Hard) affecting timer + enemy speed.
- Scripts: `PlayerMovement`, `EnemyChase`, `GameManager`, `PickUpSpawner`,
  `PickUpRotator`, `CameraFollow`, `MainMenu`, `Credits`.
- No sound, no persistence, no shop, no power-ups yet.

---

## 1. More Levels
- [ ] Add Level 3, 4, 5+ (each its own scene: `Level3`, `Level4`, ...).
- [ ] Level-select screen so you don't have to beat them in order.
- [ ] Data-drive levels instead of copy-pasting scenes: a `LevelConfig`
      ScriptableObject (pickup count, enemy count, time limit, map ref) so a
      single "Gameplay" scene can load any level. Big refactor but scales well.
- [x] Track "levels unlocked" so progress is remembered (`SaveManager`, §6).
- [x] Level-select screen script (`LevelSelect.cs`) — gates buttons by unlock
      progress, shows best score. Needs a scene + buttons wired in the editor.
- [ ] End-of-game / victory scene after the final level (currently just Credits).

## 2. Harder Levels
- [ ] More enemies per level, faster enemies, smarter chase (predict player
      position instead of chasing current position in `EnemyChase`).
- [ ] Shrinking time limits and larger pickup counts as levels climb.
- [ ] Moving hazards: spikes, sweeping walls, falling floors, lava tiles.
- [x] Smarter chase — `EnemyChase` now leads the player (predicts where
      they're heading via velocity, `leadFactor`) instead of chasing the current
      position. Freeze/shield-aware.
- [ ] Enemy variety: patrollers, ambushers, ones that split when close.
- [ ] Optional "endless" / survival mode with escalating difficulty.
- [ ] Per-level star rating (time remaining / no-damage) to reward mastery.

## 3. Maps / Arenas
- [ ] Distinct arena layouts instead of one flat plane — walls, ramps, pits,
      teleporters, narrow bridges.
- [ ] Themed environments (neon grid, ice, volcano, space) — pairs with music.
- [ ] Hazard zones and safe zones baked into the geometry.
- [ ] Consider a simple modular tile/prefab kit so new maps are fast to build.
- [ ] Make `PickUpSpawner` respect map geometry (don't spawn inside walls/pits).

## 4. Sounds & Music
- [x] Central `AudioManager` (singleton, DontDestroyOnLoad) + volume handling. (`AudioManager.cs`)
- [x] SFX hooks wired: pickup collect, win, lose, button clicks, countdown tick
      in the final 5 seconds. (GameManager / MainMenu / Credits)
- [x] Background music per scene (menu music + per-level music fields).
- [ ] Assign actual audio clips in the Inspector (see "Editor setup" below).
- [ ] Enemy-caught SFX (currently the lose sound covers this).
- [x] Options menu script with Music / SFX volume sliders (`OptionsMenu.cs`) —
      persists via `SaveManager`, applies live to `AudioManager`. Needs sliders
      wired in the editor.
- [ ] Use royalty-free / self-made audio; log attributions in README credits.

## 5. Power-Ups / Powers
- [x] In-level pickups that grant temporary abilities (`PowerUp.cs` +
      `PlayerAbilities.cs`):
      - [x] Speed boost (multiplies player speed)
      - [x] Freeze enemies (they stop chasing + can't catch)
      - [x] Shield (absorbs one enemy hit; timed)
      - [x] Time bonus (+seconds on the clock)
      - [x] Magnet (pulls nearby cubes toward the player → auto-collect)
      - [ ] Dash / short burst of speed
- [ ] Powers as an active ability the player triggers (cooldown), vs pickups.
- [ ] Visual + audio feedback when active (particle trail, HUD icon, timer) —
      pickup SFX plays; still needs visuals + a shield-break sound.
- [ ] Balance pass (durations/multipliers are placeholder values).

## 6. Shop + Memory (Persistence)
- [x] Persistent save system — `SaveManager.cs` wraps `PlayerPrefs` for coins,
      level unlocks, audio settings, best score + best time per level. Single
      swap-point if we move to a JSON file later.
- [x] Currency earned on win: coins from pickups collected + leftover time
      (`coinsPerPickup`, `winTimeBonusCoins` in GameManager). Coins persist.
- [x] Level-unlock tracking (`UnlockLevel` / `IsLevelUnlocked`) — ready for a
      level-select screen to read.
- [x] Shop (`Shop.cs`) that spends coins on items that apply in-game:
      - [x] Sphere skins (cosmetic color, `PlayerLoadout` applies equipped color)
      - [x] Permanent speed upgrade (stackable levels, `PlayerLoadout`)
      - [x] "Start with a shield" one-off (honored in `PlayerAbilities.Start`)
      - [ ] Trails / longer power durations / more skins (extend the item list)
- [x] Remember purchases + equipped items across sessions (`SaveManager`:
      `IsOwned`, `GetUpgradeLevel`, `EquippedColor`).
- [ ] High-score / best-time board per level, persisted (data exists via
      `GetBestScore` / `GetBestTime`; needs a UI to display it).

## 7. Polish & UX (supporting work)
- [x] Pause menu script (`PauseMenu.cs`) — Esc to toggle, freezes via
      Time.timeScale, Resume/Restart/Main Menu. Needs a pause panel wired in.
- [ ] Particle + screen-shake feedback on collect / win / lose.
- [ ] Countdown/urgency visuals when timer is low.
- [ ] Settings menu (audio, difficulty default, maybe controls).
- [ ] Controller/gamepad support (already on new Input System).

---

---

## Editor setup for the audio/persistence work (do this in Unity)
The code is in and null-safe (game runs with zero setup — just silent). To hear
sound and finish wiring:

1. **AudioManager object** — in the **MainMenu** scene, create an empty
   GameObject named `AudioManager`, add the `AudioManager` component. It
   survives scene loads (DontDestroyOnLoad), so it only needs to exist in the
   first scene. Leave the two AudioSource fields empty and it auto-creates them,
   or add two AudioSource components and assign them.
2. **Assign SFX clips** on that component: Pickup, Win, Lose, Button,
   CountdownTick. Any left empty simply won't play (no error).
3. **Music (optional)** — set `Menu Music` on the MainMenu object's `MainMenu`
   component, and `Level Music` on each level's `GameManager`.
4. **Nothing else needed for saving** — coins, unlocks, best score/time, and
   volume all persist automatically via `SaveManager`.

Persistence is testable now (before any audio clips exist): win a level, then
`SaveManager.Coins` / `GetBestScore` / `HighestLevelUnlocked` will be populated.
`SaveManager.ResetAll()` wipes everything for a clean test.

### Menu scripts (need UI wired in the editor)
These are coded and consume the audio/persistence layer; each needs a scene/UI:
- **OptionsMenu** (`OptionsMenu.cs`) — add to an options panel, assign the Music
  and SFX `Slider`s (set their range to 0–1). Listeners are hooked in code.
- **LevelSelect** (`LevelSelect.cs`) — new "LevelSelect" scene or panel. Fill the
  `levels` array: one entry per level with its Button, level number (1-based),
  and scene name (optional best-score TMP label). Locked levels auto-disable.
  Add a button on MainMenu to open it.
- **PauseMenu** (`PauseMenu.cs`) — add to a level, assign a hidden pause panel,
  wire its Resume/Restart/Main Menu buttons to the matching public methods.
  Esc toggles it. (It uses `Time.timeScale`, so it freezes the whole game.)

### Power-ups (need a prefab + placement in levels)
- Add `PlayerAbilities` to the **player** object (next to `PlayerMovement`).
- Make a **power-up prefab**: a small object with a **trigger collider**, the
  `PowerUp` component, and a `Type` chosen in the Inspector. Give it its own tag
  (e.g. "PowerUp") or leave it untagged — but **never tag it "PickUp"** (that tag
  is scored/collected by `PlayerMovement`).
- Place power-up prefabs in a level, or spawn them (the `PickUpSpawner` pattern
  works — point it at the power-up prefab in a second spawner object).
- Tuning lives on the components: `speedMultiplier`, `magnetRadius`,
  `magnetPullSpeed` on `PlayerAbilities`; `duration` / `timeBonusSeconds` per
  `PowerUp`. Current numbers are placeholders — balance to taste.

### Shop (need a Shop scene + PlayerLoadout on the player)
- Add **`PlayerLoadout`** to the player object (next to PlayerMovement /
  PlayerAbilities). It applies the equipped skin color and speed upgrade at
  level start; auto-finds the Renderer if you don't assign one.
- Make a **Shop** scene (add it to Build Settings) with a `Shop` component.
  Fill the `items` array — each entry: a stable `id`, `Kind` (Skin / Upgrade /
  OneOff), `cost`, a Buy `Button`, and optional status label. For skins set
  `skinColor`; for the start-shield use `Kind = OneOff` and `id = "startshield"`;
  for the speed upgrade use `Kind = Upgrade` and `id = "speed"`.
- Assign a coins label; add a Back button wired to `BackToMenu`, and a button on
  MainMenu that loads the "Shop" scene.
- Coins come from winning levels automatically — no setup needed to earn them.
- Skin color uses `Material.color`, which maps to the URP/Lit main color. If a
  skin doesn't visibly change, give the player a material whose shader exposes a
  standard base color.

### Smarter enemy (no setup — tune only)
`EnemyChase` now leads the player. Tune `leadFactor` per enemy in the Inspector
(0 = old dumb chase, ~0.5 default, higher = anticipates more aggressively).

## Suggested build order
1. Sounds + AudioManager (fast, big feel improvement).
2. PlayerPrefs persistence + high scores (foundation for shop/unlocks).
3. Power-ups in existing levels.
4. Shop scene spending earned coins.
5. Data-driven levels (`LevelConfig`) + level select.
6. New maps/arenas + harder enemy AI.

## Open questions / decisions
- Keep separate scenes per level, or refactor to one data-driven gameplay scene?
  (Refactor early — it makes "more levels" and "maps" far cheaper later.)
- PlayerPrefs vs JSON save file — start simple, migrate if needed.
- How much is cosmetic vs gameplay-affecting in the shop? (Keep it fun, not
  pay-to-win — it's single-player, so lean toward player expression + upgrades.)
