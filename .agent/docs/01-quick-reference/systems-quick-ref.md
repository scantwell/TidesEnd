# Systems Quick Reference
## Core Game Systems Overview

**Last Updated:** October 27, 2025  
**Status:** ✅ Locked

---

## System Index

1. [Breach Saturation System](#breach-saturation-system)
2. [Extraction Points & Multipliers](#extraction-points--multipliers)
3. [Progression System](#progression-system)
4. [Loot & Inventory](#loot--inventory)
5. [Death & Revival](#death--revival)
6. [Contract Modifiers](#contract-modifiers)
7. [Tidal Mechanics](#tidal-mechanics)
8. [Reality Rifts](#reality-rifts)

---

## Breach Saturation System

**What It Is:** Reality corruption meter that increases over time, creating escalating difficulty.

**Increases From:**
- Killing enemies (varies by tier)
- Time in zone (+1% per minute baseline)
- Loud actions (gunfire, explosions) (+0.5% per loud action)

**Decreases From:**
- Closing reality rifts (-10% per rift)
- Occultist's Purge Corruption ability (-20%)
- Exiting zone (resets to 0%)

---

### Saturation Effects by Level

#### 0-25% (STABLE) - Green
**Visual:**
- Light fog (standard density)
- Normal lighting
- Minimal particle effects

**Audio:**
- Distant foghorns
- Creaking wood
- Surf sounds

**Enemies:**
- **100% Tier 1** (Hollow-Eyed, Reef Walkers)
- Spawn rate: 1 per 15 seconds
- Predictable patrols

**Behavior:**
- Basic AI (idle → alert → combat)
- Fixed patrol routes

---

#### 25-50% (FRACTURING) - Yellow
**Visual:**
- Thicker fog (increased density)
- Screen vignette darkens at edges
- Drifting embers/spores particles

**Audio:**
- Eerie ambience layer
- Footsteps have reverb
- Distant whispers

**Enemies:**
- **70% Tier 1, 30% Tier 2** (Deep Ones, Tide Touched)
- Spawn rate: 1 per 10 seconds (1.5× increase)

**Behavior:**
- **Ambush spawns enabled** (enemies spawn behind players in "cleared" areas)
- **Audio hallucinations** (fake enemy sounds from random directions)

---

#### 50-75% (BREACHING) - Orange
**Visual:**
- Color desaturation (blue tint)
- Aggressive particle systems
- More visual distortions

**Audio:**
- Dissonant music layer
- Distant sounds muffled
- Reality distortion audio

**Enemies:**
- **40% Tier 1, 40% Tier 2, 20% Tier 3** (Shoggoths, Shamblers)
- Spawn rate: 1 per 7 seconds (2× increase)
- Enemies move +15% faster

**Behavior:**
- **Director AI active** (spawns enemies at chokepoints/flanks)
- **Hunter enemies spawn** (Dimensional Shamblers actively pursue)
- **Coordinated attacks** (enemies flank and focus fire)
- **Pre-placed hazards activate** (gas vents, electrical arcs)

---

#### 75-100% (COLLAPSE) - Red
**Visual:**
- Heavy color grading (purple/green tint)
- Screen shake events
- Bloom + chromatic aberration

**Audio:**
- Loud ambient noise peaks
- Audio distortion filter on all sounds
- Reality collapsing audio

**Enemies:**
- **20% Tier 1, 30% Tier 2, 30% Tier 3, 20% Tier 4** (Star Spawn, Formless)
- Spawn rate: 1 per 5 seconds (3× increase)
- **Boss-tier enemies CAN spawn**
- All enemies have enhanced stats

**Behavior:**
- **Continuous enemy pressure** (no spawn cooldowns at map edges)
- **No safe areas**
- **Environmental hazards fully active** (toxic gas, collapsing roofs, electrical arcs)

**Extraction Penalty:**
- Extract above 90% saturation = lose 25% of earned currency

---

### Saturation Increase Rates

| Action | Saturation Increase |
|--------|-------------------|
| Kill Tier 1 enemy | +1% |
| Kill Tier 2 enemy | +2% |
| Kill Tier 3 enemy | +3% |
| Kill Tier 4 enemy | +5% |
| Kill Boss | +10% |
| Loud gunfire (per shot) | +0.1% |
| Grenade explosion | +0.5% |
| Time in zone (per minute) | +1% |

**Strategic Implications:**
- **Stealth** = slower but safer (low saturation)
- **Aggression** = faster loot but dangerous (high saturation)
- Must balance speed vs. safety

---

## Extraction Points & Multipliers

**Purpose:** Risk vs. reward decision - extract early (safe) or push for higher multiplier (dangerous)

### Extraction Point 1: The Shallows
**Multiplier:** 1× (baseline)  
**Location:** Beach, rowboat evacuation  
**Channel Time:** 15 seconds  
**Risk:** Low enemy presence  
**Rewards:** Safe but minimal

### Extraction Point 2: The Lighthouse
**Multiplier:** 2× (double rewards)  
**Location:** Stone lighthouse on promontory  
**Channel Time:** 30 seconds (must light beacon first)  
**Risk:** Beacon attracts enemy swarms  
**Rewards:** Medium risk, good rewards

**Mechanics:**
- Must activate beacon (30s vulnerable)
- Beacon visible to all enemies (swarm trigger)
- Elevated position (defensive advantage)

### Extraction Point 3: The Drowned Cathedral
**Multiplier:** 3× (triple rewards)  
**Location:** Partially sunken Gothic church  
**Channel Time:** 60 seconds (extraction ritual)  
**Risk:** Boss spawns, high saturation  
**Rewards:** Highest risk, best rewards + boss loot

**Mechanics:**
- Only accessible at low tide (last 5 minutes or specific cycle)
- Boss always spawns at high saturation
- Must defend extraction ritual (60s)
- Ritual stabilizes reality temporarily (visual feedback)

---

### Extraction Reward Formula

```
Total Scrip = (Loot Collected) × (Extraction Multiplier) × (Contract Bonus)
```

**Example:**
- Loot: 600 scrip
- Extraction: Lighthouse (2×)
- Contract: Tier 2 (1.5×)
- **Total: 600 × 2 × 1.5 = 1,800 scrip**

---

## Progression System

### Two Progression Tracks

#### Track 1: Weapons (Scrip Currency)
**How It Works:**
1. Extract with loot → Earn scrip
2. Spend scrip in hub → Unlock weapons
3. Weapons shared across ALL classes

**Unlock Costs:**
- Tier 1: Free (M1903, Double-Barrel, .38 Revolver)
- Tier 2: 800-1,200 scrip (Winchester, M1911, Lee-Enfield)
- Tier 3: 2,000-3,000 scrip (Thompson, BAR)
- Tier 4: 5,000+ scrip + boss kills (Elephant Gun, Blessed Rounds)

---

#### Track 2: Abilities (Class XP + Scrip)
**How It Works:**
1. Extract successfully → Earn class XP (per class played)
2. Level up → Unlock ability upgrade tiers
3. Spend scrip + XP → Purchase upgrades

**Leveling:**
- Level 5: Unlock Tier 2 ability upgrades
- Level 10: Unlock Tier 3 ability upgrades
- Level 15: Unlock cosmetic skin

**Ability Upgrade Costs:**
- Tier 2: 500 scrip per ability (need Level 5)
- Tier 3: 1,500 scrip per ability (need Level 10)

**XP Gain:**
- Shallows extraction: 100 XP
- Lighthouse extraction: 200 XP
- Cathedral extraction: 300 XP

**XP to Level:**
- Level 1→5: 200-500 XP per level
- Level 5→10: 600 XP per level
- Level 10+: 1,000 XP per level

**Time to Max:** ~30-40 successful runs per class (15-20 hours)

---

### Class System Rules

**Starting State:**
- All 4 classes unlocked from start (no unlock required)
- All classes start at Level 1

**Team Composition:**
- **No duplicate classes allowed** (each player must be different class)
- 2-4 players per team

**Class Switching:**
- Can switch classes freely between runs
- Class XP is tracked separately per class
- Weapons are shared (unlocked account-wide)

---

## Loot & Inventory

### Inventory System

**Capacity:** 10 slots total per player

**Slot Types:**
- Currency items (stackable)
- Artifacts (non-stackable, high value)
- Consumables (healing, ammo, equipment)

---

### Currency Items (Stackable)

| Item | Slots | Value | Notes |
|------|-------|-------|-------|
| Small Scrip Pouch | 1 | 50 scrip | Common |
| Large Scrip Satchel | 2 | 150 scrip | Uncommon |
| Scrip Strongbox | 3 | 400 scrip | Rare |

---

### Artifacts (High Value, Non-Stackable)

| Item | Slots | Value | Notes |
|------|-------|-------|-------|
| Cursed Tome | 1 | 200 scrip | +5% saturation while carried |
| Elder Stone | 2 | 500 scrip | No penalty |
| Ritual Components | 1 | 100 scrip | Crafting material |
| Boss Trophies | 3 | 1,000+ scrip | Unlocks special items |

---

### Consumables (Use in Mission)

| Item | Slots | Effect |
|------|-------|--------|
| Ammo Box | 1 | Restores 2 magazines to equipped weapon |
| First Aid Kit | 1 | Heals 75% HP |
| Flare | 1 | Lights area for 60 seconds |
| Grenade | 1 | AOE damage |

---

### Strategic Inventory Choices

**Full Inventory (10 slots):**
- Must choose: Carry more loot OR carry more ammo/healing
- Cannot pick up anything if all slots full (must drop/use items)

**Risk Decision:**
- Extract early with partial loot (safer)
- Stay longer for more loot (riskier, saturation increases)

---

## Death & Revival

### Down State

**When player reaches 0 HP:**
1. Enter "downed" state (10-second timer)
2. Can be revived by teammate (3-second interaction)
3. If not revived in 10 seconds → becomes "dead"

**While Downed:**
- Cannot move or use abilities
- Can use secondary weapon (pistol only) with -50% accuracy
- Screen grays out, audio muffled

---

### Dead State

**When down timer expires:**
- Player becomes "dead" (cannot be revived)
- Can respawn at extraction point (one-time per run)
- Respawn costs 10% of team's current scrip

**Respawn Conditions:**
- Must have at least one living teammate
- Extraction point must be unlocked/visible
- Cannot respawn during boss fights

---

### Party Wipe

**If all players down/dead simultaneously:**
- Run failed (party wipe)
- Lose all loot collected
- Keep 10% of scrip earned (mercy mechanic)

**Example:**
- Collected 1,000 scrip before wipe
- Keep 100 scrip
- Must extract successfully next run to earn full rewards

---

## Contract Modifiers

**What They Are:** Optional challenge modifiers that increase rewards

**How They Work:**
- Select one contract before deployment
- Contract adds challenge + reward multiplier
- Multiplier stacks with extraction point multiplier

---

### Contract Tiers

#### Tier 1 Contracts (Unlock: Start)
**Multiplier:** 1.2×

- **"Whispers of the Deep"** - Audio distorted, rely on visual
- **"Low Tide Protocol"** - Map always at low tide, exposed
- **"Isolation Dive"** - No teammate markers, stay together

#### Tier 2 Contracts (Unlock: 10 successful runs)
**Multiplier:** 1.5×

- **"The Colour Out of Space"** - Color entity hunts you
- **"Moonless Night"** - Extreme darkness, limited light
- **"Ritual Tide"** - Tide timing unpredictable

#### Tier 3 Contracts (Unlock: 25 successful runs)
**Multiplier:** 2.0×

- **"The Silver Key"** - Saturation increases per kill, decreases per artifact
- **"Dreams in the Witch House"** - Reality unstable from start
- **"Dead of Night"** - High saturation always, boss pre-killed, speed run

---

### Contract Reward Stacking

**Formula:**
```
Total = Loot × Extraction Multiplier × Contract Multiplier
```

**Example:**
- Loot: 600 scrip
- Lighthouse: 2×
- Tier 2 Contract: 1.5×
- **Total: 600 × 2 × 1.5 = 1,800 scrip**

---

## Tidal Mechanics

**What It Is:** Water level changes affect map accessibility

**Two States:**
- **High Tide** - Beach areas flooded, different paths available
- **Low Tide** - Beach areas accessible, more loot near shore

**Cycle Timing:**
- Switches every 10 minutes (visible timer on UI)
- Cathedral only accessible during low tide

---

### High Tide Effects
- Beach areas = kill volumes (instant death if enter)
- Different routes open (elevated paths)
- Water enemies more active (Deep Ones, Reef Walkers)

### Low Tide Effects
- Beach areas accessible
- Tide pool loot spawns
- Cathedral extraction unlocked
- Water enemies less active

---

### Strategic Implications
- Plan routes around tide cycle
- Cathedral requires timing (arrive during low tide)
- Some shortcuts only available at specific tides

---

## Reality Rifts

**What They Are:** Interactive objectives scattered across map

**Visual:** Glowing particle effect, reality distortion around tear

**Mechanics:**
- Approach rift
- Hold interact (5-second channel, vulnerable)
- Rift closes (VFX, sound effect)

**Rewards:**
- Reduces Breach Saturation by 10%
- Loot cache spawns after closing (random scrip/consumables)

**Risk vs. Reward:**
- Explore further to find rifts (risk)
- Close rifts to reduce saturation (reward)
- Strategic choice: Hunt rifts vs. rush extraction

---

## Session Timeline (30-Minute Run)

**Minutes 0-2: HUB**
- Select class
- Equip weapons
- Choose contract
- Ready up

**Minutes 2-3: INSERTION**
- Rowboat deployment
- Orient in zone
- Locate extraction points

**Minutes 3-10: EARLY GAME**
- Loot nearby areas
- Close rifts if found
- Saturation: 0-15%
- Decision: Push or extract at Shallows?

**Minutes 10-20: MID GAME**
- Navigate deeper zones
- Saturation: 15-40%
- Harder enemies spawn
- Decision: Push or extract at Lighthouse?

**Minutes 20-30: LATE GAME**
- High saturation (40-70%)
- Continuous enemy pressure
- Boss fight if Cathedral
- Decision: Extract or risk party wipe?

**Minutes 30+: META-PROGRESSION**
- Return to hub
- Spend scrip
- Upgrade abilities
- Next run

---

## System Interaction Examples

### Example 1: Saturation Management
```
Team at 50% saturation:
1. Occultist uses Purge Corruption (-20%) → 30%
2. Team closes 2 reality rifts (-10% each) → 10%
3. Team can now push deeper safely
```

### Example 2: Inventory Pressure
```
Player has 8/10 slots filled:
- 2 slots remaining
- Finds Scrip Strongbox (3 slots, 400 scrip)
- Must drop 1 slot worth of items to pick up
- Drops 1 Ammo Box (less valuable)
- Picks up Strongbox
```

### Example 3: Death Spiral Prevention
```
Team at Cathedral (75% saturation):
1. Bulwark goes down (10s timer)
2. Occultist draws Healing Circle (3s vulnerable)
3. Harpooner covers Occultist (shoots approaching enemies)
4. Circle completes, Occultist revives Bulwark (3s)
5. Bulwark back up, team continues
```

---

## Quick System Stats

**Breach Saturation:**
- Starts: 0%
- Increases: +1% per minute baseline
- Critical: 75%+ (boss spawns possible)
- Penalty: Extract above 90% = -25% earned scrip

**Extraction:**
- 3 points (1×, 2×, 3× multipliers)
- Channel times: 15s, 30s, 60s
- Can extract multiple times per run (team must all extract together)

**Progression:**
- 2 tracks (Weapons via scrip, Abilities via class XP + scrip)
- 8 weapons total
- 8 abilities (2 per class × 4 classes)

**Inventory:**
- 10 slots total
- Stackable currency items
- Non-stackable artifacts
- Consumables used in mission

**Death:**
- Down → 10s timer → Dead
- Revive: 3-second interaction
- Respawn: Once per run at extraction
- Wipe: Keep 10% scrip

---

## Related Documentation

**For detailed system implementation:**
- [Extraction Mechanics](../02-design-docs/extraction-mechanics.md) - Full Breach Saturation design
- [Progression System](../02-design-docs/progression-system.md) - Detailed progression design

**For exact parameters:**
- [Balance Parameters](../03-technical-specs/balance-parameters.md) - All numerical values

**For code examples:**
- [Architecture Overview](../04-development/architecture-overview.md) - System implementation patterns

---

**This completes the Quick Reference layer. You now have fast lookups for classes, weapons, enemies, and systems.**
