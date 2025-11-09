# TIDE'S END: Extraction Mechanics Design
## Breach Saturation & Risk/Reward Decisions

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Purpose:** Explain the core tension system

---

## Document Purpose

This document explains **how extraction mechanics create tension** without artificial timers or PvP. The Breach Saturation system is your core escalation mechanic.

**Related Documents:**
- Quick Reference: [Systems Quick Ref](../01-quick-reference/systems-quick-ref.md)
- Technical Specs: [Balance Parameters](../03-technical-specs/balance-parameters.md)
- Original Design: lovecraft-setting-redesign.md + conflict#4.md

---

## Core Design Philosophy

**"Greed kills. But cowardice gains nothing."**

### The Problem with Traditional Extraction Shooters

**Hunt: Showdown's tension:** PvP threat  
**Problem for PvE:** Can't use PvP, need different pressure

**Tarkov's tension:** Lose everything on death  
**Problem:** Too punishing, drives players away

**The Cycle's tension:** Storm timer  
**Problem:** Artificial, doesn't integrate with world

### Our Solution: Breach Saturation

**Core mechanic:**
- Reality degrades the longer you stay
- No hard timer (you won't instakill die)
- Escalates naturally through gameplay
- Creates organic "time to leave" pressure
- Reversible (but at cost)

**Why it works:**
- Integrated with setting (Lovecraftian reality breakdown)
- Player-controlled escalation (your actions matter)
- Creates decision points (stay for loot vs. extract safely)
- No instant failure (graceful degradation)

---

## Breach Saturation System Deep Dive

### What IS Breach Saturation?

**In-world explanation:**
The Breach is a wound in reality. Being near it weakens the fabric of space-time around you. The longer you're exposed, the more reality "saturates" with eldritch energy. Eventually, reality collapses entirely.

**Gameplay explanation:**
A percentage meter (0-100%) that increases over time and through player actions. Higher saturation = more dangerous enemies, worse conditions, better rewards.

**Key insight:** It's not a timer. It's a TIDE. It ebbs and flows based on your actions.

---

### How Saturation Increases

#### Passive Gain (Time)
- **+1% per minute** in breach zone
- Slow, constant pressure
- Can't avoid (just being there saturates you)

**Design reason:** Creates baseline urgency. You can't stay forever even if you do nothing.

#### Active Gain (Player Actions)

| Action | Saturation Gain | Reasoning |
|--------|----------------|-----------|
| Kill enemy | +2% | Combat disturbs reality |
| Explosion/grenade | +5% | Loud, violent disruption |
| Boss fight active | +3% per minute | Boss warps reality actively |
| Loot artifact | +1% | Artifacts are saturated objects |

**Design reason:** Aggressive play = faster escalation. Stealth/avoidance = slower escalation. Player chooses playstyle.

#### Special Modifiers

**Occultist Warded Passive:**
- Saturation gain -25% for that player
- Other players still gain normally
- Creates role: "Saturation tank"

**Bulwark Anchor Point:**
- Saturation gain paused inside zone
- Creates tactical value: "Safe room"

**Occultist Sanctuary Circle:**
- Saturation gain -50% inside zone
- Healing + saturation management

**Design reason:** Abilities can mitigate saturation, but not eliminate. Strategic, not broken.

---

### How Saturation Decreases

#### Occultist Purge Corruption
- **-20% saturation** (absolute reduction)
- 90-second cooldown
- 5-second vulnerable channel

**Trade-off:** High value, but risky (vulnerable) and limited (cooldown)

#### Close Reality Rift
- **-10% saturation**
- Optional world objective
- Requires team to defend while channeling

**Trade-off:** Risk engagement (enemies spawn) for saturation reduction

#### Successful Extraction
- **Reset to 0%**
- Only way to fully clear saturation
- Incentivizes extraction before critical levels

**Design reason:** Extraction is THE solution. Saturation management is temporary relief.

---

## Saturation Effects (The Escalation)

### Tier 1: 0-25% (STABLE)

**Visual:**
- Light fog (standard maritime fog)
- Normal colors
- Minimal effects

**Audio:**
- Ambient coastal sounds (waves, wind)
- Distant foghorns
- Creaking wood

**Enemies:**
- Spawn interval: 1 per 15 seconds
- Only Tier 1 enemies (Hollow-Eyed, Reef Walkers)
- Predictable patrol patterns

**Player Experience:**
- Feels safe (relatively)
- Can explore at leisure
- Time to loot thoroughly

**Design Intent:** Learning phase. Let players get comfortable.

---

### Tier 2: 25-50% (FRACTURING)

**Visual:**
- Thicker fog (reduced visibility)
- Slight color desaturation
- Particle effects (drifting spores/embers)
- Screen vignette (edges darken slightly)

**Audio:**
- Eerie ambience layer added
- Reverb on footsteps (space feels wrong)
- Distant whispers (audio hallucinations)
- Occasional fake footsteps (audio decoy, no enemy)

**Enemies:**
- Spawn interval: 1 per 10 seconds (1.5× rate)
- Tier 1 + Tier 2 enemies (Deep One packs)
- **AMBUSH SPAWNS ENABLED** (enemies can spawn in "cleared" areas)

**Mechanics:**
- Audio hallucinations (random enemy sounds, no enemy present)
- Makes players paranoid
- Forces awareness

**Player Experience:**
- Tension rising
- Can't trust cleared areas anymore
- Starting to consider extraction

**Design Intent:** Introduce chaos. Break player expectations.

---

### Tier 3: 50-75% (BREACHING)

**Visual:**
- Heavy fog (30m visibility max)
- Color grading shift (desaturated, blue-tinted)
- More aggressive particles
- Occasional screen shake

**Audio:**
- Dissonant music layer
- Muffle distant sounds (audio occlusion)
- Louder ambient noise

**Enemies:**
- Spawn interval: 1 per 7 seconds (2× rate)
- Tier 1 + 2 + 3 enemies (Shoggoths, Shamblers)
- Enemy movement speed +15%
- Coordinated AI behavior
- **HUNTER ENEMIES SPAWN** (Dimensional Shamblers actively pursue)
- **DIRECTOR AI ACTIVE** (spawns at chokepoints, flanking positions)

**Mechanics:**
- Director AI spawns enemies tactically
- Hunter enemies track player across map
- No safe areas (continuous pressure)

**Player Experience:**
- High stress
- "We need to leave NOW"
- Every fight dangerous

**Design Intent:** Critical threshold. Staying longer is very risky.

---

### Tier 4: 75-100% (COLLAPSE)

**Visual:**
- Overwhelming fog (20m visibility)
- Heavy color grading (purple/green tint)
- Screen shake events (periodic)
- Chromatic aberration (edges of vision)
- Bloom effect (lights glare)

**Audio:**
- Loud ambient noise peaks
- Audio distortion filter (all sounds warped)
- Impossible harmonics

**Enemies:**
- Spawn interval: 1 per 5 seconds (3× rate)
- All enemy types including bosses
- Enemy damage +25%
- Enemy movement +25%
- Continuous spawn pressure

**Environmental Hazards:**
- Pre-placed hazards activate
  - Toxic gas vents (5 DPS)
  - Electrical arcs (20 damage)
  - Collapsing roof sections (trigger volumes)

**Mechanics:**
- **PENALTY:** Extract above 90% = lose 25% of earned scrip
- Reality is collapsing, team is reckless

**Player Experience:**
- Panic
- "HOW DO WE GET OUT?!"
- Fighting retreat

**Design Intent:** Punishment for greed. Escape barely possible.

---

## Saturation Decision Points

### The Three Questions

**Question 1 (25%):** "Do we push deeper or extract early?"
- Early extraction (Shallows, 1× multiplier) = safe
- Push forward = more loot, higher risk

**Question 2 (50%):** "Do we fight the boss or skip?"
- Boss guarantees 500-1200 scrip but raises saturation significantly
- Skip = safer, lower reward

**Question 3 (75%):** "Do we extract immediately or risk one more objective?"
- Greed for final artifact
- Risk total wipe

### Example Decision Flows

**Conservative Team:**
```
Start → 15% → Loot conservatively → 30% → Extract (Lighthouse, 2×)
Result: 800 scrip × 2 = 1,600 scrip, safe
```

**Balanced Team:**
```
Start → 20% → Kill enemies → 45% → Close rift (-10%) → 35% → Extract (Lighthouse, 2×)
Result: 1,200 scrip × 2 = 2,400 scrip, moderate risk
```

**Greedy Team:**
```
Start → 25% → Kill aggressively → 60% → Boss fight → 85% → Extract (Cathedral, 3×)
Result: 2,000 scrip × 3 = 6,000 scrip, high risk (barely survived)
```

**Greedy Team (Failure):**
```
Start → 30% → Boss → 70% → "One more artifact!" → 92% → Team wipe
Result: 0 scrip, lose 10% of run earnings
```

---

## Extraction Points (Risk vs. Reward)

### Extraction Point 1: The Shallows

**Location:** Beach, outside main breach zone  
**Multiplier:** 1.0× (no bonus)  
**Channel Time:** 15 seconds (rowboat pickup)  
**Risk Level:** LOW

**Advantages:**
- Always accessible
- Low enemy presence (outside core)
- Quick escape
- Safe harbor

**Disadvantages:**
- No reward multiplier (earn base scrip only)
- Feels "cowardly" (no bonus for risk)

**Best For:**
- First-time players learning
- Runs gone wrong (emergency escape)
- High saturation panic

---

### Extraction Point 2: The Lighthouse

**Location:** Elevated position, central map  
**Multiplier:** 2.0× (double rewards)  
**Channel Time:** 30 seconds (must light beacon)  
**Risk Level:** MEDIUM

**Advantages:**
- Good multiplier (2×)
- Defensible position (elevated, chokepoints)
- Still reasonable risk

**Disadvantages:**
- Must activate beacon (30-second channel, vulnerable)
- Beacon attracts enemies when lit (swarm incoming)
- Must defend beacon for full duration

**Mechanics:**
- Light beacon (interact, 30s channel)
- Beacon becomes visible across map
- All enemies alerted and converge
- Team must defend position
- After 30s, rowboat visible from catwalk

**Best For:**
- Standard extractions
- Confident teams
- Balanced risk/reward

---

### Extraction Point 3: The Drowned Cathedral

**Location:** Boss arena, deepest corruption  
**Multiplier:** 3.0× (triple rewards)  
**Channel Time:** 60 seconds (ritual channel)  
**Risk Level:** HIGH

**Advantages:**
- Highest multiplier (3×)
- Massive scrip gains
- Best loot location

**Disadvantages:**
- Boss spawns here at 50%+ saturation
- Must complete boss fight OR survive during extraction
- Longest channel time (60 seconds)
- Deepest in breach (highest saturation location)
- No easy escape if things go wrong

**Mechanics:**
- Enter cathedral (boss may spawn)
- If boss alive: Must kill OR survive while extracting
- Begin ritual extraction (60s channel)
- Continuous enemy waves during channel
- Extraction complete: Bright light, rowboat at cathedral dock

**Best For:**
- Experienced teams
- Final runs (all-in for maximum scrip)
- When already committed to boss fight

---

## Why This System Creates Tension

### 1. **No Artificial Timer**

**Bad tension (artificial):**
- "Storm incoming in 5 minutes"
- Feels game-y
- Doesn't integrate with world

**Good tension (organic):**
- "Saturation at 65%, we should think about leaving"
- Feels natural
- Integrated with Lovecraftian horror

### 2. **Player-Driven Escalation**

**Stealth player:**
- Avoids combat
- Stays at 20-30% saturation
- Lower rewards but safer

**Aggressive player:**
- Kills everything
- Hits 60-70% saturation
- Higher rewards but dangerous

**Both playstyles viable.** Tension comes from your choices.

### 3. **Reversible But Costly**

**Can reduce saturation:**
- Occultist Purge (-20%)
- Close rift (-10%)

**But:**
- Purge has 90s cooldown (limited use)
- Rifts require combat (risk)

**Result:** You can manage saturation, but not trivially. Requires teamwork and sacrifice.

### 4. **Graceful Degradation**

**Not:**
- âŒ "Timer hits zero, instant death"
- âŒ "One mistake = wipe"

**Instead:**
- âœ… Saturation at 80% is dangerous but survivable
- âœ… Can still extract at 95% (just lose some scrip)
- âœ… Escalation is gradual

**Result:** Tension without frustration. Always a chance to escape.

### 5. **Memorable "Close Calls"**

**The stories players tell:**
> "We were at 88% saturation, one guy down, Shamblers everywhere. Occultist purged to 68%, Bulwark anchored the beach, Lamplighter speed-boosted us to the boat. We made it with 12 HP between all of us. Best extraction ever."

**That's the goal.** Create moments worth retelling.

---

## Saturation vs. Other Tension Systems

### vs. Hunt: Showdown's PvP

| Hunt (PvP) | Tide's End (Saturation) |
|------------|-------------------------|
| Tension from enemy players | Tension from escalating environment |
| Can't control (players unpredictable) | Can partially control (abilities, playstyle) |
| Binary outcome (win/lose PvP) | Gradual escalation (many decisions) |
| Requires competitive mindset | Requires risk assessment |

**Result:** Similar tension, different source.

### vs. Tarkov's Permadeath

| Tarkov (Permadeath) | Tide's End (Saturation) |
|---------------------|-------------------------|
| Lose ALL gear on death | Keep class progression, lose run rewards |
| Extremely punishing | Punishing but fair |
| Discourages risk | Encourages calculated risk |
| Anxiety-inducing | Tension-building |

**Result:** Tension without toxicity.

### vs. The Cycle's Storm

| The Cycle (Storm) | Tide's End (Saturation) |
|-------------------|-------------------------|
| Fixed timer | Dynamic, player-influenced |
| Artificial deadline | Organic escalation |
| Everyone extracts at same time | Teams extract when ready |
| Feels game-y | Feels integrated |

**Result:** Immersive tension, not artificial pressure.

---

## Saturation Anti-Patterns (What NOT To Do)

### Anti-Pattern 1: Make It Too Fast
**Problem:** If saturation increases too quickly, players have no time to explore  
**Solution:** 1% per minute baseline is slow. Players control escalation through combat.

### Anti-Pattern 2: Make It Reversible Easily
**Problem:** If Occultist can spam Purge, no tension  
**Solution:** 90-second cooldown, 5-second vulnerable channel. Purge is powerful but limited.

### Anti-Pattern 3: Make High Saturation Unwinnable
**Problem:** If 80%+ is instant death, players never take risks  
**Solution:** 80% is HARD, not impossible. Extract above 90% = lose 25% scrip (penalty, not death).

### Anti-Pattern 4: Make It Invisible
**Problem:** If players don't know saturation level, they can't make informed decisions  
**Solution:** Large UI element, clear visual/audio cues per tier.

### Anti-Pattern 5: Make Extraction Too Easy
**Problem:** If extraction has no risk, no final tension  
**Solution:** Higher multiplier extractions require defense (Lighthouse beacon) or boss fight (Cathedral).

---

## Advanced Saturation Strategies

### Strategy 1: "Saturation Dance"

**Concept:** Manage saturation actively, hover at 50-60%  
**Execution:**
1. Kill enemies until 55%
2. Occultist Purge to 35%
3. Kill more enemies to 55%
4. Close rift to 45%
5. Extract at Lighthouse (2×)

**Result:** Maximum loot time, controlled risk

---

### Strategy 2: "Blitz Run"

**Concept:** Ignore saturation, sprint to Cathedral, extract fast  
**Execution:**
1. Sprint past enemies (minimal combat)
2. Reach Cathedral at 30-40% saturation
3. Extract immediately (3× multiplier)

**Result:** Fast runs, lower absolute scrip but high per-minute earnings

---

### Strategy 3: "Boss Hunter"

**Concept:** Rush to 75% saturation to force boss spawn  
**Execution:**
1. Aggressive combat to 75%+
2. Boss spawns (guaranteed at this saturation)
3. Kill boss (500-1200 scrip)
4. Extract at Cathedral (3× multiplier)

**Result:** Highest possible scrip, highest risk

---

### Strategy 4: "Occultist Carry"

**Concept:** Occultist's abilities enable extended runs  
**Execution:**
1. Occultist Purges at 50%, 70%, 90%
2. Sanctuary Circle for sustain
3. Team can push to 80-90% safely
4. Extract at Cathedral

**Result:** Occultist is MVP, team relies on support

---

## Saturation in Contract Modifiers

**Contracts can modify saturation:**

**"The Silver Key" Contract:**
- Saturation increases per kill (+4% instead of +2%)
- Saturation decreases per artifact collected (-5%)
- **Forces decision:** Fight or loot, not both

**"Moonless Night" Contract:**
- Start at 50% saturation (immediate danger)
- Faster escalation (+2% per minute)
- **Reward:** 1.5× contract multiplier

**"Dead of Night" Contract:**
- Start at 75% saturation
- Boss pre-killed (loot available, no fight)
- **Goal:** Speed-run extraction, time-attack leaderboard

---

## Tutorializing Saturation

**How to teach players this complex system:**

### Tutorial Run (AI-Narrated)

**Dr. Vance (radio):**
> "Saturation at 15%. That's stable. You're safe for now."

[Player kills enemies, saturation hits 35%]

**Dr. Vance:**
> "Saturation rising. Combat disturbs the breach. You're approaching fracture threshold."

[Saturation hits 50%, ambush spawns behind player]

**Dr. Vance:**
> "Breach fracturing! Enemies can spawn anywhere now. Stay alert."

[Saturation hits 70%]

**Dr. Vance:**
> "CRITICAL SATURATION. Extract immediately or you'll lose everything."

[Player extracts at Shallows]

**Dr. Vance:**
> "Extraction complete. Saturation reset. Remember: greed kills."

### UI Teaching

**Saturation meter:**
- Color-coded (green/yellow/orange/red)
- Threshold lines visible (25%, 50%, 75%)
- Tooltip on hover: "Reality stability. Higher = more dangerous"

**First time hitting each threshold:**
- Screen message: "BREACH FRACTURING - Reality weakening"
- Audio cue: Distinct sound per threshold
- Visual effect: Immediate change (fog thickens, etc.)

---

## Success Metrics

**Saturation system succeeds if:**
- âœ… Players regularly extract at 40-60% (balanced risk/reward)
- âœ… Some players push to 80%+ (high-skill risk-takers)
- âœ… "Close call" extractions generate excitement
- âœ… Players discuss saturation management strategies
- âœ… Occultist is valued for Purge ability

**Saturation system fails if:**
- âŒ All players extract at <30% (too punishing, no risk taken)
- âŒ All players extract at >80% (not punishing enough, no consequences)
- âŒ Players ignore saturation entirely (poor telegraphing)
- âŒ Saturation feels arbitrary (not integrated with world)

---

## Related Documentation

**For quick reference:**
- [Systems Quick Ref](../01-quick-reference/systems-quick-ref.md)

**For implementation:**
- [Balance Parameters](../03-technical-specs/balance-parameters.md)

**For other systems:**
- [Combat System Design](combat-system-design.md)
- [Progression System](progression-system.md)

---

**Breach Saturation is your core tension mechanic. Master it, and players will always feel the pressure to extract - but never feel forced.**
