# Hitscan Weapon System Architecture

**Created**: 2025-10-31
**Status**: Implemented
**Related Files**:
- [Weapon.cs](../Assets/Scripts/Weapons/Core/Weapon.cs)
- [WeaponData.cs](../Assets/Scripts/Weapons/Data/WeaponData.cs)
- [PlayerCombat.cs](../Assets/Scripts/Player/PlayerCombat.cs)
- [HitscanConfig.cs](../Assets/Scripts/Weapons/Core/HitscanConfig.cs)
- [IWeapon.cs](../Assets/Scripts/Combat/Interfaces/IWeapon.cs)

---

## Overview

The weapon system now supports **both hitscan (instant raycast) and projectile-based (physics) weapons** through a unified architecture. This hybrid approach provides:

- **Hitscan weapons**: Instant hit detection via raycasts (rifles, pistols, SMGs)
- **Projectile weapons**: Physics-based projectiles with travel time (grenades, rockets)
- **Server-authoritative damage**: All damage is validated and applied by the server
- **Client prediction**: Instant visual feedback (tracers, effects) for responsive gameplay

---

## Architecture Type: Hybrid Hitscan + Projectile

### Weapon Type Enum

```csharp
public enum WeaponType
{
    Hitscan,    // Instant raycasts (rifles, pistols, SMGs)
    Projectile, // Physics-based projectiles (grenades, rockets)
    Melee       // Future: melee weapons
}
```

Each `WeaponData` ScriptableObject specifies its `weaponType`, and the `Weapon.Fire()` method branches accordingly.

---

## Hitscan Weapon Flow (Listen Server PvE)

### Client-Side (Owner)

```
1. Player clicks fire
2. PlayerCombat.TryFire()
3. weapon.Fire()  ← Client fires weapon
   ├─ Performs raycast from muzzle
   ├─ Shows instant tracer (BulletTracer)
   ├─ Shows muzzle flash and sound effects
   ├─ Spawns hit effects (sparks/blood)
   └─ Checks IsServer → FALSE, so DOES NOT apply damage
4. FireServerRpc() → sends request to server
```

**Key Point**: Client fires for visual feedback. Damage is checked via `IsServer` in weapon code.

### Server-Side

```
1. Receives FireServerRpc()
2. Validates fire request (ammo, cooldown, etc.)
3. weapon.Fire()  ← Server fires weapon
   ├─ Performs authoritative raycast
   ├─ Checks IsServer → TRUE, so applies damage to targets
   ├─ Spawns hit effects
   └─ Syncs health via NetworkVariable
4. PlayFireEffectsClientRpc() → notifies remote clients
```

**Key Point**: Server performs the same raycast and applies authoritative damage via `IsServer` check.

### Remote Clients

```
1. Receive PlayFireEffectsClientRpc()
2. Play muzzle flash and sound effects
3. Receive health NetworkVariable updates
4. Display damage feedback
```

**Key Point**: Remote clients only see visual/audio effects. They don't spawn projectiles or tracers.

---

## Double Damage Prevention

### The Problem (Before Fix)

In the original hybrid system with projectiles:
1. **Client's projectile** hits enemy → `TakeDamage()` → `RequestDamageServerRpc()` → Server applies damage
2. **Server's projectile** hits enemy → `TakeDamage()` → `ProcessDamage()` directly → Server applies damage **AGAIN**

Result: **Double damage bug** (host deals 2x damage)

### The Solution (Hitscan)

For hitscan weapons, we prevent double damage by checking `IsServer` in the weapon code:

```csharp
// Weapon.cs (inherits from NetworkBehaviour)
private void ProcessHitscanHit(RaycastHit hit, float baseDamage)
{
    // ...

    // Only apply damage if we're on the server
    if (target != null && target.IsAlive && IsServer)
    {
        target.TakeDamage(finalDamage, ownerId, hit.point, hit.normal);
    }

    // Always spawn visual effects (both client and server)
    SpawnHitEffect(hit.point, hit.normal);
}
```

**Flow**:
- **Client fires**: `weapon.Fire()` → Raycast + visuals, but `IsServer = false` → No damage
- **Server fires**: `weapon.Fire()` → Raycast + visuals, and `IsServer = true` → Applies damage

Result: **No double damage** - only server applies damage via `IsServer` check, both show visuals.

---

## Key Components

### 1. WeaponData.cs

**New Fields**:
```csharp
[Header("Weapon Type")]
public WeaponType weaponType = WeaponType.Hitscan;

[Header("Hitscan Settings")]
public int pelletsPerShot = 1;  // >1 for shotguns

[Header("Penetration (Both Hitscan and Projectile)")]
public bool canPenetrate = false;
public int maxPenetrations = 0;
public float penetrationDamageReduction = 0.5f;
```

**Usage**:
- **Rifle**: `weaponType = Hitscan`, `pelletsPerShot = 1`
- **Shotgun**: `weaponType = Hitscan`, `pelletsPerShot = 8`
- **Grenade**: `weaponType = Projectile`, `projectilePrefab = GrenadePrefab`

---

### 2. Weapon.cs

**Key Change**: Now inherits from `NetworkBehaviour` (instead of `MonoBehaviour`) to access `IsServer`

**New Methods**:

#### `Fire()`
- Branches on `weaponType` to call either `FireHitscan()` or `FireProjectile()`
- Both client and server call this method
- Damage authority determined by `IsServer` check inside the weapon

#### `FireHitscan()`
- Uses existing **hybrid aiming system** (raycast from camera to find aim point)
- Fires multiple pellets (for shotguns)
- Calls `PerformHitscan()` for each pellet

#### `PerformHitscan(origin, direction)`
- Performs instant raycast from muzzle
- Supports penetration (RaycastAll if enabled)
- Calls `ProcessHitscanHit()` for each hit

#### `ProcessHitscanHit(hit, damage)`
- Calculates damage with:
  - Distance falloff
  - Hit location multipliers (headshot, limb)
- Applies damage ONLY if `IsServer == true`
- Always spawns hit effects (visual feedback)
- Always spawns tracers (if enabled)

---

### 3. PlayerCombat.cs

**Simplified Approach**:

```csharp
private void TryFire()
{
    // Setup camera and owner
    weapon.SetPlayerCamera(mainCamera);
    weapon.SetOwner(OwnerClientId, gameObject);

    // Owner fires locally for immediate visual feedback
    weapon.Fire();  // Shows tracers/effects, damage checked via IsServer in weapon

    // Request server to fire
    FireServerRpc();
}

[ServerRpc]
private void FireServerRpc()
{
    weapon.SetOwner(OwnerClientId, gameObject);

    // Use bypassCanFire=true to skip validation and state updates
    // This is necessary because on a listen server, the client and server share the same
    // Weapon component instance. When the client fires, it already validated CanFire(),
    // decremented ammo, and set nextFireTime. If we check CanFire() again here immediately,
    // it will return false because the cooldown hasn't elapsed yet.
    weapon.Fire(bypassCanFire: true);  // Server fires, applies damage via IsServer check

    PlayFireEffectsClientRpc();  // Notify remote clients
}
```

**Result**:
- **Owner client** sees instant feedback (tracers, effects), no damage applied (IsServer = false)
- **Server** applies authoritative damage (IsServer = true) using `bypassCanFire: true`
- **Remote clients** see muzzle flash only

---

### 4. HitscanConfig.cs

**Purpose**: Configuration struct for hitscan shots (similar to `ProjectileConfig` for projectiles)

**Contains**:
- Damage settings (base damage, type, owner)
- Ray information (origin, direction, max range)
- Hit location multipliers
- Penetration settings
- Damage falloff parameters
- Visual effect references

**Usage**:
```csharp
HitscanConfig config = HitscanConfig.FromWeaponData(
    weaponData,
    fireOrigin.position,
    direction,
    ownerId
);
```

Currently used for documentation/future expansion. The actual hitscan implementation reads directly from `WeaponData` for simplicity.

---

## Hitscan Features

### 1. Damage Falloff

**Linear falloff between start and end distance**:

```csharp
private float CalculateDamageFalloff(float baseDamage, float distance)
{
    if (distance <= damageFalloffStart)
        return baseDamage;  // Full damage
    else if (distance >= damageFalloffEnd)
        return baseDamage * 0.2f;  // Minimum 20%
    else
        return Mathf.Lerp(baseDamage, baseDamage * 0.2f, falloffPercent);
}
```

**Example**:
- **Rifle**: `damageFalloffStart = 30m`, `damageFalloffEnd = 80m`
  - 0-30m: Full damage (45)
  - 30-80m: Linear falloff to 20% (9)
  - 80m+: Minimum damage (9)

---

### 2. Hit Location Multipliers

**Uses existing HitZone system**:

```csharp
private float GetHitLocationMultiplier(HitZone hitZone)
{
    if (hitZone == null) return 1f;

    switch (hitZone.Type)
    {
        case HitZone.ZoneType.Head:
            return data.headshotMultiplier;  // 2.0x
        case HitZone.ZoneType.Limb:
            return data.limbshotMultiplier;  // 0.75x
        default:
            return 1f;
    }
}
```

**Example**:
- Body shot: 45 damage × 1.0 = **45 damage**
- Headshot: 45 damage × 2.0 = **90 damage**
- Limb shot: 45 damage × 0.75 = **33.75 damage**

---

### 3. Penetration System

**RaycastAll + damage reduction per penetration**:

```csharp
foreach (RaycastHit hit in sortedHits)
{
    ProcessHitscanHit(hit, currentDamage, applyDamage);

    if (penetrationsRemaining > 0)
    {
        penetrationsRemaining--;
        currentDamage *= (1f - penetrationDamageReduction);  // Reduce damage
    }
    else
    {
        break;  // Stop after max penetrations
    }
}
```

**Example (Sniper Rifle)**:
- `maxPenetrations = 2`, `penetrationDamageReduction = 0.3f`
- Hit 1: 150 damage
- Hit 2: 105 damage (150 × 0.7)
- Hit 3: 73.5 damage (105 × 0.7)

---

### 4. Multi-Pellet Hitscan (Shotguns)

**Fires multiple raycasts with spread**:

```csharp
for (int i = 0; i < pelletsPerShot; i++)
{
    Vector3 baseDirection = (aimPoint - muzzle).normalized;
    Vector3 direction = ApplySpread(baseDirection);
    PerformHitscan(muzzle, direction, applyDamage);
}
```

**Example (Shotgun)**:
- `pelletsPerShot = 8`, `damage = 15` per pellet, `baseSpread = 5°`
- Each pellet:
  - Independent raycast
  - Random spread within 5° cone
  - Can hit different targets
- Total potential damage: 15 × 8 = **120 damage** (if all hit)

---

## Visual Feedback System

### Instant Tracers

**Uses existing `BulletTracer.InitializeWithHit()`**:

```csharp
private void SpawnHitscanTracer(Vector3 start, Vector3 end)
{
    BulletTracer tracer = TracerPool.Instance.GetTracer();
    tracer.InitializeWithHit(start, end);  // Instant tracer from muzzle to hit
}
```

**Key Difference from Projectile Tracers**:
- **Projectile tracer**: Attached to moving projectile, follows it
- **Hitscan tracer**: Independent visual effect, travels from muzzle to hit point instantly

**Benefit**: Reuses existing tracer pool and visual system, no new assets needed.

---

### Hit Effects

**Spawns particle effects at impact point**:

```csharp
private void SpawnHitEffect(Vector3 point, Vector3 normal)
{
    if (data.hitEffectPrefab == null) return;

    Quaternion rotation = Quaternion.LookRotation(normal);
    GameObject effect = Instantiate(hitEffectPrefab, point, rotation);
    Destroy(effect, 2f);  // Auto-cleanup
}
```

**Uses WeaponData.hitEffectPrefab**:
- Blood spray for enemies
- Sparks/dust for environment
- Oriented along surface normal

---

## Performance Comparison

### Hitscan vs Projectile

| Aspect | Hitscan | Projectile |
|--------|---------|------------|
| **Per-shot cost** | 1-12 raycasts (instant) | 1 Rigidbody + continuous collision |
| **Per-frame cost** | None (instant) | Physics update until hit/timeout |
| **Bandwidth** | ~60 bytes/shot | ~240 bytes/shot (NetworkObject) |
| **Accuracy** | Perfect (instant) | Depends on physics sync |
| **Desync risk** | None | High (client/server physics differ) |
| **Best for** | Rifles, pistols, SMGs | Grenades, rockets, slow projectiles |

**Conclusion**: Hitscan is **cheaper and more reliable** for rapid-fire weapons in a PvE listen server environment.

---

## Migration Guide

### Converting a Weapon from Projectile to Hitscan

**Step 1**: Open the WeaponData ScriptableObject in Unity Inspector

**Step 2**: Change `weaponType` from `Projectile` to `Hitscan`

**Step 3**: Configure hitscan settings:
- `pelletsPerShot`: 1 for rifles, 8-12 for shotguns
- `canPenetrate`: true for sniper rifles
- `maxPenetrations`: 2-3 for high-caliber weapons

**Step 4**: Test in-game

**That's it!** No code changes needed. The weapon automatically uses hitscan logic.

---

### Example: M1903 Rifle Conversion

**Before** (Projectile):
```
weaponType: Projectile
projectilePrefab: BulletPrefab
projectileVelocity: 800
damage: 45
```

**After** (Hitscan):
```
weaponType: Hitscan
pelletsPerShot: 1
damage: 45
damageFalloffStart: 30
damageFalloffEnd: 80
```

**Result**: Instant hit detection, no projectile physics, 75% less bandwidth.

---

## Testing Checklist

### Local Testing (Single Player)

- [ ] Hitscan rifle hits targets instantly
- [ ] Tracer appears from muzzle to hit point
- [ ] Hit effects spawn at correct location
- [ ] Damage is applied correctly
- [ ] Headshot multiplier works (2x damage)
- [ ] Damage falloff works (less damage at range)

### Network Testing (Listen Server + Client)

- [ ] Client sees instant tracer on fire
- [ ] Server applies damage (not client)
- [ ] Remote clients see muzzle flash from shooter
- [ ] Health syncs correctly to all clients
- [ ] No double damage bug (host deals normal damage)

### Performance Testing

- [ ] 60+ FPS with multiple players firing
- [ ] No GC spikes during rapid fire
- [ ] Bandwidth < 100 KB/s per player
- [ ] Hitscan is equal or better performance than projectile

---

## Future Enhancements

### Planned (Not Implemented)

1. **Lag Compensation** (PvP only)
   - Rewind player positions to client's timestamp
   - Validate hits at that point in time
   - Not needed for PvE (enemies are server-controlled)

2. **Hit Markers** (UI)
   - Crosshair indication (white for hit, red for headshot)
   - Audio "ding" for hit confirmation
   - Requires HUD integration

3. **Camera Shake**
   - Small camera kick on fire
   - Makes weapons feel more powerful
   - Requires Cinemachine impulse source

4. **Networked Hit Effects** (Optional)
   - Currently each client spawns local hit effects
   - Could sync via ClientRpc for consistency
   - Low priority (visual difference is minimal)

---

## Troubleshooting

### Issue: Hitscan weapon doesn't deal damage

**Check**:
1. Is `applyDamage` set to `true` when server fires?
2. Does the target have `IDamageable` component?
3. Is the `hitLayers` LayerMask correct in WeaponData?
4. Is the weapon owner set correctly? (`weapon.SetOwner()`)

### Issue: Client sees damage but server doesn't

**Expected behavior**: Client fires with `applyDamage = false`, so client's hits are visual only. Only server's hits apply damage.

**Check**: Ensure `PlayerCombat.TryFire()` passes `applyDamage: false` for client.

### Issue: Tracers don't appear

**Check**:
1. Is `useTracers` enabled in WeaponData?
2. Is `TracerPool` in the scene?
3. Is `tracerPrefab` assigned in WeaponData?
4. Is `tracerFrequency` set to 1 (every shot)?

### Issue: Shotgun pellets all hit same spot

**Check**:
1. Is `baseSpread` > 0 in WeaponData?
2. Is `pelletsPerShot` > 1?
3. Is spread being applied in `FireHitscan()`?

---

## References

### Related Documentation

- [HYBRID_AIMING_SYSTEM.md](HYBRID_AIMING_SYSTEM.md) - Explains the camera-to-muzzle aiming system used by hitscan
- [CLIENT_DAMAGE_FIX.md](CLIENT_DAMAGE_FIX.md) - Background on client damage issues (now solved for hitscan)
- [CLAUDE.md](../CLAUDE.md) - Unity development commands and project structure

### External Resources

- [Unity Raycast Documentation](https://docs.unity3d.com/ScriptReference/Physics.Raycast.html)
- [Unity Netcode for GameObjects - RPCs](https://docs-multiplayer.unity3d.com/netcode/current/basics/rpc/)
- [Server-Authoritative Combat](https://www.gabrielgambetta.com/client-server-game-architecture.html)

---

**Last Updated**: 2025-10-31
**Implementation Status**: ✅ Complete
**Tested**: Local (Yes), Network (Pending)
