# Turnfire

## Overview  
Turnfire is a **2D turn-based artillery game** built in **Unity 6.0**.  

This document is structured to emphasize the gameplay logic and implementation details as a game developer portfolio.  

--- 

## How to Play  

1 - **Main Menu**  
   - Press Play then choose between **Singleplayer** or **Multiplayer**.  
 
  2/A - **Singleplayer Setup**  
   - Select the **number of bots** (1–3).  
   - Select the **map**.  
   - Select whether you want to play with a **timer**.  
   - Choose the **bot difficulty**.
   - Play! 
  
  2/B - **Offline (local) Multiplayer Setup**  
   - Select the **number of players** (2–4).  
   - Select the **map**.  
   - Select whether you want to play with a **timer**.  
   - Play! 

2/C - **Online Multiplayer Setup**
   - Create or join a room
   - Select  the **map** as the host.
   - Select whether you want to play with a **timer**.  
   - Wait for every player to join.
   - Play! 

3 - **Gameplay**
  
  - 3.1. **Character Action Phase**
      - Your team's turn starts with a random character
        - Move your character
        - Choose a weapon or item
        - Fire weapon / Use item
     - The next team's turn starts
      ...
      - Continue until all active teams have played this round
  - 3.2. **Package Drop Phase**
     - Packages containing items may drop from the sky
    ...
  - 3.3. **Win Condition**
     - The turns continue until at most one team remains
   
--- 

## Core Features  & Gameplay Systems 

### Characters
- The characters are implemented using **separate state, physics, logic, view and definition classes**. Through the character's interface the state changes (using the character's logic). Reacting to these changes the physics moves the rigibody and the view animates the character.
- There are multiple **character types** available implemented via ScriptableObjects as character definitions.
- Each type has different tactical roles (max health and initial items vary)
- Current types:
  - gunslinger (gun as initial item)
  - grenadier (grenade as initial item)
  - tank (knife as initial item, more health) 

### Map & Terrain System
- The game features **tile-based maps** with a **destructible terrain** system.
- The destructible terrain generation happens asynchronously with the use of coroutines:
   - First, a texture is generated from the initial tilemap as the visual of the terrain. (And the tilemap's renderer gets deactivated).
   - The visual is automatically updated each time an explosion occurs, while temporary hole colliders are placed upon the existing collider.
   - If the number of holes reaches a certain threshold the regeneration of colliders is initiated.
   - The outlines of individual pixel islands are calculated from the texture's current state.
   - Based on the outlines, polygon colliders are generated.
   - When this process finishes the newly made islands become the colliders and the temporary holes are removed (since the generated collider islands now contain the previously made holes). 

### Minimap Generation
- Based on a scene's tilemaps and characters, a minimap sprite can be automatically generated via a custom helper script. 

### Combat & Items
- The game features a 2D physics-based combat system with **ballistic projectiles, melee weapons, armors, and consumables.**
- The projectiles, explosions and explosion holes are all reused using **object pools**.
- Items are designed as two parts: the  **ScriptableObject definitions** hold the data, and **behaviors** contain the logic.
- Ranged weapons fire projectiles or laser beams, while melee weapons attack in place using hitbox and hurtbox colliders.
- Armors can block attacks and enhance the mobility of characters.
- Consumables heal characters.

### Input  
- The game uses **Unity's "New" Input System** for cross-platform input.
- Currently supported inputs: keyboard, mouse, controller.
- Each team is controlled by an input source.
- There are two types of input sources: human and bot.

### Turn Management  
- The turns and character phases are managed via a turn manager state-machine.
- A character's turn is divided into two phases: movement, and item usage phase.
- Turn state logic:
  - The turn manager selects the active team and subscribes to the input source's events while also enabling phase specific input actions. 
  - Then based on the fired input actions the manager passes it to the selected character. 
  - The character then tries to react to the input via a state change. 
  - If the character's state has been changed then the turn manager advances the current turn's phase, which continues and loops until the win condition is reached.

### Camera System 
- The game uses **Cinemachine** for smooth, dynamic camera control.
- Virtual cameras are switched when focusing on different types of objects such as characters, projectiles, packages, lasers and the overall map.

### Graphics & Animation
- The game's art style is pixelated, where the pixel resolution is global to ensure consistency for all objects.
- Every non-static game object is rendered using sprite animations.
- Characters are rendered using multiple layers of sprites to visualize equipped armors, items and type based differences. 
- Animation transitions are coded and state-driven.
- Lasers, trajectories and the sky are visualized using **unlit shaders.**
- All graphic art was made by myself using LibreSprite.

 ### UI & UX
- The UI matches the art style of the game by being pixelated.
- Custom UI scripts are used to make layouts responsive on all devices.

### Audio
- SFX audio resources are object pooled.
- A global audio manager is available to play SFX anywhere.
- All SFX and music were done by myself using Ableton Live Suite 12.

---

## Bots

### Overview
- Singleplayer mode features bots with 3 possible difficulties: easy, medium and hard
- The bots are implemented using **goal-oriented action planning**: The **bot brain** decides the goal, and the **bot controller** handles the action to reach it.
- Bots use different strategies via **bot tunings** based on their difficulty.

### Bot Logic
- First, the bot brain receives the intent to come up with a goal when the turn manager requests for input from the bot's input source.
- Then the goal is calculated depending on the current character phase (movement or item usage). 
- For the movement phase a pre-calculated **jump graph** is used in order to determine the possible positions where the character can jump based on its current position and mobility stats. **Every point is then scored** based on the bot's strategy (tuning) and the best position is then picked using a simple **soft-max function.**
- For the item usage phase **every item is simulated** using the item's behavior. If the item is a weapon then the possible firing angles are iteratively simulated as well. The best item (with the best firing angle) is then picked by evaluating the simulation results. If the character does not have any items then this action is simply skipped.
- The bot tunings on top of strategic parameters (like offense, defense and package greed) include adjustable aim precision and decision randomness parameters to maintain differences between difficulty levels in all character phases.

###  Bot Evaluation
- Bot tuning parameters were first set to an ad-hoc value and then adjusted after continuous evaluation and re-tuning.
- During evaluation around 200 1v1 matches were fast played on all possible maps where every bot difficulty played against every other difficulty. 
- The round evaluation stats include: match outcome (win/tie/lose), suicide count, remaining team health, skipped move count, damage dealt, friendly fire damage dealt, etc.
- Based on the results the tunings were manually adjusted while logical fixes and upgrades were applied.
- As a final result bots with relatively difficult tunings against less difficult tunings tend to converge towards a win/tie/lose ratio of 60/10/30.

---

## Online Multiplayer

### Overview
- Online Multiplayer is implemented via **Netcode for GameObjects** using a **server-authoritative** architecture.

## Room Management
- The multiplayer rooms are created and managed using **Unity Relay** to connect with the use of **join codes.**
- The host sets up the game by choosing the map and the timer, clients join and wait until the game starts.

## Scene Startup
- The gameplay startup is synchronized with the help of a **network gate** that waits for all clients to be ready between different phases of the startup.
- The host initializes, spawns and gives ownership to the network objects.

## Gameplay
- The clients (as owners) use the input source associated with their team to send input requests to the server.
- The server controls the turn manager which receives these inputs and forwards them to the currently active character.
- The character then acts by changing its state.
- Reacting to this change the server moves the character or begins to use the selected item, while clients only visualize the changes using NetworkTransform and the character's view class.
- When using an item, projectiles, lasers and explosions can be spawned which are synchronized the same way: the server spawns and moves them while the clients react visually.
- Turn states are also synchronized using the previously mentioned network gate to ensure the previous state has been finished on all clients before proceeding to the next one.
- Terrain destruction is broadcast by the server to all clients and the visuals are updated locally. (On the client-side the terrain is purely visual since the physics are server-authoritative.)

---

### Notes & Possible Improvements
- This game is completed and available at: https://fleonardo.itch.io/turnfire
- Since the game is turn-based I decided not to use client-side prediction aside from Netcode's built-in NetworkTransform and NetworkRigidbody.
- I might improve upon the bot evaluation later by automating the parameter tuning with a neural network to reach a standard 70/30 win/lose ratio.
