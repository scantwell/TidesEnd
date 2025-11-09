# Quick Debug Setup Guide

## 3-Minute Setup for Weapon Visual Debugging

### What You Get
- 🟡 Yellow/Cyan spheres showing aim point and muzzle position (in Game view!)
- 🟢 Green lines showing bullet trajectory
- 📊 F3 panel with real-time aiming data
- ✅ Works in multiplayer (host and clients)

### Setup (4 steps)

#### 1. Add Debug Components to Player
```
Player Prefab → Add Component → Weapon Debug UI
Player Prefab → Add Component → Weapon Debug Visualizer
```

#### 2. Link to Weapon Manager (Auto-links if on same GameObject)
```
Player → Weapon Manager → Debug → Debug UI = [auto-finds on Player]
Player → Weapon Manager → Debug → Debug Visualizer = [auto-finds on Player]
```

#### 3. Enable on Weapon Prefabs (Optional)
```
Weapon Prefab → Weapon Component → Debug → Show Visual Debug = ☑
```

**Done!** WeaponManager automatically connects everything.

### Usage

1. **Enter Play Mode**
2. **Press F3** to toggle info panel
3. **Fire weapon** and watch:
   - Yellow sphere = where you're aiming
   - Cyan sphere = gun muzzle (spawn point)
   - Green line = bullet trajectory

### How It Works

**Automatic Linking**:
```
WeaponManager.SpawnWeapons()
    ↓
Creates weapon from prefab
    ↓
Calls weapon.SetDebugUI(debugUI)
    ↓
Weapon now has debug UI reference
    ↓
When firing: Updates UI panel with data
```

### Troubleshooting

**Problem**: No spheres appear
- **Fix**: Enable "Show Visual Debug" on weapon prefab

**Problem**: F3 panel doesn't show
- **Fix**: Check WeaponManager → Debug UI is assigned

**Problem**: Panel shows but data doesn't update
- **Fix**: Ensure debugUI is linked in WeaponManager (should auto-link in Awake)

### Benefits of Auto-Linking

✅ **No manual setup per weapon** - WeaponManager handles it
✅ **Works with weapon switching** - All weapons share same UI
✅ **Prefab-friendly** - Set visual debug on prefab, applies to all instances
✅ **Multiplayer-safe** - Each player has their own debug UI
✅ **Hot-reloadable** - Change settings in prefab, see changes immediately

### Advanced: Per-Weapon Debug Settings

Want debug on rifle but not pistol?

1. Open Rifle prefab → Enable "Show Visual Debug"
2. Open Pistol prefab → Disable "Show Visual Debug"
3. WeaponManager still links UI to both
4. Only rifle shows spheres/lines when fired

### Related Documentation

- [VISUAL_DEBUG_SETUP.md](VISUAL_DEBUG_SETUP.md) - Full documentation
- [HYBRID_AIMING_SYSTEM.md](HYBRID_AIMING_SYSTEM.md) - How aiming works
- [WEAPON_ROOT_ROTATION_FIX.md](WEAPON_ROOT_ROTATION_FIX.md) - Weapon rotation system
