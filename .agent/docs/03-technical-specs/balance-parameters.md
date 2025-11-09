# TIDE'S END: Balance Parameters Reference
## Consolidated Numerical Values for Tuning

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Purpose:** Single source of truth for all balance numbers

---

## Document Purpose

This document consolidates **ALL numerical balance values** in one place for easy adjustment. If you need to tune difficulty, progression, or game feel, start here.

**Related Documents:**
- [Class Specifications](class-specifications.md)
- [Weapon Specifications](weapon-specifications.md)
- [Enemy Specifications](enemy-specifications.md)

---

## CLASS BASE STATS

| Class | Health | Move Speed | Sprint Mult | Crouch Mult |
|-------|--------|------------|-------------|-------------|
| Bulwark | 130 | 5.0 | 1.4 | 0.5 |
| Lamplighter | 100 | 6.0 | 1.5 | 0.6 |
| Harpooner | 100 | 5.5 | 1.45 | 0.55 |
| Occultist | 100 | 5.2 | 1.4 | 0.55 |

**Notes:**
- Base player health: 100 HP (Bulwark exception: 130 HP)
- Sprint multiplier applies to base move speed
- Crouch multiplier reduces move speed

---

## CLASS ABILITY COOLDOWNS

| Class | Ability 1 | Ability 2 |
|-------|-----------|-----------|
| Bulwark | Anchor Point (60s) | Bastion Shield (90s) |
| Lamplighter | Flare Shot (45s) | Pathfinder's Mark (60s) |
| Harpooner | Whaling Harpoon (50s) | Reel & Frenzy (90s) |
| Occultist | Sanctuary Circle (75s) | Purge Corruption (90s) |

**Balance Philosophy:**
- Utility abilities: 45-60s (frequent use)
- Control abilities: 60-75s (moderate use)
- High-impact abilities: 90-120s (infrequent use)

---

## CLASS ABILITY PARAMETERS

### Bulwark

| Ability | Parameter | Value |
|---------|-----------|-------|
| Steadfast | Max HP Bonus | +30 |
| Steadfast | Damage Resist | +15% below 50% HP |
| Steadfast | Knockback Reduction | -50% |
| Anchor Point | Cooldown | 60s |
| Anchor Point | Cast Time | 0.8s |
| Anchor Point | Radius | 10m |
| Anchor Point | Duration | 20s |
| Anchor Point | Enemy Slow | 30% |
| Anchor Point | Device HP | 200 |
| Bastion Shield | Cooldown | 90s |
| Bastion Shield | Deploy Time | 3.0s |
| Bastion Shield | Shield HP | 500 |
| Bastion Shield | Duration | 30s |
| Bastion Shield | Block Arc | 120° |

### Lamplighter

| Ability | Parameter | Value |
|---------|-----------|-------|
| Pathfinder | Move Speed Bonus | +20% |
| Pathfinder | Fog Vision Bonus | +30% |
| Pathfinder | Footstep Volume | -40% |
| Flare Shot | Cooldown | 45s |
| Flare Shot | Cast Time | 0.5s |
| Flare Shot | Radius | 20m |
| Flare Shot | Duration | 30s |
| Pathfinder's Mark | Cooldown | 60s |
| Pathfinder's Mark | Cast Time | 1.0s |
| Pathfinder's Mark | Radius | 15m |
| Pathfinder's Mark | Duration | 20s |
| Pathfinder's Mark | Speed Bonus | +40% |

### Harpooner

| Ability | Parameter | Value |
|---------|-----------|-------|
| Maritime Predator | Weak Point Damage | +20% |
| Maritime Predator | Reload Speed | +15% |
| Maritime Predator | Ammo Capacity | +2 |
| Whaling Harpoon | Cooldown | 50s |
| Whaling Harpoon | Cast Time | 1.2s |
| Whaling Harpoon | Damage | 60 |
| Whaling Harpoon | Max Range | 30m |
| Whaling Harpoon | Pull Force | 20.0 |
| Whaling Harpoon | Pull Duration | 2.0s |
| Reel & Frenzy | Cooldown | 90s |
| Reel & Frenzy | Duration | 10s |
| Reel & Frenzy | Fire Rate Bonus | +30% |
| Reel & Frenzy | Reload Speed Bonus | +50% |
| Reel & Frenzy | Move Penalty | -15% |
| Reel & Frenzy | Damage Bonus | +15% |

### Occultist

| Ability | Parameter | Value |
|---------|-----------|-------|
| Warded | Saturation Slowdown | -25% |
| Warded | Healing Capacity | +1 item |
| Warded | Healing Effectiveness | +25% on others |
| Sanctuary Circle | Cooldown | 75s |
| Sanctuary Circle | Cast Time | 3.0s |
| Sanctuary Circle | Radius | 8m |
| Sanctuary Circle | Duration | 25s |
| Sanctuary Circle | Heal Per Second | 3% HP |
| Sanctuary Circle | Saturation Reduction | -50% |
| Sanctuary Circle | Damage Resist | +10% |
| Purge Corruption | Cooldown | 90s |
| Purge Corruption | Cast Time | 5.0s |
| Purge Corruption | Saturation Reduction | -20% |
| Purge Corruption | Shield Amount | 50 HP |
| Purge Corruption | Shield Duration | 30s |
| Purge Corruption | Radius | 15m |

---

## WEAPON DAMAGE VALUES

| Weapon | Body | Head | Limb | Fire Rate (RPM) | Magazine | Effective Range |
|--------|------|------|------|-----------------|----------|-----------------|
| M1903 Springfield | 85 | 170 | 64 | 15 | 5 | 50m |
| Double-Barrel | 144* | 216* | 108* | 120 | 2 | 8m |
| .38 Revolver | 35 | 70 | 26 | 180 | 6 | 15m |
| Winchester 1897 | 128* | 192* | 96* | 100 | 5 | 10m |
| M1911 Pistol | 40 | 80 | 30 | 240 | 7 | 20m |
| Lee-Enfield | 75 | 150 | 56 | 30 | 10 | 55m |
| Thompson SMG | 22 | 44 | 17 | 700 | 30 | 15m |
| BAR | 50 | 100 | 38 | 500 | 20 | 35m |
| Elephant Gun | 200 | 400 | 150 | 8 | 2 | 60m |

*Shotgun damage = max damage (all pellets hit)

**Multipliers:**
- Headshot: 2.0×
- Limbshot: 0.75×
- Blessed Rounds vs Eldritch: 1.5×
- Blessed Rounds vs Corrupted: 0.75×

---

## WEAPON RELOAD TIMES

| Weapon | Normal Reload | Empty Reload | Moving Reload |
|--------|---------------|--------------|---------------|
| M1903 Springfield | 3.5s | 4.4s | 5.3s |
| Double-Barrel | 2.8s | 2.8s | 3.6s |
| .38 Revolver | 2.2s | 2.5s | 2.9s |
| Winchester 1897 | 0.8s/shell | 0.8s/shell | 1.0s/shell |
| M1911 Pistol | 2.0s | 2.4s | 2.5s |
| Lee-Enfield | 3.0s | 3.6s | 4.2s |
| Thompson SMG | 2.5s | 2.9s | 3.3s |
| BAR | 3.0s | 3.6s | 4.5s |
| Elephant Gun | 4.5s | 4.5s | 9.0s |

**Modifiers:**
- Empty reload: × 1.25 (except Double-Barrel, Elephant Gun)
- Moving reload: × 1.5 normal reload
- Harpooner passive: All reloads × 0.85

---

## WEAPON UNLOCK COSTS

| Tier | Weapons | Cost (Scrip) | Requirements |
|------|---------|--------------|--------------|
| 1 | M1903, Double-Barrel, .38 | Free | Starting weapons |
| 2 | Winchester, M1911, Lee-Enfield | 800-1,200 | Currency only |
| 3 | Thompson, BAR | 2,000-3,000 | Currency only |
| 4 | Elephant Gun | 5,000 | 3 boss kills + currency |
| Special | Blessed Rounds | 5,000 | Level 15 any class |

---

## ENEMY STATS

### Basic Enemies

| Enemy | HP | Move Speed | Damage | Attack Cooldown | Detection Range |
|-------|-------|------------|--------|-----------------|-----------------|
| Hollow-Eyed | 100 | 4.0 / 5.5 | 15 | 1.5s | 15m |
| Reef Walker | 150 | 3.5 | 25 | 3.0s | 25m |
| Deep One | 120 | 5.5 / 7.0 | 20 | 1.2s | 20m |
| Tide Touched | 200 | 6.0 | 30 (×3) | 2.0s | 18m |
| Shoggoth (Mini) | 300 | 3.0 | 10/sec | Continuous | 30m |
| Dimensional Shambler | 200 | 7.5 | 40 | 5.0s | 40m |

**Notes:**
- Move Speed format: Standard / Combat (if different)
- Deep Ones spawn in packs of 3-5
- Shoggoths split at 50% HP
- Dimensional Shamblers teleport every 8s

### Boss Enemies

| Boss | HP | Phases | Special Mechanics |
|------|-------|--------|-------------------|
| The Drowned Priest | 2,000 | 3 | Summons adds, corruption zones, ritual pulse |
| The Reef Leviathan | 2,500 | 2 | Tentacle sweep, whirlpool, submerge |
| The Color | 1,500 | 4 | Possession, color drain, reality distortion |

---

## ENEMY LOOT DROP RATES

| Enemy | Drop Chance | Scrip (Min-Max) | Ammo Chance |
|-------|-------------|-----------------|-------------|
| Hollow-Eyed | 10% | 5-10 | 5% |
| Reef Walker | 15% | 10-20 | 10% |
| Deep One | 20% | 15-25 | 10% |
| Tide Touched | 25% | 20-35 | 15% |
| Shoggoth | 30% | 30-50 | 20% |
| Shambler | 35% | 35-60 | 25% |
| Drowned Priest | 100% | 500-800 | N/A |
| Reef Leviathan | 100% | 600-1000 | N/A |
| The Color | 100% | 700-1200 | N/A |

**Boss Loot:**
- Always drops boss token (required for progression)
- Always drops unique trophy
- Guaranteed 500-1200 scrip
- 10-25% chance for rare special drops

---

## BREACH SATURATION EFFECTS

### Visual & Audio Changes

| Saturation % | Fog Density | Color Grading | Audio | VFX |
|--------------|-------------|---------------|-------|-----|
| 0-25% | Light | Normal | Ambient coastal | None |
| 25-50% | Medium | Slight desaturation | Eerie ambience | Drifting particles |
| 50-75% | Heavy | Blue-tinted | Dissonant music | Aggressive particles |
| 75-100% | Very Heavy | Purple/green tint | Loud ambient peaks | Screen shake, chromatic aberration |

### Gameplay Effects

| Saturation % | Spawn Interval | Enemy Behavior | Special |
|--------------|----------------|----------------|---------|
| 0-25% | 1 per 15s | Predictable patrols | None |
| 25-50% | 1 per 10s | Ambush spawns enabled | Audio hallucinations |
| 50-75% | 1 per 7s | Director AI, +15% speed | Hunter enemies spawn |
| 75-100% | 1 per 5s | +25% damage/speed | Boss enemies can spawn |

### Saturation Gain Rates

| Action | Saturation Gain |
|--------|-----------------|
| Time in zone | +1% per minute |
| Kill enemy | +2% |
| Loud action (explosion) | +5% |
| Boss fight active | +3% per minute |

### Saturation Reduction

| Method | Saturation Reduction |
|--------|---------------------|
| Close reality rift | -10% |
| Occultist Purge Corruption | -20% |
| Extract successfully | Reset to 0% |

**Special:**
- Occultist Warded passive: Saturation gain -25% for that player
- Bulwark Anchor Point: Saturation gain paused inside zone
- Occultist Sanctuary Circle: Saturation gain -50% inside zone

---

## PROGRESSION VALUES

### Scrip Economy

| Activity | Scrip Earned |
|----------|--------------|
| Enemy kills | 5-60 (varies by enemy) |
| Loot caches | 50-400 |
| Close rift | 100 |
| Complete contract | 50-200 bonus |
| Boss kill | 500-1200 |

**Extraction Multipliers:**
- The Shallows: 1.0×
- The Lighthouse: 2.0×
- The Drowned Cathedral: 3.0×

**Contract Modifiers:**
- Tier 1 Contracts: 1.2× multiplier
- Tier 2 Contracts: 1.5× multiplier
- Tier 3 Contracts: 2.0× multiplier

**Total Scrip Formula:**
```
Total = (Collected Scrip + Contract Bonus) × Extraction Multiplier × Contract Modifier
```

**Example:**
```
600 scrip collected
+100 contract bonus
= 700 scrip
× 2.0 (Lighthouse extraction)
= 1,400 scrip
× 1.5 (Tier 2 contract)
= 2,100 scrip total earned
```

### Class Leveling

| Level | XP Required | Unlocks |
|-------|-------------|---------|
| 1 | 0 | Base class |
| 2 | 500 | - |
| 3 | 1,000 | - |
| 4 | 1,500 | - |
| 5 | 2,000 | Tier 2 ability upgrades |
| 6 | 2,500 | - |
| 7 | 3,000 | - |
| 8 | 3,500 | - |
| 9 | 4,000 | - |
| 10 | 5,000 | Tier 3 ability upgrades |
| 11-14 | +1,000 each | - |
| 15 | 10,000 | Class mastery cosmetic |

**XP Gain:**
- Successful extraction: 200 XP
- Boss kill: 500 XP
- Complete contract: 100 XP bonus
- Failed run (team wipe): 20 XP

### Ability Upgrade Costs

| Tier | Unlock Level | Scrip Cost |
|------|--------------|------------|
| Tier 1 | Level 1 | Free |
| Tier 2 | Level 5 | 500 |
| Tier 3 | Level 10 | 1,500 |

**Note:** Must pay scrip cost AND reach level requirement to unlock.

---

## PLAYER STATS & MODIFIERS

### Base Player Stats

| Stat | Value |
|------|-------|
| Base Health | 100 HP |
| Base Move Speed | 5.5 m/s |
| Sprint Multiplier | 1.4× |
| Crouch Multiplier | 0.5× |
| Jump Height | 2.0 m |
| Interaction Range | 3.0 m |
| Inventory Slots | 10 |

### Accuracy Modifiers

| State | Accuracy Modifier |
|-------|-------------------|
| Standing still | 100% (no penalty) |
| Moving | -40% accuracy |
| Jumping | -75% accuracy |
| Crouching | +10% accuracy |
| Aiming down sights | +20% accuracy |

### Damage Taken Modifiers

| Source | Modifier |
|--------|----------|
| Headshot from enemy | 1.5× damage |
| Fall damage (>5m) | 20 damage per meter |
| Corruption ground | 5 DPS |
| Drowning | 10 DPS |
| Fire/hazards | 15 DPS |

---

## GAME ECONOMY TARGETS

### Early Game (Runs 1-10)

**Typical Run:**
- Duration: 15-20 minutes
- Scrip earned: 400-800
- Deaths: Common
- Weapon tier: 1-2

**Progression:**
- Unlock Tier 2 weapons (800-1,200 scrip)
- Reach class level 5 (unlock Tier 2 upgrades)
- Start attempting Tier 2 contracts

### Mid Game (Runs 11-30)

**Typical Run:**
- Duration: 20-25 minutes
- Scrip earned: 1,000-2,000
- Deaths: Occasional
- Weapon tier: 2-3

**Progression:**
- Unlock Tier 3 weapons (2,000-3,000 scrip)
- Reach class level 10 (unlock Tier 3 upgrades)
- Start boss fights
- Attempt Tier 3 contracts

### Late Game (Runs 31+)

**Typical Run:**
- Duration: 25-30 minutes
- Scrip earned: 2,000-4,000
- Deaths: Rare
- Weapon tier: 3-4

**Progression:**
- Unlock Elephant Gun (5,000 scrip + 3 boss kills)
- Max out multiple classes (level 15)
- Master Tier 3 contracts
- Optimize builds

---

## DIFFICULTY TUNING LEVERS

### If Game Too Easy:

1. **Increase enemy HP:** +10-20%
2. **Increase spawn rate:** Reduce intervals by 2 seconds each tier
3. **Reduce player damage:** -10-15%
4. **Reduce ammo drops:** -50% drop chance
5. **Increase Breach Saturation gain:** +50% gain rate

### If Game Too Hard:

1. **Decrease enemy damage:** -10-20%
2. **Increase player HP:** +20 base HP
3. **Increase ammo capacity:** +1-2 magazines starting
4. **Reduce spawn rate:** Increase intervals by 2-3 seconds
5. **Add more loot caches:** +30% spawn rate

### If Progression Too Slow:

1. **Increase scrip drops:** +50% from all sources
2. **Reduce unlock costs:** -25% for Tier 2-3 weapons
3. **Increase XP gain:** +50% per activity
4. **Add bonus scrip events:** Random 2× scrip zones

### If Progression Too Fast:

1. **Reduce scrip multipliers:** Extraction points 0.8×, 1.5×, 2.5×
2. **Increase unlock costs:** +25-50% for Tier 3-4
3. **Reduce XP gain:** -25% per activity
4. **Add scrip sinks:** Cosmetics, consumables

---

## TESTING REFERENCE VALUES

### Target Times to Kill (TTK)

**Player vs. Enemy (with Tier 1 weapons):**
- Hollow-Eyed: 2-3 seconds (1-2 shots M1903)
- Reef Walker: 3-4 seconds (2-3 shots M1903)
- Deep One: 3-4 seconds (2-3 shots)
- Elite enemies: 8-12 seconds (4-6 shots + abilities)
- Bosses: 2-3 minutes (team effort)

**Enemy vs. Player:**
- Hollow-Eyed: 7 hits to down (105 seconds if unopposed)
- Reef Walker: 4 hits to down (12 seconds)
- Deep One: 5 hits to down (6 seconds)
- Elite enemies: 2-3 hits to down (4-6 seconds)

### Target Session Metrics

| Metric | Target Value |
|--------|--------------|
| Average run duration | 25 minutes |
| Player deaths per run | 0-1 (experienced players) |
| Enemies killed per run | 30-50 |
| Scrip earned per run | 1,000-2,000 (mid-game) |
| Ammunition remaining | 10-20% (forces conservation) |
| Breach Saturation at extraction | 40-60% (optimal) |

---

## QUICK TUNING GUIDE

**Use this table to quickly adjust balance:**

| Problem | Solution | Parameter to Change |
|---------|----------|---------------------|
| Players die too often | Increase player HP or reduce enemy damage | Player base HP, enemy damage values |
| Combat feels too slow | Increase weapon damage or reduce enemy HP | Weapon damage, enemy HP |
| Runs too short | Reduce breach saturation gain rate | Saturation gain per action |
| Too much ammo | Reduce starting ammo, reduce drop rates | Starting magazines, loot drop % |
| Abilities not impactful | Reduce cooldowns, increase effect strength | Ability cooldowns, ability parameters |
| Bosses too easy | Increase boss HP, reduce weak point damage | Boss stats, weak point multipliers |
| Progression too grindy | Increase scrip rewards, reduce costs | Scrip drops, unlock costs |
| Enemies too predictable | Increase ambush spawn chance, reduce intervals | Spawn director AI weights |

---

## VERSION HISTORY

### v1.0 (Current)
- Initial balance values based on design documents
- All parameters set for prototype testing

**Next Steps:**
- Playtest and gather data
- Adjust values based on metrics
- Document changes in future versions

---

## Related Documentation

**For implementation:**
- [Class Specifications](class-specifications.md)
- [Weapon Specifications](weapon-specifications.md)
- [Enemy Specifications](enemy-specifications.md)

**For design context:**
- [Combat System Design](../02-design-docs/combat-system-design.md)
- [Game Overview](../01-quick-reference/game-overview.md)

---

**This document is the single source of truth for all numerical balance values. When tuning, update here first, then propagate to other documents.**
