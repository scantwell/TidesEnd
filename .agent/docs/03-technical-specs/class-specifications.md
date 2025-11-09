# TIDE'S END: Class Technical Specifications
## Implementation-Ready Ability Parameters

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Format:** JSON-compatible parameters for Unity ScriptableObjects

---

## Document Purpose

This document provides **exact numerical parameters** for implementing all class abilities. All values are ready for direct use in Unity ScriptableObjects or configuration files.

**Related Documents:**
- Quick Reference: [classes-quick-ref.md](../01-quick-reference/classes-quick-ref.md)
- Design Context: [combat-system-design.md](../02-design-docs/combat-system-design.md)
- Balance Values: [balance-parameters.md](balance-parameters.md)

---

## CLASS 1: BULWARK (Tank/Anchor)

### Base Stats

```json
{
  "className": "Bulwark",
  "classRole": "Tank",
  "baseHealth": 130,
  "baseMovementSpeed": 5.0,
  "sprintMultiplier": 1.4,
  "crouchMultiplier": 0.5
}
```

### Passive Ability: "Steadfast"

```json
{
  "abilityName": "Steadfast",
  "abilityType": "Passive",
  "parameters": {
    "maxHealthBonus": 30,
    "damageResistanceThreshold": 0.5,
    "damageResistanceAmount": 0.15,
    "knockbackReduction": 0.5
  },
  "description": "Grants +30 max HP, +15% damage resistance below 50% health, -50% knockback"
}
```

**Implementation Notes:**
- `maxHealthBonus`: Add to base 100 HP = 130 HP total
- `damageResistanceThreshold`: Triggers at 50% health (65 HP)
- `damageResistanceAmount`: Multiply incoming damage by 0.85
- `knockbackReduction`: Multiply knockback force by 0.5

### Active Ability 1: "Anchor Point"

```json
{
  "abilityName": "Anchor Point",
  "abilityType": "Deployable",
  "cooldown": 60.0,
  "castTime": 0.8,
  "throwParameters": {
    "maxThrowDistance": 15.0,
    "throwArc": 0.3,
    "projectileSpeed": 20.0
  },
  "zoneParameters": {
    "radius": 10.0,
    "duration": 20.0,
    "destructible": true,
    "deviceHealth": 200
  },
  "effects": {
    "breachSaturationIncrease": 0.0,
    "enemyMovementSpeedMultiplier": 0.7,
    "allyKnockbackImmunity": true
  },
  "visualFX": {
    "devicePrefab": "Anchor_Brass_Device",
    "zoneMaterial": "Golden_Shimmer_Field",
    "pulseInterval": 1.5,
    "glowColor": "#FFD700"
  },
  "audioFX": {
    "deploySound": "Anchor_Deploy",
    "activeSound": "Anchor_Hum_Loop",
    "destroySound": "Anchor_Break"
  }
}
```

**Implementation Notes:**
- Throwable physics object with arc trajectory
- On ground contact: instantiate zone GameObject
- Zone effect applies to all entities in radius using sphere collider trigger
- Breach Saturation increase = 0 while inside zone (pauses timer)
- Enemy speed: multiply AI agent speed by 0.7
- Device can be destroyed: track HP, explode when HP <= 0

### Active Ability 2: "Bastion Shield"

```json
{
  "abilityName": "Bastion Shield",
  "abilityType": "Deployable",
  "cooldown": 90.0,
  "deployTime": 3.0,
  "pickupTime": 5.0,
  "shieldParameters": {
    "health": 500,
    "blockArc": 120.0,
    "duration": 30.0,
    "playerCanShootThrough": true,
    "enemyBlocked": true
  },
  "physicsParameters": {
    "isPhysical": true,
    "colliderHeight": 2.0,
    "colliderWidth": 1.5,
    "colliderThickness": 0.3
  },
  "visualFX": {
    "shieldPrefab": "Riot_Shield_Wards",
    "hitGlowColor": "#4169E1",
    "hitParticles": "Shield_Impact_Sparks",
    "destroyParticles": "Shield_Shatter"
  },
  "audioFX": {
    "deploySound": "Shield_Plant",
    "hitSound": "Shield_Impact",
    "destroySound": "Shield_Break"
  }
}
```

**Implementation Notes:**
- Player is vulnerable during `deployTime` (3 seconds)
- Shield spawns as static world object with collider
- Rotates to face player's forward direction on deploy
- Block arc: 120° cone in front of shield
- Allies can shoot through (ignore collision for ally bullets)
- Enemies cannot pass (physical collider blocks movement)
- Track shield HP: damage from hits reduces HP, destroyed at 0 HP
- Optional: Allow pickup/redeploy (requires `pickupTime` channel)

### Ability Upgrade Paths

#### Anchor Point Upgrades

```json
{
  "tier1": {
    "name": "Base Anchor Point",
    "unlockLevel": 1,
    "unlockCost": 0,
    "parameters": {
      "radius": 10.0,
      "duration": 20.0,
      "deviceHealth": 200
    }
  },
  "tier2": {
    "name": "Reinforced Anchor",
    "unlockLevel": 5,
    "unlockCost": 500,
    "changes": {
      "deviceHealth": 300,
      "duration": 25.0
    }
  },
  "tier3": {
    "name": "Reality Lock",
    "unlockLevel": 10,
    "unlockCost": 1500,
    "changes": {
      "deviceHealth": 400,
      "duration": 30.0,
      "newEffect": {
        "enemyDamageReduction": 0.15
      }
    }
  }
}
```

#### Bastion Shield Upgrades

```json
{
  "tier1": {
    "name": "Base Bastion Shield",
    "unlockLevel": 1,
    "unlockCost": 0,
    "parameters": {
      "health": 500,
      "duration": 30.0
    }
  },
  "tier2": {
    "name": "Fortified Barrier",
    "unlockLevel": 5,
    "unlockCost": 500,
    "changes": {
      "health": 750,
      "blockArc": 140.0
    }
  },
  "tier3": {
    "name": "Warded Bulwark",
    "unlockLevel": 10,
    "unlockCost": 1500,
    "changes": {
      "health": 1000,
      "blockArc": 160.0,
      "newEffect": {
        "alliesGainDamageResistance": 0.10
      }
    }
  }
}
```

---

## CLASS 2: LAMPLIGHTER (Scout/Intel)

### Base Stats

```json
{
  "className": "Lamplighter",
  "classRole": "Scout",
  "baseHealth": 100,
  "baseMovementSpeed": 6.0,
  "sprintMultiplier": 1.5,
  "crouchMultiplier": 0.6
}
```

### Passive Ability: "Pathfinder"

```json
{
  "abilityName": "Pathfinder",
  "abilityType": "Passive",
  "parameters": {
    "movementSpeedBonus": 0.20,
    "fogVisionBonus": 0.30,
    "footstepVolumeMultiplier": 0.60
  },
  "description": "+20% move speed, see 30% further through fog, 40% quieter footsteps"
}
```

**Implementation Notes:**
- `movementSpeedBonus`: Multiply base speed by 1.20
- `fogVisionBonus`: Increase fog render distance by 30% for this player
- `footstepVolumeMultiplier`: Reduce footstep audio range by 40%

### Active Ability 1: "Flare Shot"

```json
{
  "abilityName": "Flare Shot",
  "abilityType": "Projectile",
  "cooldown": 45.0,
  "castTime": 0.5,
  "projectileParameters": {
    "arcHeight": 15.0,
    "projectileSpeed": 25.0,
    "gravity": 9.8,
    "maxRange": 50.0
  },
  "flareParameters": {
    "radius": 20.0,
    "duration": 30.0,
    "lightIntensity": 5.0,
    "revealEnemies": true,
    "revealThroughWalls": true
  },
  "visualFX": {
    "projectilePrefab": "Flare_Projectile",
    "flareLight": "Point_Light_White",
    "enemyOutlineColor": "#FF0000",
    "flareParticles": "Flare_Glow_Particles"
  },
  "audioFX": {
    "launchSound": "Flare_Gun_Shot",
    "hangSound": "Flare_Hiss_Loop",
    "expireSound": "Flare_Fizzle"
  }
}
```

**Implementation Notes:**
- Fire projectile with arc trajectory (parabola)
- On apex: flare hangs in air (SetKinematic = true)
- Create Point Light at flare position (intensity 5.0, range 20m)
- Enemy reveal: Use sphere overlap to detect enemies in radius
- Apply outline shader to revealed enemies (visible through walls)
- Outline persists for full duration (30s)
- Flare expires: destroy GameObject, remove outlines

### Active Ability 2: "Pathfinder's Mark"

```json
{
  "abilityName": "Pathfinder's Mark",
  "abilityType": "PlacedZone",
  "cooldown": 60.0,
  "castTime": 1.0,
  "zoneParameters": {
    "radius": 15.0,
    "duration": 20.0,
    "movementSpeedBonus": 0.40
  },
  "visualFX": {
    "groundDecal": "Glowing_Trail_Markers",
    "particleEffect": "Speed_Boost_Aura",
    "glowColor": "#00FF00"
  },
  "audioFX": {
    "castSound": "Mark_Place",
    "enterZoneSound": "Speed_Boost_Activate",
    "activeSound": "Wind_Rush_Loop"
  }
}
```

**Implementation Notes:**
- Place zone at ground position under crosshair (raycast)
- Zone rendered as decal + particle ring
- Use trigger collider to detect players entering/exiting
- On enter: multiply player speed by 1.40
- On exit: reset player speed to normal
- Duration timer: destroy zone after 20 seconds

### Ability Upgrade Paths

#### Flare Shot Upgrades

```json
{
  "tier1": {
    "name": "Base Flare Shot",
    "unlockLevel": 1,
    "unlockCost": 0,
    "parameters": {
      "radius": 20.0,
      "duration": 30.0
    }
  },
  "tier2": {
    "name": "Bright Flare",
    "unlockLevel": 5,
    "unlockCost": 500,
    "changes": {
      "radius": 25.0,
      "duration": 40.0
    }
  },
  "tier3": {
    "name": "Revealing Light",
    "unlockLevel": 10,
    "unlockCost": 1500,
    "changes": {
      "radius": 30.0,
      "duration": 50.0,
      "newEffect": {
        "revealedEnemiesTakeBonusDamage": 0.10
      }
    }
  }
}
```

#### Pathfinder's Mark Upgrades

```json
{
  "tier1": {
    "name": "Base Pathfinder's Mark",
    "unlockLevel": 1,
    "unlockCost": 0,
    "parameters": {
      "radius": 15.0,
      "movementSpeedBonus": 0.40
    }
  },
  "tier2": {
    "name": "Swift Path",
    "unlockLevel": 5,
    "unlockCost": 500,
    "changes": {
      "radius": 20.0,
      "movementSpeedBonus": 0.50
    }
  },
  "tier3": {
    "name": "Quickening Field",
    "unlockLevel": 10,
    "unlockCost": 1500,
    "changes": {
      "radius": 25.0,
      "movementSpeedBonus": 0.60,
      "newEffect": {
        "reloadSpeedBonus": 0.20
      }
    }
  }
}
```

---

## CLASS 3: HARPOONER (Damage/Control)

### Base Stats

```json
{
  "className": "Harpooner",
  "classRole": "Damage",
  "baseHealth": 100,
  "baseMovementSpeed": 5.5,
  "sprintMultiplier": 1.45,
  "crouchMultiplier": 0.55
}
```

### Passive Ability: "Maritime Predator"

```json
{
  "abilityName": "Maritime Predator",
  "abilityType": "Passive",
  "parameters": {
    "weakPointDamageBonus": 0.20,
    "reloadSpeedBonus": 0.15,
    "specialAmmoCapacityBonus": 2
  },
  "description": "+20% weak point damage, +15% reload speed, +2 special ammo capacity"
}
```

**Implementation Notes:**
- `weakPointDamageBonus`: Multiply headshot damage by 1.20
- `reloadSpeedBonus`: Divide reload time by 1.15
- `specialAmmoCapacityBonus`: Add +2 to blessed rounds capacity

### Active Ability 1: "Whaling Harpoon"

```json
{
  "abilityName": "Whaling Harpoon",
  "abilityType": "Projectile",
  "cooldown": 50.0,
  "castTime": 1.2,
  "harpoonParameters": {
    "projectileSpeed": 40.0,
    "maxRange": 30.0,
    "damage": 60,
    "pullForce": 20.0,
    "pullDuration": 2.0,
    "canPullBosses": false
  },
  "targetingParameters": {
    "requiresLineOfSight": true,
    "hitboxSize": 0.3,
    "canHitMultiple": false
  },
  "visualFX": {
    "harpoonPrefab": "Whaling_Harpoon_Projectile",
    "chainPrefab": "Harpoon_Chain",
    "hitParticles": "Harpoon_Impact_Blood",
    "pullTrail": "Drag_Trail"
  },
  "audioFX": {
    "fireSound": "Harpoon_Fire",
    "impactSound": "Harpoon_Impact_Flesh",
    "pullSound": "Chain_Pull_Loop"
  }
}
```

**Implementation Notes:**
- Fire line-cast raycast on button press
- On hit enemy: apply damage (60), spawn harpoon at hit point
- Pull mechanic: Apply force to enemy NavMeshAgent toward player
- Duration: Pull for 2 seconds total
- Boss immunity: Check enemy type, skip pull if boss (still deals damage)
- Visual chain: LineRenderer from player to harpoon during pull
- After duration: destroy harpoon, remove pull force

### Active Ability 2: "Reel & Frenzy"

```json
{
  "abilityName": "Reel & Frenzy",
  "abilityType": "Buff",
  "cooldown": 90.0,
  "castTime": 0.5,
  "frenzyParameters": {
    "duration": 10.0,
    "fireRateBonus": 0.30,
    "reloadSpeedBonus": 0.50,
    "movementPenalty": 0.15,
    "damageBonus": 0.15
  },
  "visualFX": {
    "handGlow": "Frenzy_Hand_Aura",
    "screenVignette": "Red_Edge_Vignette",
    "weaponTrail": "Weapon_Swing_Trail"
  },
  "audioFX": {
    "activateSound": "Frenzy_Activate",
    "heartbeatLoop": "Heartbeat_Fast_Loop",
    "deactivateSound": "Frenzy_Exhale"
  }
}
```

**Implementation Notes:**
- On activate: apply stat modifiers to player for duration
- `fireRateBonus`: Divide time between shots by 1.30
- `reloadSpeedBonus`: Divide reload time by 1.50
- `movementPenalty`: Multiply movement speed by 0.85
- `damageBonus`: Multiply outgoing damage by 1.15
- Duration timer: remove all modifiers after 10 seconds
- Screen effect: Post-processing vignette effect
- Audio: Loop heartbeat sound for full duration

### Ability Upgrade Paths

#### Whaling Harpoon Upgrades

```json
{
  "tier1": {
    "name": "Base Whaling Harpoon",
    "unlockLevel": 1,
    "unlockCost": 0,
    "parameters": {
      "damage": 60,
      "pullDuration": 2.0
    }
  },
  "tier2": {
    "name": "Barbed Harpoon",
    "unlockLevel": 5,
    "unlockCost": 500,
    "changes": {
      "damage": 90,
      "pullDuration": 2.5
    }
  },
  "tier3": {
    "name": "Reaver's Lance",
    "unlockLevel": 10,
    "unlockCost": 1500,
    "changes": {
      "damage": 120,
      "pullDuration": 3.0,
      "newEffect": {
        "pulledEnemiesTakeBonusDamage": 0.20
      }
    }
  }
}
```

#### Reel & Frenzy Upgrades

```json
{
  "tier1": {
    "name": "Base Reel & Frenzy",
    "unlockLevel": 1,
    "unlockCost": 0,
    "parameters": {
      "duration": 10.0,
      "fireRateBonus": 0.30
    }
  },
  "tier2": {
    "name": "Extended Frenzy",
    "unlockLevel": 5,
    "unlockCost": 500,
    "changes": {
      "duration": 12.0,
      "fireRateBonus": 0.40
    }
  },
  "tier3": {
    "name": "Bloodlust",
    "unlockLevel": 10,
    "unlockCost": 1500,
    "changes": {
      "duration": 15.0,
      "fireRateBonus": 0.50,
      "newEffect": {
        "killsDuringFrenzyExtendDuration": 2.0
      }
    }
  }
}
```

---

## CLASS 4: OCCULTIST (Support/Healer)

### Base Stats

```json
{
  "className": "Occultist",
  "classRole": "Support",
  "baseHealth": 100,
  "baseMovementSpeed": 5.2,
  "sprintMultiplier": 1.4,
  "crouchMultiplier": 0.55
}
```

### Passive Ability: "Warded"

```json
{
  "abilityName": "Warded",
  "abilityType": "Passive",
  "parameters": {
    "breachSaturationSlowdown": 0.25,
    "healingItemCapacityBonus": 1,
    "healingEffectivenessOnOthers": 0.25
  },
  "description": "Breach Saturation increases 25% slower, +1 healing item capacity, +25% healing effectiveness on others"
}
```

**Implementation Notes:**
- `breachSaturationSlowdown`: Multiply saturation gain rate by 0.75 for this player
- `healingItemCapacityBonus`: Add +1 to max healing item inventory
- `healingEffectivenessOnOthers`: When Occultist uses healing item on ally, multiply healing by 1.25

### Active Ability 1: "Sanctuary Circle"

```json
{
  "abilityName": "Sanctuary Circle",
  "abilityType": "PlacedZone",
  "cooldown": 75.0,
  "castTime": 3.0,
  "circleParameters": {
    "radius": 8.0,
    "duration": 25.0,
    "healPerSecond": 3.0,
    "breachSaturationReduction": 0.50,
    "damageResistanceBonus": 0.10
  },
  "visualFX": {
    "ritualCircle": "Golden_Rune_Circle",
    "healingParticles": "Gentle_Golden_Motes",
    "runeGlow": "Pulsing_Runes"
  },
  "audioFX": {
    "castSound": "Circle_Draw_Chant",
    "activeSound": "Healing_Hum_Loop",
    "enterSound": "Healing_Embrace",
    "expireSound": "Circle_Fade"
  }
}
```

**Implementation Notes:**
- Player is vulnerable during `castTime` (3 seconds channel)
- Circle spawns at player's ground position
- Use trigger collider to track allies inside radius
- Healing: Apply 3% max HP per second to allies (trigger ontick every 1s)
- Breach Saturation: Multiply saturation gain by 0.5 while inside
- Damage resistance: Reduce incoming damage by 10% while inside
- Duration timer: destroy circle after 25 seconds

### Active Ability 2: "Purge Corruption"

```json
{
  "abilityName": "Purge Corruption",
  "abilityType": "ChanneledAoE",
  "cooldown": 90.0,
  "castTime": 5.0,
  "purgeParameters": {
    "breachSaturationReduction": 20.0,
    "radius": 15.0,
    "shieldAmount": 50,
    "shieldDuration": 30.0
  },
  "channelParameters": {
    "canMove": false,
    "canBeInterrupted": true,
    "interruptOnDamage": true
  },
  "visualFX": {
    "channelAura": "Dark_Purple_Energy",
    "pulseEffect": "Reality_Stabilize_Wave",
    "shieldEffect": "Purple_Shield_Overlay",
    "colorReturnEffect": "World_Color_Return"
  },
  "audioFX": {
    "channelSound": "Corruption_Purge_Channel",
    "releaseSound": "Reality_Pulse",
    "shieldApplySound": "Shield_Grant"
  }
}
```

**Implementation Notes:**
- Player cannot move during channel (disable movement)
- Interruptible: If player takes damage, cancel ability, trigger cooldown
- On successful completion:
  - Reduce global Breach Saturation by 20% (absolute reduction)
  - Sphere overlap to find allies in 15m radius
  - Grant 50 HP shield to all allies (additional HP buffer, absorbed first)
  - Shield expires after 30 seconds or when depleted
- Visual: Brief color saturation increase (reality stabilizes temporarily)

### Ability Upgrade Paths

#### Sanctuary Circle Upgrades

```json
{
  "tier1": {
    "name": "Base Sanctuary Circle",
    "unlockLevel": 1,
    "unlockCost": 0,
    "parameters": {
      "radius": 8.0,
      "healPerSecond": 3.0
    }
  },
  "tier2": {
    "name": "Expanded Sanctuary",
    "unlockLevel": 5,
    "unlockCost": 500,
    "changes": {
      "radius": 12.0,
      "duration": 40.0
    }
  },
  "tier3": {
    "name": "Consecrated Ground",
    "unlockLevel": 10,
    "unlockCost": 1500,
    "changes": {
      "radius": 15.0,
      "duration": 45.0,
      "healPerSecond": 4.0,
      "damageResistanceBonus": 0.20
    }
  }
}
```

#### Purge Corruption Upgrades

```json
{
  "tier1": {
    "name": "Base Purge Corruption",
    "unlockLevel": 1,
    "unlockCost": 0,
    "parameters": {
      "breachSaturationReduction": 20.0,
      "shieldAmount": 50
    }
  },
  "tier2": {
    "name": "Greater Purge",
    "unlockLevel": 5,
    "unlockCost": 500,
    "changes": {
      "breachSaturationReduction": 25.0,
      "shieldAmount": 75,
      "castTime": 4.0
    }
  },
  "tier3": {
    "name": "Reality Anchor",
    "unlockLevel": 10,
    "unlockCost": 1500,
    "changes": {
      "breachSaturationReduction": 30.0,
      "shieldAmount": 100,
      "castTime": 3.5,
      "newEffect": {
        "temporarilyStunNearbyEnemies": 3.0
      }
    }
  }
}
```

---

## Unity Implementation Example

### ScriptableObject Structure

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "ClassData", menuName = "TidesEnd/ClassData")]
public class ClassData : ScriptableObject
{
    [Header("Class Identity")]
    public string className;
    public ClassRole classRole;
    
    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseMovementSpeed = 5.5f;
    public float sprintMultiplier = 1.4f;
    public float crouchMultiplier = 0.5f;
    
    [Header("Abilities")]
    public AbilityData passiveAbility;
    public AbilityData activeAbility1;
    public AbilityData activeAbility2;
}

[CreateAssetMenu(fileName = "AbilityData", menuName = "TidesEnd/AbilityData")]
public class AbilityData : ScriptableObject
{
    [Header("Basic Info")]
    public string abilityName;
    public AbilityType abilityType;
    
    [Header("Timing")]
    public float cooldown = 60f;
    public float castTime = 0f;
    public float duration = 0f;
    
    [Header("Parameters")]
    public float radius = 0f;
    public float damage = 0f;
    public float healAmount = 0f;
    public float speedMultiplier = 1f;
    
    [Header("VFX")]
    public GameObject vfxPrefab;
    public Material zoneMaterial;
    public Color effectColor = Color.white;
    
    [Header("Audio")]
    public AudioClip castSound;
    public AudioClip activeSound;
    public AudioClip endSound;
}

public enum ClassRole
{
    Tank,
    Scout,
    Damage,
    Support
}

public enum AbilityType
{
    Passive,
    Deployable,
    Projectile,
    PlacedZone,
    Buff,
    ChanneledAoE
}
```

### Ability Cooldown Manager

```csharp
public class AbilityCooldownManager : MonoBehaviour
{
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();
    
    public bool IsOnCooldown(string abilityName)
    {
        if (!cooldowns.ContainsKey(abilityName)) return false;
        return cooldowns[abilityName] > 0f;
    }
    
    public float GetCooldownRemaining(string abilityName)
    {
        if (!cooldowns.ContainsKey(abilityName)) return 0f;
        return cooldowns[abilityName];
    }
    
    public void StartCooldown(string abilityName, float duration)
    {
        cooldowns[abilityName] = duration;
    }
    
    private void Update()
    {
        List<string> keys = new List<string>(cooldowns.Keys);
        foreach (string key in keys)
        {
            if (cooldowns[key] > 0f)
            {
                cooldowns[key] -= Time.deltaTime;
                if (cooldowns[key] <= 0f)
                {
                    cooldowns[key] = 0f;
                    OnCooldownComplete(key);
                }
            }
        }
    }
    
    private void OnCooldownComplete(string abilityName)
    {
        // Play ready sound, update UI, etc.
    }
}
```

---

## Testing Checklist

**Per Class:**
- [ ] Base stats match specification
- [ ] Passive ability effects apply correctly
- [ ] Ability 1 cooldown timer works
- [ ] Ability 1 effects apply correctly
- [ ] Ability 2 cooldown timer works
- [ ] Ability 2 effects apply correctly
- [ ] VFX spawn at correct times
- [ ] Audio plays at correct times
- [ ] Ability upgrades modify values correctly

**Global Systems:**
- [ ] Cooldown manager tracks all abilities
- [ ] UI displays cooldown timers correctly
- [ ] Abilities can be interrupted appropriately
- [ ] Network synchronization (multiplayer)
- [ ] Save/load ability upgrade progress

---

## Related Documentation

**For quick reference:**
- [Classes Quick Ref](../01-quick-reference/classes-quick-ref.md)

**For balance tuning:**
- [Balance Parameters](balance-parameters.md)

**For combat design:**
- [Combat System Design](../02-design-docs/combat-system-design.md)

---

**This document provides exact parameters ready for Unity implementation. All JSON can be loaded via ScriptableObjects or configuration files.**
