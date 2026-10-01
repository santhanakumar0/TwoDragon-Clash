<div align="center">

# 🐉 TWO DRAGON CLASH
### *Rise of the Last Ember*

**A top-down 3D dragon duel set inside an ancient temple arena.**
Fly, claw, and breathe fire against a rival dragon, and watch for a hidden surprise.

![Unity](https://img.shields.io/badge/Unity-6-000000?style=for-the-badge&logo=unity&logoColor=white)
![Language](https://img.shields.io/badge/C%23-Gameplay-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Genre](https://img.shields.io/badge/Genre-3D%20Combat-C0392B?style=for-the-badge)
![Camera](https://img.shields.io/badge/Camera-Top--Down-D4A017?style=for-the-badge)
![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Mobile%20Controls-2980B9?style=for-the-badge)

<img src="docs/screenshots/MainMenu.png" alt="Two Dragon Clash - Main Menu" width="85%"/>

</div>

---

## 📖 About the Game

**Two Dragon Clash** is a 3D dragon combat game built in **Unity**. You play a fiery red dragon and fight a rival blue dragon in a richly detailed temple arena, viewed from a **top-down camera**.

Combat mixes quick melee strikes, a hold-to-charge **flame attack**, and **aerial attacks**. A health bar above each dragon shows who is winning the duel.

> 🎁 **There is one more thing.** A secret spell is hidden somewhere in the game. Finding it is part of the fun.

---

## 🖼️ Screenshots

### 🏯 Main Menu
The first thing a player sees: a temple hall with a guardian statue, an idle blue dragon in the arena, and clear **PLAY** / **QUIT** options.

<div align="center">
<img src="docs/screenshots/MainMenu.png" alt="Main Menu" width="90%"/>
</div>

---

### 🎬 Opening Screen & HUD
Every duel opens with the two dragons at opposite ends of the arena. The HUD is minimal so the arena stays in view:

- 🟢 **Health bars** above each dragon
- 🕹️ **Virtual joystick** (bottom-left) for movement
- 🔘 **Action buttons** (bottom-right) for attacks and abilities

<div align="center">
<img src="docs/screenshots/OpeningScreen_HUD.png" alt="Opening Screen and HUD" width="90%"/>
</div>

---

### ⚔️ Battle Scene
Both dragons clash at the arena's centre. Health bars show the fight in real time.

| Key | Attack | Description |
|:---:|---|---|
| **F** | 🗡️ Light Attack | Fast, low-damage strike for quick pressure |
| **G** | 🐾 Claw Attack | Heavier melee slash |
| **J** *(hold)* | 🔥 Flame Attack | Hold to breathe a continuous stream of fire |

<div align="center">
<img src="docs/screenshots/Battle.png" alt="Battle Scene" width="90%"/>
</div>

---

### 🪽 Fly / Air Attack
Take to the air and strike from above with the aerial attack. Flames and impact effects fire as the dragons collide mid-arena.

<div align="center">
<img src="docs/screenshots/AirAttack.png" alt="Fly / Air Attack" width="75%"/>
</div>

> 🍄 **Secret spell:** a special heal spell is hidden in the game. It is not listed in the controls and is meant to surprise the player. Spoilers are intentionally left out of this README.

---

## ✨ Features

- 🐲 **Dragon vs Dragon duel** against an AI-controlled opponent
- 🎥 **Top-down camera** that keeps the whole arena in view
- ⚔️ **Three attack types:** light, claw, and a hold-to-fire flame breath
- 🪽 **Flying and aerial combat**
- ❤️ **Live health bars** on both dragons
- 🕹️ **Touch-style HUD** with a joystick and action buttons
- 💥 **Visual effects** for flames, hits, and impacts
- 🏯 **Detailed temple arena** with atmospheric lighting
- 🎁 **Hidden surprise spell** for players to discover

---

## 🎮 Controls

| Action | Input |
|---|---|
| Move | Virtual joystick / movement keys |
| Light Attack | `F` |
| Claw Attack | `G` |
| Flame Attack | Hold `J` |
| Fly / Air Attack | On-screen wing button |
| 🎁 Secret | *Discover it yourself* |

---

## 🛠️ Built With

| Tool | Purpose |
|---|---|
| **Unity 6** | Game engine |
| **C#** | Gameplay scripting |
| **Universal Render Pipeline (URP)** | Rendering and lighting |
| **Unity UI** | Menu, HUD, and health bars |
| **Particle System / VFX** | Flame and impact effects |
| **Unity Asset Store** | Dragon models and temple environment |

---

## 🚀 Getting Started

### Prerequisites
- [Unity Hub](https://unity.com/unity-hub) with a **Unity 6** editor installed

### Run the project
1. Clone the repository
   ```bash
   git clone https://github.com/<your-username>/<your-repo>.git
   ```
2. Open **Unity Hub**, click **Add → Add project from disk**, and select the cloned folder.
3. Let Unity import all assets (the first import may take a few minutes).
4. Open the main menu scene from `Assets/Scenes/`.
5. Press **Play ▶️** to start the game.

---

## 🧭 Project Structure

```
Two-Dragon-Clash/
├── Assets/
│   ├── Scenes/        # Main Menu and Battle scenes
│   ├── Scripts/       # Player, enemy AI, health, and attack logic
│   ├── Prefabs/       # Dragons, VFX, and UI prefabs
│   ├── Materials/
│   ├── Animations/
│   └── UI/            # Menu, HUD, and ability icons
├── Packages/
├── ProjectSettings/
├── docs/screenshots/  # Images used in this README
└── README.md
```

> Update the folder names above to match your actual repository before publishing.

---

## 🤖 AI Usage Transparency

This project was built with AI assistance. Meta AI helped with most of the code, Claude with planning and assets, and ChatGPT with ideas, story, and documentation. Fixes such as the flame VFX rotation and the continuous enemy flame bug were done by hand.

---

## 👤 Developer

**Santhanakumar S**
Game design, integration, and testing.

---

## 📝 License

Created for learning and portfolio purposes. Third-party models, textures, and audio belong to their respective creators and are subject to the Unity Asset Store Terms of Service.

<div align="center">

**🐉 Two Dragon Clash: Rise of the Last Ember 🔥**

</div>
