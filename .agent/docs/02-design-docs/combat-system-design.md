# TIDE'S END: Combat System Design
## The Three Pillars: Gunplay, Abilities, and Enemy Design

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Purpose:** Explain WHY the combat works this way

---

## Document Purpose

This document explains the **design philosophy** behind Tide's End's combat system. If you need exact numbers, see the technical specs. If you want to understand the design thinking, read this.

**Related Documents:**
- Quick Reference: [Classes Quick Ref](../01-quick-reference/classes-quick-ref.md)
- Technical Specs: [Class Specifications](../03-technical-specs/class-specifications.md)
- Original Design: Combat System Doc (uploaded by user)

---

## Core Design Philosophy

**"Guns deal damage. Abilities create opportunities. Teamwork wins fights."**

### The 60/30/10 Split

**60% Gunplay:**
- You're ALWAYS shooting
- Positioning and aim matter
- Every bullet counts (ammo scarcity)
- Guns feel adequate, not exceptional

**30% Abilities:**
- Create tactical advantages
- Enable team synergy
- Memorable moments
- On cooldowns (not spammable)

**10% Equipment:**
- Grenades, traps, utility
- Supplement core gameplay
- Not build-defining

### Why This Split Works

**Problem:** Hunt: Showdown's gun feel took $10M+ and 6 years. You can't match that.

**Solution:** Make abilities so impactful that "good enough" guns feel great in context.

**Example:**
- Bad: Shoot enemy, gun feels weak, enemy bullet sponge
- Good: Lamplighter flares → you see enemy through fog → headshot feels AMAZING → enemy drops fast

The ability creates the setup. The gun gets the payoff. Both feel good.

---

## The 15-Second Combat Loop

Every fight follows this rhythm:

### **Phase 1: Positioning (3 seconds)**

**What happens:**
- Team identifies threat
- Classes deploy abilities
- Players take advantageous positions

**Example:**
```
Lamplighter: "Enemies ahead, lighting them up!"
[Flare shot fires, reveals 3 Deep Ones]

Bulwark: "Anchor down, shield up!"
[Plants shield facing enemy approach]

Occultist: "Circle behind shield!"
[Draws healing circle]

Harpooner: "I'll pull stragglers."
[Readies harpoon]
```

**Result:** Team has tactical advantage BEFORE shooting starts.

---

### **Phase 2: Engagement (8 seconds)**

**What happens:**
- Team shoots from positions
- Abilities enable kills
- Ammo management

**Example:**
```
[Deep Ones charge toward team]

Bulwark: "They're slowing in the anchor zone!"
[Deep Ones move 30% slower]

Team: [Focus fire on slowed enemies]
[Pop-pop-pop-pop - rifle shots]

Harpooner: "Pulling the runner!"
[Harpoons fleeing Deep One, drags it back]

Team: [Concentrates fire, enemy drops]

Occultist: "I'm at 70% health, staying in circle."
[Regenerates while shooting]
```

**Result:** Abilities don't get kills. Guns do. But abilities make the kills POSSIBLE.

---

### **Phase 3: Adaptation (4 seconds)**

**What happens:**
- Situation changes
- Team responds
- Reposition or retreat

**Example:**
```
[Reinforcements spawn behind team]

Lamplighter: "Behind us!"

Bulwark: "Anchor cooldown 20 seconds, moving!"

Team: [Repositions, uses cover]

Harpooner: "Activating Frenzy!"
[Buff activates, increased fire rate]

Team: [Burns down new wave quickly]

Occultist: "We're good, saturation at 45%."
```

**Result:** Combat is dynamic. No static camping. Always adapting.

---

## Why This Loop Works

### 1. **Abilities Solve the "PvE is Boring" Problem**

**Bad PvE:**
- Stand in doorway
- Shoot predictable enemies
- Repeat for 30 minutes
- Boring

**Good PvE (Ours):**
- Deploy anchor to slow boss
- Harpoon adds to create kill zones
- Healing circles for recovery
- Speed zones for repositioning
- DYNAMIC, not static

### 2. **Cooldowns Force Decisions**

**Without cooldowns:**
- Spam best ability forever
- Optimal play is repetitive
- No tension

**With cooldowns:**
- "Do I use anchor now or save for extraction?"
- "Should I harpoon this elite or wait for boss?"
- "Flare now or when more enemies spawn?"
- MEANINGFUL decisions every 45-90 seconds

### 3. **Teamwork is Emergent, Not Forced**

**Forced teamwork (bad):**
- "You MUST have all 4 classes"
- "This door requires 4 players"
- Feels artificial

**Emergent teamwork (good):**
- Bulwark's shield protects Occultist's healing circle
- Lamplighter's flare reveals enemies for Harpooner's harpoon
- Abilities naturally synergize
- Team discovers combos themselves

---

## Combat Scenarios (Practical Examples)

### Scenario 1: Extraction Hold (The Lighthouse)

**Setup:**
- Team reaches Lighthouse (2× extraction)
- Must activate beacon (30-second channel)
- Breach Saturation at 55%

**Team Composition:**
- Bulwark (Tank)
- Lamplighter (Scout)
- Harpooner (Damage)
- Occultist (Support)

**The Fight:**

**Seconds 0-5: Setup**
```
Bulwark: "I'll shield the entrance. Occultist, circle here."
[Plants Bastion Shield facing door]

Occultist: "Drawing circle behind shield."
[Sanctuary Circle active]

Lamplighter: "Lighting outside approaches."
[Flare Shot reveals incoming enemies]

Harpooner: "Starting beacon activation."
[30-second channel begins]
```

**Seconds 5-15: First Wave**
```
[5 Hollow-Eyed + 2 Reef Walkers approach]

Lamplighter: "Hollow-Eyed rushing, Reef Walkers on the rocks!"

Team: [Focus fire Hollow-Eyed through shield]
[Enemies can't pass shield, teammates shoot over it]

Harpooner: [Still channeling beacon]

Reef Walkers: [Fire harpoons from range]
[Shield absorbs projectiles]

Occultist: "Shield taking damage, but holding!"
```

**Seconds 15-25: Adaptation**
```
[Shield breaks, 3 Deep Ones spawn BEHIND team (ambush)]

Lamplighter: "BEHIND US!"

Bulwark: "Anchor down!"
[Throws anchor behind team, slows Deep Ones]

Harpooner: "Beacon at 80%, almost there!"

Team: [Splits fire - some toward door, some toward ambush]

Occultist: "Healing circle keeping us up!"
```

**Seconds 25-30: Clutch**
```
Harpooner: "BEACON ACTIVE!"
[Lighthouse beam lights up]

[Rowboat visible approaching shore]

Bulwark: "Everyone to the boat!"

Lamplighter: "Speed zone on the stairs!"
[Pathfinder's Mark placed]

Team: [Sprints down stairs with +40% speed]

Occultist: "Saturation at 62%, we're good!"

[Team extracts successfully]
```

**Why This Scenario Works:**
- Every class contributed meaningfully
- Abilities created advantages (shield bought time, anchor stopped ambush)
- Guns handled actual kills
- Team adapted to changing situation
- Tension high (shield break, ambush, timer pressure)

---

### Scenario 2: Boss Fight (The Drowned Priest)

**Setup:**
- Team enters Drowned Cathedral
- Boss fight initiated
- Phase 1: Boss summons adds every 15 seconds

**Team Composition:**
- Bulwark, Lamplighter, Harpooner, Occultist

**Phase 1 (100-66% Boss HP):**

**Opening:**
```
Occultist: "Purging corruption before we start!"
[Channels Purge Corruption, grants shields to all]

Team: "50 HP shields up!"

Bulwark: "Destroying left totem first."
[Totem is boss weak point, 500 HP]

Team: [Focus fire totem]
[Totem destroyed: Boss takes +25% damage]
```

**Add Management:**
```
Boss: [Summons 5 Hollow-Eyed]

Lamplighter: "Adds spawning, flare up!"
[Reveals all add spawn points]

Harpooner: "I'll pull them together."
[Harpoons one add, pulls toward team]

Team: [AoE damage from shotguns]
[Adds cleared quickly]

Occultist: "Circle down for recovery."
[Healing Circle placed]
```

**Phase 2 (66-33% Boss HP):**

**Corruption Zones:**
```
Boss: [Creates 3 moving corruption zones, 20 DPS]

Lamplighter: "Corruption zones spreading!"

Bulwark: "Anchor on the altar, fight there!"
[Reality anchor stops saturation gain]

Team: [Positions in anchor zone]
[Kites around corruption zones while shooting boss]

Harpooner: "Activating Frenzy!"
[Fire rate +30%, reload speed +50%]
[Burns boss HP quickly]
```

**Phase 3 (33-0% Boss HP):**

**ENRAGED:**
```
Boss: [All abilities active - adds + zones + pulses]

Occultist: "RITUAL PULSE INCOMING!"
[Boss charges AoE attack]

Lamplighter: "Speed zone, MOVE!"
[Team sprints out of pulse radius]

Boss: [Pulse hits, but team dodged]

Bulwark: "Shield facing boss, burn him down!"
[Plants shield, team shoots from behind it]

Harpooner: "Pulling adds away from Occultist!"
[Uses harpoon tactically]

Team: [Coordinated burst damage]

Boss: [Dies at 0 HP]

ALL: "BOSS DOWN!"
[800 scrip + Boss Token + Trophy]
```

**Why This Boss Fight Works:**
- Multi-phase keeps it fresh
- Weak points (totems) add strategy
- Add management tests teamwork
- Abilities essential (shield blocks pulse, speed dodges, etc.)
- Final phase is chaotic but winnable
- Reward feels earned

---

### Scenario 3: Emergency Extraction (Things Go Wrong)

**Setup:**
- Team deep in map
- Breach Saturation at 85% (DANGER)
- Low ammo (20% remaining)
- One player at 30% HP

**The Situation:**
```
Occultist: "We need to extract NOW. Saturation critical."

Bulwark: "Nearest extraction is the Shallows, 200m away."

Lamplighter: "I'm at 15 rounds left."

Harpooner: "I'm wounded, 30 HP."

Occultist: "I can heal you when we get to safety."
```

**The Sprint:**
```
Lamplighter: "Speed zone on our path!"
[Pathfinder's Mark placed ahead]

Team: [Sprints through speed zone]
[+40% movement speed]

[Dimensional Shambler teleports in]

Bulwark: "SHAMBLER!"

Harpooner: "I'll harpoon it, keep running!"
[Harpoons Shambler, pulls it off course]

Team: [Doesn't stop to fight, keeps running]
```

**The Last Stand:**
```
[Reach beach extraction point]

Occultist: "Purging corruption!"
[Channels 5 seconds, reduces saturation to 65%]

[Rowboat visible, 15-second pickup timer]

Bulwark: "Anchor on the beach!"
[Reality stabilizer placed]

[Enemy wave spawns - 8 enemies incoming]

Lamplighter: "Flare on the tree line!"
[Reveals all enemies]

Team: [Last stand, uses remaining ammo]

Harpooner: "I'm out of ammo!"
[Switches to melee or pistol]

Occultist: "10 SECONDS!"

[Rowboat arrives]

Team: [Sprints to boat while shooting]

[Barely escapes, 2 players at <20% HP]

ALL: "WE MADE IT!"
```

**Why This Scenario Works:**
- High tension (low resources, high saturation)
- Every ability mattered (speed to escape, anchor to stabilize, purge to reduce saturation)
- Teamwork under pressure
- Clutch moment (harpoon to delay Shambler)
- Relief at successful extraction

---

## Class Design Philosophy

### The Core Principle: **"Every class creates opportunities differently"**

**Bulwark creates opportunities by:**
- Stabilizing positions (anchor, shield)
- Tanking damage (high HP, resist)
- Controlling space (enemies must path around shield)

**Lamplighter creates opportunities by:**
- Revealing information (flare, vision)
- Enabling mobility (speed zones)
- Scouting ahead safely (quiet, fast)

**Harpooner creates opportunities by:**
- Controlling enemy position (harpoon pull)
- Burst damage windows (Frenzy buff)
- Punishing retreating enemies

**Occultist creates opportunities by:**
- Sustaining team (healing circle)
- Resetting pressure (Purge Corruption)
- Extending engagements (shields, healing)

### Why No Overlapping Roles?

**Bad design:**
- Two classes that both tank
- Player thinks "Why not bring two Bulwarks?"
- Redundancy, no variety

**Good design:**
- Each class fills unique niche
- Team says "We need a Lamplighter for vision"
- All 4 classes viable, none mandatory

---

## Enemy Design Philosophy

### The Core Principle: **"Simple AI, complex scenarios"**

**Bad approach:**
- Perfect AI that flanks, uses cover, coordinates
- Takes 1 year to program
- Still predictable once learned

**Good approach:**
- Simple state machines (Idle, Alert, Combat)
- Enemy VARIETY creates complexity
- Players adapt to enemy COMBINATIONS, not smart AI

### Enemy Archetypes

**Rusher (Hollow-Eyed, Deep Ones):**
- Forces movement
- Punishes static play
- "We can't just sit here!"

**Ranged (Reef Walkers):**
- Forces use of cover
- Punishes poor positioning
- "Get behind the shield!"

**Elite (Tide Touched, Shoggoths):**
- Forces prioritization
- Can't be ignored
- "Kill that Shoggoth before it splits!"

**Hunter (Dimensional Shambler):**
- Forces awareness
- Punishes tunnel vision
- "Watch for the teleport sound!"

**Boss (Drowned Priest, etc.):**
- Forces coordination
- Tests all skills learned
- "We need perfect execution!"

### Why This Works

**Mixed encounters create depth:**
- Rushers + Ranged = must balance offense and defense
- Rushers + Elite = must prioritize targets
- Ranged + Hunter = must maintain awareness
- All three = chaos requires teamwork

**Players develop strategies:**
- "Always kill Shoggoths first"
- "Harpoon the Shamblers when they appear"
- "Shield blocks Reef Walkers, ignore them"
- Meta develops naturally

---

## Weapon Design Philosophy

### The Core Principle: **"Adequate guns in a great context"**

**You're NOT building:**
- Hunt: Showdown's gun feel (impossible budget)
- Perfect reload animations (too expensive)
- Realistic ballistics (unnecessary complexity)

**You ARE building:**
- Functional shooting that feels OK
- Clear weapon roles (close/medium/long)
- Satisfying hit feedback (blood spray, sounds)

### Weapon Roles

**Rifles (M1903, Lee-Enfield):**
- Role: Precision damage at range
- Feel: Slow, deliberate, impactful
- Context: Shoot revealed enemies from safety

**Shotguns (Double-Barrel, Winchester):**
- Role: Close-range burst damage
- Feel: Powerful but punishing (long reload)
- Context: Rushers in your face

**Pistols (.38, M1911):**
- Role: Emergency backup
- Feel: Reliable but weak
- Context: Out of primary ammo

**Automatics (Thompson, BAR):**
- Role: Sustained DPS
- Feel: Spray control required
- Context: Sustained engagements, bosses

**Special (Elephant Gun):**
- Role: Boss killer
- Feel: Massive, slow, earned
- Context: Endgame power fantasy

### Why 8 Weapons is Enough

**Problem with 20+ weapons:**
- Balance nightmare
- Too many similar weapons
- Players ignore most

**Solution with 8 weapons:**
- Each weapon distinct role
- Progression clear (Tier 1→2→3→4)
- All weapons viable in correct context

**Comparison:**
- Hunt: Showdown: 50+ weapons (amazing variety, impossible scope)
- Deep Rock Galactic: 6-8 weapons per class (manageable, clear roles)
- Yours: 8 shared weapons (efficient, clear progression)

---

## Ability Design Philosophy

### The Core Principle: **"Abilities feel impactful, not mandatory"**

**Bad ability design:**
- Must use ability every cooldown (optimal play is boring)
- Ability deals damage directly (replaces guns)
- Ability has no counterplay (press button to win)

**Good ability design:**
- Situational use (timing matters)
- Creates opportunities (enables gun kills)
- Has trade-offs (vulnerable while casting)

### Ability Design Patterns

**Pattern 1: Zone Control (Anchor, Circle, Shield)**
- Place in world
- Creates safe area
- Trade-off: Limits mobility

**Pattern 2: Information (Flare, Vision)**
- Reveals enemies/paths
- Enables team decisions
- Trade-off: Alerts enemies

**Pattern 3: Enemy Control (Harpoon)**
- Affects enemy position
- Creates kill setups
- Trade-off: Single target

**Pattern 4: Buff (Frenzy)**
- Enhances stats temporarily
- Creates damage windows
- Trade-off: Duration limited

**Pattern 5: Sustain (Healing, Purge)**
- Recovers resources
- Extends survival
- Trade-off: Vulnerable while casting

### Why These Patterns Work

**Each pattern teaches different skill:**
- Zone Control = positioning
- Information = awareness
- Enemy Control = target selection
- Buff = timing
- Sustain = resource management

**Combined = high skill ceiling, low skill floor**

---

## Differentiation Summary

### vs. Hunt: Showdown

| Hunt | Tide's End |
|------|------------|
| 100% gun mastery | 60% guns, 40% abilities |
| Perfect gun feel required | Adequate guns + great abilities |
| PvP creates tension | Abilities + teamwork create tension |
| Solo viable | Team-dependent by design |
| 50+ weapons | 8 weapons (efficient scope) |

**Key Difference:** Hunt is a gun game with PvP. You're an ability game with guns.

### vs. Deep Rock Galactic

| Deep Rock | Tide's End |
|-----------|------------|
| Whimsical dwarves | Grimdark horror |
| Infinite ammo resupply | Scarcity matters |
| Procedural caves | Hand-crafted zones |
| 4 classes mandatory | Flexible composition |
| Mining & defense | Extraction & looting |

**Key Difference:** Deep Rock is ability-focused PvE. You're hybrid (guns + abilities) extraction.

### vs. Left 4 Dead / GTFO

| L4D / GTFO | Tide's End |
|------------|------------|
| Linear levels | Open extraction routes |
| No class abilities | Signature abilities |
| Horde survival | Tactical extraction |
| Zombies | Lovecraftian horrors |

**Key Difference:** L4D is pure gunplay co-op. You add class abilities for depth.

---

## What Makes This Combat Fun?

### 1. **The "Clutch Play" Moments**

**Setup:**
- Team overwhelmed
- Low HP, low ammo
- Breach Saturation critical

**The Moment:**
```
Occultist: "Purging corruption!"
[Reduces saturation 20%, buys time]

Bulwark: "Anchor on extraction!"
[Stabilizes reality, slows enemies]

Lamplighter: "Speed zone to boat!"
[Team escapes at last second]

ALL: "HOLY SHIT WE MADE IT!"
```

**Why it feels good:** Every ability mattered. Team coordination saved the run.

### 2. **The "Perfect Execution"**

**Setup:**
- Boss fight
- Team knows the strategy

**The Fight:**
```
[Everyone uses abilities at optimal times]
[Shield blocks boss pulse]
[Harpoon pulls adds away]
[Healing circle keeps team healthy]
[Flare reveals weak points]

[Boss dies, no one goes down]

ALL: "THAT WAS CLEAN!"
```

**Why it feels good:** Mastery. Team played optimally.

### 3. **The "Emergent Discovery"**

**Setup:**
- New player tries random combo

**The Discovery:**
```
New Player: "What if I harpoon the enemy INTO the healing circle?"

[Tries it]

[Enemy pulled into Occultist's circle]
[Takes damage from being in hostile zone]

Team: "WAIT THAT WORKS?!"

[New meta born]
```

**Why it feels good:** Players discover synergies themselves. Game rewards creativity.

---

## Core Design Principles (Never Compromise)

1. **Guns always deal damage** - Abilities create opportunities, guns execute
2. **Abilities require positioning** - No "press button to win"
3. **Team composition matters** - Solo struggles, teams thrive
4. **Simple AI, complex scenarios** - Enemy variety over smart AI
5. **Every shot matters** - Ammo scarcity creates tension
6. **Cooldowns force decisions** - Not ability spam, strategic timing
7. **Classes have clear identity** - Each feels totally different
8. **Achievable scope** - 8 guns, 8 abilities, simple systems

---

## Success Metrics

**The combat system succeeds if:**
- âœ… Players say "I main [class]"
- âœ… Team composition discussions happen ("We need an Occultist")
- âœ… Clutch moments generate excitement
- âœ… Abilities create memorable stories
- âœ… Gunplay feels adequate (not amazing, not terrible)
- âœ… 15-second loop is identifiable in all fights
- âœ… Players discover ability combos themselves

**The combat system fails if:**
- âŒ All classes feel the same
- âŒ Abilities ignored (guns only)
- âŒ Optimal play is static camping
- âŒ No emergent teamwork
- âŒ Gun feel so bad it's frustrating

---

## Related Documentation

**For quick facts:**
- [Classes Quick Ref](../01-quick-reference/classes-quick-ref.md)
- [Weapons Quick Ref](../01-quick-reference/weapons-quick-ref.md)

**For implementation:**
- [Class Specifications](../03-technical-specs/class-specifications.md)
- [Weapon Specifications](../03-technical-specs/weapon-specifications.md)
- [Enemy Specifications](../03-technical-specs/enemy-specifications.md)

**For other systems:**
- [Extraction Mechanics](extraction-mechanics.md)
- [Progression System](progression-system.md)

---

**This document explains the "why" behind combat design. Now go make it fun.**
