# 🤖 First Game Project

A third-person 3D platformer game built with **Unity 6**, featuring a robot character navigating levels, collecting stars, and respawning on death falls. Created as a learning project using Unity''s Starter Assets framework.

---

## 🎮 Gameplay Overview

- Control a **robot character** (Timmy Robot) through a 3D environment
- **Collect stars** scattered throughout the level
- Avoid falling off platforms — the player **respawns** at the start if they fall below the level
- A **HUD counter** tracks remaining collectibles in real time
- Dynamic environment with **moving platforms**, **stairs**, and **wall lights**

---

## 🛠️ Built With

| Tool / Package | Version |
|---|---|
| Unity Editor | 6000.5.5f1 (Unity 6) |
| Universal Render Pipeline (URP) | 17.5.0 |
| Input System | 1.19.0 |
| Cinemachine | 3.1.6 |
| TextMesh Pro (via UGUI) | 2.5.0 |
| Unity Learn IET Framework | 6.0.6 |

---

## 📁 Project Structure

```
first game proj/
├── Assets/
│   ├── Audio/                  # Global audio assets
│   ├── Materials/              # Shared materials
│   ├── Prefabs/                # Reusable prefabs
│   │   ├── Collectible_Star    # Spinning/bobbing star pickup
│   │   ├── Moving_Platform     # Animated moving platform
│   │   ├── PlayerRobot         # Player character prefab
│   │   ├── Stairs              # Stair geometry
│   │   └── Wall_Light_Left/Right
│   ├── Scenes/
│   │   └── GetStarted_Scene    # Main game scene
│   ├── Skyboxes/               # Skybox assets
│   ├── SourceFiles/
│   │   ├── Animation/          # Animation controllers & clips
│   │   ├── Fonts/              # Custom fonts
│   │   ├── InputSystem/        # Input action maps
│   │   ├── Materials/          # Source materials
│   │   ├── Models/             # 3D models
│   │   ├── Resources/          # Runtime-loaded resources
│   │   ├── Scripts/            # C# game scripts (see below)
│   │   ├── Settings/           # URP renderer settings
│   │   ├── SoundFX/            # Sound effect clips
│   │   ├── StarterAssets/      # Unity Starter Assets
│   │   ├── Textures/           # Texture maps
│   │   ├── TimmyRobot/         # Robot character assets
│   │   └── VFX/                # Visual effects
│   ├── TextMesh Pro/           # TMP resources
│   ├── Tutorials/              # Unity tutorial assets
│   ├── UI/                     # UI assets and layouts
│   └── VFX/                    # Particle effects
├── Packages/
│   └── manifest.json           # Package dependencies
├── ProjectSettings/            # Unity project configuration
└── README.md
```

---

## 📜 Scripts

All custom scripts are located in `Assets/SourceFiles/Scripts/`.

### `ThirdPersonController.cs`
The core player movement controller (from Unity Starter Assets, extended).
- Walk, sprint, jump, and fall mechanics
- Camera-relative movement using Cinemachine
- Footstep and landing audio integration
- Exposes `ResetCameraRotation()` for respawn support

| Parameter | Default | Description |
|---|---|---|
| `MoveSpeed` | 2.0 m/s | Normal movement speed |
| `SprintSpeed` | 5.335 m/s | Sprint movement speed |
| `JumpHeight` | 1.2 | Jump height |
| `Gravity` | -15.0 | Custom gravity value |

---

### `Pickup.cs`
Handles collectible star behaviour.
- Continuously **rotates** and **bobs** up and down
- On player collision: spawns a **particle effect** and destroys the star

| Parameter | Default | Description |
|---|---|---|
| `rotationSpeed` | 100°/s | Spin speed |
| `bobbingAmount` | 0.1 | Vertical bob amplitude |
| `bobbingSpeed` | 1.0 | Vertical bob frequency |

---

### `UpdateCollectibleCount.cs`
Updates a **TextMeshProUGUI** element with the current number of remaining collectibles.
- Dynamically finds all `Pickup` objects in the scene each frame
- Displays: `Collectibles remaining: N`

---

### `RespawnPlayer.cs`
Handles player death and respawning when the player falls below a Y threshold.
- Detects when `transform.position.y < yThreshold` (default: `-5`)
- Teleports player back to spawn position/rotation
- Resets camera rotation via `ThirdPersonController`
- Resets vertical velocity using reflection
- Plays a respawn sound effect

---

### `MotionAudioController.cs`
Controls audio playback tied to player movement states (footsteps, landing).

---

## 🚀 Getting Started

### Prerequisites
- [Unity 6](https://unity.com/releases/editor/whats-new/6000.5.5) (version `6000.5.5f1`) — download via [Unity Hub](https://unity.com/unity-hub)

### Opening the Project
1. Clone or download this repository
2. Open **Unity Hub**
3. Click **Open** → select the `first game proj` folder
4. Unity will import all assets automatically (this may take a few minutes on first open)
5. Open the scene: `Assets/Scenes/GetStarted_Scene.unity`
6. Press **Play** ▶️ to run the game in the editor

### Controls

| Action | Keyboard / Mouse |
|---|---|
| Move | `W` `A` `S` `D` or Arrow Keys |
| Sprint | Hold `Left Shift` |
| Jump | `Space` |
| Look / Camera | Mouse |

---

## 🎨 Assets & Credits

- **Player Character**: Timmy Robot (custom model)
- **Movement System**: Based on [Unity Starter Assets – Third Person Controller](https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526)
- **Rendering**: Universal Render Pipeline (URP)
- **Camera**: Cinemachine Virtual Camera

---

## 📝 License

This project was created for learning purposes. Starter Assets are subject to [Unity''s Asset Store Terms of Service](https://unity.com/legal/as-terms).
