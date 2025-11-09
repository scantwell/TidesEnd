# TIDE'S END: Roguelite Contracts Design
## Challenge Modifiers & Replayability

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Purpose:** Provide endless variety through contract modifiers

---

## Document Purpose

This document explains the **contract system** - optional challenge modifiers that increase rewards while changing gameplay. Contracts are your primary tool for long-term replayability and skill expression.

**Related Documents:**
- Quick Reference: [Systems Quick Ref](../01-quick-reference/systems-quick-ref.md)
- Technical Specs: [Balance Parameters](../03-technical-specs/balance-parameters.md)

---

## Core Contract Philosophy

**"More challenge = more reward. You choose the difficulty."**

### Why Contracts Matter

**Problem:** Base game gets repetitive after 30-50 runs  
**Solution:** Contracts add variety without requiring new content

**Benefits:**
- Infinite variety (combine multiple modifiers)
- Self-balancing difficulty (players choose challenge level)
- Skill expression (contracts reward mastery)
- Leaderboards (compete on specific contracts)

---

## Contract System Structure

### Three Tiers

**Tier 1 Contracts:**
- Available from start
- Moderate difficulty increase
- 1.2× scrip multiplier
- Example: "Audio cues reversed"

**Tier 2 Contracts:**
- Unlock after 10 successful extractions
- Significant difficulty increase
- 1.5× scrip multiplier
- Example: "Start at 50% saturation"

**Tier 3 Contracts:**
- Unlock after 25 successful extractions OR specific achievement
- Extreme difficulty increase
- 2.0× scrip multiplier
- Example: "Reality unstable from start"

### Contract Stacking

**Players can select 1-3 contracts per run:**
- 1 contract: Base multiplier
- 2 contracts: Multipliers add (+0.2 bonus)
- 3 contracts: Multipliers add (+0.4 bonus)

**Example:**
- "Moonless Night" (1.5×) + "Low Tide Protocol" (1.2×) = 2.7× + 0.2 stacking bonus = **2.9× total**

**Max multiplier:** 6.0× (3 Tier 3 contracts + stacking bonuses)

---

## TIER 1 CONTRACTS (Available from Start)

### Contract: "Whispers of the Deep"

**Modifier:** Audio cues reversed/distorted  
**Challenge:** Must rely on visual identification, not sound  
**Multiplier:** 1.2×

**Specifics:**
- Enemy footsteps sound like they're behind you (even if in front)
- Gunshots echo incorrectly (wrong direction)
- Ability sounds delayed by 1 second
- Ambient audio normal (waves, wind, etc.)

**Why it's interesting:**
- Forces visual awareness
- Disorienting but learnable
- Lamplighter's vision abilities become more valuable
- Team communication required ("Where's that sound coming from?")

**Best for:** Players who don't rely on audio heavily

---

### Contract: "Low Tide Protocol"

**Modifier:** Map permanently at low tide  
**Challenge:** More areas accessible, but exposed to shore attacks  
**Multiplier:** 1.2×

**Specifics:**
- All water areas are shallow or drained
- Tide pools reveal extra loot spawns
- Beach areas fully accessible
- BUT: More spawn points active (shore enemies)
- Deep Ones spawn MORE frequently (water is their element)

**Why it's interesting:**
- More loot opportunities
- Different map traversal
- High enemy spawn pressure
- Risk/reward: explore new areas vs. fight more enemies

**Best for:** Teams who want more loot, can handle enemy pressure

---

### Contract: "Isolation Dive"

**Modifier:** No teammate markers on HUD  
**Challenge:** Must stay together through awareness, not UI  
**Multiplier:** 1.2×

**Specifics:**
- No teammate nameplates
- No teammate health bars
- No teammate ability cooldown indicators
- Can still see teammates physically (just no UI)

**Why it's interesting:**
- Encourages tight team formations
- Forces verbal communication
- Punishes split-up teams
- Lamplighter's flare becomes teammate locator

**Best for:** Coordinated teams with voice chat

---

### Contract: "Fog of War"

**Modifier:** Fog density increased 50%  
**Challenge:** Reduced visibility, harder to spot enemies  
**Multiplier:** 1.2×

**Specifics:**
- Visual range reduced from 50m to 25m
- Enemies emerge from fog suddenly
- Lamplighter's flare MORE valuable (cuts through fog)
- Rely on audio cues more (footsteps, growls)

**Why it's interesting:**
- Atmospheric horror enhanced
- Ambushes more effective
- Close-range weapons favored
- Lamplighter becomes MVP

**Best for:** Teams with Lamplighter, players who want horror vibe

---

### Contract: "Armory Restriction: Sidearms Only"

**Modifier:** Can only use pistols (.38, M1911)  
**Challenge:** Low damage output, must conserve ammo  
**Multiplier:** 1.3×

**Specifics:**
- Primary weapons disabled
- Only sidearms equippable
- Ammo spawn rate unchanged (relative abundance)
- Abilities MORE important (since guns weak)

**Why it's interesting:**
- Ability-focused gameplay
- Headshots essential (low damage)
- Resource management critical
- Makes players appreciate primary weapons

**Best for:** Skilled players who want pure challenge

---

## TIER 2 CONTRACTS (Unlock after 10 Extractions)

### Contract: "The Colour Out of Space"

**Modifier:** The Color (boss enemy) hunts you throughout run  
**Challenge:** Persistent threat, must evade or fight  
**Multiplier:** 1.5×

**Specifics:**
- The Color spawns at 25% saturation (much earlier than normal)
- Patrols map, hunting players
- Can be evaded (hide, use Lamplighter speed zones)
- OR fought (high risk, boss loot if killed)
- Color drains color from environment (visual cue of proximity)

**Why it's interesting:**
- Boss threat throughout run (not just extraction)
- Stealth vs. aggression decision
- High tension (boss can appear anytime)
- Huge reward if killed (500-1200 scrip + 1.5× multiplier)

**Best for:** Experienced teams who can handle bosses

---

### Contract: "Moonless Night"

**Modifier:** Extreme darkness, limited visibility  
**Challenge:** Must use flares/lanterns sparingly  
**Multiplier:** 1.5×

**Specifics:**
- Lighting reduced 80% (very dark)
- Flares essential (Lamplighter ability)
- Environmental light sources still work (lighthouse, fires)
- Enemies have advantage (see you first)
- Flashlights available (limited battery: 5 minutes total)

**Why it's interesting:**
- Resource management (light sources)
- Lamplighter becomes essential
- Stealth viable (enemies can't see you either)
- Horror amplified (fear of darkness)

**Best for:** Teams with Lamplighter, horror enthusiasts

---

### Contract: "Ritual Tide"

**Modifier:** High tide arrives unpredictably  
**Challenge:** Must watch tidal markers, routes change dynamically  
**Multiplier:** 1.5×

**Specifics:**
- Tide changes every 5-8 minutes (randomized)
- Some areas flood unexpectedly (kill volumes)
- Must watch tide markers (ropes with flags)
- Extraction points affected (Shallows only during low tide)
- Creates urgency ("Tide coming, move NOW!")

**Why it's interesting:**
- Dynamic map hazards
- Forces adaptive routing
- Prevents static camping
- Environmental storytelling (realistic tide behavior)

**Best for:** Players who want environmental challenge

---

### Contract: "Swarm Protocol"

**Modifier:** Enemy spawn rate doubled  
**Challenge:** Overwhelming enemy numbers  
**Multiplier:** 1.5×

**Specifics:**
- Spawn interval halved (15s → 7.5s at low saturation)
- Pack sizes increased (+50%)
- Director AI MORE aggressive
- Continuous pressure

**Why it's interesting:**
- Pure combat challenge
- Ammo management critical
- Abilities essential for crowd control
- Tests team coordination under pressure

**Best for:** Teams who want non-stop action

---

### Contract: "Blessing Withdrawal"

**Modifier:** Blessed Rounds disabled  
**Challenge:** No special ammo advantage vs. eldritch enemies  
**Multiplier:** 1.5×

**Specifics:**
- Cannot equip Blessed Rounds
- Eldritch enemies take normal damage only
- Bosses harder (no 1.5× damage bonus)
- Forces gun skill and ability use

**Why it's interesting:**
- Removes endgame crutch
- Makes players appreciate Blessed Rounds
- Pure skill check

**Best for:** Players with maxed gear testing skill

---

## TIER 3 CONTRACTS (Unlock after 25 Extractions)

### Contract: "The Silver Key"

**Modifier:** Saturation increases per kill, decreases per artifact  
**Challenge:** Must choose - fight OR loot, not both  
**Multiplier:** 2.0×

**Specifics:**
- Each kill: +4% saturation (doubled from normal +2%)
- Each artifact looted: -5% saturation
- Forces non-combat loot focus OR minimal looting combat focus
- Can't do both effectively

**Why it's interesting:**
- Fundamental gameplay change
- Stealth becomes viable strategy
- Artifact hunting prioritized
- OR ignore artifacts, fight conservatively

**Best for:** Experienced players who want strategic challenge

---

### Contract: "Dreams in the Witch House"

**Modifier:** Reality unstable from start  
**Challenge:** High saturation effects active, enemies confused too  
**Multiplier:** 2.0×

**Specifics:**
- Start at 50% saturation (normally 0%)
- Visual/audio effects active immediately
- BUT: Enemies also disoriented (25% slower reactions)
- Boss can spawn early (50% instead of 75%)
- Occultist Purge MORE valuable

**Why it's interesting:**
- Immediate chaos
- Symmetric challenge (enemies affected too)
- Occultist essential
- Tests adaptation under pressure

**Best for:** Skilled teams who want chaos

---

### Contract: "Dead of Night"

**Modifier:** Permanent high saturation, boss pre-killed  
**Challenge:** Speed-run extraction under extreme conditions  
**Multiplier:** 2.0×

**Specifics:**
- Start at 75% saturation (RED zone)
- Boss already dead (loot available in arena, no fight)
- Continuous enemy pressure
- Goal: Extract as fast as possible
- **Leaderboard tracked:** Fastest extraction time

**Why it's interesting:**
- Speed-run focus
- No boss fight (just survive and extract)
- Pure execution test
- Competitive leaderboard

**Best for:** Speed-runners, leaderboard chasers

---

### Contract: "Permadeath Protocol"

**Modifier:** Single death = instant extraction failure  
**Challenge:** Zero mistakes allowed  
**Multiplier:** 2.5×

**Specifics:**
- Any player death = entire team fails instantly
- No revive system
- Must extract with zero deaths
- Highest multiplier available

**Why it's interesting:**
- Ultimate challenge
- Forces perfect execution
- Every decision critical
- Highest possible rewards

**Best for:** Elite players seeking prestige

---

### Contract: "Convergence Event"

**Modifier:** All 3 bosses spawn simultaneously  
**Challenge:** Must fight or evade 3 bosses at once  
**Multiplier:** 3.0×

**Specifics:**
- Drowned Priest, Reef Leviathan, and The Color all spawn at 50% saturation
- Bosses patrol different areas (can be evaded separately)
- Killing any boss: Normal boss rewards (500-1200 scrip each)
- Killing ALL bosses: +5,000 scrip bonus
- Highest risk, highest reward

**Why it's interesting:**
- Endgame ultimate challenge
- Can evade OR fight (player choice)
- Massive reward potential (up to 3,600 + 5,000 = 8,600 scrip)
- Prestigious achievement

**Best for:** Master-level teams

---

## Special Event Contracts (Seasonal/Limited)

### Contract: "Tides of Madness" (Halloween Event)

**Modifier:** Reality shifts every 5 minutes  
**Multiplier:** 2.0×

**Specifics:**
- Map geometry shifts (doors move, rooms rearrange)
- Extraction points rotate locations
- Hallucinations intensified
- Time-limited event (October only)

---

### Contract: "The Deep Freeze" (Winter Event)

**Modifier:** Map covered in ice and snow  
**Multiplier:** 1.8×

**Specifics:**
- Movement slowed on ice (slippery)
- Visibility reduced (blizzard conditions)
- Fire hazards replaced with ice hazards
- Thematic visual overhaul

---

### Contract: "Summer Solstice" (Summer Event)

**Modifier:** Breach weakened temporarily  
**Multiplier:** 0.8× (NEGATIVE multiplier - easier run)

**Specifics:**
- Saturation gain rate halved
- Enemies 20% weaker
- Celebration event (casual players can enjoy)

---

## Contract Unlocks & Progression

### Unlock Conditions

**Tier 1 Contracts:**
- All available from start
- No unlock required

**Tier 2 Contracts:**
- Unlock after 10 successful extractions
- OR unlock specific contracts via achievements:
  - "The Colour Out of Space": Kill The Color 3 times
  - "Moonless Night": Extract from Cathedral at night (saturation 75%+)

**Tier 3 Contracts:**
- Unlock after 25 successful extractions
- OR unlock via prestige achievements:
  - "Permadeath Protocol": Reach Level 15 on any class
  - "Convergence Event": Kill all 3 bosses in career

---

## Contract Rotation & Featured Contracts

### Weekly Featured Contract

**Concept:** One contract selected each week with bonus rewards

**Benefits:**
- Featured contract: +100 bonus scrip
- Leaderboard for featured contract resets weekly
- Community participates in same challenge

**Example Week:**
- Monday: "Moonless Night" is featured
- All players attempting it compete on same leaderboard
- Top 10 by end of week: Cosmetic reward

**Psychology:** Creates shared community experience, encourages experimentation

---

## Contract Strategy Guide

### Easiest Contracts (Beginner-Friendly)

1. "Low Tide Protocol" - More loot, manageable enemy increase
2. "Fog of War" - Atmospheric, not mechanically harder
3. "Isolation Dive" - If using voice chat, minimal impact

### Hardest Contracts (Expert-Only)

1. "Permadeath Protocol" - Zero margin for error
2. "Convergence Event" - 3 bosses simultaneously
3. "Dreams in the Witch House" - Immediate chaos

### Best Scrip/Hour Contracts

1. "The Silver Key" (2.0×) - Fast stealth runs, loot-focused
2. "Swarm Protocol" (1.5×) - Fast combat, if team handles it
3. "Dead of Night" (2.0×) - Speed-run focused

### Most Fun Contracts (Community Favorites)

1. "The Colour Out of Space" - Boss hunt gameplay
2. "Ritual Tide" - Dynamic environment
3. "Moonless Night" - Horror atmosphere enhanced

---

## Contract Combinations (Synergies & Anti-Synergies)

### Good Combinations

**"Moonless Night" + "Fog of War":**
- 1.5× + 1.2× = 2.7× + 0.2 stacking = **2.9× total**
- Synergy: Both reduce visibility, one strategy counters both
- Lamplighter essential

**"Low Tide" + "Swarm Protocol":**
- 1.2× + 1.5× = 2.7× + 0.2 stacking = **2.9× total**
- More enemies BUT more loot to offset ammo consumption

### Bad Combinations (Anti-Synergy)

**"The Silver Key" + "Swarm Protocol":**
- Silver Key: Saturation +4% per kill
- Swarm: 2× enemy spawn rate
- Result: Saturation skyrockets uncontrollably
- Near-impossible without perfect Occultist play

**"Isolation Dive" + "Moonless Night":**
- Isolation: No teammate markers
- Moonless: Can't see teammates in darkness
- Result: Team constantly loses each other

---

## Contract Design Principles

### Principle 1: **Challenge Through Change, Not Tedium**

**Bad:** "Enemies have 2× HP" (tedious bullet sponges)  
**Good:** "Enemies spawn from different locations" (tactical challenge)

### Principle 2: **Avoid Pure Stat Inflation**

**Bad:** "Everything does 2× damage" (cheap difficulty)  
**Good:** "Audio cues reversed" (skill-testing mechanic)

### Principle 3: **Reward Should Match Challenge**

**Easy modifier:** 1.2× multiplier  
**Moderate modifier:** 1.5× multiplier  
**Extreme modifier:** 2.0-3.0× multiplier

### Principle 4: **Some Contracts Should Change Optimal Strategy**

**"The Silver Key":** Stealth becomes viable  
**"Dead of Night":** Speed-running prioritized  
**"Convergence Event":** Boss hunting focus

### Principle 5: **Contracts Should Enable New Builds**

**"Moonless Night":** Lamplighter essential  
**"Swarm Protocol":** Engineer turrets shine  
**"Permadeath Protocol":** Occultist healing critical

---

## Leaderboards & Competition

### Global Leaderboards

**Highest Scrip Single Run:**
- Track best single extraction
- Encourages contract stacking
- Reset monthly

**Fastest Cathedral Extraction:**
- Speed-run focused
- "Dead of Night" contract popular
- Reset weekly

**Most Boss Kills (Career):**
- Long-term prestige
- Track across account
- Never resets

### Contract-Specific Leaderboards

**Each Tier 3 contract has its own leaderboard:**
- "Permadeath Protocol": Most successful extractions
- "Convergence Event": Fastest 3-boss kills
- "Dead of Night": Fastest speed-runs

---

## Future Contract Ideas (Post-Launch)

**"Architect's Nightmare":**
- Map geometry non-Euclidean (impossible angles visible)
- Hallways loop, stairs go nowhere
- Psychological horror focus

**"The Hunt":**
- One player randomly designated "Marked"
- Marked player constantly pursued by hunter enemy
- Must protect Marked player to extract

**"Primordial Deep":**
- Map flooded (all water deep)
- Must swim between locations
- Deep Ones everywhere

**"Temporal Rift":**
- Time moves backwards (visual effect)
- Enemy deaths rewind (must re-kill)
- Surreal challenge

---

## Success Metrics

**Contract system succeeds if:**
- âœ… 60%+ players try at least one contract
- âœ… 30%+ players regularly play with contracts
- âœ… Contract variety keeps game fresh past 50 runs
- âœ… Players discuss "best contract for X class"
- âœ… Featured weekly contracts generate engagement

**Contract system fails if:**
- âŒ <20% players try contracts (too intimidating)
- âŒ One contract dominates (balance issue)
- âŒ Contracts feel mandatory (should be optional)
- âŒ Rewards don't justify difficulty

---

## Related Documentation

**For quick reference:**
- [Systems Quick Ref](../01-quick-reference/systems-quick-ref.md)

**For implementation:**
- [Balance Parameters](../03-technical-specs/balance-parameters.md)

**For other systems:**
- [Extraction Mechanics](extraction-mechanics.md)
- [Progression System](progression-system.md)

---

**Contracts transform replayability from "doing the same thing" to "doing the same thing DIFFERENTLY." Endless variety from finite content.**
