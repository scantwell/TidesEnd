# Weapon Root Rotation Fix

## Problem

The WeaponRoot transform was not rotating to match the player's aim direction. This caused a visual disconnect where:
- **First-person view**: Bullets correctly spawn from camera center (working as intended)
- **Scene view**: Bullet tracers visually spawn from a static WeaponRoot position that doesn't rotate with player look direction

## Root Cause

The WeaponRoot transform had no rotation logic:
1. Player body rotates on Y-axis (horizontal look)
2. Camera target rotates on X-axis (vertical look)
3. **WeaponRoot remained at identity rotation** - never updated

## Solution

Created `WeaponRootController` component that rotates the WeaponRoot to match the player's camera aim direction in `LateUpdate()`.

### Implementation

#### New Component: WeaponRootController

**Location**: `Assets/Scripts/Weapons/Core/WeaponRootController.cs`

**Purpose**: Rotates weapon root to match camera aim direction

**Key Features**:
- Uses player camera forward direction for accurate aim
- Falls back to camera target if camera unavailable (remote players)
- Optional smooth rotation with configurable speed
- Works for both local and remote players
- Includes Gizmo visualization in editor

#### Integration with NetworkedFPSController

**Changes**: Added WeaponRootController setup in player spawn logic

**Local Player**:
```csharp
weaponRootController.SetCameraTarget(cameraTarget);
weaponRootController.SetPlayerCamera(Camera.main);
```

**Remote Player**:
```csharp
weaponRootController.SetCameraTarget(cameraTarget);
weaponRootController.enabled = true;
```

## Setup Instructions

### 1. Add Component to Player Prefab

1. Open `Assets/Prefabs/Player.prefab`
2. Select the **WeaponRoot** child object
3. Add Component → Scripts → Tides End → Weapons → **Weapon Root Controller**
4. Configure settings:
   - **Camera Target**: Assign PlayerCameraRoot transform (should auto-find)
   - **Smooth Rotation**: `true` (recommended)
   - **Rotation Speed**: `20` (default, adjust for feel)

### 2. Link to NetworkedFPSController

1. Select the root **Player** GameObject
2. Find the **Networked FPS Controller** component
3. Assign the WeaponRootController reference:
   - **Weapon Root Controller**: Drag the WeaponRoot object here

### 3. Test

1. Enter Play Mode
2. Move mouse to look around
3. Open Scene view (keep Game view visible)
4. Observe:
   - WeaponRoot now rotates to match camera aim
   - Bullet tracers spawn in correct direction from scene view
   - First-person aiming unchanged (still correct)

## Technical Details

### Rotation Logic

```csharp
void LateUpdate()
{
    Vector3 aimDirection = playerCamera.transform.forward;
    Quaternion targetRotation = Quaternion.LookRotation(aimDirection);

    if (smoothRotation)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    else
        transform.rotation = targetRotation;
}
```

### Why LateUpdate?

- Runs after camera rotation is finalized in `Update()`
- Ensures WeaponRoot always matches the latest camera orientation
- Prevents one-frame lag between camera and weapon rotation

### Remote Player Support

Remote players don't have an active camera, so the controller falls back to:
1. Camera target transform rotation (PlayerCameraRoot)
2. Combined rotation of player body (yaw) + camera pitch

This ensures weapon visuals match where remote players are aiming.

## Files Modified

### Created
- [Assets/Scripts/Weapons/Core/WeaponRootController.cs](../Assets/Scripts/Weapons/Core/WeaponRootController.cs)

### Modified
- [Assets/Scripts/Player/NetworkedFPSController.cs](../Assets/Scripts/Player/NetworkedFPSController.cs)
  - Added WeaponRootController field
  - Added auto-find in Awake()
  - Added setup in SetupLocalPlayer()
  - Added setup in SetupRemotePlayer()

## Related Issues

- ✅ Fixed: Bullet tracers spawn from wrong direction in scene view
- ✅ Fixed: Weapon visuals don't rotate with player aim
- ✅ Maintained: First-person aiming accuracy (unchanged)
- ✅ Supported: Both local and remote players

## Future Enhancements

Potential improvements for weapon rotation:
1. **Weapon sway**: Add procedural animation based on movement
2. **Recoil rotation**: Apply visual recoil to weapon root
3. **Aim down sights**: Adjust weapon position/rotation when aiming
4. **Network sync**: Sync WeaponRoot rotation for remote players (currently using camera target)
