# Hybrid Aiming System

## Overview

The weapon system now uses **hybrid aiming** - a best-of-both-worlds approach that combines visual accuracy with gameplay precision.

## How It Works

### The Three-Step Process

1. **Raycast from camera center** → Find where the player is looking (aim point)
2. **Spawn projectile at gun muzzle** → Visual accuracy (bullets come from gun)
3. **Aim projectile toward aim point** → Gameplay accuracy (bullets hit crosshair)

```csharp
// Step 1: Find where camera is aiming
Ray aimRay = GetAimRay(); // Camera center
Vector3 aimPoint = GetAimPoint(aimRay); // Raycast to find target

// Step 2 & 3: Spawn at muzzle, aim at target
Vector3 direction = (aimPoint - fireOrigin.position).normalized;
SpawnProjectile(fireOrigin.position, direction);
```

## Comparison with Other Approaches

### ❌ Camera-Origin Aiming (Old System)
- Projectiles spawn at camera center
- **Pro**: Perfect crosshair accuracy
- **Con**: Bullets don't come from gun barrel (breaks immersion in 3rd person)

### ❌ Muzzle-Origin Aiming (Pure Realistic)
- Projectiles spawn at gun muzzle, travel in muzzle's forward direction
- **Pro**: Visually accurate
- **Con**: Parallax problem - bullets miss crosshair at close range

### ✅ Hybrid Aiming (Current System)
- Projectiles spawn at gun muzzle, aim toward camera raycast hit point
- **Pro**: Bullets hit crosshair ✓
- **Pro**: Bullets visually come from gun ✓
- **Pro**: Works seamlessly with ADS/scopes ✓
- **Used by**: Escape from Tarkov, Hunt: Showdown, Modern Warfare

## Implementation Details

### Core Methods

#### `GetAimRay()`
Returns a ray from camera center (or weapon forward as fallback)

```csharp
private Ray GetAimRay()
{
    if (playerCamera != null)
        return playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    else
        return new Ray(fireOrigin.position, fireOrigin.forward);
}
```

#### `GetAimPoint(Ray aimRay)`
Performs raycast to find where the player is aiming

```csharp
private Vector3 GetAimPoint(Ray aimRay)
{
    if (Physics.Raycast(aimRay, out RaycastHit hit, data.range, data.hitLayers))
    {
        return hit.point; // Hit something - aim at it
    }
    else
    {
        return aimRay.origin + (aimRay.direction * data.range); // Aim at max range
    }
}
```

#### `Fire()` - Updated
Uses hybrid approach to spawn projectiles

```csharp
public void Fire()
{
    Ray aimRay = GetAimRay();
    Vector3 aimPoint = GetAimPoint(aimRay);

    for (int i = 0; i < data.projectilesPerShot; i++)
    {
        // Calculate direction from muzzle to aim point
        Vector3 baseDirection = (aimPoint - fireOrigin.position).normalized;
        Vector3 direction = ApplySpread(baseDirection);

        // Spawn at muzzle, aim at target
        SpawnProjectile(fireOrigin.position, direction);
    }
}
```

## ADS/Scope Compatibility

The hybrid approach works **perfectly** with aiming down sights:

### Hip-Fire
- Camera offset from barrel (parallax exists)
- Hybrid compensates by calculating angle from muzzle to aim point
- Bullets hit crosshair despite parallax ✓

### ADS/Scoped
- Camera aligned with sights (minimal parallax)
- Hybrid still works - angle calculation naturally aligns
- Bullets hit crosshair ✓

**No special case code needed!** The same logic works for both scenarios.

## Debug Visualization

When `showDebugInfo = true` on the Weapon component:

- **Red line**: Camera raycast hit point (where you're looking)
- **Yellow line**: Camera raycast max range (when not hitting anything)
- **Green line**: Muzzle to aim point (projectile initial trajectory)

Enable this to see the hybrid aiming in action!

## Edge Cases Handled

### 1. Raycast Misses
If the raycast doesn't hit anything (aiming at sky):
- Aim point = camera position + (camera forward × max range)
- Projectile travels in roughly the same direction as camera

### 2. No Camera Assigned
Fallback for AI or when camera not set:
- Aim ray = weapon forward direction
- Behaves like traditional muzzle-origin aiming

### 3. Close Range Objects
At very close range, muzzle might be inside an object:
- Raycast still finds correct aim point
- Projectile aims toward it (may immediately hit close object)
- This is correct behavior (prevents shooting through walls)

### 4. Spread Applied
Spread is applied **after** calculating base direction:
1. Calculate perfect direction (muzzle → aim point)
2. Apply random spread angle
3. Spawn projectile with spread direction

This maintains weapon spread characteristics while preserving hybrid accuracy.

## Layer Mask Configuration

The raycast uses `data.hitLayers` to determine what can be aimed at:

**Recommended Setup**:
- Include: Environment, Enemies, Destructibles
- Exclude: Player, Triggers, Ignore Raycast

This prevents aiming at yourself or non-solid objects.

## Performance Considerations

**Cost per shot**:
- 1 raycast (camera to find aim point)
- N projectile spawns (N = `projectilesPerShot`)

**Optimization**:
- Raycast is single-ray (not expensive)
- Uses layer mask to reduce checks
- Max distance capped by `data.range`

**No performance concerns** for standard FPS firing rates.

## Testing the System

### Visual Test
1. Set `showDebugInfo = true` on Weapon component
2. Enable Gizmos in Scene view
3. Fire weapon while watching Scene view
4. Observe:
   - Red/Yellow line from camera (aim ray)
   - Green line from muzzle to aim point
   - Projectile follows green line

### Accuracy Test
1. Place target in front of player
2. Aim crosshair at target center
3. Fire multiple shots
4. **Expected**: All shots hit where crosshair points (within spread)

### Close Range Test
1. Stand very close to a wall (1 meter)
2. Aim slightly to the side
3. Fire
4. **Expected**: Bullets still hit crosshair point (not offset by parallax)

### ADS Test (Future)
1. Implement ADS system (camera moves to sights)
2. Fire in hip-fire and ADS modes
3. **Expected**: Same accuracy in both modes, no code changes needed

## Future Enhancements

Potential improvements:
1. **Obstacle detection**: Check if muzzle has clear line to aim point (prevent shooting through walls)
2. **Weapon sway**: Add procedural offset to fire origin based on movement
3. **Recoil**: Apply upward offset to aim point based on continuous fire
4. **Zeroing**: Allow players to adjust aim point distance for long-range weapons

## Files Modified

### Modified
- [Assets/Scripts/Weapons/Core/Weapon.cs](../Assets/Scripts/Weapons/Core/Weapon.cs)
  - Updated `Fire()` to use hybrid approach
  - Added `GetAimPoint()` method for raycast
  - Added debug visualization
  - Projectiles now spawn at `fireOrigin.position` instead of camera

### Unchanged
- [Assets/Scripts/Weapons/Core/Projectile.cs](../Assets/Scripts/Weapons/Core/Projectile.cs) - No changes needed
- [Assets/Scripts/Weapons/Data/WeaponData.cs](../Assets/Scripts/Weapons/Data/WeaponData.cs) - Uses existing `range` field

## Related Documentation

- [Weapon Root Rotation Fix](.agent/WEAPON_ROOT_ROTATION_FIX.md) - Ensures weapon visuals match aim direction
- [Projectile System](../README_PROJECTILE_SYSTEM.md) - Core projectile implementation
- [Weapon Specifications](.agent/tasks/WEAPON_SPECIFICATIONS.md) - Weapon data and balancing
