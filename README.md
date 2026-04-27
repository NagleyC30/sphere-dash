# Sphere Dash

A 3D game built in Unity 6 for a final project. Roll your sphere across the arena, collect all the pickups before time runs out, and avoid the enemy.

## How to Play

- **Move:** WASD or Arrow Keys
- **Goal:** Collect all glowing cubes before the timer hits zero
- **Avoid:** The red enemy sphere; if it touches you, you lose
- **Win:** Collect every glowing cubes to advance to the next level
- **Lose:** Run out of time or get caught by the enemy

## Features

- Main Menu with three difficulty settings (Easy, Medium, Hard)
  - Difficulty affects the countdown timer length and enemy speed
- Two complete levels
  - Level 1: 12 glowing cubes, 1 enemy
  - Level 2: 16 glowing cubes, 2 enemies, less time
- Glowing cubes spawn in randomized positions each run
- AI enemy that chases the player
- HUD with live score counter and countdown timer
- Win and lose conditions with restart on failure
- Credits scene
- Built with Unity's new Input System (Unity 6 default)

## Scenes

| Scene | Description |
|-------|-------------|
| MainMenu | Difficulty selection screen |
| Level1 | First level — 12 glowing cubes, 1 enemy |
| Level2 | Second level — 16 glowing cubes, 2 enemies |
| Credits | Credits and asset attributions |

## Scripts

| Script | Purpose |
|--------|---------|
| `PlayerMovement.cs` | WASD/arrow key movement via Rigidbody forces |
| `EnemyChase.cs` | AI that moves toward the player, speed scales with difficulty |
| `GameManager.cs` | Score, timer, win/lose logic, difficulty application |
| `PickUpSpawner.cs` | Randomly places glowing cubes with minimum spacing |
| `PickUpRotator.cs` | Rotates pickup cubes |
| `CameraFollow.cs` | Smooth camera tracking |
| `MainMenu.cs` | Difficulty selection and scene loading |
| `Credits.cs` | Returns to Main Menu |

## Built With

- [Unity 6](https://unity.com/)
- Unity New Input System
- TextMeshPro

## Credits

- CIS 376: Colin Nagley
- Font: [Orbitron](https://fonts.google.com/specimen/Orbitron) by The League of Moveable Type (OFL License)
- All game objects built using Unity primitive shapes. No external 3D assets used
