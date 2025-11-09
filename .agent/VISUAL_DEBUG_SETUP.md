# Visual Debug System for Hybrid Aiming

## Overview

The weapon system now includes comprehensive visual debugging tools that work **in Game view** without requiring Scene view. This makes it much easier to debug aiming issues, especially in multiplayer.

## Debug Tools Created

### 1. WeaponDebugVisualizer
**Location**: `Assets/Scripts/Weapons/Debug/WeaponDebugVisualizer.cs`

**What it does**: Creates 3D spheres and lines visible in Game view to visualize the hybrid aiming system

**Visual Indicators**:
- **Yellow sphere**: Aim point (where camera raycast hit)
- **Cyan sphere**: Muzzle position (where bullet spawns)
- **Red line**: Camera to aim point (what you're looking at)
- **Green line**: Muzzle to aim point (bullet trajectory)
- **Faded green line**: Extended trajectory (where bullet will travel)

### 2. WeaponDebugUI
**Location**: `Assets/Scripts/Weapons/Debug/WeaponDebugUI.cs`

**What it does**: Shows on-screen panel with real-time aiming data

**Information Displayed**:
- Weapon name
- Camera position
- Muzzle position
- Aim point coordinates
- Raycast hit status (YES/NO)
- Distance to target
- Projectile direction
- Color-coded legend

**Controls**:
- Press **F3** to toggle panel on/off

## Setup Instructions

### Step 1: Add Debug Components to Player

1. Open `Assets/Prefabs/Player.prefab`
2. Select the root **Player** GameObject
3. Add Component → Scripts → Tides End → Weapons → **Weapon Debug UI**
   - Show Panel: `false` (toggle with F3 in-game)
   - Toggle Key: `F3`
   - Font Size: `18`
4. Add Component → Scripts → Tides End → Weapons → **Weapon Debug Visualizer**
   - Enable Visualization: `true`
   - Visual Duration: `2` (seconds spheres/lines stay visible)
   - Sphere Size: `0.1`
   - Line Width: `0.02`

### Step 2: Link Debug Components to WeaponManager

1. Still in Player prefab, select the root **Player** GameObject
2. Find the **Weapon Manager** component
3. In the **Debug** section:
   - **Debug UI**: Drag the Player GameObject (auto-finds WeaponDebugUI component)
   - **Debug Visualizer**: Drag the Player GameObject (auto-finds WeaponDebugVisualizer component)

**Note**: If both components are on the same GameObject as WeaponManager, they'll be auto-found in Awake(). Manual linking is only needed if they're on different GameObjects.

**That's it!** WeaponManager will automatically link the debug components to all weapons when they're spawned.

### Step 3: Enable Visual Debug on Weapons (Optional)

To see 3D spheres and lines in Game view:

1. Open your weapon prefab (e.g., `Assets/Prefabs/Weapons/Rifle.prefab`)
2. Find the **Weapon** component
3. In the **Debug** section:
   - Enable **Show Visual Debug** checkbox

This setting will be saved to the prefab, so all instances of that weapon will have visual debug enabled.

### Step 4: Enable WeaponDebugVisualizer (Automatic)

The visualizer creates itself automatically when visual debug is enabled. No setup needed!

## Usage

### Basic Testing

1. **Enter Play Mode**
2. **Press F3** to show debug panel
3. **Fire weapon** and observe:
   - Yellow sphere appears where you're aiming
   - Cyan sphere at gun muzzle
   - Green line shows bullet trajectory
   - UI panel updates with coordinates

### What to Look For

#### Correct Hybrid Aiming:
- ✅ Cyan sphere (muzzle) is at gun barrel tip
- ✅ Yellow sphere (aim point) is where crosshair points
- ✅ Green line connects muzzle to aim point
- ✅ Bullet tracers follow green line
- ✅ UI shows "Raycast Hit: YES" when aiming at objects

#### Common Issues:

**Issue**: Cyan sphere is at camera, not muzzle
- **Problem**: Weapon's `fireOrigin` not set correctly
- **Fix**: Assign muzzle transform to Weapon → Fire Origin field

**Issue**: Yellow sphere way off from crosshair
- **Problem**: Camera raycast hitting wrong layer
- **Fix**: Check WeaponData → Hit Layers mask

**Issue**: Green line doesn't match bullet tracer
- **Problem**: Spread is very high, or visual debug showing base direction
- **Fix**: Expected behavior - spread applied after visualization

**Issue**: No spheres/lines appear
- **Problem**: Visual debug not enabled
- **Fix**: Enable "Show Visual Debug" on Weapon component

## Debug Options

### Weapon Component Settings

**Show Debug Info** (bool):
- Enables console logging
- Shows Debug.DrawLine in Scene view
- Logs ammo, fire events, owner changes

**Show Visual Debug** (bool):
- Enables 3D spheres and lines in Game view
- Enables UI panel updates
- Shows real-time aiming visualization

**Debug UI** (reference):
- Link to WeaponDebugUI component
- Required for UI panel to work
- Can be null if you only want 3D visuals

### WeaponDebugVisualizer Settings

Located on the auto-created WeaponDebugVisualizer GameObject:

- **Enable Visualization**: Toggle 3D markers on/off
- **Visual Duration**: How long spheres/lines persist (default: 2 seconds)
- **Sphere Size**: Size of markers (default: 0.1)
- **Line Width**: Thickness of trajectory lines (default: 0.02)
- **Colors**: Customize colors for each element

### WeaponDebugUI Settings

- **Show Panel**: Toggle panel visibility
- **Toggle Key**: Keyboard shortcut (default: F3)
- **Font Size**: Text size (default: 18)
- **Text Color**: Panel text color
- **Background Color**: Panel background color

## Multiplayer Testing

The visual debug system works great for testing multiplayer aiming:

### Host Testing
1. Start as host
2. Enable visual debug on host weapon
3. Press F3 to see panel
4. Fire and observe spheres/lines
5. Verify bullets spawn at muzzle (cyan sphere)
6. Verify bullets hit where aimed (yellow sphere)

### Client Testing
Same as above - each player has their own debug visuals

### Remote Player Observation
- You can see other players' muzzle flashes
- You can see their bullet tracers
- But you won't see their debug spheres (local only)

## Performance

**Impact**: Minimal
- Spheres/lines auto-destroy after 2 seconds
- Only creates objects when firing
- UI panel uses immediate-mode GUI (no permanent objects)
- Safe to leave enabled during normal gameplay

**Recommendation**: Enable during testing, disable for production builds

## Troubleshooting

### "WeaponDebugVisualizer does not exist in the current context"
- **Cause**: Unity hasn't compiled the new files yet
- **Fix**: Wait for Unity to finish compiling, or restart Unity

### "No spheres appear when firing"
- **Cause**: Show Visual Debug not enabled
- **Fix**: Enable on Weapon component

### "UI panel doesn't show"
- **Cause**: Debug UI reference not set, or panel hidden
- **Fix**: Press F3, or link Debug UI field in Weapon component

### "Spheres appear but in wrong colors"
- **Cause**: Custom colors set in WeaponDebugVisualizer
- **Fix**: Reset colors to defaults in the visualizer component

### "Lines are too thin to see"
- **Cause**: Line width too small for your resolution
- **Fix**: Increase Line Width in WeaponDebugVisualizer settings (try 0.05)

## Files Created

### New Scripts
- [Assets/Scripts/Weapons/Debug/WeaponDebugVisualizer.cs](../Assets/Scripts/Weapons/Debug/WeaponDebugVisualizer.cs)
  - Creates 3D spheres and lines visible in Game view
  - Singleton pattern, auto-creates itself

- [Assets/Scripts/Weapons/Debug/WeaponDebugUI.cs](../Assets/Scripts/Weapons/Debug/WeaponDebugUI.cs)
  - On-screen UI panel with aiming data
  - Toggle with F3 key

### Modified Scripts
- [Assets/Scripts/Weapons/Core/Weapon.cs](../Assets/Scripts/Weapons/Core/Weapon.cs)
  - Added `showVisualDebug` field
  - Added `debugUI` reference field (auto-set by WeaponManager)
  - Added `SetDebugUI()` method for WeaponManager to call
  - Integrated visualizer and UI calls in `Fire()`
  - Updated `GetAimPoint()` to return raycast hit status

- [Assets/Scripts/Weapons/Core/WeaponManager.cs](../Assets/Scripts/Weapons/Core/WeaponManager.cs)
  - Added `debugUI` field in Debug section
  - Added `SetupWeaponDebug()` method
  - Automatically links debug UI to weapons when spawned
  - Finds WeaponDebugUI component if not manually assigned

## Example Debugging Session

**Goal**: Verify hybrid aiming works correctly at close range

1. Start game, spawn in a test scene
2. Press F3 to show debug panel
3. Walk up to a wall (1 meter away)
4. Aim at a specific spot on the wall
5. Fire weapon
6. **Observe**:
   - Yellow sphere appears on wall where you aimed ✓
   - Cyan sphere at gun muzzle ✓
   - Green line from muzzle to yellow sphere ✓
   - UI shows distance: ~1.0m ✓
   - Bullet tracer follows green line ✓
   - Hit decal appears at yellow sphere ✓

**Result**: Hybrid aiming working correctly!

## Color Legend Quick Reference

When visual debug is enabled:

- 🟡 **Yellow Sphere** = Aim Point (where crosshair targets)
- 🔵 **Cyan Sphere** = Muzzle (where bullet spawns)
- 🔴 **Red Line** = Camera Raycast (Scene view only)
- 🟢 **Green Line** = Bullet Trajectory (muzzle to target)
- 🟢 **Faded Green Line** = Extended Trajectory (max range)

## Next Steps

After verifying hybrid aiming works:
1. Disable visual debug for normal gameplay
2. Leave debug UI component on player (F3 still works)
3. Use when debugging future aiming issues
4. Expand visualizer for scope/ADS testing when implemented
