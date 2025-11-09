# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DeadDrop is a multiplayer FPS game built with Unity 6000.2.8f1 using Unity Netcode for GameObjects. The game supports both local networking and Steam P2P multiplayer via the Facepunch transport.

## Documentation Structure

This project maintains comprehensive documentation in the `.agent` folder:

- **Tasks**: PRD & implementation plans for each feature
- **System**: Documents the current state of the system (project structure, tech stack, integration points, etc.)
- **SOP**: Best practices for executing certain tasks

**Before planning any implementation, always read [.agent/README.md](.agent/README.md) first to get context.**

After implementing features, update the `.agent` docs to reflect the current state of the system.

## Unity Development Commands

### Opening the Project
- Open Unity Hub and load the project from `f:\Code\Unity\DeadDrop`
- Unity version required: **6000.2.8f1**

### Building
- Build from Unity Editor: File > Build Settings
- Select platform (PC, Mac & Linux Standalone)
- Click "Build" or "Build and Run"

### Testing
Unity uses Test Framework for unit and integration tests:
- Window > General > Test Runner
- Run PlayMode tests for gameplay features
- Run EditMode tests for editor tools and utilities

## Project Structure

### Assets Organization

```
Assets/
├── Scripts/
│   ├── Runtime/           # Core runtime systems (SteamNetworkManager, TransportSwitcher, UINetworkManager)
│   ├── Player/            # Player movement, combat, and networking (NetworkedFPSController, PlayerCombat)
│   ├── Combat/            # Damage system, health, interfaces (IDamageable, IWeapon)
│   ├── Weapons/
│   │   ├── Core/         # Weapon, WeaponManager, WeaponView
│   │   ├── Data/         # WeaponData ScriptableObjects
│   │   └── Effects/      # TracerPool, BulletTracer
│   ├── Enemy/            # EnemyAI
│   ├── Spawning/         # SpawnManager, SpawnPoint
│   └── UI/               # HUDManager, PauseMenu
├── Scenes/               # Game scenes
├── Prefabs/              # Reusable game objects
├── Data/                 # ScriptableObject data assets
├── Materials/            # Materials and textures
└── Settings/             # Project settings
```

## Core Architecture

### Networking Architecture
- **Framework**: Unity Netcode for GameObjects (v2.6.0)
- **Transport Layer**: Dual-mode support
  - Local networking via Unity Transport
  - Steam P2P via Facepunch.Steamworks transport
- **Authority Model**: Server-authoritative for gameplay logic
- **Network Variables**: Used for synchronized state (health, weapon state, etc.)
- **RPCs**: ClientRpc for visual effects, ServerRpc for player actions

### Key Networking Patterns
1. **Server Authority**: All gameplay logic (damage, spawning, death) executes on server
2. **Client Prediction**: Movement and camera handled locally for owning client
3. **State Synchronization**: NetworkVariables automatically sync state to all clients
4. **Remote Procedure Calls**:
   - `ServerRpc`: Client → Server (player inputs, actions)
   - `ClientRpc`: Server → All Clients (effects, notifications)

### Combat System
- **IDamageable Interface**: Implemented by any object that can take damage
- **DamageInfo Struct**: Encapsulates damage amount, type, source, hit location
- **Damage Types**: Normal, Fire, Explosive, Poison, Electric (extensible enum)
- **Health Component**: NetworkBehaviour with server-authoritative health management
  - Supports headshots with configurable radius
  - Optional health regeneration
  - Death/revival system with ClientRpc notifications

### Weapon System
- **WeaponManager**: Manages multiple weapons per player, handles switching
- **Weapon**: Base weapon logic (firing, reloading, ammo)
- **WeaponData**: ScriptableObject for weapon configuration (damage, fire rate, range)
- **WeaponView**: Visual representation and animation
- Weapons are spawned from prefabs at runtime, not directly in the scene

### Player Controller
- **NetworkedFPSController**: Character movement with WASD + mouse look
  - Uses Unity's CharacterController for physics
  - Cinemachine for camera management
  - Owner-only input processing
  - Sprint, jump, gravity
- **Camera Setup**:
  - CinemachineCamera with priority system
  - Only local player's camera is active (priority 10)
  - Remote players have disabled cameras (priority 0)
  - AudioListener enabled only for local player

### Input System
- Uses Unity's new Input System package (v1.14.2)
- Input actions defined in `Assets/InputSystem_Actions.inputactions`
- Keyboard/Mouse input accessed via `Keyboard.current` and `Mouse.current`

## Key Dependencies

From [Packages/manifest.json](Packages/manifest.json):

### Multiplayer
- `com.unity.netcode.gameobjects` (2.6.0) - Core networking
- `com.community.netcode.transport.facepunch` - Steam P2P transport
- `com.unity.multiplayer.tools` (2.2.6) - Profiling and debugging

### Core Unity Packages
- `com.unity.inputsystem` (1.14.2) - New input system
- `com.unity.cinemachine` (3.1.4) - Camera management
- `com.unity.collections` (2.5.7) - Native collections for performance
- `com.unity.burst` (1.8.25) - Burst compiler for performance
- `com.unity.mathematics` (1.3.2) - Math library

### Rendering
- `com.unity.render-pipelines.universal` (17.2.0) - URP rendering

## Development Patterns

### Adding a Networked Component
1. Inherit from `NetworkBehaviour` (not MonoBehaviour)
2. Override `OnNetworkSpawn()` for initialization
3. Use `IsServer`, `IsClient`, `IsOwner` for authority checks
4. Use `NetworkVariable<T>` for synchronized state
5. Use `[ServerRpc]` and `[ClientRpc]` for remote calls
6. Always check `IsServer` before modifying authoritative state

Example:
```csharp
public class MyNetworkedComponent : NetworkBehaviour
{
    private NetworkVariable<int> score = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            score.Value = 0;
        }
    }

    [ServerRpc]
    public void AddScoreServerRpc(int points)
    {
        score.Value += points;
    }
}
```

### Implementing a New Weapon
1. Create WeaponData ScriptableObject in [Assets/Data](Assets/Data)
2. Create weapon prefab with Weapon component
3. Assign WeaponData to Weapon component
4. Add prefab to WeaponManager's weapon list
5. Implement IWeapon if custom weapon logic needed

### Adding Enemy Types
1. Create enemy prefab with Health component
2. Implement EnemyAI or custom AI script
3. Add to SpawnManager's spawn pool
4. Configure spawn points in scene

### Steam Integration
- SteamManager handles Steam API initialization
- TransportSwitcher detects and switches between local/Steam transport
- Steam lobbies and invites handled by Facepunch transport
- Test locally without Steam using Unity Transport

## Common Development Tasks

### Running Multiplayer Tests
1. Build the game first (File > Build Settings > Build)
2. Launch built executable as Host
3. Launch Unity Editor and connect as Client
4. Or use Unity Multiplayer Play Mode for in-editor testing (Window > Multiplayer Play Mode)

### Debugging Networking Issues
1. Enable Netcode logging: NetworkManager > Log Level = Developer
2. Use Multiplayer Tools: Window > Multiplayer > Netcode Profiler
3. Check `IsServer`, `IsClient`, `IsOwner` states
4. Verify RPCs have correct suffix (`ServerRpc` or `ClientRpc`)
5. Ensure NetworkVariables are only modified on server

### Script Compilation
- Unity automatically compiles C# scripts on save
- Check Console window for compile errors
- Assembly definitions (.asmdef) are not used in this project (standard Unity compilation)
