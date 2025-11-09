# Player & Enemy Ability System Implementation Plan

**Version:** 1.0
**Last Updated:** 2025-10-28
**Status:** Planning
**Estimated Duration:** 12 weeks

---

## Overview

This document outlines the implementation of a **universal ability system** for Tide's End that supports both player class abilities and enemy/boss abilities through a shared, reusable architecture.

### Key Design Principles

- **Entity-Agnostic**: Same core framework for players, enemies, and bosses
- **Server-Authoritative**: All ability execution validated on server (anti-cheat)
- **ScriptableObject-Driven**: Data-driven design for easy balancing and iteration
- **Modular Effects**: Compose complex abilities from simple effect building blocks
- **AI-Friendly**: Built-in conditions system for enemy ability decision-making
- **Network-Optimized**: Designed for Unity Netcode from the ground up

---

## Supported Ability Users

### Players (4 Classes)
- **Bulwark**: Anchor Point (zone), Bastion Shield (deployable)
- **Lamplighter**: Flare Shot (projectile), Pathfinder's Mark (zone)
- **Harpooner**: Whaling Harpoon (projectile), Reel & Frenzy (buff)
- **Occultist**: Sanctuary Circle (zone), Purge Corruption (channeled AoE)

### Regular Enemies
- **Tide Touched**: Phase ability (transform, pass through walls)
- **Deep One**: Leap attack (projectile with knockback)
- **Dimensional Shambler**: Teleport behind player

### Boss Enemies
- **Drowned Priest**: Summon Wave, Corrupt Zone, Ritual Pulse
- **Reef Leviathan**: Tentacle Sweep, Whirlpool, Submerge
- **The Color**: Possess Teammate, Color Drain, Reality Distortion

---

## Phase 1: Universal Core Architecture

### 1.1 Ability Data Layer (Universal)
**Location:** `Assets/Scripts/Abilities/Data/`

**Files to Create:**
- `AbilityData.cs` - ScriptableObject base for ALL abilities
- `ClassData.cs` - ScriptableObject defining each player class
- `AbilityUpgradeData.cs` - ScriptableObject for upgrade tiers (players only)

**`AbilityData.cs`** - Core data structure:

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "TidesEnd/Abilities/AbilityData")]
public class AbilityData : ScriptableObject
{
    [Header("Identity")]
    public string abilityName;
    public string description;
    public Sprite icon;
    public AbilityType abilityType;
    public AbilityCategory category; // Player, Enemy, Boss

    [Header("Timing")]
    public float cooldown = 60f;
    public float castTime = 0f;
    public float duration = 0f;
    public bool canCastWhileCasting = false;

    [Header("Targeting")]
    public TargetingMode targetingMode; // Self, Enemy, Ally, Ground, Direction
    public float range = 0f;
    public float radius = 0f;
    public bool requiresLineOfSight = false;

    [Header("Effects")]
    public AbilityEffect[] effects; // Array of effects to apply

    [Header("VFX/Audio")]
    public GameObject castVFXPrefab;
    public GameObject activeVFXPrefab;
    public GameObject impactVFXPrefab;
    public AudioClip castSound;
    public AudioClip activeSound;
    public AudioClip endSound;

    [Header("AI Parameters (for enemies)")]
    public float aiUsagePriority = 0.5f; // 0-1, how eagerly AI uses this
    public AIUseCondition[] aiUseConditions; // When AI should use this ability
}

public enum AbilityType
{
    Passive,        // Always active (player passives, boss auras)
    Deployable,     // Placed objects (anchor, totems)
    Projectile,     // Fired projectiles (flare, harpoon)
    PlacedZone,     // Ground zones (sanctuary, whirlpool)
    Buff,           // Self buffs (frenzy)
    ChanneledAoE,   // Channeled effects (purge)
    SummonMinions,  // Boss summons
    Teleport,       // Shambler teleport
    Possess,        // Color possession
    Transform       // Phase abilities
}

public enum AbilityCategory
{
    Player,
    Enemy,
    Boss
}

public enum TargetingMode
{
    Self,           // Caster only
    SingleEnemy,    // One enemy
    SingleAlly,     // One ally
    AllEnemies,     // All enemies in range
    AllAllies,      // All allies in range
    Ground,         // Position on ground
    Direction,      // Direction from caster
    Random          // Random valid target
}
```

---

### 1.2 Ability Effect System (Modular)
**Location:** `Assets/Scripts/Abilities/Effects/`

**Files to Create:**
- `AbilityEffect.cs` - Abstract base class
- `DamageEffect.cs` - Apply damage
- `HealEffect.cs` - Apply healing
- `ModifySpeedEffect.cs` - Change movement speed
- `ModifyDamageEffect.cs` - Change damage output
- `KnockbackEffect.cs` - Apply knockback force
- `PullEffect.cs` - Pull toward caster
- `StunEffect.cs` - Stun/disable entity
- `ShieldEffect.cs` - Grant temporary HP
- `RevealEffect.cs` - Reveal enemies (outline shader)
- `SummonEffect.cs` - Spawn minions
- `TeleportEffect.cs` - Teleport caster
- `PossessEffect.cs` - Take control of target
- `TransformEffect.cs` - Change entity state
- `ModifySaturationEffect.cs` - Change breach saturation

**`AbilityEffect.cs`** - Base class:

```csharp
using System;
using UnityEngine;

[Serializable]
public abstract class AbilityEffect
{
    public EffectType effectType;
    public float magnitude;
    public float duration;
    public bool affectsAllies = true;
    public bool affectsEnemies = true;
    public bool affectsSelf = true;

    public abstract void Apply(AbilityUser caster, GameObject target, AbilityContext context);
    public abstract void Remove(GameObject target);
}

public enum EffectType
{
    Damage,
    Heal,
    ModifySpeed,
    ModifyDamage,
    Knockback,
    Pull,
    Stun,
    Root,
    Shield,
    Reveal,
    Summon,
    Teleport,
    Possess,
    Transform,
    AreaDenial,
    ModifySaturation
}
```

**Example Effect Implementation - `DamageEffect.cs`:**

```csharp
using System;
using UnityEngine;

[Serializable]
public class DamageEffect : AbilityEffect
{
    public DamageType damageType;
    public bool ignoreArmor;

    public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
    {
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            var damageInfo = new DamageInfo
            {
                damage = magnitude,
                damageType = damageType,
                source = caster.gameObject,
                hitPosition = context.hitPosition
            };
            damageable.TakeDamage(damageInfo);
        }
    }

    public override void Remove(GameObject target)
    {
        // Instant effect, no removal needed
    }
}
```

**Example Effect Implementation - `ModifySpeedEffect.cs`:**

```csharp
using System;
using UnityEngine;

[Serializable]
public class ModifySpeedEffect : AbilityEffect
{
    public bool isMultiplier = true; // true = multiply, false = add

    public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
    {
        if (target.TryGetComponent<IMovable>(out var movable))
        {
            if (isMultiplier)
                movable.AddSpeedMultiplier(effectType.ToString(), magnitude);
            else
                movable.AddSpeedBonus(effectType.ToString(), magnitude);
        }
    }

    public override void Remove(GameObject target)
    {
        if (target.TryGetComponent<IMovable>(out var movable))
        {
            movable.RemoveSpeedModifier(effectType.ToString());
        }
    }
}
```

**Example Effect Implementation - `SummonEffect.cs`:**

```csharp
using System;
using UnityEngine;

[Serializable]
public class SummonEffect : AbilityEffect
{
    public GameObject summonPrefab;
    public int summonCount = 1;
    public float summonRadius = 5f;
    public float summonDuration = 0f; // 0 = permanent

    public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
    {
        for (int i = 0; i < summonCount; i++)
        {
            Vector3 spawnPos = context.targetPosition + Random.insideUnitSphere * summonRadius;
            spawnPos.y = context.targetPosition.y;

            GameObject summon = GameObject.Instantiate(summonPrefab, spawnPos, Quaternion.identity);

            if (summonDuration > 0)
            {
                GameObject.Destroy(summon, summonDuration);
            }
        }
    }

    public override void Remove(GameObject target) { }
}
```

**Example Effect Implementation - `TeleportEffect.cs`:**

```csharp
using System;
using UnityEngine;

[Serializable]
public class TeleportEffect : AbilityEffect
{
    public TeleportMode mode;
    public float teleportRange = 20f;
    public bool teleportBehindTarget = false;

    public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
    {
        Vector3 teleportPosition = CalculateTeleportPosition(caster, context);
        caster.transform.position = teleportPosition;

        // Trigger VFX/audio
        if (context.vfxPrefab != null)
        {
            GameObject.Instantiate(context.vfxPrefab, caster.transform.position, Quaternion.identity);
        }
    }

    private Vector3 CalculateTeleportPosition(AbilityUser caster, AbilityContext context)
    {
        if (teleportBehindTarget && context.targetEntity != null)
        {
            Vector3 targetForward = context.targetEntity.transform.forward;
            return context.targetEntity.transform.position - targetForward * 5f;
        }
        return context.targetPosition;
    }

    public override void Remove(GameObject target) { }
}

public enum TeleportMode
{
    ToPosition,
    ToTarget,
    BehindTarget,
    RandomNearby
}
```

---

### 1.3 Universal Ability User Component
**Location:** `Assets/Scripts/Abilities/Core/`

**Files to Create:**
- `AbilityUser.cs` - Main component attached to players AND enemies
- `AbilityContext.cs` - Context data structure
- `IMovable.cs` - Interface for speed modification

**`AbilityUser.cs`** - NetworkBehaviour for all entities:

```csharp
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class AbilityUser : NetworkBehaviour
{
    [Header("Entity Configuration")]
    public EntityType entityType; // Player, Enemy, Boss
    public string entityID; // "Bulwark", "DrownedPriest", etc.

    [Header("Abilities")]
    public AbilityData passiveAbility;
    public AbilityData[] activeAbilities; // Can have 1-10+ abilities

    [Header("State")]
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();
    private Dictionary<string, AbilityInstance> activeAbilityInstances = new Dictionary<string, AbilityInstance>();
    private bool isCasting = false;
    private bool isInterruptible = true;

    // Events for UI and AI
    public event System.Action<AbilityData> OnAbilityActivated;
    public event System.Action<AbilityData> OnAbilityCooldownStarted;
    public event System.Action<AbilityData> OnAbilityCooldownComplete;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Apply passive ability
        if (passiveAbility != null)
        {
            ApplyPassiveAbility();
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        UpdateCooldowns();
        UpdateActiveAbilities();
    }

    // Server-authoritative ability activation
    public bool TryActivateAbility(int abilityIndex, AbilityContext context)
    {
        if (!IsServer)
        {
            Debug.LogError("Abilities can only be activated on server!");
            return false;
        }

        if (abilityIndex < 0 || abilityIndex >= activeAbilities.Length)
            return false;

        AbilityData ability = activeAbilities[abilityIndex];

        // Validate ability can be used
        if (!CanUseAbility(ability))
            return false;

        // Start cast
        StartAbilityCast(ability, context);
        return true;
    }

    private bool CanUseAbility(AbilityData ability)
    {
        // Check cooldown
        if (IsOnCooldown(ability))
            return false;

        // Check if already casting
        if (isCasting && !ability.canCastWhileCasting)
            return false;

        // Check resources (can extend for mana, etc.)
        // Check status (stunned, silenced, etc.)

        return true;
    }

    private void StartAbilityCast(AbilityData ability, AbilityContext context)
    {
        if (ability.castTime > 0)
        {
            // Start cast timer
            isCasting = true;
            StartCoroutine(CastAbilityCoroutine(ability, context));
        }
        else
        {
            // Instant cast
            ExecuteAbility(ability, context);
        }
    }

    private System.Collections.IEnumerator CastAbilityCoroutine(AbilityData ability, AbilityContext context)
    {
        float castTimer = 0f;

        while (castTimer < ability.castTime)
        {
            castTimer += Time.deltaTime;

            // Check for interruption
            if (isInterruptible && ShouldInterruptCast())
            {
                InterruptCast(ability);
                yield break;
            }

            yield return null;
        }

        // Cast complete
        isCasting = false;
        ExecuteAbility(ability, context);
    }

    private void ExecuteAbility(AbilityData ability, AbilityContext context)
    {
        // Create ability instance based on type
        AbilityInstance instance = AbilityFactory.CreateAbilityInstance(ability, this, context);

        if (instance == null)
        {
            Debug.LogError($"Failed to create ability instance for {ability.abilityName}");
            return;
        }

        // Execute the ability
        instance.Execute();

        // Track active instances (for duration-based abilities)
        if (ability.duration > 0)
        {
            activeAbilityInstances[ability.abilityName] = instance;
        }

        // Start cooldown
        StartCooldown(ability);

        // Notify listeners
        OnAbilityActivated?.Invoke(ability);

        // Sync to clients
        NotifyAbilityActivatedClientRpc(ability.abilityName, context);
    }

    [ClientRpc]
    private void NotifyAbilityActivatedClientRpc(string abilityName, AbilityContext context)
    {
        // Play VFX/audio on all clients
        // Update UI if this is local player
    }

    private void StartCooldown(AbilityData ability)
    {
        cooldowns[ability.abilityName] = ability.cooldown;
        OnAbilityCooldownStarted?.Invoke(ability);
    }

    private void UpdateCooldowns()
    {
        List<string> completedCooldowns = new List<string>();

        foreach (var kvp in cooldowns)
        {
            cooldowns[kvp.Key] -= Time.deltaTime;

            if (cooldowns[kvp.Key] <= 0)
            {
                completedCooldowns.Add(kvp.Key);
            }
        }

        foreach (string abilityName in completedCooldowns)
        {
            cooldowns.Remove(abilityName);
            // Find ability data and trigger event
            // OnAbilityCooldownComplete?.Invoke(ability);
        }
    }

    private void UpdateActiveAbilities()
    {
        List<string> expiredAbilities = new List<string>();

        foreach (var kvp in activeAbilityInstances)
        {
            kvp.Value.Update(Time.deltaTime);

            if (kvp.Value.IsExpired())
            {
                kvp.Value.Cleanup();
                expiredAbilities.Add(kvp.Key);
            }
        }

        foreach (string abilityName in expiredAbilities)
        {
            activeAbilityInstances.Remove(abilityName);
        }
    }

    public bool IsOnCooldown(AbilityData ability)
    {
        return cooldowns.ContainsKey(ability.abilityName) && cooldowns[ability.abilityName] > 0;
    }

    public float GetCooldownRemaining(AbilityData ability)
    {
        if (!cooldowns.ContainsKey(ability.abilityName))
            return 0f;
        return cooldowns[ability.abilityName];
    }

    private void ApplyPassiveAbility()
    {
        // Apply passive effects on spawn
    }

    private bool ShouldInterruptCast()
    {
        // Check for damage, stun, etc.
        return false;
    }

    private void InterruptCast(AbilityData ability)
    {
        isCasting = false;
        // Trigger cooldown on interruption
        StartCooldown(ability);
    }
}

public enum EntityType
{
    Player,
    Enemy,
    Boss
}
```

**`AbilityContext.cs`** - Context data structure:

```csharp
using UnityEngine;

[System.Serializable]
public struct AbilityContext
{
    public GameObject targetEntity;
    public Vector3 targetPosition;
    public Vector3 targetDirection;
    public Vector3 hitPosition;
    public GameObject vfxPrefab;
}
```

---

### 1.4 Ability Instance System
**Location:** `Assets/Scripts/Abilities/Core/`

**Files to Create:**
- `AbilityInstance.cs` - Abstract base class
- `AbilityFactory.cs` - Factory pattern for creating instances

**`AbilityInstance.cs`**:

```csharp
public abstract class AbilityInstance
{
    protected AbilityData abilityData;
    protected AbilityUser caster;
    protected AbilityContext context;
    protected float elapsed = 0f;

    public AbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
    {
        this.abilityData = data;
        this.caster = caster;
        this.context = context;
    }

    public abstract void Execute();
    public abstract void Update(float deltaTime);
    public abstract void Cleanup();

    public bool IsExpired()
    {
        return abilityData.duration > 0 && elapsed >= abilityData.duration;
    }
}
```

**`AbilityFactory.cs`**:

```csharp
using UnityEngine;

public static class AbilityFactory
{
    public static AbilityInstance CreateAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
    {
        switch (data.abilityType)
        {
            case AbilityType.Deployable:
                return new DeployableAbilityInstance(data, caster, context);
            case AbilityType.Projectile:
                return new ProjectileAbilityInstance(data, caster, context);
            case AbilityType.PlacedZone:
                return new ZoneAbilityInstance(data, caster, context);
            case AbilityType.Buff:
                return new BuffAbilityInstance(data, caster, context);
            case AbilityType.ChanneledAoE:
                return new ChanneledAbilityInstance(data, caster, context);
            case AbilityType.SummonMinions:
                return new SummonAbilityInstance(data, caster, context);
            case AbilityType.Teleport:
                return new TeleportAbilityInstance(data, caster, context);
            case AbilityType.Possess:
                return new PossessAbilityInstance(data, caster, context);
            case AbilityType.Transform:
                return new TransformAbilityInstance(data, caster, context);
            default:
                Debug.LogError($"No instance type for {data.abilityType}");
                return null;
        }
    }
}
```

---

## Phase 2: Ability Type Implementations

### 2.1 Deployable Ability Instance
**Location:** `Assets/Scripts/Abilities/Instances/`

**Files to Create:**
- `DeployableAbilityInstance.cs`
- `DeployableObject.cs` - Component on spawned objects

**Example: Bulwark Anchor Point, Drowned Priest Ritual Totems**

```csharp
public class DeployableAbilityInstance : AbilityInstance
{
    private GameObject deployedObject;

    public DeployableAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        : base(data, caster, context) { }

    public override void Execute()
    {
        // Spawn deployable object
        deployedObject = GameObject.Instantiate(
            abilityData.activeVFXPrefab,
            context.targetPosition,
            Quaternion.identity
        );

        // Apply effects based on ability data
        if (deployedObject.TryGetComponent<DeployableObject>(out var deployable))
        {
            deployable.Initialize(abilityData, caster);
        }
    }

    public override void Update(float deltaTime)
    {
        elapsed += deltaTime;
    }

    public override void Cleanup()
    {
        if (deployedObject != null)
        {
            GameObject.Destroy(deployedObject);
        }
    }
}
```

**`DeployableObject.cs`** - Component on deployed objects:

```csharp
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class DeployableObject : NetworkBehaviour
{
    private AbilityData abilityData;
    private AbilityUser owner;
    private List<GameObject> entitiesInZone = new List<GameObject>();

    public void Initialize(AbilityData data, AbilityUser owner)
    {
        this.abilityData = data;
        this.owner = owner;

        // Set up trigger collider for zone
        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.radius = data.radius;
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        // Apply ability effects to entities entering zone
        foreach (var effect in abilityData.effects)
        {
            if (ShouldApplyEffect(effect, other.gameObject))
            {
                effect.Apply(owner, other.gameObject, new AbilityContext());
            }
        }

        entitiesInZone.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        // Remove effects when leaving zone
        foreach (var effect in abilityData.effects)
        {
            effect.Remove(other.gameObject);
        }

        entitiesInZone.Remove(other.gameObject);
    }

    private bool ShouldApplyEffect(AbilityEffect effect, GameObject target)
    {
        // Check if target is valid for this effect
        bool isAlly = IsAlly(target);
        bool isEnemy = !isAlly;

        if (isAlly && !effect.affectsAllies) return false;
        if (isEnemy && !effect.affectsEnemies) return false;

        return true;
    }

    private bool IsAlly(GameObject target)
    {
        // Implement faction checking
        // For now, simple check: players are allies of players, enemies of enemies
        bool ownerIsPlayer = owner.entityType == EntityType.Player;
        bool targetIsPlayer = target.CompareTag("Player");
        return ownerIsPlayer == targetIsPlayer;
    }
}
```

---

### 2.2 Projectile Ability Instance
**Location:** `Assets/Scripts/Abilities/Instances/`

**Files to Create:**
- `ProjectileAbilityInstance.cs`
- `AbilityProjectile.cs` - Component on projectile objects

**Example: Lamplighter Flare Shot, Harpooner Whaling Harpoon**

```csharp
public class ProjectileAbilityInstance : AbilityInstance
{
    private GameObject projectile;

    public ProjectileAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        : base(data, caster, context) { }

    public override void Execute()
    {
        // Spawn projectile
        projectile = GameObject.Instantiate(
            abilityData.activeVFXPrefab,
            caster.transform.position + caster.transform.forward,
            Quaternion.LookRotation(context.targetDirection)
        );

        if (projectile.TryGetComponent<AbilityProjectile>(out var proj))
        {
            proj.Initialize(abilityData, caster, context);
        }
    }

    public override void Update(float deltaTime)
    {
        elapsed += deltaTime;
    }

    public override void Cleanup()
    {
        if (projectile != null)
        {
            GameObject.Destroy(projectile);
        }
    }
}
```

---

### 2.3 Zone Ability Instance
**Location:** `Assets/Scripts/Abilities/Instances/`

**Files to Create:**
- `ZoneAbilityInstance.cs`

**Example: Pathfinder's Mark, Sanctuary Circle, Reef Leviathan Whirlpool**

```csharp
public class ZoneAbilityInstance : AbilityInstance
{
    private GameObject zoneObject;

    public ZoneAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        : base(data, caster, context) { }

    public override void Execute()
    {
        // Create zone at target position
        zoneObject = GameObject.Instantiate(
            abilityData.activeVFXPrefab,
            context.targetPosition,
            Quaternion.identity
        );

        // Use DeployableObject component for zone logic
        if (zoneObject.TryGetComponent<DeployableObject>(out var zone))
        {
            zone.Initialize(abilityData, caster);
        }
    }

    public override void Update(float deltaTime)
    {
        elapsed += deltaTime;
    }

    public override void Cleanup()
    {
        if (zoneObject != null)
        {
            GameObject.Destroy(zoneObject);
        }
    }
}
```

---

### 2.4 Summon Ability Instance
**Location:** `Assets/Scripts/Abilities/Instances/`

**Files to Create:**
- `SummonAbilityInstance.cs`

**Example: Drowned Priest Summon Wave**

```csharp
using System.Collections.Generic;
using UnityEngine;

public class SummonAbilityInstance : AbilityInstance
{
    private List<GameObject> summonedEntities = new List<GameObject>();

    public SummonAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        : base(data, caster, context) { }

    public override void Execute()
    {
        // Find summon effect
        SummonEffect summonEffect = System.Array.Find(
            abilityData.effects,
            e => e.effectType == EffectType.Summon
        ) as SummonEffect;

        if (summonEffect == null) return;

        // Spawn minions
        for (int i = 0; i < summonEffect.summonCount; i++)
        {
            Vector3 spawnPos = CalculateSpawnPosition(i, summonEffect);
            GameObject summon = GameObject.Instantiate(
                summonEffect.summonPrefab,
                spawnPos,
                Quaternion.identity
            );

            // Set up summon (give it target, set faction, etc.)
            if (summon.TryGetComponent<EnemyAI>(out var ai))
            {
                ai.SetTarget(FindNearestPlayer());
                ai.SetSummoner(caster.gameObject);
            }

            summonedEntities.Add(summon);
        }
    }

    private Vector3 CalculateSpawnPosition(int index, SummonEffect effect)
    {
        // Spawn in circle around caster
        float angle = (360f / effect.summonCount) * index;
        Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * effect.summonRadius;
        return caster.transform.position + offset;
    }

    private GameObject FindNearestPlayer()
    {
        // Implement player finding logic
        return GameObject.FindGameObjectWithTag("Player");
    }

    public override void Update(float deltaTime)
    {
        elapsed += deltaTime;

        // Clean up destroyed summons
        summonedEntities.RemoveAll(s => s == null);
    }

    public override void Cleanup()
    {
        // Optionally destroy summons when ability expires
        // Or let them persist
    }
}
```

---

### 2.5 Teleport Ability Instance
**Location:** `Assets/Scripts/Abilities/Instances/`

**Files to Create:**
- `TeleportAbilityInstance.cs`

**Example: Dimensional Shambler Teleport**

```csharp
using UnityEngine;

public class TeleportAbilityInstance : AbilityInstance
{
    public TeleportAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        : base(data, caster, context) { }

    public override void Execute()
    {
        TeleportEffect teleportEffect = System.Array.Find(
            abilityData.effects,
            e => e.effectType == EffectType.Teleport
        ) as TeleportEffect;

        if (teleportEffect == null) return;

        // Calculate teleport destination
        Vector3 destination = CalculateTeleportDestination(teleportEffect);

        // Play teleport out VFX
        if (abilityData.castVFXPrefab != null)
        {
            GameObject.Instantiate(abilityData.castVFXPrefab, caster.transform.position, Quaternion.identity);
        }

        // Make invisible during teleport (for Shambler)
        if (caster.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.enabled = false;
        }

        // Actually teleport
        caster.transform.position = destination;

        // Play teleport in VFX
        if (abilityData.impactVFXPrefab != null)
        {
            GameObject.Instantiate(abilityData.impactVFXPrefab, destination, Quaternion.identity);
        }

        // Make visible again
        if (renderer != null)
        {
            renderer.enabled = true;
        }
    }

    private Vector3 CalculateTeleportDestination(TeleportEffect effect)
    {
        switch (effect.mode)
        {
            case TeleportMode.ToPosition:
                return context.targetPosition;

            case TeleportMode.BehindTarget:
                if (context.targetEntity != null)
                {
                    Vector3 targetForward = context.targetEntity.transform.forward;
                    return context.targetEntity.transform.position - targetForward * 10f;
                }
                break;

            case TeleportMode.RandomNearby:
                Vector2 randomCircle = Random.insideUnitCircle * effect.teleportRange;
                return caster.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        return caster.transform.position;
    }

    public override void Update(float deltaTime) { }
    public override void Cleanup() { }
}
```

---

### 2.6 Buff Ability Instance
**Location:** `Assets/Scripts/Abilities/Instances/`

**Files to Create:**
- `BuffAbilityInstance.cs`

**Example: Harpooner Reel & Frenzy**

```csharp
public class BuffAbilityInstance : AbilityInstance
{
    private bool effectsApplied = false;

    public BuffAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        : base(data, caster, context) { }

    public override void Execute()
    {
        // Apply all buff effects to caster
        foreach (var effect in abilityData.effects)
        {
            if (effect.affectsSelf)
            {
                effect.Apply(caster, caster.gameObject, context);
            }
        }

        effectsApplied = true;
    }

    public override void Update(float deltaTime)
    {
        elapsed += deltaTime;
    }

    public override void Cleanup()
    {
        if (effectsApplied)
        {
            // Remove all effects
            foreach (var effect in abilityData.effects)
            {
                effect.Remove(caster.gameObject);
            }
        }
    }
}
```

---

### 2.7 Channeled AoE Ability Instance
**Location:** `Assets/Scripts/Abilities/Instances/`

**Files to Create:**
- `ChanneledAbilityInstance.cs`

**Example: Occultist Purge Corruption**

```csharp
public class ChanneledAbilityInstance : AbilityInstance
{
    private bool channeling = true;

    public ChanneledAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        : base(data, caster, context) { }

    public override void Execute()
    {
        // Channel has completed, apply effects
        // Find all targets in radius
        Collider[] targets = Physics.OverlapSphere(caster.transform.position, abilityData.radius);

        foreach (var target in targets)
        {
            foreach (var effect in abilityData.effects)
            {
                if (ShouldApplyToTarget(target.gameObject, effect))
                {
                    effect.Apply(caster, target.gameObject, context);
                }
            }
        }
    }

    private bool ShouldApplyToTarget(GameObject target, AbilityEffect effect)
    {
        // Check ally/enemy flags
        bool isAlly = IsAlly(target);

        if (isAlly && !effect.affectsAllies) return false;
        if (!isAlly && !effect.affectsEnemies) return false;

        return true;
    }

    private bool IsAlly(GameObject target)
    {
        bool casterIsPlayer = caster.entityType == EntityType.Player;
        bool targetIsPlayer = target.CompareTag("Player");
        return casterIsPlayer == targetIsPlayer;
    }

    public override void Update(float deltaTime)
    {
        elapsed += deltaTime;
    }

    public override void Cleanup() { }
}
```

---

## Phase 3: AI Integration for Enemy Abilities

### 3.1 Enemy AI Ability Decision System
**Location:** `Assets/Scripts/Enemy/AI/`

**Files to Create:**
- `EnemyAbilityAI.cs` - AI component for using abilities
- `AIUseCondition.cs` - Condition data structure

**`EnemyAbilityAI.cs`**:

```csharp
using UnityEngine;

public class EnemyAbilityAI : MonoBehaviour
{
    private AbilityUser abilityUser;
    private EnemyAI enemyAI;

    [Header("AI Configuration")]
    public float abilityCheckInterval = 1f; // Check every second if should use ability
    private float nextCheckTime = 0f;

    private void Start()
    {
        abilityUser = GetComponent<AbilityUser>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        if (!abilityUser.IsServer) return;
        if (enemyAI.currentState != AIState.Combat) return;

        if (Time.time >= nextCheckTime)
        {
            ConsiderUsingAbilities();
            nextCheckTime = Time.time + abilityCheckInterval;
        }
    }

    private void ConsiderUsingAbilities()
    {
        // Evaluate each ability
        for (int i = 0; i < abilityUser.activeAbilities.Length; i++)
        {
            AbilityData ability = abilityUser.activeAbilities[i];

            // Skip if on cooldown
            if (abilityUser.IsOnCooldown(ability))
                continue;

            // Check AI use conditions
            if (ShouldUseAbility(ability))
            {
                // Build context for ability
                AbilityContext context = BuildAbilityContext(ability);

                // Try to use ability
                if (abilityUser.TryActivateAbility(i, context))
                {
                    Debug.Log($"{gameObject.name} used ability: {ability.abilityName}");
                    break; // Only use one ability per check
                }
            }
        }
    }

    private bool ShouldUseAbility(AbilityData ability)
    {
        // Check AI use conditions
        foreach (var condition in ability.aiUseConditions)
        {
            if (!EvaluateCondition(condition))
                return false;
        }

        // Random chance based on priority (prevents too predictable)
        return Random.value < ability.aiUsagePriority;
    }

    private bool EvaluateCondition(AIUseCondition condition)
    {
        switch (condition.conditionType)
        {
            case AIConditionType.HealthBelow:
                float healthPercent = GetComponent<Health>().CurrentHealth / GetComponent<Health>().MaxHealth;
                return healthPercent < condition.threshold;

            case AIConditionType.DistanceToTargetLessThan:
                if (enemyAI.currentTarget != null)
                {
                    float distance = Vector3.Distance(transform.position, enemyAI.currentTarget.position);
                    return distance < condition.threshold;
                }
                break;

            case AIConditionType.DistanceToTargetGreaterThan:
                if (enemyAI.currentTarget != null)
                {
                    float distance = Vector3.Distance(transform.position, enemyAI.currentTarget.position);
                    return distance > condition.threshold;
                }
                break;

            case AIConditionType.AlliesNearby:
                Collider[] allies = Physics.OverlapSphere(transform.position, condition.threshold);
                int allyCount = 0;
                foreach (var collider in allies)
                {
                    if (collider.CompareTag("Enemy") && collider.gameObject != gameObject)
                        allyCount++;
                }
                return allyCount >= condition.minCount;

            case AIConditionType.Always:
                return true;
        }

        return true;
    }

    private AbilityContext BuildAbilityContext(AbilityData ability)
    {
        AbilityContext context = new AbilityContext();

        switch (ability.targetingMode)
        {
            case TargetingMode.Self:
                context.targetEntity = gameObject;
                context.targetPosition = transform.position;
                break;

            case TargetingMode.SingleEnemy:
                context.targetEntity = enemyAI.currentTarget?.gameObject;
                context.targetPosition = enemyAI.currentTarget != null
                    ? enemyAI.currentTarget.position
                    : transform.position;
                break;

            case TargetingMode.Ground:
                // For zones, place at current position or near target
                context.targetPosition = enemyAI.currentTarget != null
                    ? enemyAI.currentTarget.position
                    : transform.position;
                break;
        }

        return context;
    }
}
```

**`AIUseCondition.cs`**:

```csharp
using System;

[Serializable]
public struct AIUseCondition
{
    public AIConditionType conditionType;
    public float threshold;
    public int minCount;
}

public enum AIConditionType
{
    HealthBelow,                    // Use when health < threshold
    DistanceToTargetLessThan,       // Use when close to target
    DistanceToTargetGreaterThan,    // Use when far from target
    AlliesNearby,                   // Use when allies nearby
    Always                          // Use whenever off cooldown
}
```

---

## Phase 4: Player Integration

### 4.1 Player Input Integration
**Location:** `Assets/Scripts/Player/`

**Files to Create:**
- `PlayerAbilityInput.cs` - Handles player input for abilities

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityInput : MonoBehaviour
{
    private AbilityUser abilityUser;
    private Camera playerCamera;

    private void Start()
    {
        abilityUser = GetComponent<AbilityUser>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        // Only process input for local player
        if (!abilityUser.IsOwner) return;

        // Check for ability input
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            TryUseAbility(0); // First ability
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryUseAbility(1); // Second ability
        }
    }

    private void TryUseAbility(int abilityIndex)
    {
        if (abilityIndex >= abilityUser.activeAbilities.Length)
            return;

        AbilityData ability = abilityUser.activeAbilities[abilityIndex];

        // Build context based on targeting mode
        AbilityContext context = BuildContextForAbility(ability);

        // Request ability activation via ServerRpc
        RequestActivateAbilityServerRpc(abilityIndex, context);
    }

    [ServerRpc]
    private void RequestActivateAbilityServerRpc(int abilityIndex, AbilityContext context)
    {
        abilityUser.TryActivateAbility(abilityIndex, context);
    }

    private AbilityContext BuildContextForAbility(AbilityData ability)
    {
        AbilityContext context = new AbilityContext();

        switch (ability.targetingMode)
        {
            case TargetingMode.Self:
                context.targetEntity = gameObject;
                context.targetPosition = transform.position;
                break;

            case TargetingMode.Ground:
                // Raycast to find ground position
                Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, ability.range))
                {
                    context.targetPosition = hit.point;
                }
                break;

            case TargetingMode.Direction:
                context.targetDirection = playerCamera.transform.forward;
                break;
        }

        return context;
    }
}
```

---

### 4.2 Player UI Integration
**Location:** `Assets/Scripts/UI/Abilities/`

**Files to Create:**
- `AbilityUIPanel.cs` - HUD display for abilities
- `AbilitySlot.cs` - Individual ability slot UI

**`AbilityUIPanel.cs`**:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIPanel : MonoBehaviour
{
    [Header("References")]
    public AbilitySlot ability1Slot;
    public AbilitySlot ability2Slot;

    private AbilityUser localPlayerAbilityUser;

    private void Start()
    {
        // Find local player
        // localPlayerAbilityUser = FindLocalPlayer().GetComponent<AbilityUser>();

        // Subscribe to events
        localPlayerAbilityUser.OnAbilityCooldownStarted += OnCooldownStarted;
        localPlayerAbilityUser.OnAbilityCooldownComplete += OnCooldownComplete;

        // Initialize slots
        if (localPlayerAbilityUser.activeAbilities.Length > 0)
            ability1Slot.Initialize(localPlayerAbilityUser.activeAbilities[0]);

        if (localPlayerAbilityUser.activeAbilities.Length > 1)
            ability2Slot.Initialize(localPlayerAbilityUser.activeAbilities[1]);
    }

    private void Update()
    {
        if (localPlayerAbilityUser == null) return;

        // Update cooldown displays
        if (localPlayerAbilityUser.activeAbilities.Length > 0)
        {
            float cooldown = localPlayerAbilityUser.GetCooldownRemaining(localPlayerAbilityUser.activeAbilities[0]);
            ability1Slot.UpdateCooldown(cooldown);
        }

        if (localPlayerAbilityUser.activeAbilities.Length > 1)
        {
            float cooldown = localPlayerAbilityUser.GetCooldownRemaining(localPlayerAbilityUser.activeAbilities[1]);
            ability2Slot.UpdateCooldown(cooldown);
        }
    }

    private void OnCooldownStarted(AbilityData ability)
    {
        // Update UI to show ability on cooldown
    }

    private void OnCooldownComplete(AbilityData ability)
    {
        // Update UI to show ability ready
    }
}
```

**`AbilitySlot.cs`**:

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image abilityIcon;
    public Image cooldownFill;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI keybindText;

    private AbilityData abilityData;

    public void Initialize(AbilityData ability)
    {
        abilityData = ability;
        abilityIcon.sprite = ability.icon;
        cooldownFill.fillAmount = 0f;
    }

    public void UpdateCooldown(float remaining)
    {
        if (abilityData == null) return;

        if (remaining > 0)
        {
            cooldownFill.fillAmount = remaining / abilityData.cooldown;
            cooldownText.text = Mathf.CeilToInt(remaining).ToString();
        }
        else
        {
            cooldownFill.fillAmount = 0f;
            cooldownText.text = "";
        }
    }
}
```

---

## Phase 5: ScriptableObject Data Creation

### 5.1 Player Ability Data

Create the following ScriptableObject assets in `Assets/Data/Abilities/Player/`:

**Bulwark Abilities:**

1. **Steadfast (Passive)**
```
abilityName: "Steadfast"
abilityType: Passive
category: Player
effects:
  - ModifyHealth: +30
  - ModifyDamageResistance: +0.15 (when below 50% HP)
  - ModifyKnockback: -0.5
```

2. **Anchor Point**
```
abilityName: "Anchor Point"
abilityType: Deployable
category: Player
cooldown: 60
castTime: 0.8
duration: 20
targetingMode: Ground
range: 15
radius: 10
effects:
  - ModifySaturation: 0.0 (pause saturation)
  - ModifySpeed: 0.7 (enemies only)
```

3. **Bastion Shield**
```
abilityName: "Bastion Shield"
abilityType: Deployable
category: Player
cooldown: 90
castTime: 3.0
duration: 30
targetingMode: Self
effects:
  - CreateShield: 500 HP, 120° arc
```

**Lamplighter Abilities:**

1. **Pathfinder (Passive)**
```
abilityName: "Pathfinder"
abilityType: Passive
category: Player
effects:
  - ModifySpeed: +0.20
  - ModifyVision: +0.30 (fog)
  - ModifyFootstepVolume: -0.40
```

2. **Flare Shot**
```
abilityName: "Flare Shot"
abilityType: Projectile
category: Player
cooldown: 45
castTime: 0.5
duration: 30
targetingMode: Direction
range: 40
radius: 20
effects:
  - Reveal: enemies in radius
```

3. **Pathfinder's Mark**
```
abilityName: "Pathfinder's Mark"
abilityType: PlacedZone
category: Player
cooldown: 60
castTime: 1.0
duration: 20
targetingMode: Ground
range: 20
radius: 15
effects:
  - ModifySpeed: +0.40 (allies only)
```

**Harpooner Abilities:**

1. **Maritime Predator (Passive)**
```
abilityName: "Maritime Predator"
abilityType: Passive
category: Player
effects:
  - ModifyWeakPointDamage: +0.20
  - ModifyReloadSpeed: +0.15
  - ModifySpecialAmmoCapacity: +2
```

2. **Whaling Harpoon**
```
abilityName: "Whaling Harpoon"
abilityType: Projectile
category: Player
cooldown: 50
castTime: 1.2
targetingMode: Direction
range: 30
effects:
  - Damage: 60
  - Pull: 20 force, 2 seconds
```

3. **Reel & Frenzy**
```
abilityName: "Reel & Frenzy"
abilityType: Buff
category: Player
cooldown: 90
castTime: 0.5
duration: 10
targetingMode: Self
effects:
  - ModifyFireRate: +0.30
  - ModifyReloadSpeed: +0.50
  - ModifyDamage: +0.15
  - ModifySpeed: -0.15
```

**Occultist Abilities:**

1. **Warded (Passive)**
```
abilityName: "Warded"
abilityType: Passive
category: Player
effects:
  - ModifySaturation: -0.25 (gain rate)
  - ModifyItemCapacity: +1 (healing)
  - ModifyHealingEffectiveness: +0.25 (on others)
```

2. **Sanctuary Circle**
```
abilityName: "Sanctuary Circle"
abilityType: PlacedZone
category: Player
cooldown: 75
castTime: 3.0
duration: 25
targetingMode: Ground
radius: 8
effects:
  - Heal: 3% max HP per second
  - ModifySaturation: -0.50 (increase rate)
  - ModifyDamageResistance: +0.10
```

3. **Purge Corruption**
```
abilityName: "Purge Corruption"
abilityType: ChanneledAoE
category: Player
cooldown: 90
castTime: 5.0
targetingMode: Self
radius: 15
effects:
  - ModifySaturation: -20 (global)
  - Shield: 50 HP to allies in radius
```

---

### 5.2 Enemy Ability Data

Create the following ScriptableObject assets in `Assets/Data/Abilities/Enemies/`:

**Tide Touched:**
```
abilityName: "Phase Through Walls"
abilityType: Transform
category: Enemy
cooldown: 12
duration: 2
targetingMode: Self
aiUsagePriority: 0.15
aiUseConditions:
  - DistanceToTargetGreaterThan: 10
effects:
  - Transform: invulnerable, pass through walls
```

**Deep One:**
```
abilityName: "Leap Attack"
abilityType: Projectile
category: Enemy
cooldown: 8
targetingMode: SingleEnemy
range: 8
aiUsagePriority: 0.8
aiUseConditions:
  - DistanceToTargetLessThan: 8
effects:
  - Damage: 20
  - Knockback: 5
```

**Dimensional Shambler:**
```
abilityName: "Teleport Behind"
abilityType: Teleport
category: Enemy
cooldown: 8
targetingMode: SingleEnemy
range: 20
aiUsagePriority: 1.0
aiUseConditions:
  - Always
effects:
  - Teleport: BehindTarget, 10m
```

---

### 5.3 Boss Ability Data

Create the following ScriptableObject assets in `Assets/Data/Abilities/Bosses/`:

**Drowned Priest:**

1. **Summon Wave**
```
abilityName: "Summon Wave"
abilityType: SummonMinions
category: Boss
cooldown: 15
castTime: 2.0
targetingMode: Self
aiUsagePriority: 1.0
aiUseConditions:
  - Always
effects:
  - Summon: Hollow-Eyed x5, 10m radius
```

2. **Corrupt Zone**
```
abilityName: "Corrupt Zone"
abilityType: PlacedZone
category: Boss
cooldown: 20
duration: 10
targetingMode: Ground
radius: 5
aiUsagePriority: 0.7
effects:
  - Damage: 20 per second
```

3. **Ritual Pulse**
```
abilityName: "Ritual Pulse"
abilityType: ChanneledAoE
category: Boss
cooldown: 12
targetingMode: Self
radius: 15
aiUsagePriority: 0.9
effects:
  - Damage: 50
  - Knockback: 10
```

**Reef Leviathan:**

1. **Tentacle Sweep**
```
abilityName: "Tentacle Sweep"
abilityType: ChanneledAoE
category: Boss
cooldown: 8
castTime: 2.0 (telegraph)
targetingMode: Direction
radius: 15
aiUsagePriority: 0.8
effects:
  - Damage: 60
  - Knockback: 15
```

2. **Whirlpool**
```
abilityName: "Whirlpool"
abilityType: PlacedZone
category: Boss
cooldown: 25
duration: 10
targetingMode: Ground
radius: 12
aiUsagePriority: 0.6
effects:
  - Pull: 5 force toward center
  - Damage: 15 per second
```

3. **Submerge**
```
abilityName: "Submerge"
abilityType: Transform
category: Boss
cooldown: 30
duration: 5
targetingMode: Self
aiUsagePriority: 0.5
aiUseConditions:
  - HealthBelow: 0.5
effects:
  - Transform: invulnerable
  - Teleport: RandomNearby, 20m (on reemergence)
  - Damage: 80 AoE on reemergence
```

**The Color:**

1. **Possess Teammate**
```
abilityName: "Possess Teammate"
abilityType: Possess
category: Boss
cooldown: 30
duration: 8
targetingMode: Random (player)
aiUsagePriority: 0.8
effects:
  - Possess: control target for duration
```

2. **Color Drain (Passive)**
```
abilityName: "Color Drain"
abilityType: Passive
category: Boss
radius: 10
effects:
  - Damage: 5 per second in radius
```

3. **Reality Distortion**
```
abilityName: "Reality Distortion"
abilityType: ChanneledAoE
category: Boss
cooldown: 20
duration: 5
targetingMode: Self
radius: 30
aiUsagePriority: 0.6
effects:
  - ReverseControls: all players in radius
```

---

## Phase 6: Supporting Systems

### 6.1 Interfaces and Supporting Components

**Files to Create:**

**`IMovable.cs`** - Interface for movement modification:
```csharp
public interface IMovable
{
    void AddSpeedMultiplier(string source, float multiplier);
    void AddSpeedBonus(string source, float bonus);
    void RemoveSpeedModifier(string source);
}
```

**Update `NetworkedFPSController.cs`** to implement `IMovable`:
```csharp
public class NetworkedFPSController : NetworkBehaviour, IMovable
{
    private Dictionary<string, float> speedMultipliers = new Dictionary<string, float>();
    private Dictionary<string, float> speedBonuses = new Dictionary<string, float>();

    public void AddSpeedMultiplier(string source, float multiplier)
    {
        speedMultipliers[source] = multiplier;
    }

    public void AddSpeedBonus(string source, float bonus)
    {
        speedBonuses[source] = bonus;
    }

    public void RemoveSpeedModifier(string source)
    {
        speedMultipliers.Remove(source);
        speedBonuses.Remove(source);
    }

    private float GetFinalMovementSpeed()
    {
        float baseSpeed = moveSpeed;

        // Apply bonuses (additive)
        foreach (var bonus in speedBonuses.Values)
        {
            baseSpeed += bonus;
        }

        // Apply multipliers (multiplicative)
        foreach (var multiplier in speedMultipliers.Values)
        {
            baseSpeed *= multiplier;
        }

        return baseSpeed;
    }
}
```

**Update `EnemyAI.cs`** to implement `IMovable` similarly.

---

### 6.2 VFX and Audio Integration

**VFX Prefabs Needed:**
Create placeholder prefabs (can be replaced with final art later):

- `Anchor_Zone_VFX.prefab` - Golden shimmer field
- `Shield_Deployed_VFX.prefab` - Riot shield with wards
- `Flare_Light_VFX.prefab` - Bright light source
- `Speed_Zone_VFX.prefab` - Ground decal with particles
- `Harpoon_Chain_VFX.prefab` - LineRenderer chain
- `Healing_Circle_VFX.prefab` - Golden rune circle
- `Teleport_In_VFX.prefab` - Reality distortion
- `Teleport_Out_VFX.prefab` - Reality distortion
- `Summon_Spawn_VFX.prefab` - Dark emergence
- `Whirlpool_VFX.prefab` - Swirling water

**Audio Clips Needed:**
Source or create audio for:

- Ability cast sounds (deploy, fire, channel)
- Ability active sounds (loops for zones, shields)
- Ability end sounds (expire, break, complete)
- Impact sounds (hit, explosion, teleport)

---

## Implementation Timeline

### Week 1-2: Universal Core
- [ ] Create `AbilityData.cs` ScriptableObject
- [ ] Create `AbilityEffect.cs` base class
- [ ] Implement 5 common effects (Damage, Heal, ModifySpeed, Summon, Teleport)
- [ ] Create `AbilityUser.cs` component
- [ ] Create `AbilityInstance.cs` framework
- [ ] Create `AbilityFactory.cs`
- [ ] Test basic ability activation on dummy object

**Deliverable:** Can activate a test ability on server

---

### Week 3-4: Ability Type Instances
- [ ] Implement `DeployableAbilityInstance.cs`
- [ ] Implement `DeployableObject.cs` component
- [ ] Implement `ProjectileAbilityInstance.cs`
- [ ] Implement `ZoneAbilityInstance.cs`
- [ ] Implement `BuffAbilityInstance.cs`
- [ ] Implement `SummonAbilityInstance.cs`
- [ ] Implement `TeleportAbilityInstance.cs`
- [ ] Test each ability type independently

**Deliverable:** All ability types can be executed

---

### Week 5-6: Player Abilities
- [ ] Create all player passive ability ScriptableObjects
- [ ] Create all 8 player active ability ScriptableObjects
- [ ] Implement remaining `AbilityEffect` subclasses as needed
- [ ] Create `PlayerAbilityInput.cs`
- [ ] Add `AbilityUser` component to player prefab
- [ ] Test each player class's abilities
- [ ] Create `IMovable` interface and update controllers

**Deliverable:** All 4 player classes have working abilities

---

### Week 7-8: Enemy Abilities
- [ ] Create `EnemyAbilityAI.cs` component
- [ ] Create enemy ability ScriptableObjects (Tide Touched, Deep One, Shambler)
- [ ] Add `AbilityUser` component to enemy prefabs
- [ ] Implement `AIUseCondition` evaluation
- [ ] Test enemy AI using abilities in combat
- [ ] Tune AI usage priorities and conditions

**Deliverable:** Enemies intelligently use abilities

---

### Week 9-10: Boss Abilities
- [ ] Create all boss ability ScriptableObjects
- [ ] Add `AbilityUser` to boss prefabs
- [ ] Implement `PossessAbilityInstance.cs` for The Color
- [ ] Implement boss-specific effects
- [ ] Test all 3 boss fights with abilities
- [ ] Tune boss ability timings and damage

**Deliverable:** All 3 bosses have working ability sets

---

### Week 11-12: UI, Polish & Testing
- [ ] Create `AbilityUIPanel.cs` and `AbilitySlot.cs`
- [ ] Design and implement ability UI layout
- [ ] Add ability tooltips
- [ ] Create all VFX placeholder prefabs
- [ ] Add audio clips to all abilities
- [ ] Network testing (multiplayer synchronization)
- [ ] Balance pass on cooldowns and effects
- [ ] Bug fixing and edge case handling
- [ ] Performance optimization (object pooling)

**Deliverable:** Polished, network-ready ability system

---

## Testing Strategy

### Unit Tests
- [ ] Test ability cooldown tracking
- [ ] Test ability effect application/removal
- [ ] Test AI condition evaluation
- [ ] Test network synchronization

### Integration Tests
- [ ] Test player abilities in solo mode
- [ ] Test player abilities in co-op
- [ ] Test enemy abilities in combat
- [ ] Test boss abilities in boss fights
- [ ] Test ability interactions (stacking, overlapping)

### Playtesting Focus
- [ ] Do abilities feel impactful?
- [ ] Are cooldowns appropriate?
- [ ] Do enemies use abilities intelligently?
- [ ] Do bosses feel challenging but fair?
- [ ] Is the UI clear and responsive?
- [ ] Are there any exploits or broken interactions?

---

## Success Criteria

✅ **Functionality:**
- All 4 player classes have 1 passive + 2 active abilities working
- All enemy abilities (Tide Touched, Deep One, Shambler) functional
- All 3 bosses have full ability sets (9 total boss abilities)
- Abilities are networked and synchronized in multiplayer

✅ **AI Integration:**
- Enemy AI uses abilities based on conditions
- Boss AI uses abilities at appropriate times
- No ability spam or broken AI behavior

✅ **Polish:**
- All abilities have VFX and audio
- UI displays cooldowns and ability info clearly
- Abilities feel responsive (< 100ms input lag)
- No critical bugs or crashes

✅ **Performance:**
- Ability system runs at 60 FPS with 4 players + 20 enemies
- Network bandwidth < 10 KB/s per player for abilities
- Object pooling prevents garbage collection spikes

✅ **Extensibility:**
- New abilities can be created via ScriptableObjects
- New ability types can be added with minimal code changes
- System is documented and maintainable

---

## Dependencies

**Must be completed first:**
- Player movement system ✅ (exists)
- Health/damage system ✅ (exists)
- Weapon system ✅ (exists)
- Basic enemy AI ✅ (exists)
- Networking foundation ✅ (exists)

**Can be parallel:**
- Breach Saturation system (needed for saturation-modifying abilities)
- Progression/XP system (needed for ability upgrades)
- Scrip currency system (needed for ability upgrades)

**Needed later (post-implementation):**
- Ability upgrade system (Tier 2/3 variants)
- Class selection UI (pre-game lobby)
- Class leveling system (XP and level-gating upgrades)

---

## Future Enhancements (Post-Launch)

**Tier 2 & 3 Ability Upgrades:**
- Implement `AbilityUpgradeData.cs`
- Create upgrade UI
- Add upgrade unlock logic (class level + scrip cost)
- Create upgraded variants of all abilities

**Additional Ability Types:**
- Channeled healing beams
- Chain lightning effects
- Wall/barrier creation
- Time dilation zones
- Status effect applications (poison, fire, etc.)

**Advanced AI:**
- Cooperative enemy ability combos
- Boss phase-based ability priority changes
- Context-aware ability targeting (prioritize low HP players, etc.)

**Ability Customization:**
- Mutators for abilities (increased range, reduced cooldown, etc.)
- Ability loadout swapping (choose 2 of 3 available per class)
- Prestige/mastery bonuses for frequently used abilities

---

## Notes and Considerations

**Performance:**
- Use object pooling for projectiles, zones, and VFX
- Limit simultaneous active abilities to prevent performance issues
- Consider culling VFX for abilities far from local player

**Network:**
- All ability activation is server-authoritative (anti-cheat)
- Client prediction for local player (instant feedback)
- Compress ability context data to minimize bandwidth
- Use ClientRpc sparingly (only for VFX/audio)

**Balance:**
- Player abilities should feel powerful but not broken
- Enemy abilities should create interesting challenges
- Boss abilities should require coordination to counter
- Cooldowns should prevent ability spam but allow frequent use

**UX:**
- Abilities should have clear visual and audio feedback
- UI should always show cooldown and availability
- Telegraphs for dangerous abilities (boss attacks)
- Tooltips explain what each ability does

---

**This implementation plan provides a complete, production-ready ability system that serves both players and enemies through a unified, extensible architecture.**
