# Galapagos: Vampire Survivors-like Project

A 2D roguelike game inspired by Vampire Survivors, set in an infinite sea where a fisherman battles waves of sea monsters using harpoons and various weapons.

## 🎮 Game Overview

- **Genre:** 2D Roguelike / Survival
- **Engine:** Unity 6000.0.42f1
- **Platform:** PC (Windows)
- **Visual Style:** Flat 2D aesthetic (hand-drawn style on paper) with an infinite sea tilemap.

## ✨ Key Features

- **Infinite Map:** Seamlessly looping sea environment.
- **Weapon System:**
  - Mouse-directed attacks (Melee & Ranged).
  - Auto-targeting "Scanner" system to locate nearest enemies.
- **Optimization:**
  - **PoolManager:** Efficient object pooling for monsters, projectiles, and effects to maintain high performance with large enemy counts.
- **Dynamic Spawning:**
  - Weighted spawn system via `Spawner` and `SpawnData`.
  - Separate logic for general mobs and special elite monsters.

## 🦀 Monsters

The game features various enemies with distinct behaviors implemented via State Machines:

### 1. Basic Enemies
- **Chasers:** Simple logic that relentlessly follows the player.

### 2. Special Enemies (State Machine Based)
- **BombCrab:** Tracks the player, stops within range, and lobs a bomb that explodes after a delay. Features a "bobbing" animation when the bomb lands in the water.
- **DrillCrab:** Detects the player and charges rapidly after a warning indication.
- **ShootingFish (WIP):** A ranged enemy (Lionfish concept) that fires a spread of spines.

## 🛠️ Technical Implementation

- **GameManager:** Central hub managing game state, time, player survival, levels, and experience.
- **Input System:** Uses Unity's new Input System for responsive controls.
- **Visual Effects:**
  - Custom shaders (`RadialFill`, `LineFill`) for attack warning indicators.
  - specialized particle effects for explosions and hits.

## 🚀 Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/YourUsername/Galapagos_VampireSurvivors.git
   ```
2. Open the project in **Unity 6000.0.42f1** or later.
3. Open the `Assets/Scenes/SampleScene.unity` scene.
4. Press **Play** to start.

## 📝 Recent Updates
- Implemented state patterns for `BombCrab` and `DrillCrab`.
- Added custom shaders for warning indicators.
- Refactored `EnemyStats` for centralized stat management.
- Fixed physics issues with enemy knockback and sliding.

---
*Developed by Team Galapagos*
