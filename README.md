# Turnfire

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
- Since the game is turn-based I decided not to use client-side prediction aside from Netcode's built-in NetworkTransform and NetworkRigidbody.
