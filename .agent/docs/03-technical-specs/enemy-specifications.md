# TIDE'S END: Enemy Technical Specifications
## Implementation-Ready AI and Spawn Parameters

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Format:** State machine definitions and JSON parameters

---

## Document Purpose

This document provides **exact AI behavior specifications** and spawn logic for all enemies. Includes state machine diagrams, transition conditions, and numerical parameters.

**Related Documents:**
- Quick Reference: [enemies-quick-ref.md](../01-quick-reference/enemies-quick-ref.md)
- Design Context: [combat-system-design.md](../02-design-docs/combat-system-design.md)
- Balance Values: [balance-parameters.md](balance-parameters.md)

---

## AI STATE MACHINE OVERVIEW

### Core State Machine (All Enemies)

```
[IDLE] ──(see/hear player)──> [ALERT] ──(confirm threat)──> [COMBAT]
   â†'                              ↓                             ↓
   └────(lose track)────────────────┘                             │
                                                                   │
[DEAD] <─────────────────────(HP <= 0)────────────────────────────┘
```

### State Definitions

**IDLE:**
- Wander around spawn area
- Play idle animations
- Check for player detection periodically

**ALERT:**
- Investigate player location
- Move toward last known position
- Heightened detection range
- Notify nearby enemies

**COMBAT:**
- Execute attack behavior
- Navigate toward player
- Use special abilities (if any)
- Maintain aggro

**DEAD:**
- Play death animation
- Spawn loot (if applicable)
- Ragdoll physics
- Despawn after delay

---

## TIER 1 ENEMIES (Breach Saturation 0-25%)

### Hollow-Eyed (Basic Rusher)

```json
{
  "enemyName": "Hollow-Eyed",
  "enemyType": "CorruptedHuman",
  "tier": 1,
  "archetype": "Rusher",
  
  "stats": {
    "health": 100,
    "movementSpeed": 4.0,
    "combatSpeed": 5.5,
    "detectionRange": 15.0,
    "attackRange": 2.0
  },
  
  "combat": {
    "damage": 15,
    "attackCooldown": 1.5,
    "attackWindup": 0.5,
    "lungeDistance": 3.0,
    "canInterruptAttack": false
  },
  
  "ai": {
    "strafeChance": 0.0,
    "dodgeChance": 0.0,
    "flankingBehavior": false,
    "callForHelp": true,
    "helpRadius": 20.0
  },
  
  "audioVisual": {
    "idleSound": "Hollow_Idle_Moan",
    "alertSound": "Hollow_Alert_Growl",
    "attackSound": "Hollow_Attack_Scream",
    "deathSound": "Hollow_Death_Gurgle",
    "footstepSound": "Shamble_Footsteps"
  },
  
  "loot": {
    "dropChance": 0.10,
    "scrip": {
      "min": 5,
      "max": 10
    },
    "ammoChance": 0.05
  }
}
```

**AI Behavior:**
```
IDLE:
  - Random wander within 10m radius of spawn
  - Idle animation every 5-10 seconds
  - Detection check every 0.5 seconds
  
ALERT:
  - Move to last known player position
  - If reach position without finding player: return to IDLE after 10s
  - Alert nearby Hollow-Eyed within 20m (they enter ALERT too)
  
COMBAT:
  - NavMeshAgent seek player
  - When within attackRange (2m): trigger attack
  - Attack: 0.5s windup → lunge forward 3m → deal damage on contact
  - After attack: 1.5s cooldown before next attack
  - Never break aggro (stay in COMBAT until dead or player escapes 50m)
```

**Implementation Notes:**
- Simple state machine (3 states)
- NavMeshAgent for pathfinding
- Animator for attack telegraphs
- Sphere collider for attack hitbox

---

### Reef Walker (Ranged Cultist)

```json
{
  "enemyName": "Reef Walker",
  "enemyType": "CorruptedHuman",
  "tier": 1,
  "archetype": "Ranged",
  
  "stats": {
    "health": 150,
    "movementSpeed": 3.5,
    "combatSpeed": 3.5,
    "detectionRange": 25.0,
    "preferredRange": 15.0,
    "minimumRange": 8.0
  },
  
  "combat": {
    "damage": 25,
    "attackCooldown": 3.0,
    "projectileSpeed": 30.0,
    "projectileGravity": true,
    "aimPrediction": false
  },
  
  "ai": {
    "strafeChance": 0.3,
    "dodgeChance": 0.0,
    "maintainDistance": true,
    "backpedalIfPlayerClose": true,
    "callForHelp": true
  },
  
  "audioVisual": {
    "idleSound": "Reef_Idle_Breathing",
    "alertSound": "Reef_Alert_Helmet_Clang",
    "attackSound": "Harpoon_Fire",
    "deathSound": "Reef_Death_Gurgle",
    "projectilePrefab": "Harpoon_Projectile"
  },
  
  "loot": {
    "dropChance": 0.15,
    "scrip": {
      "min": 10,
      "max": 20
    },
    "ammoChance": 0.10
  }
}
```

**AI Behavior:**
```
IDLE:
  - Patrol between 2-3 waypoints
  - Long detection range (25m)
  
ALERT:
  - Move to elevated position if available (check for higher ground)
  - Notify nearby allies
  
COMBAT:
  - Maintain distance (preferredRange = 15m)
  - If player closer than minimumRange (8m): backpedal while firing
  - If player at preferredRange: strafe (30% chance per second)
  - Attack: 
    1. Aim at player (instant lock)
    2. Fire harpoon projectile
    3. 3s cooldown
  - Projectile: Ballistic arc, 30 m/s speed, affected by gravity
```

**Implementation Notes:**
- Ranged combat AI (distance keeper)
- Projectile physics (ballistic arc)
- Simple strafe behavior (NavMeshAgent move to side points)

---

## TIER 2 ENEMIES (Breach Saturation 25-50%)

### Deep One (Pack Hunter)

```json
{
  "enemyName": "Deep One",
  "enemyType": "Eldritch",
  "tier": 2,
  "archetype": "Rusher",
  
  "stats": {
    "health": 120,
    "movementSpeed": 5.5,
    "combatSpeed": 7.0,
    "waterSpeed": 9.0,
    "detectionRange": 20.0,
    "attackRange": 2.5
  },
  
  "combat": {
    "damage": 20,
    "attackCooldown": 1.2,
    "leapAttack": true,
    "leapDistance": 8.0,
    "leapCooldown": 8.0
  },
  
  "ai": {
    "packBehavior": true,
    "packSize": {
      "min": 3,
      "max": 5
    },
    "flankingBehavior": true,
    "ambushFromWater": true,
    "coordinatedAttacks": true
  },
  
  "audioVisual": {
    "idleSound": "Deep_Idle_Croaks",
    "alertSound": "Deep_Alert_Screech",
    "attackSound": "Deep_Attack_Hiss",
    "leapSound": "Deep_Leap",
    "deathSound": "Deep_Death_Wail",
    "waterEmergenceVFX": "Water_Splash_Emergence"
  },
  
  "loot": {
    "dropChance": 0.20,
    "scrip": {
      "min": 15,
      "max": 25
    },
    "ammoChance": 0.10
  }
}
```

**AI Behavior:**
```
SPECIAL SPAWNING:
  - Always spawns in packs (3-5 units)
  - If water nearby: spawn submerged, emerge when player approaches
  - Emergence: Play splash VFX, dramatic reveal
  
IDLE:
  - Pack stays grouped (maintain 5m radius formation)
  - One designated "alpha" leads patrol
  
ALERT:
  - Alpha alerts entire pack instantly
  - Pack converges on player from multiple angles
  
COMBAT:
  - Coordinated flanking:
    * Calculate player's forward direction
    * 1-2 Deep Ones attack from front
    * 2-3 Deep Ones circle to flanks/rear
  
  - Leap attack (every 8 seconds):
    * Jump 8m toward player
    * Deal damage on landing
    * Bypass shields/cover
  
  - If 50% faster movement in water volumes
```

**Pack Coordination Logic:**
```csharp
public class DeepOnePackAI : MonoBehaviour
{
    public List<DeepOne> packMembers;
    public DeepOne alpha;
    
    private void Update()
    {
        if (alpha.currentState == AIState.Combat)
        {
            AssignFlankingPositions();
        }
    }
    
    private void AssignFlankingPositions()
    {
        Vector3 playerPos = PlayerManager.GetClosestPlayer().position;
        Vector3 playerForward = PlayerManager.GetClosestPlayer().forward;
        
        // Assign roles
        int frontalAttackers = Mathf.Min(2, packMembers.Count / 2);
        
        for (int i = 0; i < packMembers.Count; i++)
        {
            if (i < frontalAttackers)
            {
                // Attack from front
                packMembers[i].SetTargetPosition(playerPos + playerForward * 5f);
            }
            else
            {
                // Flank from sides/rear
                float angle = (i - frontalAttackers) * 120f;
                Vector3 flankPos = Quaternion.Euler(0, angle, 0) * -playerForward * 8f;
                packMembers[i].SetTargetPosition(playerPos + flankPos);
            }
        }
    }
}
```

---

### Tide Touched (Mutated Animal)

```json
{
  "enemyName": "Tide Touched",
  "enemyType": "Eldritch",
  "tier": 2,
  "archetype": "Elite",
  
  "stats": {
    "health": 200,
    "movementSpeed": 6.0,
    "detectionRange": 18.0,
    "attackRange": 3.0
  },
  
  "combat": {
    "damage": 30,
    "attackCooldown": 2.0,
    "multiHit": true,
    "hitsPerAttack": 3,
    "phaseAbility": true
  },
  
  "ai": {
    "unpredictableMovement": true,
    "phaseChance": 0.15,
    "phaseCooldown": 12.0,
    "blocksPassages": true
  },
  
  "phaseAbility": {
    "duration": 2.0,
    "canPassThroughWalls": true,
    "invulnerable": true,
    "reappearNearPlayer": true,
    "reappearDistance": 5.0
  },
  
  "audioVisual": {
    "idleSound": "Tide_Idle_Chittering",
    "alertSound": "Tide_Alert_Shriek",
    "attackSound": "Tide_Attack_Claws",
    "phaseSound": "Tide_Phase_Warp",
    "phaseVFX": "Reality_Distortion",
    "multiEyesGlow": "Eldritch_Eye_Glow"
  },
  
  "loot": {
    "dropChance": 0.25,
    "scrip": {
      "min": 20,
      "max": 35
    },
    "ammoChance": 0.15
  }
}
```

**AI Behavior:**
```
IDLE/ALERT:
  - Unpredictable movement (zigzag patterns)
  - Sometimes stops suddenly, sometimes lunges
  
COMBAT:
  - Standard melee attacks (30 damage, 3-hit combo)
  
  - Phase ability (15% chance every second if off cooldown):
    1. Become semi-transparent (invulnerable)
    2. Move through walls toward player
    3. After 2 seconds: reappear within 5m of player
    4. Immediately attack
    5. 12 second cooldown
  
  - Environmental blocker:
    * Large collision size blocks narrow passages
    * Forces players to kill or find alternate route
```

**Implementation Notes:**
- Phase mechanic: Disable collisions, enable transparency shader
- Unpredictable movement: NavMeshAgent with random offset destinations
- Large collider for passage blocking

---

## TIER 3 ENEMIES (Breach Saturation 50-75%)

### Shoggoth (Mini)

```json
{
  "enemyName": "Shoggoth (Mini)",
  "enemyType": "Eldritch",
  "tier": 3,
  "archetype": "Elite",
  
  "stats": {
    "health": 300,
    "movementSpeed": 3.0,
    "detectionRange": 30.0,
    "attackRange": 5.0
  },
  
  "combat": {
    "damage": 10,
    "damageType": "PerSecond",
    "areaOfEffect": true,
    "corruptsGround": true,
    "splitOnDamage": true
  },
  
  "splitMechanic": {
    "healthThreshold": 0.5,
    "numberSplits": 2,
    "splitHealth": 150,
    "splitSize": 0.7
  },
  
  "ai": {
    "oozeMovement": true,
    "ignoreTerrain": true,
    "areaDenial": true,
    "corruptedGroundDamage": 5
  },
  
  "audioVisual": {
    "idleSound": "Shoggoth_Bubbling_Loop",
    "alertSound": "Shoggoth_Forming_Eyes",
    "attackSound": "Shoggoth_Engulf",
    "splitSound": "Shoggoth_Split",
    "corruptionVFX": "Ground_Corruption_Trail",
    "bodyVFX": "Bubbling_Black_Mass"
  },
  
  "loot": {
    "dropChance": 0.30,
    "scrip": {
      "min": 30,
      "max": 50
    },
    "ammoChance": 0.20,
    "specialDropChance": 0.10
  }
}
```

**AI Behavior:**
```
MOVEMENT:
  - Slow, oozing movement (3 m/s)
  - Can move through small gaps (ignores terrain obstacles)
  - Leaves corruption trail behind (ground takes damage texture)
  
COMBAT:
  - Contact damage (10 damage per second while touching)
  - Corruption trail: Players standing on it take 5 damage per second
  
  - Split mechanic:
    * When health drops below 50%: split into 2 smaller Shoggoths
    * Each split has 150 HP (50% of original)
    * Splits are 70% the size
    * Splits can split again at 75 HP (25% of original max)
    * Maximum 4 splits total (original → 2 → 4)
  
AREA DENIAL:
  - Forces players to constantly move (corruption damage)
  - Blocks narrow passages (large hitbox)
  - Creates hazard zones that persist
```

**Implementation Notes:**
- Corruption trail: Instantiate decal GameObjects with damage trigger
- Split mechanic: Instantiate 2 new enemies at death, destroy original
- Ooze movement: Slow NavMeshAgent, ignore height differences

---

### Dimensional Shambler (Hunter)

```json
{
  "enemyName": "Dimensional Shambler",
  "enemyType": "Eldritch",
  "tier": 3,
  "archetype": "Elite",
  
  "stats": {
    "health": 200,
    "movementSpeed": 7.5,
    "detectionRange": 40.0,
    "attackRange": 2.0
  },
  
  "combat": {
    "damage": 40,
    "attackWindup": 0.3,
    "attackCooldown": 5.0,
    "teleportAttack": true
  },
  
  "teleportAbility": {
    "cooldown": 8.0,
    "maxRange": 20.0,
    "preferredDistance": 10.0,
    "teleportsBehindPlayer": true,
    "invisibleWhileTeleporting": true
  },
  
  "ai": {
    "hitAndRun": true,
    "invisibleUntilAttack": true,
    "audioTelegraph": true,
    "tracksSingleTarget": true
  },
  
  "audioVisual": {
    "teleportInSound": "Shambler_Appear",
    "teleportOutSound": "Shambler_Vanish",
    "attackSound": "Shambler_Strike",
    "movementSound": "Reality_Distortion_Ambient",
    "visibilityShader": "Invisibility_Distortion"
  },
  
  "loot": {
    "dropChance": 0.35,
    "scrip": {
      "min": 35,
      "max": 60
    },
    "ammoChance": 0.25,
    "specialDropChance": 0.15
  }
}
```

**AI Behavior:**
```
SPECIAL MECHANICS:
  - Always invisible (distortion shader only)
  - Players can see distortion effect when close (5m)
  - Audio telegraph: Hear dimensional warping sound before attack
  
IDLE/ALERT:
  - Invisible, slow patrol
  - Long detection range (40m)
  
COMBAT (Hit-and-Run):
  1. Select target player (tracks for full fight)
  2. Teleport near target (prefer behind, 10m distance)
  3. Brief telegraph (0.3s wind-up, visible during this)
  4. Attack (40 damage melee strike)
  5. Immediately teleport away (20m in random direction)
  6. Return to invisible
  7. Wait 8 seconds, repeat
  
COUNTERPLAY:
  - Audio cues warn of incoming attack
  - Brief visibility window during attack
  - Predictable timing (8s between attacks)
  - Team awareness required (call out positions)
```

**Implementation Notes:**
- Invisibility: Transparent shader with distortion VFX
- Teleport: Disable renderer, reposition, re-enable renderer
- Audio spatial: 3D sound at teleport-in location before appear

---

## BOSS ENEMIES (Breach Saturation 75-100%)

### The Drowned Priest

```json
{
  "bossName": "The Drowned Priest",
  "bossType": "Eldritch",
  "encounterLocation": "Drowned Cathedral",
  
  "stats": {
    "health": 2000,
    "movementSpeed": 0.0,
    "floatHeight": 3.0,
    "attackRange": 30.0
  },
  
  "phases": [
    {
      "phaseNumber": 1,
      "healthRange": [100, 66],
      "behavior": "SummonWaves",
      "waveInterval": 15.0
    },
    {
      "phaseNumber": 2,
      "healthRange": [66, 33],
      "behavior": "DamageZones",
      "zoneCount": 3,
      "zoneDamage": 20
    },
    {
      "phaseNumber": 3,
      "healthRange": [33, 0],
      "behavior": "Combined",
      "enraged": true
    }
  ],
  
  "abilities": [
    {
      "abilityName": "Summon Wave",
      "cooldown": 15.0,
      "summonCount": 5,
      "summonType": "Hollow-Eyed"
    },
    {
      "abilityName": "Corrupt Zone",
      "cooldown": 20.0,
      "duration": 10.0,
      "damage": 20,
      "radius": 5.0
    },
    {
      "abilityName": "Ritual Pulse",
      "cooldown": 12.0,
      "damage": 50,
      "knockback": 10.0,
      "radius": 15.0
    }
  ],
  
  "weakPoints": [
    {
      "name": "Ritual Totem 1",
      "health": 500,
      "position": "Left Side",
      "destroyedEffect": "Boss takes +25% damage"
    },
    {
      "name": "Ritual Totem 2",
      "health": 500,
      "position": "Right Side",
      "destroyedEffect": "Boss takes +25% damage"
    },
    {
      "name": "Ritual Totem 3",
      "health": 500,
      "position": "Rear",
      "destroyedEffect": "Boss takes +25% damage"
    }
  ],
  
  "loot": {
    "guaranteed": true,
    "scrip": {
      "min": 500,
      "max": 800
    },
    "bossToken": true,
    "specialDrop": "Drowned Priest Trophy"
  }
}
```

**Boss Fight Mechanics:**
```
SETUP:
  - Boss floats above altar (invulnerable)
  - 3 Ritual Totems around arena (500 HP each)
  - Must destroy totems to damage boss
  
PHASE 1 (100-66% HP):
  - Every 15 seconds: Summon 5 Hollow-Eyed adds
  - Totems can be destroyed for +25% damage each
  - Boss uses Ritual Pulse (50 damage AoE knockback)
  
PHASE 2 (66-33% HP):
  - Adds stop spawning
  - Boss creates 3 corruption zones (20 DPS each)
  - Zones move slowly, players must avoid
  - Ritual Pulse now faster (every 8 seconds)
  
PHASE 3 (33-0% HP):
  - ENRAGED: All abilities active
  - Summon waves + Corruption zones + Pulse
  - Movement speed increases 50%
  - Damage increases 25%
  
STRATEGY:
  - Destroy totems first (makes boss vulnerable)
  - Manage adds during Phase 1
  - Position carefully during Phase 2 (corruption zones)
  - Burst damage during Phase 3 before overwhelmed
```

---

### The Reef Leviathan

```json
{
  "bossName": "The Reef Leviathan",
  "bossType": "Eldritch",
  "encounterLocation": "Coastal Arena",
  
  "stats": {
    "health": 2500,
    "movementSpeed": 4.0,
    "swimSpeed": 6.0,
    "attackRange": 15.0
  },
  
  "phases": [
    {
      "phaseNumber": 1,
      "healthRange": [100, 50],
      "behavior": "Standard",
      "submergeCount": 0
    },
    {
      "phaseNumber": 2,
      "healthRange": [50, 0],
      "behavior": "Enraged",
      "submergeCount": 3
    }
  ],
  
  "abilities": [
    {
      "abilityName": "Tentacle Sweep",
      "cooldown": 8.0,
      "damage": 60,
      "sweepArc": 180,
      "knockback": 15.0,
      "telegraph": 2.0
    },
    {
      "abilityName": "Whirlpool",
      "cooldown": 25.0,
      "duration": 10.0,
      "pullForce": 5.0,
      "damage": 15,
      "radius": 12.0
    },
    {
      "abilityName": "Submerge",
      "cooldown": 30.0,
      "duration": 5.0,
      "invulnerable": true,
      "reemergeAttack": true,
      "reemergeDamage": 80
    }
  ],
  
  "weakPoints": [
    {
      "name": "Head",
      "damageMultiplier": 2.0,
      "vulnerable": "Phase 1 only"
    },
    {
      "name": "Barnacle Clusters",
      "damageMultiplier": 1.5,
      "count": 4,
      "health": 200
    }
  ],
  
  "loot": {
    "guaranteed": true,
    "scrip": {
      "min": 600,
      "max": 1000
    },
    "bossToken": true,
    "specialDrop": "Reef Leviathan Trophy"
  }
}
```

**Boss Fight Mechanics:**
```
ARENA:
  - Partially flooded coastal area
  - Water slows players (movement -30%)
  - Leviathan moves faster in water
  
PHASE 1 (100-50% HP):
  - Tentacle Sweep: Boss rears up (2s telegraph), sweeps 180° arc
    * 60 damage + knockback
    * Dodge by moving behind boss
  
  - Whirlpool: Creates water vortex (12m radius)
    * Pulls players toward center (5 m/s pull force)
    * 15 damage per second
    * Lasts 10 seconds
    * Position: Use abilities to escape or tank with shield
  
  - Weak point: Head takes 2× damage (aim carefully)
  
PHASE 2 (50-0% HP):
  - ENRAGED: Faster attacks (cooldowns -25%)
  - Submerge ability unlocked:
    * Boss dives underwater (becomes invulnerable)
    * 5 seconds later: Erupts near a player
    * 80 damage AoE explosion on reemergence
    * Audio cue: Rumbling before eruption
  
  - Barnacle weak points appear:
    * 4 clusters on body (200 HP each)
    * Destroying barnacles: Boss takes +50% damage temporarily (10s)
  
STRATEGY:
  - Phase 1: Focus head shots, dodge tentacle sweeps
  - Phase 2: Destroy barnacles for damage windows
  - During submerge: Spread out to minimize reemergence damage
```

---

### The Color (Out of Space)

```json
{
  "bossName": "The Color",
  "bossType": "Eldritch",
  "encounterLocation": "Corrupted Grove",
  
  "stats": {
    "health": 1500,
    "movementSpeed": 5.0,
    "incorporeal": true,
    "immuneToPhysical": false
  },
  
  "phases": [
    {
      "phaseNumber": 1,
      "healthRange": [100, 75],
      "behavior": "Possess",
      "possessionFrequency": "Low"
    },
    {
      "phaseNumber": 2,
      "healthRange": [75, 50],
      "behavior": "Drain",
      "colorDrainSpeed": "Medium"
    },
    {
      "phaseNumber": 3,
      "healthRange": [50, 25],
      "behavior": "Aggressive",
      "possessionFrequency": "High"
    },
    {
      "phaseNumber": 4,
      "healthRange": [25, 0],
      "behavior": "Desperate",
      "colorDrainSpeed": "Fast"
    }
  ],
  
  "abilities": [
    {
      "abilityName": "Possess Teammate",
      "cooldown": 30.0,
      "duration": 8.0,
      "controlPlayer": true,
      "breakCondition": "Deal 100 damage to possessed player"
    },
    {
      "abilityName": "Color Drain",
      "cooldown": 0.0,
      "passive": true,
      "damagePerSecond": 5,
      "radius": 10.0
    },
    {
      "abilityName": "Reality Distortion",
      "cooldown": 20.0,
      "duration": 5.0,
      "effect": "Reverse player controls"
    }
  ],
  
  "weaknesses": [
    {
      "name": "Darkness",
      "effect": "Takes 2× damage in shadowed areas",
      "implementation": "Check light level at boss position"
    },
    {
      "name": "Flares",
      "effect": "Stunned for 3 seconds by Lamplighter flare",
      "cooldown": 15.0
    }
  ],
  
  "loot": {
    "guaranteed": true,
    "scrip": {
      "min": 700,
      "max": 1200
    },
    "bossToken": true,
    "specialDrop": "The Color Trophy",
    "uniqueDrop": "Prismatic Artifact (rare)"
  }
}
```

**Boss Fight Mechanics:**
```
UNIQUE MECHANICS:
  - Incorporeal entity (glowing impossible colors)
  - Drains color from environment (desaturation effect)
  - Weakened by darkness/shadows
  
PHASE 1 (100-75% HP):
  - Possess Teammate (30s cooldown):
    * Color possesses one random player
    * Possessed player controlled by AI (attacks teammates)
    * Team must deal 100 damage to possessed player to break
    * Possessed player takes reduced damage (50%)
  
  - Color Drain (passive):
    * 10m radius around boss
    * 5 damage per second to players inside
    * Environment loses color (visual feedback)
  
PHASE 2 (75-50% HP):
  - Color Drain speed increases (10 DPS)
  - Reality Distortion (20s cooldown):
    * Reverses player movement controls
    * Lasts 5 seconds
    * Disorienting but survivable
  
PHASE 3 (50-25% HP):
  - Possession frequency increases (20s cooldown)
  - Can now possess 2 players simultaneously
  - Color Drain radius increases (15m)
  
PHASE 4 (25-0% HP):
  - DESPERATE: All abilities active
  - Color Drain 15 DPS
  - Possession cooldown 15s
  - Boss movement speed +50%
  
STRATEGY:
  - Lamplighter: Use flares to stun boss (buys 3 seconds)
  - Fight in shadowed areas for 2× damage
  - During possession: Team calls out who's possessed, focus fire
  - Avoid standing in Color Drain radius
  - Bulwark: Shield can block Color Drain damage
```

---

## SPAWN DIRECTOR SYSTEM

### Spawn Rate by Breach Saturation

```json
{
  "spawnRates": {
    "0-25%": {
      "interval": 15.0,
      "spawnPoints": "Fixed",
      "behavior": "Predictable patrols"
    },
    "25-50%": {
      "interval": 10.0,
      "spawnPoints": "Fixed + Ambush",
      "behavior": "Can spawn behind players"
    },
    "50-75%": {
      "interval": 7.0,
      "spawnPoints": "Director AI",
      "behavior": "Spawns at chokepoints/flanks"
    },
    "75-100%": {
      "interval": 5.0,
      "spawnPoints": "Continuous",
      "behavior": "No safe areas"
    }
  }
}
```

### Spawn Composition Rules

```json
{
  "0-25% Saturation": {
    "Hollow-Eyed": 0.70,
    "Reef Walker": 0.30
  },
  "25-50% Saturation": {
    "Hollow-Eyed": 0.50,
    "Reef Walker": 0.30,
    "Deep One Pack": 0.20
  },
  "50-75% Saturation": {
    "Hollow-Eyed": 0.30,
    "Reef Walker": 0.20,
    "Deep One Pack": 0.30,
    "Tide Touched": 0.10,
    "Shoggoth": 0.05,
    "Dimensional Shambler": 0.05
  },
  "75-100% Saturation": {
    "Hollow-Eyed": 0.20,
    "Reef Walker": 0.15,
    "Deep One Pack": 0.25,
    "Tide Touched": 0.15,
    "Shoggoth": 0.10,
    "Dimensional Shambler": 0.10,
    "Boss Enemies": 0.05
  }
}
```

### Director AI Logic

```csharp
public class SpawnDirector : MonoBehaviour
{
    public float currentSaturation = 0f;
    public List<SpawnPoint> allSpawnPoints;
    public Dictionary<SpawnPoint, float> spawnWeights;
    
    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + GetSpawnInterval();
        }
    }
    
    private float GetSpawnInterval()
    {
        if (currentSaturation < 25f) return 15f;
        if (currentSaturation < 50f) return 10f;
        if (currentSaturation < 75f) return 7f;
        return 5f;
    }
    
    private void SpawnEnemy()
    {
        // Select enemy type based on saturation
        string enemyType = SelectEnemyType();
        
        // Select spawn point based on saturation
        SpawnPoint spawnPoint = SelectSpawnPoint();
        
        // Instantiate enemy
        Instantiate(enemyPrefabs[enemyType], spawnPoint.position, spawnPoint.rotation);
    }
    
    private SpawnPoint SelectSpawnPoint()
    {
        if (currentSaturation < 25f)
        {
            // Fixed patrol spawns only
            return allSpawnPoints.Where(sp => sp.type == SpawnType.Patrol).Random();
        }
        else if (currentSaturation < 50f)
        {
            // 50% fixed, 50% ambush (behind players)
            bool useAmbush = Random.value > 0.5f;
            if (useAmbush)
                return GetAmbushSpawn();
            else
                return allSpawnPoints.Where(sp => sp.type == SpawnType.Patrol).Random();
        }
        else if (currentSaturation < 75f)
        {
            // Director AI: Spawn at tactical positions
            return GetTacticalSpawn();
        }
        else
        {
            // Continuous pressure: Spawn anywhere
            return allSpawnPoints.Random();
        }
    }
    
    private SpawnPoint GetAmbushSpawn()
    {
        // Find spawn points behind player or in "cleared" areas
        Vector3 playerPos = PlayerManager.GetAveragePlayerPosition();
        Vector3 playerForward = PlayerManager.GetAveragePlayerForward();
        
        return allSpawnPoints
            .Where(sp => Vector3.Dot((sp.position - playerPos).normalized, playerForward) < 0)
            .OrderBy(sp => Vector3.Distance(sp.position, playerPos))
            .First();
    }
    
    private SpawnPoint GetTacticalSpawn()
    {
        // Spawn at chokepoints or flanking positions
        Vector3 playerPos = PlayerManager.GetAveragePlayerPosition();
        
        return allSpawnPoints
            .Where(sp => sp.type == SpawnType.Tactical)
            .OrderByDescending(sp => CalculateTacticalValue(sp, playerPos))
            .First();
    }
    
    private float CalculateTacticalValue(SpawnPoint sp, Vector3 playerPos)
    {
        float distanceValue = 1f / Vector3.Distance(sp.position, playerPos);
        float chokePointValue = sp.isChokepoint ? 2f : 1f;
        float flankValue = IsFlankingPosition(sp.position, playerPos) ? 1.5f : 1f;
        
        return distanceValue * chokePointValue * flankValue;
    }
}
```

---

## Testing Checklist

**Per Enemy Type:**
- [ ] Health values correct
- [ ] Movement speed correct
- [ ] Attack damage correct
- [ ] AI state transitions work
- [ ] Detection range functional
- [ ] Audio plays at correct times
- [ ] VFX spawn correctly
- [ ] Loot drops correctly

**Boss Enemies:**
- [ ] All phases trigger at correct HP thresholds
- [ ] All abilities function correctly
- [ ] Weak points take correct damage
- [ ] Phase transitions smooth
- [ ] Loot guaranteed on death

**Spawn System:**
- [ ] Spawn intervals correct per saturation level
- [ ] Enemy composition matches saturation
- [ ] Director AI spawns tactically (50%+ saturation)
- [ ] Ambush spawns work (25%+ saturation)
- [ ] No spawn overlaps or stuck enemies

---

## Related Documentation

**For quick reference:**
- [Enemies Quick Ref](../01-quick-reference/enemies-quick-ref.md)

**For balance tuning:**
- [Balance Parameters](balance-parameters.md)

**For combat design:**
- [Combat System Design](../02-design-docs/combat-system-design.md)

---

**This document provides exact AI specifications ready for Unity implementation using state machines and NavMeshAgent systems.**
