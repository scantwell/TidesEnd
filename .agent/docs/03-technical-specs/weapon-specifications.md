# TIDE'S END: Weapon Technical Specifications
## Implementation-Ready Weapon Parameters

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Format:** JSON-compatible parameters for Unity implementation

---

## Document Purpose

This document provides **exact numerical parameters** for implementing all weapons. All values are ready for direct use in Unity ScriptableObjects.

**Related Documents:**
- Quick Reference: [weapons-quick-ref.md](../01-quick-reference/weapons-quick-ref.md)
- Design Context: [combat-system-design.md](../02-design-docs/combat-system-design.md)
- Balance Values: [balance-parameters.md](balance-parameters.md)

---

## TIER 1 WEAPONS (Free/Starting)

### M1903 Springfield (Bolt-Action Rifle)

```json
{
  "weaponName": "M1903 Springfield",
  "weaponType": "Rifle",
  "tier": 1,
  "unlockCost": 0,
  
  "damage": {
    "bodyshot": 85,
    "headshot": 170,
    "limbshot": 64
  },
  
  "fireRate": {
    "roundsPerMinute": 15,
    "timeBetweenShots": 4.0,
    "fireMode": "BoltAction"
  },
  
  "ammunition": {
    "magazineCapacity": 5,
    "startingMagazines": 3,
    "totalStartingAmmo": 15,
    "maxReserveAmmo": 30
  },
  
  "reload": {
    "normalReloadTime": 3.5,
    "emptyReloadMultiplier": 1.25,
    "movingReloadPenalty": 1.5,
    "canCancelReload": false
  },
  
  "accuracy": {
    "baseSpread": 0.05,
    "movingSpreadPenalty": 0.50,
    "jumpingSpreadPenalty": 0.75,
    "crouchAccuracyBonus": 0.10
  },
  
  "recoil": {
    "verticalKick": 8.0,
    "horizontalKick": 2.0,
    "recoilRecoveryTime": 1.5,
    "pattern": "StrongVertical"
  },
  
  "range": {
    "effectiveRange": 50.0,
    "maxRange": 100.0,
    "damageFalloffStart": 30.0,
    "damageFalloffEnd": 80.0
  },
  
  "handling": {
    "aimDownSightTime": 0.4,
    "sprintToFireTime": 0.6,
    "weaponSwapTime": 1.2
  }
}
```

**Balance Notes:**
- High damage per shot (2-shot kill on most enemies)
- Slow fire rate forces precision
- Long reload punishes misses
- Effective at range

---

### Double-Barrel Shotgun

```json
{
  "weaponName": "Double-Barrel Shotgun",
  "weaponType": "Shotgun",
  "tier": 1,
  "unlockCost": 0,
  
  "damage": {
    "pelletDamage": 18,
    "pelletsPerShot": 8,
    "maxDamagePerShot": 144,
    "headshotMultiplier": 1.5
  },
  
  "fireRate": {
    "roundsPerMinute": 120,
    "timeBetweenShots": 0.5,
    "timeBetweenBarrels": 0.15,
    "fireMode": "DoubleBarrel"
  },
  
  "ammunition": {
    "magazineCapacity": 2,
    "startingMagazines": 3,
    "totalStartingAmmo": 6,
    "maxReserveAmmo": 20
  },
  
  "reload": {
    "normalReloadTime": 2.8,
    "emptyReloadMultiplier": 1.0,
    "movingReloadPenalty": 1.3,
    "canCancelReload": false
  },
  
  "accuracy": {
    "pelletSpread": 12.0,
    "movingSpreadPenalty": 0.30,
    "jumpingSpreadPenalty": 0.60,
    "crouchAccuracyBonus": 0.15
  },
  
  "recoil": {
    "verticalKick": 15.0,
    "horizontalKick": 5.0,
    "recoilRecoveryTime": 1.0,
    "pattern": "MassiveKickback"
  },
  
  "range": {
    "effectiveRange": 8.0,
    "maxRange": 20.0,
    "damageFalloffStart": 5.0,
    "damageFalloffEnd": 15.0
  },
  
  "handling": {
    "aimDownSightTime": 0.3,
    "sprintToFireTime": 0.5,
    "weaponSwapTime": 1.0
  }
}
```

**Balance Notes:**
- Devastating close range (one-shot weak enemies)
- Only 2 shots before long reload
- Massive spread at distance
- High risk/reward weapon

---

### .38 Revolver (Sidearm)

```json
{
  "weaponName": ".38 Revolver",
  "weaponType": "Pistol",
  "tier": 1,
  "unlockCost": 0,
  
  "damage": {
    "bodyshot": 35,
    "headshot": 70,
    "limbshot": 26
  },
  
  "fireRate": {
    "roundsPerMinute": 180,
    "timeBetweenShots": 0.33,
    "fireMode": "SemiAuto"
  },
  
  "ammunition": {
    "magazineCapacity": 6,
    "startingMagazines": 2,
    "totalStartingAmmo": 12,
    "maxReserveAmmo": 36
  },
  
  "reload": {
    "normalReloadTime": 2.2,
    "emptyReloadMultiplier": 1.15,
    "movingReloadPenalty": 1.3,
    "canCancelReload": false
  },
  
  "accuracy": {
    "baseSpread": 0.15,
    "movingSpreadPenalty": 0.25,
    "jumpingSpreadPenalty": 0.60,
    "crouchAccuracyBonus": 0.10
  },
  
  "recoil": {
    "verticalKick": 4.0,
    "horizontalKick": 2.0,
    "recoilRecoveryTime": 0.4,
    "pattern": "ModerateVertical"
  },
  
  "range": {
    "effectiveRange": 15.0,
    "maxRange": 40.0,
    "damageFalloffStart": 10.0,
    "damageFalloffEnd": 30.0
  },
  
  "handling": {
    "aimDownSightTime": 0.2,
    "sprintToFireTime": 0.3,
    "weaponSwapTime": 0.8
  }
}
```

**Balance Notes:**
- Reliable backup weapon
- Fast swap time for emergency use
- Low damage requires multiple hits
- Good accuracy for sidearm

---

## TIER 2 WEAPONS (800-1,200 Scrip)

### Winchester 1897 (Pump Shotgun)

```json
{
  "weaponName": "Winchester 1897",
  "weaponType": "Shotgun",
  "tier": 2,
  "unlockCost": 1000,
  
  "damage": {
    "pelletDamage": 16,
    "pelletsPerShot": 8,
    "maxDamagePerShot": 128,
    "headshotMultiplier": 1.5
  },
  
  "fireRate": {
    "roundsPerMinute": 100,
    "timeBetweenShots": 0.6,
    "fireMode": "PumpAction"
  },
  
  "ammunition": {
    "magazineCapacity": 5,
    "startingMagazines": 3,
    "totalStartingAmmo": 15,
    "maxReserveAmmo": 30
  },
  
  "reload": {
    "normalReloadTime": 0.8,
    "shellByShellReload": true,
    "movingReloadPenalty": 1.2,
    "canCancelReload": true
  },
  
  "accuracy": {
    "pelletSpread": 10.0,
    "movingSpreadPenalty": 0.25,
    "jumpingSpreadPenalty": 0.50,
    "crouchAccuracyBonus": 0.15
  },
  
  "recoil": {
    "verticalKick": 12.0,
    "horizontalKick": 3.0,
    "recoilRecoveryTime": 0.7,
    "pattern": "StrongVertical"
  },
  
  "range": {
    "effectiveRange": 10.0,
    "maxRange": 25.0,
    "damageFalloffStart": 6.0,
    "damageFalloffEnd": 18.0
  },
  
  "handling": {
    "aimDownSightTime": 0.35,
    "sprintToFireTime": 0.5,
    "weaponSwapTime": 1.1
  }
}
```

**Balance Notes:**
- More shots than double-barrel
- Shell-by-shell reload (can cancel early)
- Slightly tighter spread
- Sustained close-range power

---

### M1911 Pistol (Semi-Auto Sidearm)

```json
{
  "weaponName": "M1911 Pistol",
  "weaponType": "Pistol",
  "tier": 2,
  "unlockCost": 800,
  
  "damage": {
    "bodyshot": 40,
    "headshot": 80,
    "limbshot": 30
  },
  
  "fireRate": {
    "roundsPerMinute": 240,
    "timeBetweenShots": 0.25,
    "fireMode": "SemiAuto"
  },
  
  "ammunition": {
    "magazineCapacity": 7,
    "startingMagazines": 3,
    "totalStartingAmmo": 21,
    "maxReserveAmmo": 42
  },
  
  "reload": {
    "normalReloadTime": 2.0,
    "emptyReloadMultiplier": 1.2,
    "movingReloadPenalty": 1.25,
    "canCancelReload": false
  },
  
  "accuracy": {
    "baseSpread": 0.12,
    "movingSpreadPenalty": 0.20,
    "jumpingSpreadPenalty": 0.55,
    "crouchAccuracyBonus": 0.12
  },
  
  "recoil": {
    "verticalKick": 3.5,
    "horizontalKick": 1.5,
    "recoilRecoveryTime": 0.35,
    "pattern": "MinimalKick"
  },
  
  "range": {
    "effectiveRange": 20.0,
    "maxRange": 45.0,
    "damageFalloffStart": 12.0,
    "damageFalloffEnd": 35.0
  },
  
  "handling": {
    "aimDownSightTime": 0.2,
    "sprintToFireTime": 0.25,
    "weaponSwapTime": 0.7
  }
}
```

**Balance Notes:**
- Upgraded sidearm
- Faster fire rate than .38
- Better accuracy and handling
- Still backup weapon role

---

### Lee-Enfield (Fast Bolt Rifle)

```json
{
  "weaponName": "Lee-Enfield",
  "weaponType": "Rifle",
  "tier": 2,
  "unlockCost": 1200,
  
  "damage": {
    "bodyshot": 75,
    "headshot": 150,
    "limbshot": 56
  },
  
  "fireRate": {
    "roundsPerMinute": 30,
    "timeBetweenShots": 2.0,
    "fireMode": "BoltAction"
  },
  
  "ammunition": {
    "magazineCapacity": 10,
    "startingMagazines": 3,
    "totalStartingAmmo": 30,
    "maxReserveAmmo": 50
  },
  
  "reload": {
    "normalReloadTime": 3.0,
    "emptyReloadMultiplier": 1.2,
    "movingReloadPenalty": 1.4,
    "canCancelReload": false
  },
  
  "accuracy": {
    "baseSpread": 0.06,
    "movingSpreadPenalty": 0.45,
    "jumpingSpreadPenalty": 0.70,
    "crouchAccuracyBonus": 0.12
  },
  
  "recoil": {
    "verticalKick": 6.0,
    "horizontalKick": 1.5,
    "recoilRecoveryTime": 1.2,
    "pattern": "MildVertical"
  },
  
  "range": {
    "effectiveRange": 55.0,
    "maxRange": 110.0,
    "damageFalloffStart": 35.0,
    "damageFalloffEnd": 85.0
  },
  
  "handling": {
    "aimDownSightTime": 0.45,
    "sprintToFireTime": 0.6,
    "weaponSwapTime": 1.3
  }
}
```

**Balance Notes:**
- Faster bolt action than M1903
- Larger magazine (10 rounds)
- Slightly less damage per shot
- Better sustained fire

---

## TIER 3 WEAPONS (2,000-3,000 Scrip)

### Thompson SMG (Full-Auto)

```json
{
  "weaponName": "Thompson SMG",
  "weaponType": "SMG",
  "tier": 3,
  "unlockCost": 2500,
  
  "damage": {
    "bodyshot": 22,
    "headshot": 44,
    "limbshot": 17
  },
  
  "fireRate": {
    "roundsPerMinute": 700,
    "timeBetweenShots": 0.086,
    "fireMode": "FullAuto"
  },
  
  "ammunition": {
    "magazineCapacity": 30,
    "startingMagazines": 3,
    "totalStartingAmmo": 90,
    "maxReserveAmmo": 150
  },
  
  "reload": {
    "normalReloadTime": 2.5,
    "emptyReloadMultiplier": 1.15,
    "movingReloadPenalty": 1.3,
    "canCancelReload": false
  },
  
  "accuracy": {
    "baseSpread": 0.20,
    "movingSpreadPenalty": 0.30,
    "jumpingSpreadPenalty": 0.60,
    "crouchAccuracyBonus": 0.15,
    "sustainedFireSpreadIncrease": 0.05
  },
  
  "recoil": {
    "verticalKick": 2.5,
    "horizontalKick": 1.5,
    "recoilRecoveryTime": 0.3,
    "pattern": "ClimbingSpray",
    "recoilPerShot": 0.5
  },
  
  "range": {
    "effectiveRange": 15.0,
    "maxRange": 35.0,
    "damageFalloffStart": 10.0,
    "damageFalloffEnd": 25.0
  },
  
  "handling": {
    "aimDownSightTime": 0.3,
    "sprintToFireTime": 0.4,
    "weaponSwapTime": 1.0
  }
}
```

**Balance Notes:**
- First full-auto weapon
- High fire rate, low damage per shot
- Recoil climbs during sustained fire
- Close-medium range specialist

---

### BAR (Browning Automatic Rifle)

```json
{
  "weaponName": "BAR",
  "weaponType": "AutoRifle",
  "tier": 3,
  "unlockCost": 3000,
  
  "damage": {
    "bodyshot": 50,
    "headshot": 100,
    "limbshot": 38
  },
  
  "fireRate": {
    "roundsPerMinute": 500,
    "timeBetweenShots": 0.12,
    "fireMode": "FullAuto"
  },
  
  "ammunition": {
    "magazineCapacity": 20,
    "startingMagazines": 3,
    "totalStartingAmmo": 60,
    "maxReserveAmmo": 100
  },
  
  "reload": {
    "normalReloadTime": 3.0,
    "emptyReloadMultiplier": 1.2,
    "movingReloadPenalty": 1.5,
    "canCancelReload": false
  },
  
  "accuracy": {
    "baseSpread": 0.10,
    "movingSpreadPenalty": 0.40,
    "jumpingSpreadPenalty": 0.75,
    "crouchAccuracyBonus": 0.20,
    "sustainedFireSpreadIncrease": 0.08
  },
  
  "recoil": {
    "verticalKick": 5.0,
    "horizontalKick": 2.0,
    "recoilRecoveryTime": 0.5,
    "pattern": "HeavyClimb",
    "recoilPerShot": 1.0
  },
  
  "range": {
    "effectiveRange": 35.0,
    "maxRange": 70.0,
    "damageFalloffStart": 25.0,
    "damageFalloffEnd": 55.0
  },
  
  "handling": {
    "aimDownSightTime": 0.5,
    "sprintToFireTime": 0.7,
    "weaponSwapTime": 1.5
  }
}
```

**Balance Notes:**
- High damage automatic rifle
- Heavy recoil requires control
- Slower fire rate than Thompson
- Medium-long range power

---

## TIER 4 WEAPONS (5,000+ Scrip + Requirements)

### Elephant Gun (Anti-Material Rifle)

```json
{
  "weaponName": "Elephant Gun",
  "weaponType": "HeavyRifle",
  "tier": 4,
  "unlockCost": 5000,
  "unlockRequirement": "Kill 3 Bosses",
  
  "damage": {
    "bodyshot": 200,
    "headshot": 400,
    "limbshot": 150
  },
  
  "fireRate": {
    "roundsPerMinute": 8,
    "timeBetweenShots": 7.5,
    "fireMode": "BreakAction"
  },
  
  "ammunition": {
    "magazineCapacity": 2,
    "startingMagazines": 2,
    "totalStartingAmmo": 4,
    "maxReserveAmmo": 12
  },
  
  "reload": {
    "normalReloadTime": 4.5,
    "emptyReloadMultiplier": 1.0,
    "movingReloadPenalty": 2.0,
    "canCancelReload": false
  },
  
  "accuracy": {
    "baseSpread": 0.02,
    "movingSpreadPenalty": 0.80,
    "jumpingSpreadPenalty": 0.95,
    "crouchAccuracyBonus": 0.15
  },
  
  "recoil": {
    "verticalKick": 25.0,
    "horizontalKick": 8.0,
    "recoilRecoveryTime": 3.0,
    "pattern": "MassiveKickback",
    "knockbackPlayer": true
  },
  
  "range": {
    "effectiveRange": 60.0,
    "maxRange": 120.0,
    "damageFalloffStart": 40.0,
    "damageFalloffEnd": 100.0,
    "penetration": true
  },
  
  "handling": {
    "aimDownSightTime": 0.8,
    "sprintToFireTime": 1.2,
    "weaponSwapTime": 2.0
  },
  
  "specialProperties": {
    "canPenetrateEnemies": true,
    "staggersLargeEnemies": true,
    "knockbackForce": 50.0
  }
}
```

**Balance Notes:**
- Massive single-shot damage
- Can penetrate through multiple enemies
- Extreme recoil and slow handling
- Boss-killer weapon

---

## SPECIAL AMMO TYPE

### Blessed Rounds

```json
{
  "ammoName": "Blessed Rounds",
  "ammoType": "Special",
  "unlockCost": 5000,
  "unlockRequirement": "Reach Level 15 on Any Class",
  
  "properties": {
    "compatibleWeapons": "All",
    "maxCapacity": 10,
    "capacityBonusHarpooner": 2,
    "cannotBeResupplied": true
  },
  
  "damageModifiers": {
    "vsEldritchEnemies": 1.50,
    "vsCorruptedHumans": 0.75,
    "vsBosses": 1.25
  },
  
  "visualEffects": {
    "bulletTracer": "Golden_Tracer",
    "impactEffect": "Holy_Burst",
    "muzzleFlash": "Blessed_Flash"
  },
  
  "audioEffects": {
    "fireSound": "Blessed_Shot",
    "impactSound": "Holy_Impact"
  }
}
```

**Usage Notes:**
- Load into any weapon (replaces normal ammo)
- Very limited capacity (10 rounds max)
- Cannot be resupplied during mission
- Must choose when to use strategically

---

## Damage Calculation System

### Base Damage Formula

```csharp
public float CalculateDamage(
    float baseDamage,
    float distance,
    string hitLocation,
    string enemyType,
    bool isBlessedAmmo
)
{
    // 1. Apply hit location multiplier
    float locationMultiplier = 1.0f;
    switch (hitLocation)
    {
        case "head":
            locationMultiplier = 2.0f;
            break;
        case "limb":
            locationMultiplier = 0.75f;
            break;
        default: // body
            locationMultiplier = 1.0f;
            break;
    }
    
    float damage = baseDamage * locationMultiplier;
    
    // 2. Apply distance falloff
    float falloffMultiplier = CalculateFalloff(distance);
    damage *= falloffMultiplier;
    
    // 3. Apply blessed rounds modifier if applicable
    if (isBlessedAmmo)
    {
        if (IsEldritchEnemy(enemyType))
            damage *= 1.5f;
        else if (IsCorruptedHuman(enemyType))
            damage *= 0.75f;
    }
    
    return damage;
}

private float CalculateFalloff(float distance)
{
    // Returns multiplier between 1.0 (close) and 0.2 (far)
    if (distance <= damageFalloffStart)
        return 1.0f; // Full damage
    
    if (distance >= damageFalloffEnd)
        return 0.2f; // Minimum 20% damage
    
    // Linear falloff between start and end
    float range = damageFalloffEnd - damageFalloffStart;
    float distanceInFalloff = distance - damageFalloffStart;
    float falloffPercent = distanceInFalloff / range;
    
    return Mathf.Lerp(1.0f, 0.2f, falloffPercent);
}
```

### Recoil System

```csharp
public class WeaponRecoil : MonoBehaviour
{
    [Header("Recoil Parameters")]
    public float verticalKick = 5.0f;
    public float horizontalKick = 2.0f;
    public float recoilRecoveryTime = 1.0f;
    
    private Vector2 currentRecoil;
    private Vector2 targetRecoil;
    
    public void ApplyRecoil()
    {
        // Random horizontal direction
        float horizontalRecoil = Random.Range(-horizontalKick, horizontalKick);
        
        // Add to target recoil
        targetRecoil += new Vector2(horizontalRecoil, verticalKick);
    }
    
    private void Update()
    {
        // Smoothly interpolate current recoil to target
        currentRecoil = Vector2.Lerp(
            currentRecoil,
            targetRecoil,
            Time.deltaTime * 10f
        );
        
        // Apply recoil to camera
        transform.localRotation = Quaternion.Euler(
            -currentRecoil.y,
            currentRecoil.x,
            0f
        );
        
        // Recover recoil over time
        targetRecoil = Vector2.Lerp(
            targetRecoil,
            Vector2.zero,
            Time.deltaTime / recoilRecoveryTime
        );
    }
}
```

### Accuracy System

```csharp
public class WeaponAccuracy : MonoBehaviour
{
    [Header("Accuracy Parameters")]
    public float baseSpread = 0.1f;
    public float movingSpreadPenalty = 0.4f;
    public float jumpingSpreadPenalty = 0.75f;
    public float crouchAccuracyBonus = 0.1f;
    
    public Vector3 GetBulletDirection(Vector3 aimDirection)
    {
        float currentSpread = CalculateCurrentSpread();
        
        // Add random spread within cone
        float spreadX = Random.Range(-currentSpread, currentSpread);
        float spreadY = Random.Range(-currentSpread, currentSpread);
        
        Vector3 spread = new Vector3(spreadX, spreadY, 0);
        
        return (aimDirection + spread).normalized;
    }
    
    private float CalculateCurrentSpread()
    {
        float spread = baseSpread;
        
        // Check player state
        if (playerController.isMoving)
            spread *= (1f + movingSpreadPenalty);
        
        if (playerController.isJumping)
            spread *= (1f + jumpingSpreadPenalty);
        
        if (playerController.isCrouching)
            spread *= (1f - crouchAccuracyBonus);
        
        return spread;
    }
}
```

---

## Unity ScriptableObject Implementation

### Weapon Data Structure

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "TidesEnd/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    public string weaponName;
    public WeaponType weaponType;
    public int tier;
    public int unlockCost;
    public string unlockRequirement;
    
    [Header("Damage")]
    public float bodyshotDamage;
    public float headshotMultiplier = 2f;
    public float limbshotMultiplier = 0.75f;
    
    [Header("Fire Rate")]
    public float roundsPerMinute;
    public FireMode fireMode;
    
    [Header("Ammunition")]
    public int magazineCapacity;
    public int startingMagazines;
    public int maxReserveAmmo;
    
    [Header("Reload")]
    public float normalReloadTime;
    public float emptyReloadMultiplier = 1.25f;
    public bool reloadCancelable = false;
    
    [Header("Recoil")]
    public float verticalKick;
    public float horizontalKick;
    public float recoilRecoveryTime;
    
    [Header("Accuracy")]
    public float baseSpread;
    public float movingSpreadPenalty = 0.40f;
    
    [Header("Range")]
    public float effectiveRange;
    public float damageFalloffStart;
    public float damageFalloffEnd;
    
    [Header("References")]
    public GameObject weaponPrefab;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public GameObject muzzleFlashVFX;
}

public enum WeaponType
{
    Rifle,
    Shotgun,
    Pistol,
    SMG,
    AutoRifle,
    HeavyRifle
}

public enum FireMode
{
    SemiAuto,
    BoltAction,
    PumpAction,
    FullAuto,
    BreakAction,
    DoubleBarrel
}
```

---

## Weapon Balance Guidelines

### DPS Targets by Tier

```
Tier 1: 40-60 DPS
Tier 2: 60-80 DPS
Tier 3: 80-120 DPS
Tier 4: 120-180 DPS
```

### Shots to Kill (Body Shots vs 100 HP Enemy)

```
M1903 Springfield: 2 shots
Double-Barrel: 1 shot (close range)
.38 Revolver: 3 shots
Winchester 1897: 1-2 shots
M1911: 3 shots
Lee-Enfield: 2 shots
Thompson: 5 shots
BAR: 2 shots
Elephant Gun: 1 shot
```

### Ammo Economy

**Starting ammo should last:**
- Conservative play: 20-25 minutes
- Aggressive play: 10-15 minutes
- Encourages ammo box pickups and resupply abilities

---

## Testing Checklist

**Per Weapon:**
- [ ] Damage values correct at all ranges
- [ ] Fire rate timing accurate
- [ ] Reload timing accurate
- [ ] Recoil pattern matches specification
- [ ] Audio/VFX trigger correctly
- [ ] Ammo tracking works
- [ ] Headshot detection works

**Global Systems:**
- [ ] Damage falloff curves implemented
- [ ] Moving accuracy penalty works
- [ ] Reload canceling (where applicable)
- [ ] Blessed Rounds damage modifiers work

---

## Related Documentation

**For quick reference:**
- [Weapons Quick Ref](../01-quick-reference/weapons-quick-ref.md)

**For balance tuning:**
- [Balance Parameters](balance-parameters.md)

**For combat design:**
- [Combat System Design](../02-design-docs/combat-system-design.md)

---

**This document provides exact parameters ready for Unity implementation. All JSON can be loaded via ScriptableObjects.**
