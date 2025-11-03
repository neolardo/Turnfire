# Turnfire

## Overview  
Turnfire is a **2D turn-based artillery game** built in **Unity 6.0**.  

This document is structured to emphasize **gameplay logic and implementation details**, showcasing the use of **design patterns, Unity systems, and scalable architecture** as a game developer portfolio.  

--- 

## How to Play  

1 - **Main Menu**  
   - Choose between **Singleplayer** or **Multiplayer**.  
 
  2/A - **Singleplayer Setup**  
   - Select the **number of bots** (1–3).  
   - Select the **map**.  
   - Select whether you want to play with a **timer**.  
   - Choose the **bot difficulty**.
   - Play! 
  
  2/B - **Multiplayer Setup**  
   - Select the **number of players** (2–4).  
   - Select the **map**.  
   - Select whether you want to play with a **timer**.  
   - Play! 

3 - **Gameplay**
  
  - 3.1. **Character Action Phase**
      - Your team's turn starts with a random character
        - Move your character
        - Choose a weapon
        - Fire weapon
     - The next team's turn starts
      ...
      - Continue until all active teams have played this round
  - 3.2. **Package Drop Phase**
     - Packages containing items may drop from the sky
    ...
  - 3.3. **Win condition**
     - The turns continue one team remains
   
--- 

## Features  & Gameplay Systems 

### Characters
- Separated classes for character data, behavior and animator
- Multiple **character types** per team implemented via **ScriptableObjects**
- Each type has different tactical roles (max health and initial items vary)
- Current types:
  - gunslinger (gun as initial item)
  - grenadier (grenade as initial item)
  - tank (no initial item, but more health) 

### Map & Terrain System
- The game features **tile-based maps** with a **destructible terrain** system.
- The destructible terrain generation happens asynchronously with the usage of coroutines:
   - First, a texture is generated from the initial tilemap as the visual of the terrain. (And the tilemap's renderer gets deactivated).
   - The visual is automatically updated each time an explosion happens, while temporary hole colliders are placed upon the existing collider.
   - If the number of holes reaches a certain threshold the regeneration of colliders is initiated.
   - The outlines of individual pixel islands are calculated from the texture's current state.
   - Based on the outlines, polygon colliders are generated.
   - When this process finishes the newly made islands become the collider and the temporary holes are removed (since the generated collider islands now contain the previously made holes). 

### Minimap Generation
- Based on a scene's tilemaps and characters a minimap sprite can be automatically generated via a custom helper script. 

### Combat & Items
- 2D **physics based** combat system with collision detection
- **Object pools** for projectiles and explosions
- Items are designed in two parts, using the **type object and strategy patterns** for modularity:
  - **ScriptableObject definitions** holding the data, and
  -  **behaviors** holding the logic 

### Input & Platforms  
- **Unity's New Input System** for cross-platform input
- Currently supported inputs: keyboard, mouse, controller 

### Turn Management  
- **State-machine classes** for turn phases and character action phases

### Camera System 
- **Cinemachine** for smooth, dynamic camera control
- Multiple virtual cameras for characters, projectiles, packages and the overall map view 

### Graphics & Animation
- Sprite animations for characters and explosions
- Layered sprites for character types and teams
- Coded, state-driven animation transitions 

### Audio
- Object pooled SFX audio resources
- Global audio manager

### UI/UX  
- Pixel art UI with custom scaler classes 
- Minimal shader code for UI elements

### Bots
- **GOAP (Goal-Oriented Action Planning) bots** for strategic AI decision-making (To be implemented...) 

### Multiplayer
- Offline multiplayer
- Online multiplayer via **Netcode for GameObjects** (To be implemented...) 

--- 

## Notes
- This project is under development.
- The current build (ver0.5) includes a fully implemented local multiplayer version of the game.

--- 

## Next Steps
- Online multiplayer
- Singleplayer with bots
- Consumable items
- More weapons, maps, character types