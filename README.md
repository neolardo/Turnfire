# Turnfire - Game Design Document

## Overview  
Turnfire is a **2D turn-based artillery game** built in **Unity 6.0 LTS** with support for **singleplayer (AI bots)** and **multiplayer (Netcode for GameObjects)**. The game combines classic artillery mechanics with tactical team-based play, procedural map alterations, and  Unity features such as **Shader Graph, VFX Graph, UI Toolkit, Cinemachine, and the new Input System**.  

The GDD is structured to emphasize **gameplay logic and implementation details**, showcasing the use of **design patterns, Unity systems, and scalable architecture** as a game developer portfolio.  

---

## How to Play  

1. **Main Menu**  
   - Choose between **Singleplayer** or **Multiplayer**.  

2. **Singleplayer Setup**  
   - Select the **number of teams** (e.g., 2–4).  
   - Choose the **team type**.  
   - Select the **map**.  
   - Set the **time limit per turn**.  
   - Choose the **AI bot difficulty** (Easy, Normal, Hard).  
   - Play! 

3. **Multiplayer Setup**  
   - Choose to **Host** or **Join** a game.  
   - Host sets up:  
     - **Number of teams**  
     - **Team type**  
     - **Map**  
     - **Time limit per turn**  
   - Other players can join via the **lobby system**.
   - Play!  

---

## Core Gameplay Loop  

1. **Turn Phase**  
   - Player 1 moves → Player 1 fires  
   - Player 2 moves → Player 2 fires  
   - ...
   - Continue until all active players have completed their actions.  

2. **Item Phase**  
   - Items spawn or fall from the sky.  
   - Certain items may modify the terrain or alter gameplay (e.g., bombs, terrain manipulators).  

3. **Next Turn**  
   - New turn starts after all players have moved and items resolved.  

4. **End Condition**  
   - The game ends when only **one player/team remains**.  

---

## Features  

### Characters & Teams  
- Multiple character classes per team.  
- Each class has different tactical roles (movement range, number of shots, initial items vary)  
- Class types: #TODO

### Map & Environment  
- **Tile-based maps** with destructible terrain.  
- Terrain visuals handled via **Shader Graph and custom shaders**.  
- Mesh recalculated and smoothly interpolated when terrain changes.  
- Items and projectiles can **modify terrain dynamically**.  

### Singleplayer & AI  
- **GOAP (Goal-Oriented Action Planning) bots** for strategic AI decision-making.  
- AI considers movement, aiming, terrain, and item use.  

### Multiplayer  
- **Netcode for GameObjects** for online play.  
- Synchronization of:  
  - Player turns  
  - Projectile physics  
  - Terrain changes  
  - Item spawns  

### Input & Platforms  
- **Unity New Input System** for cross-platform input.  
- Supports **mobile (touch controls)** and **desktop (mouse/keyboard/controller)**.  

---

## Gameplay Systems  

### Turn Management  
- Implemented with a **State Machine**:  
  - **Move State → Shoot State → Move State → ... → Shoot State → Item Spawn State → Check Win Condition → Next Turn**.  
- Ensures deterministic flow across singleplayer and multiplayer.  

### Combat & Physics  
- Weapons fire projectiles.  
- **Physics vs. Scripted System** (to be prototyped and tested):  
  - *Physics-based*: realistic trajectories, collision detection.  
  - *Script-based*: deterministic calculations, easier multiplayer sync.  

### Terrain System  
- Uses **tile-based map with mesh interpolation**.  
- Modified dynamically by:  
  - Projectiles  
  - Explosions  
  - Terrain-altering items  
- Optimized with **Flyweight Pattern** to reduce memory overhead for tiles.  

### Items & Weapons  
- Defined as **ScriptableObjects**:  
  - Store stats, effects, and visuals.  
  - Trigger **C# events** on use (e.g., `OnItemUsed`, `OnTerrainChanged`).  
- Weapons/items created using the **Type Object Pattern** for flexible extension.  

### Camera System (Cinemachine)  
- Uses **Unity Cinemachine** for smooth, dynamic camera control.  
- Automatically adjusts framing based on:  
  - Active player  
  - Projectile trajectories  
  - Explosions or terrain changes  
- Provides **smooth transitions and zooming** across different devices.  
- Ensures readability on **mobile and desktop** without manual camera coding.  

---

## Technical Design  

### Key Patterns & Architectures  
- **Component Pattern** – -Gameplay objects (movement, shooting, health).  
- **Strategy Pattern** – Different classes with different initial capabilities / strategies.  
- **State Machine** – Turn flow control and AI decision states.  
- **Flyweight Pattern** – Optimize tile storage and repeated visual assets.  
- **Object Pooling** – Projectiles, VFX, and items reused to minimize instantiation cost.  
- **Type Object Pattern** – Weapons/items as extensible types without hardcoding.  
- **ScriptableObjects + Events** – Decouple game logic from data definitions.  

### Animation & VFX  
- **Sprite animations** for characters and weapons.  
- **Coded animation transitions** (state-driven).  
- **VFX Graph** for explosions, smoke, and item drops.  

### Rendering  
- **Universal Render Pipeline**
- **Shader Graph** for terrain blending and materials.  
- **Custom shaders** for deformable terrain and special effects (e.g., glowing pickups).  

### UI/UX  
- **UI Toolkit** for menus, HUD, turn indicators, and inventory management.  
- Cross-platform scaling for desktop/mobile.  

### Networking  
- **Netcode for GameObjects** manages:  
  - Turn sync  
  - Movement and projectile trajectory sync  
  - Terrain modification sync   

---
