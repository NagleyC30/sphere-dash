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
- [ ] Track "levels unlocked" so progress is remembered (ties into §6 memory).
- [ ] End-of-game / victory scene after the final level (currently just Credits).

## 2. Harder Levels
- [ ] More enemies per level, faster enemies, smarter chase (predict player
      position instead of chasing current position in `EnemyChase`).
- [ ] Shrinking time limits and larger pickup counts as levels climb.
- [ ] Moving hazards: spikes, sweeping walls, falling floors, lava tiles.
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
- [ ] SFX: pickup collect, win, lose, enemy-caught, button clicks, countdown
      tick in final seconds.
- [ ] Background music per scene/theme (menu vs gameplay vs credits).
- [ ] Central `AudioManager` (singleton) + volume handling.
- [ ] Options menu with Music / SFX volume sliders (persist via §6).
- [ ] Use royalty-free / self-made audio; log attributions in README credits.

## 5. Power-Ups / Powers
- [ ] In-level pickups that grant temporary abilities:
      - Speed boost
      - Slow-motion / freeze enemies
      - Shield (survive one enemy hit)
      - Time bonus (+seconds)
      - Magnet (pull nearby cubes)
      - Dash / short burst of speed
- [ ] Powers as an active ability the player triggers (cooldown), vs pickups.
- [ ] Visual + audio feedback when active (particle trail, HUD icon, timer).
- [ ] Balance so powers feel good but harder levels still matter.

## 6. Shop + Memory (Persistence)
- [ ] Currency: earn coins from pickups / level completion / leftover time.
- [ ] Persistent save system — start with `PlayerPrefs` (coins, unlocks,
      settings, high scores, levels completed). Move to a JSON save file if it
      grows.
- [ ] Shop scene to spend coins on:
      - Sphere skins / colors / trails (cosmetic)
      - Permanent upgrades (base speed, starting shield, longer power duration)
      - Unlocking power-ups or new levels/maps
- [ ] Remember purchases + equipped items across sessions.
- [ ] High-score / best-time board per level, persisted.

## 7. Polish & UX (supporting work)
- [ ] Pause menu (resume / restart / quit to menu).
- [ ] Particle + screen-shake feedback on collect / win / lose.
- [ ] Countdown/urgency visuals when timer is low.
- [ ] Settings menu (audio, difficulty default, maybe controls).
- [ ] Controller/gamepad support (already on new Input System).

---

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
