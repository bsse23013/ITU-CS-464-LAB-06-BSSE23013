# CS464 Lab 06: Enemies, Hazards and Power-ups

Roll number: BSSE23013


This is my Lab 06 submission. It has two parts:

1. The game from Lecture 07, recreated: a rolling ball with patrolling guards, spikes, a crusher, power-ups, hearts, lives and a lose screen with a restart.
2. That game stretched into 5 levels. Every level is its own scene with its own layout, goal and difficulty.

## How to run it

1. Open the project folder in Unity Hub (Unity 6000.3.24f1).
2. Open `Assets/Scenes/Menu.unity` and press Play.
3. Pick a level from the menu.

You can also open any `LevelX` scene directly and press Play. The scenes are in Build Settings in this order: Menu, Level1, Level2, Level3, Level4, Level5. "Next level" just loads the next scene in that list.

## Controls

| Key | What it does |
|-----|--------------|
| WASD or arrow keys | Roll the ball |
| R | Restart the level |
| Esc | Back to the menu |

## The five levels

| # | Scene | Goal | What is in it |
|---|-------|------|---------------|
| 1 | Level1 | Collect all 6 coins | One slow guard going up and down, a patch of spikes, a shield and a speed boost. No timer. |
| 2 | Level2 | Reach the green pad in 30 seconds | Three walls to zig-zag around, two guards, two spike patches. Cyan coins add 3 seconds. |
| 3 | Level3 | Survive 25 seconds | Four crushers that flash orange before they hit, plus two guards walking the lanes between them. |
| 4 | Level4 | Collect 6 gems | Guards that chase you when they see you, and lose you when you break line of sight. Walls give you cover. |
| 5 | Level5 | Collect 5 keys, then reach the goal in 100 seconds | Spikes, a crusher in the middle, a patrolling guard and a chasing guard. The goal stays grey until all keys are taken. |

Difficulty goes up by adding things from the lecture one at a time: a plain patrol first, then a timer, then crushers, then chasing guards, then everything together with a time limit.

## What the game has (checklist from the lab)

- Two or more guards that patrol waypoints (Levels 2 to 5; Level 1 has one slow guard to ease in)
- Spikes (Levels 1, 2, 5)
- A crusher that warns before it strikes (Levels 3 and 5)
- Two power-ups: speed boost (blue cube) and shield (light sphere that blocks one hit)
- Hearts (3) and lives (3), shown at the top of the screen
- A lose screen with Retry, and a win screen for each level

## Scripts

All scripts are in `Assets/`.

| Script | What it does |
|--------|--------------|
| `PlayerController` | Moves the ball with `AddForce`, and holds the speed boost timer |
| `Health` | Hearts, the short invulnerability after a hit, and the shield |
| `GameManager` | One per level scene. Score, lives, timer, respawn, win and lose, and the HUD |
| `MainMenu` | The start screen with a button per level |
| `EnemyPatrol` | Walks a list of waypoints (loop, ping-pong or random) |
| `EnemyGuard` | Patrol, chase and return states, with a sight check that walls can block |
| `Hazard` | Spikes, guard bodies and crushers. Damage plus knockback, and the rest, warn and strike timing for crushers |
| `Collectible` | Coins, gems and keys. Cyan ones add time |
| `PowerUp` | Speed boost and shield pickups |
| `Goal` | The exit pad. Grey while locked, green when open |
| `Spinner` | Makes pickups spin and bob |
| `GameInput` | Reads keys with either the old Input Manager or the new Input System |

The settings for each level (coins needed, time limit, lives) are on the `GameManager` object in that level's scene, so they can be changed in the Inspector without touching code.

## Things I learned or got stuck on

- A shield has to start the invulnerability timer too, otherwise one hit used up the shield and the next frame still hurt.
- Guards check `Vector3 ==` on arrival only because `MoveTowards` lands exactly on the target.
- The respawn has a short delay so a knockback that was already queued cannot hit the new life.
- Time has to be reset to 1 when a scene loads, because the win and lose screens set `Time.timeScale` to 0.

