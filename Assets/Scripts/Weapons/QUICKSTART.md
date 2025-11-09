# Projectile Weapon System - Quick Start Guide

## 5-Minute Setup

### 1. Add Managers to Scene (30 seconds)

Create two empty GameObjects in your scene:

```
Hierarchy:
├── ProjectilePool
└── TracerPool
```

- Add `ProjectilePool.cs` component to `ProjectilePool` GameObject
- Add `TracerPool.cs` component to `TracerPool` GameObject
- On TracerPool, assign your tracer prefab (if you have one)

### 2. Create Projectile Prefab (2 minutes)

1. Create → Empty GameObject → Name: `Projectile_Rifle`

2. Add components:
   - `Projectile` (script)
   - `Rigidbody`
   - `SphereCollider`

3. Configure:
   - **Rigidbody**: Uncheck "Use Gravity", Set Collision Detection to "Continuous Dynamic"
   - **SphereCollider**: Check "Is Trigger", Set Radius to 0.05

4. Set Layer to "Projectile" (create layer if needed)

5. Drag to `Assets/Prefabs/Weapons/Projectiles/` to create prefab

### 3. Setup Character Hit Zones (1 minute)

On your player/enemy character:

1. Find head, body, arm, leg colliders (or add them)
2. Add `HitZone.cs` component to each collider
3. Set each HitZone type:
   - Head collider → Zone Type: Head
   - Body collider → Zone Type: Body
   - Arm/Leg colliders → Zone Type: Limb
4. Make sure all are **Triggers** (check "Is Trigger")

### 4. Create Weapon Data (1 minute)

1. Right-click in Project → `Create/Game/Weapon Data`
2. Name: `WD_TestRifle`
3. Set basic values:
   - Weapon Name: "Test Rifle"
   - Damage: 50
   - Fire Rate: 5 (5 rounds per second)
   - Magazine Size: 30
   - **Projectile Prefab**: Drag your `Projectile_Rifle` prefab here
   - Projectile Velocity: 300
   - Use Tracers: Uncheck for now

### 5. Test It! (30 seconds)

On your weapon GameObject:

1. `Weapon` component → Data: Assign `WD_TestRifle`
2. `Weapon` component → Fire Origin: Create child GameObject at barrel tip
3. Press Play
4. Call `weapon.Fire()` from your input code

You should see projectiles spawn and travel!

---

## Minimal Input Code

Add this to your player controller:

```csharp
using UnityEngine;

public class TestWeaponInput : MonoBehaviour
{
    private Weapon weapon;

    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.SetPlayerCamera(Camera.main);
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            weapon.Fire();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.Reload();
        }
    }
}
```

---

## Troubleshooting

**Projectiles don't spawn:**
→ Check ProjectilePool exists in scene
→ Check projectile prefab assigned to WeaponData

**Projectiles spawn but don't move:**
→ Check Rigidbody "Use Gravity" is OFF (we use custom gravity)
→ Check Projectile Velocity > 0 in WeaponData

**Projectiles pass through enemies:**
→ Check HitZone colliders are Triggers
→ Check enemy has IDamageable component (like Health.cs)

**No damage:**
→ Check HitZone's Owner field auto-detected the IDamageable component
→ Enable "Show Debug Info" on Projectile.cs to see hit logs

---

## Next Steps

1. **Add visual polish:**
   - Create tracer prefab with TrailRenderer
   - Add muzzle flash VFX
   - Add hit effect particles

2. **Create more weapons:**
   - Duplicate WeaponData asset
   - Adjust values per weapon type
   - Create weapon-specific projectile prefabs if needed

3. **Add HUD:**
   - Display `weapon.CurrentAmmo` and `weapon.ReserveAmmo`
   - Show reload indicator when `weapon.IsReloading`

4. **Tune balance:**
   - Adjust damage values
   - Tweak fire rates
   - Test damage falloff ranges

---

## Full Documentation

See [README_PROJECTILE_SYSTEM.md](README_PROJECTILE_SYSTEM.md) for complete documentation.
