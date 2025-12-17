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

<img width="828" height="466" alt="image" src="https://github.com/user-attachments/assets/9aae1716-13a7-459f-9022-7239eb852755" />

<img width="824" height="456" alt="image" src="https://github.com/user-attachments/assets/d4eaf2ca-b693-4ff0-9333-1b4c9859de37" />

<img width="820" height="468" alt="image" src="https://github.com/user-attachments/assets/ae7018d7-ac40-44f1-9b71-733a3e391cc5" />

<img width="813" height="459" alt="image" src="https://github.com/user-attachments/assets/6c629794-3b0a-4bb4-867e-22c288515ecf" />

<img width="819" height="462" alt="image" src="https://github.com/user-attachments/assets/4f25f637-64c0-42fd-a6c7-61150f9d996d" />

<img width="838" height="471" alt="image" src="https://github.com/user-attachments/assets/3b5b1563-6283-40fc-b713-ce8a42fe66fb" />

<img width="826" height="479" alt="image" src="https://github.com/user-attachments/assets/f039dfb4-f116-43ae-b48b-625e8956f8ea" />

<img width="803" height="462" alt="image" src="https://github.com/user-attachments/assets/ba5d583e-a44f-4edc-bc0a-ba47663944bd" />


## 🛠️ Tech Stack

- **Engine:** Unity 6
- **Language:** C#
- **Rendering:** ShaderLab / HLSL
- **Platform:** Windows (PC)

---

## 📂 Project Structure (Simplified)


