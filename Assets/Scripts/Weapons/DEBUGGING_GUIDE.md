# Weapon System Debugging Guide

## Common Issues and Solutions

### Issue 1: Shooting Damages Self

**Symptoms:**
- When you fire, you take damage
- Health decreases immediately when shooting
- Console shows your own GameObject being hit

**Root Cause:**
Projectile is hitting the player's own HitZone colliders before leaving the player's collision area.

**Solution Applied:**
Added `IsChildOf()` check in `Projectile.OnTriggerEnter()` to ignore:
1. The owner GameObject
2. Any children of the owner GameObject
3. Any GameObject whose IDamageable component is the owner or child of owner

**How to Verify Fix:**
1. Enable debug logging on Projectile: Check "Show Debug Info" in Inspector
2. Fire weapon
3. Console should show: `"Projectile ignored owner collision: [HitZone name]"`

---

### Issue 2: Projectiles Don't Spawn

**Symptoms:**
- No projectiles appear when firing
- No tracer effects
- Console errors about ProjectilePool

**Debugging Steps:**

1. **Check ProjectilePool exists:**
   ```csharp
   // Console should NOT show this error:
   "ProjectilePool not found in scene!"
   ```
   **Fix:** Add GameObject with ProjectilePool component to scene

2. **Check projectile prefab assigned:**
   ```csharp
   // Console should NOT show this error:
   "No projectile prefab assigned to [weapon name]!"
   ```
   **Fix:** Assign projectile prefab in WeaponData asset

3. **Enable debug on Weapon:**
   - Select weapon GameObject
   - Check "Show Debug Info"
   - Fire weapon
   - Should see: `"Spawned projectile: [weapon], Dir: [direction], Tracer: [true/false]"`

---

### Issue 3: Projectiles Pass Through Enemies

**Symptoms:**
- Projectiles fly through enemies without dealing damage
- No hit effects
- No damage numbers

**Debugging Steps:**

1. **Check Layer Collision Matrix:**
   - Edit → Project Settings → Physics
   - Ensure "Projectile" layer collides with "HitZone" layer
   - Ensure "Projectile" does NOT collide with "Projectile" layer

2. **Check HitZone Setup:**
   - Enemy/Player should have HitZone components on colliders
   - HitZone colliders must be **Triggers** ✓
   - HitZone should auto-detect IDamageable owner

3. **Enable Debug on Projectile:**
   - Projectile prefab → Check "Show Debug Info"
   - Fire at enemy
   - Should see: `"Projectile hit [enemy name] in [Head/Body/Limb] for [damage]"`

4. **Check IDamageable Implementation:**
   - Enemy must have component implementing IDamageable (like Health.cs)
   - Component must have `IsAlive = true`
   - Component must implement `TakeDamage()` method

---

### Issue 4: No Tracers Appearing

**Symptoms:**
- Projectiles work but no visual trail
- No tracer effects

**Debugging Steps:**

1. **Check TracerPool:**
   ```
   Scene must have GameObject with TracerPool component
   TracerPool must have tracer prefab assigned
   ```

2. **Check WeaponData Settings:**
   - Use Tracers: ✓ Enabled
   - Tracer Frequency: 1 (every shot) for testing

3. **Check Tracer Prefab:**
   - Must have BulletTracer component
   - Must have TrailRenderer component
   - TrailRenderer Time should be > 0.1

4. **Console Warnings:**
   ```
   "TracerPool not found in scene!" → Add TracerPool to scene
   "Tracer prefab not assigned..." → Assign prefab in TracerPool
   ```

---

### Issue 5: Damage Not Matching Specs

**Symptoms:**
- Damage values seem wrong
- Headshots not doing 2x damage
- Distance falloff not working

**Debugging Steps:**

1. **Enable Projectile Debug:**
   - Shows calculated damage per hit
   - Format: `"Projectile hit [target] in [zone] for [finalDamage] (base: [base], mult: [multiplier])"`

2. **Check WeaponData Values:**
   - Base Damage: Should match specs
   - Headshot Multiplier: 2.0
   - Limbshot Multiplier: 0.75
   - Falloff Start/End: Match specs

3. **Check HitZone Configuration:**
   - Head colliders → Zone Type: Head
   - Body colliders → Zone Type: Body
   - Arm/Leg colliders → Zone Type: Limb

4. **Test at Different Ranges:**
   - Close range (< falloff start): Full damage
   - Mid range: Partial damage
   - Far range (> falloff end): 20% minimum damage

---

## Debug Workflow

### Quick Debug Checklist

When something doesn't work:

1. ☐ Enable "Show Debug Info" on Weapon
2. ☐ Enable "Show Debug Info" on Projectile prefab
3. ☐ Enable "Show Debug Info" on ProjectilePool
4. ☐ Open Console window (Ctrl+Shift+C)
5. ☐ Clear Console
6. ☐ Fire weapon
7. ☐ Read console messages carefully

### Reading Debug Output

**Good output (everything working):**
```
[Weapon] Spawned projectile: M1903 Springfield, Dir: (0.0, 0.1, 1.0), Tracer: true
[Projectile] Projectile hit Enemy_Zombie in Head for 170 damage (base: 85, mult: 2.0)
[ProjectilePool] Projectile_Rifle: 49 pooled, 1 active
```

**Bad output (self-damage issue):**
```
[Projectile] Projectile hit Player in Body for 85 damage (base: 85, mult: 1.0)
[Health] Player took 85 damage from self
```

**Bad output (no hit detection):**
```
[Weapon] Spawned projectile: M1903 Springfield, Dir: (0.0, 0.1, 1.0), Tracer: true
[Projectile] Projectile expired after 5 seconds. Distance traveled: 1500
// No hit messages = collision not working
```

---

## Unity Inspector Settings

### Projectile Prefab Checklist

```
Projectile (GameObject)
├── Projectile (Script)
│   └── Show Debug Info: ✓ (for debugging)
├── Rigidbody
│   ├── Mass: 0.01
│   ├── Use Gravity: ✗
│   ├── Is Kinematic: ✗
│   ├── Interpolate: Interpolate
│   └── Collision Detection: Continuous Dynamic
├── SphereCollider
│   ├── Is Trigger: ✓ IMPORTANT
│   ├── Radius: 0.05
│   └── Layer: Projectile
└── TrailRenderer (optional)
    ├── Time: 0.3
    └── Width: 0.05 → 0.01
```

### HitZone Collider Checklist

```
Head (GameObject)
├── SphereCollider
│   ├── Is Trigger: ✓ IMPORTANT
│   ├── Radius: 0.15
│   └── Layer: HitZone
└── HitZone (Script)
    ├── Zone Type: Head
    ├── Custom Multiplier: 0 (use default)
    └── Owner Object: (leave empty, auto-detects)
```

### WeaponData Asset Checklist

```
WD_M1903_Springfield (ScriptableObject)
├── Damage: 85
├── Fire Rate: 0.25
├── Projectile Prefab: Projectile_Rifle ← MUST BE ASSIGNED
├── Projectile Velocity: 853
├── Projectile Lifetime: 2
├── Projectile Gravity: 0
├── Damage Falloff Start: 30
├── Damage Falloff End: 80
├── Headshot Multiplier: 2.0
├── Limbshot Multiplier: 0.75
├── Use Tracers: ✓
└── Tracer Frequency: 1
```

---

## Performance Debugging

### Too Many Active Projectiles

**Symptoms:**
- FPS drops when firing
- Console warnings: "Pool exhausted"

**Check:**
```
ProjectilePool Debug Stats:
Projectile_Rifle: 0 pooled, 500 active ← BAD (pool exhausted)
Total: 500 projectiles
```

**Solutions:**
1. Reduce projectile lifetime (2 seconds is good)
2. Increase pool size
3. Check projectiles are returning to pool properly
4. Fix any infinite loops preventing pool return

### Memory Leaks

**Symptoms:**
- Memory usage grows over time
- Game gets slower the longer you play

**Debug:**
1. Open Profiler (Window → Analysis → Profiler)
2. Look at "Memory" section
3. If "Total Reserved" keeps growing → Memory leak

**Common Causes:**
- Projectiles not returning to pool (fix: check ForceStop() calls)
- Tracers not returning to pool (fix: check tracer cleanup)
- Coroutines not stopping (fix: StopAllCoroutines in ForceStop)

---

## Network Debugging (Multiplayer)

### Projectiles Only Appear for One Player

**Issue:** Client-side vs Server-side spawning

**Solution:** Projectiles should spawn on server and sync to clients via NetworkObject

**Debug:**
1. Check if Projectile prefab has NetworkObject component
2. Check if projectile.Spawn() is called on server
3. Use "Show Debug Info" to see which machine spawns projectile

### Damage Not Syncing

**Issue:** Client hits but server doesn't register

**Solution:** Damage must be applied on server (authority)

**Check:**
- Projectile collision happens on server
- TakeDamage() is called with server authority
- Health component syncs via NetworkVariable

---

## Advanced Debugging with Gizmos

### Visualize Projectile Path

Enable Gizmos in Projectile.cs (already implemented):
- Green sphere: Damage falloff start distance
- Yellow sphere: Damage falloff end distance
- Red line: Projectile velocity vector

**How to use:**
1. Select projectile in Hierarchy while game running
2. Scene view shows gizmos
3. Verify falloff ranges match specs

---

## Troubleshooting Commands

### Clear All Active Projectiles

```csharp
// Call this from Debug menu or console
ProjectilePool.Instance.ClearAllProjectiles();
```

### Reset Weapon State

```csharp
Weapon weapon = GetComponent<Weapon>();
weapon.Initialize(); // Resets ammo, stops reloading
```

### Force Pool Stats

```csharp
var stats = ProjectilePool.Instance.GetPoolStats(projectilePrefab);
Debug.Log($"Pooled: {stats.pooled}, Active: {stats.active}");
```

---

## Getting Help

If you're still stuck after trying these steps:

1. **Enable all debug flags**
2. **Take screenshot of:**
   - Console output
   - Projectile Inspector
   - WeaponData Inspector
   - HitZone Inspector
   - Layer Collision Matrix
3. **Note:**
   - What you expect to happen
   - What actually happens
   - Steps to reproduce

---

## Summary: Debug Priority

**First, check these (most common issues):**
1. ✅ ProjectilePool exists in scene
2. ✅ TracerPool exists in scene
3. ✅ Projectile prefab assigned to WeaponData
4. ✅ HitZone colliders are Triggers
5. ✅ Layer collision matrix configured
6. ✅ Enable debug logging

**Then, investigate:**
- Console errors
- Debug log output
- Inspector values
- Scene hierarchy

**Most issues are:**
- Missing component in scene (Pool)
- Missing reference (prefab not assigned)
- Wrong settings (collider not trigger)
- Layer issues (collision matrix)

---

**Happy Debugging!** 🐛🔫
