# Projectile-Based Weapon System

## Overview

This weapon system uses **physical projectiles** instead of hitscan raycasts. Bullets are actual GameObjects that travel through the world, providing:
- Visual feedback (see bullets travel)
- Dodge-able projectiles at range
- Natural penetration support
- Better 1920s weapon feel
- Easy network synchronization

---

## Core Components

### 1. **Projectile.cs**
Physical bullet that travels through space and applies damage on hit.

**Features:**
- Rigidbody-based physics movement
- Distance tracking for damage falloff
- Hit location detection (head/body/limb)
- Penetration support
- Automatic lifetime management
- Object pooling integration

### 2. **ProjectilePool.cs**
Object pool manager for projectiles.

**Features:**
- Separate pools per projectile type
- Pre-warming support
- Automatic expansion
- Debug statistics

### 3. **ProjectileConfig**
Struct containing all projectile parameters.

**Key Fields:**
- `baseDamage` - Starting damage
- `velocity` - Speed in units/second
- `falloffStart/End` - Distance ranges
- `headshotMultiplier` - Head damage multiplier
- `canPenetrate` - Enable penetration
- `maxPenetrations` - Max enemies to penetrate

### 4. **HitZone.cs**
Defines hit locations on characters for damage multipliers.

**Zone Types:**
- **Head**: 2x damage (default)
- **Body**: 1x damage (default)
- **Limb**: 0.75x damage (default)

### 5. **BulletTracer.cs** (Updated)
Visual tracer effect that attaches to projectiles.

**Modes:**
- **Standalone**: Moves independently
- **Attached**: Child of projectile (recommended)

---

## Scene Setup

### Step 1: Add Required Managers

Add to your scene (one time setup):

```
Hierarchy:
├── ProjectilePool (empty GameObject with ProjectilePool component)
└── TracerPool (empty GameObject with TracerPool component)
```

**ProjectilePool Settings:**
- Default Pool Size: 100
- Expand Pool: ✓
- Max Pool Size: 500

**TracerPool Settings:**
- Tracer Prefab: [Assign your tracer prefab]
- Pool Size: 50
- Expand Pool: ✓

### Step 2: Create Projectile Prefab

1. Create empty GameObject: `Projectile_Rifle`
2. Add components:
   - `Projectile.cs`
   - `Rigidbody`
   - `SphereCollider` (trigger)
   - `TrailRenderer` (optional)
   - `Light` (optional)

**Rigidbody Settings:**
- Mass: 0.01
- Drag: 0
- Angular Drag: 0
- Use Gravity: ✗ (we use custom gravity)
- Is Kinematic: ✗
- Interpolate: Interpolate
- Collision Detection: Continuous Dynamic

**Collider Settings:**
- Is Trigger: ✓
- Radius: 0.05 (small)
- Layer: Projectile (create custom layer)

3. Save as prefab in `Assets/Prefabs/Weapons/Projectiles/`

### Step 3: Create Tracer Prefab (Optional)

1. Create empty GameObject: `Tracer_Rifle`
2. Add components:
   - `BulletTracer.cs`
   - `TrailRenderer`
   - `Light`

**TrailRenderer Settings:**
- Time: 0.3
- Width: 0.05 → 0.01
- Color: Yellow/Orange gradient
- Material: Additive particle material

**Light Settings:**
- Type: Point
- Range: 5
- Intensity: 2
- Color: Yellow/Orange

3. Save as prefab

### Step 4: Setup Character Hit Zones

On player/enemy character models:

1. Add colliders for each body part:
   - Head: SphereCollider
   - Body: CapsuleCollider
   - Arms/Legs: CapsuleColliders

2. Add `HitZone.cs` to each collider

3. Configure HitZone:
   - Set Zone Type (Head/Body/Limb)
   - Leave Owner Object empty (auto-detects)

4. Ensure all hit zone colliders are **Triggers** ✓

**Example Hierarchy:**
```
Player
├── Health (IDamageable)
├── Head (SphereCollider + HitZone: Head)
├── Body (CapsuleCollider + HitZone: Body)
├── ArmLeft (CapsuleCollider + HitZone: Limb)
├── ArmRight (CapsuleCollider + HitZone: Limb)
├── LegLeft (CapsuleCollider + HitZone: Limb)
└── LegRight (CapsuleCollider + HitZone: Limb)
```

### Step 5: Create WeaponData Asset

1. Right-click in Project → `Create/Game/Weapon Data`
2. Name it: `WD_M1903_Springfield`
3. Configure parameters:

**Basic:**
- Weapon Name: "M1903 Springfield"
- Damage: 85
- Fire Rate: 0.25 (rounds per second)
- Range: 100

**Ammo:**
- Magazine Size: 5
- Max Ammo: 30
- Reload Time: 3.5

**Accuracy:**
- Base Spread: 0.5
- Movement Spread Multiplier: 2.0

**Damage Falloff:**
- Falloff Start: 30
- Falloff End: 80

**Hit Location Multipliers:**
- Headshot Multiplier: 2.0
- Limbshot Multiplier: 0.75

**Projectile Settings:**
- **Projectile Prefab**: [Drag Projectile_Rifle prefab]
- Projectile Velocity: 853 (realistic rifle speed)
- Projectile Lifetime: 2.0
- Projectile Gravity: 0 (no drop for hitscan feel)

**Visual Effects:**
- Muzzle Flash Prefab: [Optional]
- Hit Effect Prefab: [Optional]
- Use Tracers: ✓
- Tracer Frequency: 1 (every bullet)

### Step 6: Setup Weapon GameObject

1. Create weapon GameObject: `M1903_Springfield`
2. Add components:
   - `Weapon.cs`
   - `WeaponView.cs`
3. Add child object: `FireOrigin` (where bullets spawn)

**Weapon Component:**
- Data: [Assign WD_M1903_Springfield]
- Fire Origin: [Assign FireOrigin transform]
- Player Camera: [Will be set at runtime]

4. Save as prefab in `Assets/Prefabs/Weapons/`

---

## Usage in Code

### Basic Weapon Firing

```csharp
// Get weapon component
Weapon weapon = GetComponent<Weapon>();

// Set player camera for accurate aiming
weapon.SetPlayerCamera(Camera.main);

// Set owner ID (for network gameplay)
weapon.SetOwner(NetworkManager.Singleton.LocalClientId);

// Fire weapon (call from input handler)
if (Input.GetButtonDown("Fire1"))
{
    weapon.Fire();
}

// Reload
if (Input.GetKeyDown(KeyCode.R))
{
    weapon.Reload();
}
```

### Integrating with Player Controller

```csharp
public class PlayerWeaponController : MonoBehaviour
{
    private Weapon currentWeapon;
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        currentWeapon = GetComponentInChildren<Weapon>();

        if (currentWeapon != null)
        {
            currentWeapon.SetPlayerCamera(playerCamera);
            currentWeapon.SetOwner(GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    private void Update()
    {
        if (currentWeapon == null) return;

        // Fire
        if (Input.GetButton("Fire1") && currentWeapon.CanFire())
        {
            currentWeapon.Fire();
        }

        // Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
        }
    }
}
```

### Pre-warming Projectile Pool

```csharp
public class GameInitializer : MonoBehaviour
{
    [SerializeField] private List<GameObject> projectilePrefabs;

    private void Start()
    {
        // Pre-warm pools for better performance
        foreach (GameObject prefab in projectilePrefabs)
        {
            ProjectilePool.Instance.InitializePool(prefab, 50);
        }
    }
}
```

---

## Weapon Configuration Examples

### M1903 Springfield (Bolt-Action Rifle)
```
Damage: 85
Fire Rate: 0.25 (15 RPM)
Velocity: 853 m/s
Gravity: 0 (hitscan feel)
Falloff: 30-80m
Spread: 0.5°
```

### Double-Barrel Shotgun
```
Damage: 18 per pellet
Projectiles Per Shot: 8
Fire Rate: 2.0 (120 RPM)
Velocity: 400 m/s
Gravity: 2.0 (noticeable drop)
Falloff: 5-15m
Spread: 12°
```

### Thompson SMG
```
Damage: 22
Fire Rate: 11.67 (700 RPM)
Velocity: 280 m/s
Gravity: 0.5 (slight drop)
Falloff: 10-25m
Spread: 2°
```

### Elephant Gun (Heavy Rifle)
```
Damage: 200
Fire Rate: 0.133 (8 RPM)
Velocity: 640 m/s
Gravity: 0
Falloff: 40-100m
Penetration: ✓ (5 enemies)
Spread: 0.2°
```

---

## Physics & Collision Setup

### Layer Configuration

Create these layers:
- `Projectile` (layer 10)
- `Player` (layer 8)
- `Enemy` (layer 9)
- `HitZone` (layer 11)

### Collision Matrix

Edit → Project Settings → Physics → Layer Collision Matrix:

```
              Projectile  Player  Enemy  HitZone
Projectile    ✗          ✗       ✗      ✓
Player        ✗          ✗       ✓      ✗
Enemy         ✗          ✓       ✗      ✗
HitZone       ✓          ✗       ✗      ✗
```

**Key Rules:**
- Projectiles only collide with HitZones
- Projectiles don't collide with each other
- Players and enemies collide normally (for movement)

### Assigning Layers

- Projectile prefabs → `Projectile` layer
- HitZone colliders → `HitZone` layer
- Character bodies → `Player` or `Enemy` layer

---

## Debugging

### Enable Debug Info

On components, check "Show Debug Info":
- `Projectile.cs` - Shows damage, distance, hits
- `Weapon.cs` - Shows firing, ammo, spawning
- `ProjectilePool.cs` - Shows pool statistics

### Debug Visualizations

`ProjectilePool` shows on-screen stats when debug enabled:
```
Projectile Pool Stats
Projectile_Rifle: 45 pooled, 5 active
Projectile_Shotgun: 38 pooled, 12 active
Total: 100
```

### Common Issues

**Projectiles spawn but don't move:**
- Check Rigidbody isn't kinematic
- Check projectile velocity > 0
- Ensure ProjectilePool exists in scene

**Projectiles pass through enemies:**
- Check HitZone colliders are triggers
- Check Projectile collider is trigger
- Verify layer collision matrix
- Ensure enemy has IDamageable component

**Damage not applied:**
- Check enemy implements IDamageable interface
- Check HitZone has reference to owner
- Enable debug info on Projectile to see hits

**Pool exhausted warnings:**
- Increase pool size
- Enable pool expansion
- Check for projectile leaks (not returning to pool)

---

## Performance Optimization

### Best Practices

1. **Pre-warm pools** at game start
2. **Limit active projectiles** (max 100-200)
3. **Use short lifetimes** (1-3 seconds)
4. **Disable expensive features** on projectiles:
   - Lights (use sparingly)
   - Complex trails
   - Physics interactions beyond triggers

### Optimization Settings

**For 60 FPS with 100 projectiles:**
- Use `Continuous Dynamic` collision only on fast projectiles
- Use `Interpolate` for smooth visuals
- Keep colliders simple (spheres, not meshes)
- Limit tracer particle count
- Use object pooling (already implemented)

### Profiling

Monitor these stats:
- Active projectile count (ProjectilePool debug)
- Physics performance (Profiler → Physics)
- GC allocations (should be zero during gameplay with pooling)

---

## Network Synchronization (Future)

When adding Netcode support:

1. Add `NetworkObject` to projectile prefab
2. Add `NetworkRigidbody` to projectile prefab
3. Make `Weapon` inherit from `NetworkBehaviour`
4. Wrap `Fire()` in `[ServerRpc]`:

```csharp
[ServerRpc]
public void FireServerRpc(Vector3 origin, Vector3 direction)
{
    // Server spawns projectile
    Projectile proj = ProjectilePool.Instance.GetProjectile(data.projectilePrefab);
    proj.GetComponent<NetworkObject>().Spawn();
    proj.Initialize(config, ownerObject);
}
```

Server authoritative = projectiles spawn on server, sync to clients.

---

## Testing Checklist

- [ ] ProjectilePool and TracerPool in scene
- [ ] Projectile prefab has all required components
- [ ] WeaponData asset configured with projectile prefab
- [ ] Character has HitZones with correct zone types
- [ ] Character implements IDamageable interface
- [ ] Layer collision matrix configured
- [ ] Weapon has Fire Origin transform
- [ ] Camera assigned to weapon at runtime
- [ ] Fire weapon and see projectile spawn
- [ ] Projectile travels in correct direction
- [ ] Projectile applies damage on hit
- [ ] Headshots deal 2x damage
- [ ] Limb shots deal 0.75x damage
- [ ] Tracers appear (if enabled)
- [ ] Projectiles return to pool after hit/lifetime
- [ ] No errors in console

---

## API Reference

### Weapon.cs

```csharp
// Fire the weapon (spawns projectiles)
public void Fire()

// Reload the weapon
public void Reload()

// Check if weapon can fire
public bool CanFire()

// Set owner for damage attribution
public void SetOwner(ulong networkOwnerId)

// Set camera for aiming
public void SetPlayerCamera(Camera camera)

// Add reserve ammo
public void AddAmmo(int amount)
```

### Projectile.cs

```csharp
// Initialize and launch projectile
public void Initialize(ProjectileConfig config, GameObject owner)

// Force stop and return to pool
public void ForceStop()
```

### ProjectilePool.cs

```csharp
// Pre-warm pool for specific projectile type
public void InitializePool(GameObject prefab, int size)

// Get projectile from pool
public Projectile GetProjectile(GameObject prefab)

// Return projectile to pool
public void ReturnProjectile(Projectile projectile)

// Clear all active projectiles
public void ClearAllProjectiles()
```

### HitZone.cs

```csharp
// Zone type (Head, Body, Limb)
public ZoneType Type { get; }

// Damage multiplier for this zone
public float Multiplier { get; }

// Owner that receives damage
public IDamageable Owner { get; }
```

---

## Credits

Projectile system designed for Tide's End - a 1920s Lovecraftian co-op extraction shooter.

Based on weapon specifications from [.agent/docs/03-technical-specs/weapon-specifications.md]
