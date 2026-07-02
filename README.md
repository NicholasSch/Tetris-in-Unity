# 🧱 Tetris Core — Unity Game Engine Implementation

A fully functional, modern reconstruction of the classic tile-matching puzzle game *Tetris*, built natively inside the **Unity Game Engine**. 

This repository contains both a **ready-to-play standalone executable** and the full source code project space for developers looking to modify or study the codebase.

---

## 📄 Project Overview

This implementation focuses on classic Tetris mechanics translated into Unity's component-driven architecture. The game tracks a $10 \times 20$ structural logic grid. As random polyomino shapes (Tetrominos) drop into the active view matrix, players must position and rotate them to form solid horizontal lines. Clearing rows triggers point accumulation and updates global board states, while letting blocks stack to the upper boundary triggers a definitive game-over sequence.

### Core Architecture Features
* **Grid Matrix State Manipulation:** Manages an absolute multi-dimensional coordinate board layer tracking persistent transformations, clearing filled horizontal lines dynamically, and shifting remaining blocks downward.
* **Component-Driven Input Pooling:** Continuously samples local execution loops to process discrete movement vectors, rotations, and drop speed modifiers.
* **Advanced Font & UI Engine:** Employs customized TextMesh Pro shader graphs and custom TrueType Fonts (`.ttf`) to provide clean, crisp, scannable retro text layouts.

---

## 📂 Project Architecture

The core project assets, configurations, and ready-to-run files are organized into the following directory structure:

```text
Tetris Unity/
│
├── Build/                          # 🚀 Ready-to-Play Standalone Executable
│   └── (Contains the pre-compiled game files)
│
└── TetrisOW/                       # 🛠️ Source Code Project Space (Unity Editor)
    ├── Assets/
    │   ├── Scenes/                 # Principal operational gameplay loop scenes
    │   ├── Scripts/                # Component logic handlers (Board, Piece, Tetromino, Grid)
    │   └── Sprites/                # Sprites used for grid and pieces creation
    ├── ProjectSettings/            # Input axes, physics tags, and target engine bindings
    └── Packages/                   # Managed Unity registry dependency clusters
```

---

## 🎮 Game Controls & Mechanics

The game architecture samples standard machine hardware keyboard mappings:

* ⬅️ / ➡️ **Left / Right Arrow Keys:** Shifts the active moving block horizontally across the grid coordinate boundaries.
* ⬆️ **Up Arrow Key:** Rotates the current Tetromino shape $90^\circ$ clockwise, executing bounding box adjustment checks to prevent clipping through walls.
* ⬇️ **Down Arrow Key (Soft Drop):** Increases the falling block speed modifier temporarily to expedite placement.
* ⎵ **Spacebar (Hard Drop):** Instantly locks the active piece down onto the lowest available valid grid coordinates.

---

## 🚀 How to Play & Run the Project

### Option A: Play Instantly (No Unity Required)
You do not need to download or install the Unity Editor to play the game. A pre-compiled build is provided directly in the repository:
1. Clone or download this repository as a `.zip` file to your local machine.
2. Open the root folder and navigate into the `Build/` directory.
3. Double-click the game executable (e.g., `Tetris.exe` or the corresponding system application binary) to launch and play immediately!

### Option B: Open Source Code (For Developers Only)
If you want to tweak, edit, or rebuild the C# source scripts from scratch, open the project in the editor environment:
1. Launch **Unity Hub**, click **Add**, and select the inner `TetrisOW` directory folder.
2. Open the project and wait for the Unity package manager to resolve internal TextMesh Pro dependencies and compile metadata assets.
3. Navigate to `Assets/Scenes/` in the project pane and double-click the main scene file to load the gameplay board layout directly into your editor view window.
