# 🎲 BoardGame

**2.5D board game** built with **Unity**, featuring multiple players, characters, and interactive board mechanics.

---

## 🕹️ About

BoardGame is a 2.5D turn-based board game where players roll dice, move across a board, and compete using different characters.  
The project focuses on clean game architecture, reusable systems, and scalable UI/game logic.

---

## ✅ Completed Features

- 🎵 **Global Audio System**
  - Music & SFX handled via a **Singleton SettingsManager**
  - Audio persists across scenes (`DontDestroyOnLoad`)
  - Music volume slider
  - SFX volume slider
  - Music on/off toggle
  - PlayerPrefs save/load for audio settings

- ⚙️ **Settings System**
  - Unified settings logic
  - UI sliders and toggles synced with saved values
  - No duplicated AudioSources between scenes

- 🖱️ **Multiple Cursor System**
  - Custom cursor styles
  - Cursor switching logic implemented

- 🎮 **Main Menu Core**
  - Start & Quit buttons
  - Background music
  - UI layout prepared for animations

- 🎲 **Board Scene**
  - 2.5D board layout
  - Dice system (rollable/throwable)
  - Player spawn points
  - Basic camera setup

- 🧠 **Game Architecture**
  - Central GameManager
  - ScriptHolder for shared logic
  - Scene-based flow (MainMenu → Level → etc.)

---

## 🚧 To-Do List (Updated)

### UI & UX
- [x] Main menu animations
- [x] Leaderboard UI
- [x] Settings panel animations
- [x] UI sound feedback polish

### Gameplay
- [x] Character selection screen with animations
- [x] Full turn-based logic for multiple players
- [x] Player movement animations
- [x] Special tiles (bonus, trap, ladder, etc.)
- [x] Win / lose conditions

### Systems
- [x] Audio Mixer integration
- [x] Master volume support
- [x] Save / Load game state
- [x] AI players
- [x] Input rebinding

### Camera
- [x] Dynamic game camera
- [x] Smooth transitions between turns
- [x] Camera focus on active player

### Scenes
- [x] Leaderboard scene
- [x] Settings scene separation (optional)
- [x] End game / results scene

---

## 🛠️ Tech Stack

- **Engine:** Unity 6
- **Language:** C#
- **Rendering:** ShaderLab / HLSL
- **Platform:** Windows (PC)

---

## 📂 Project Structure (Simplified)


