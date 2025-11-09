# Where Debug Components Live

## Quick Answer

Both debug components should live on the **Player GameObject** (same GameObject that has WeaponManager):

```
Player (GameObject)
├── WeaponManager          ← Manages all weapons
├── WeaponDebugUI          ← UI panel (F3 to toggle)
└── WeaponDebugVisualizer  ← 3D spheres and lines
```

## Why This Design?

### Player-Scoped Components
Both debug components are **player-specific**:
- Each player has their own debug UI panel
- Each player sees their own 3D markers
- Remote players don't see your debug visuals

### Singleton Pattern
WeaponDebugVisualizer uses a singleton pattern:
- Only one instance exists per player
- `Instance` property gives access to the active visualizer
- If multiple exist, duplicates are destroyed automatically

### Auto-Linking by WeaponManager
WeaponManager finds both components automatically in `Awake()`:
```csharp
if (debugUI == null)
    debugUI = GetComponent<WeaponDebugUI>();

if (debugVisualizer == null)
    debugVisualizer = GetComponent<WeaponDebugVisualizer>();
```

This means:
- ✅ If on same GameObject → auto-found
- ✅ If manually linked → uses your reference
- ✅ If missing → weapons work fine, debug just disabled

## Setup Instructions

### Recommended: Both on Player
```
Player Prefab:
1. Add WeaponDebugUI component
2. Add WeaponDebugVisualizer component
3. WeaponManager auto-finds both
```

### Alternative: Manual Linking
If you want debug components elsewhere (not recommended):
```
Player Prefab:
1. Create separate GameObject (e.g., "DebugTools")
2. Add WeaponDebugUI to DebugTools
3. Add WeaponDebugVisualizer to DebugTools
4. Manually link both in WeaponManager → Debug section
```

## Multiplayer Considerations

### Each Player Has Own Debug Components
```
Host Player:
├── WeaponManager
├── WeaponDebugUI       ← Host sees this
└── WeaponDebugVisualizer ← Host sees spheres from this

Client Player:
├── WeaponManager
├── WeaponDebugUI       ← Client sees this
└── WeaponDebugVisualizer ← Client sees spheres from this
```

### What Each Player Sees
- **Own debug UI**: Yes (toggle with F3)
- **Own 3D markers**: Yes (spheres/lines when firing)
- **Other players' debug UI**: No
- **Other players' 3D markers**: No

This is correct behavior - debug tools are local to each player.

## Component Lifecycle

### WeaponDebugUI
- **Created**: Manually added to Player prefab
- **Initialized**: Awake() sets up GUI styles
- **Updated**: Every time weapon fires (if showVisualDebug enabled)
- **Destroyed**: When Player GameObject destroyed

### WeaponDebugVisualizer
- **Created**: Manually added to Player prefab
- **Initialized**: Awake() registers as singleton instance
- **Used**: When weapons call `Instance.VisualizeShotHybrid()`
- **Cleaned**: Spheres/lines auto-destroy after `visualDuration` seconds
- **Destroyed**: When Player GameObject destroyed (clears singleton reference)

## Common Questions

### Q: Can I have one visualizer for all players?
**A**: No, each player needs their own. The singleton pattern is per-scene, and each player spawns as a separate prefab instance.

### Q: What if I don't add WeaponDebugVisualizer?
**A**: Weapons will check `if (WeaponDebugVisualizer.Instance != null)` before using it. If null, no 3D markers appear, but everything else works fine.

### Q: Can I disable debug at runtime?
**A**: Yes!
- **UI Panel**: Press F3 or call `debugUI.Hide()`
- **3D Markers**: Uncheck `enableVisualization` on WeaponDebugVisualizer
- **Both**: Disable the components

### Q: Why not make WeaponDebugVisualizer auto-create itself?
**A**: We used to do this (auto-create on first access), but it's better to explicitly add it to the Player prefab because:
1. You can configure settings in Inspector before runtime
2. It's visible in the hierarchy (not hidden)
3. You can disable it easily without code changes
4. It follows Unity's "what you see is what you get" philosophy

## Troubleshooting

### "WeaponDebugVisualizer.Instance is null"
**Cause**: Component not added to Player
**Fix**: Add WeaponDebugVisualizer component to Player prefab

### "Multiple WeaponDebugVisualizers detected!"
**Cause**: Component added to multiple GameObjects
**Fix**: Remove duplicates, keep only one (on Player)

### "Debug UI panel shows but data doesn't update"
**Cause**: WeaponManager → Debug UI field not linked
**Fix**: Link it manually or ensure it's on same GameObject for auto-find

### "3D spheres appear but in wrong scene"
**Cause**: WeaponDebugVisualizer on wrong player instance
**Fix**: Ensure each player prefab has its own WeaponDebugVisualizer

## Summary

**Simple Rule**: Add both debug components to the **Player GameObject** and let WeaponManager auto-find them. Done!

```
Player GameObject
├── NetworkedFPSController
├── PlayerCombat
├── WeaponManager          ← This finds the debug components below
├── Health
├── WeaponDebugUI          ← F3 panel
└── WeaponDebugVisualizer  ← 3D markers
```
