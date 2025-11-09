# TIDE'S END: Progression System Design
## Two-Track Progression & Player Retention

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Purpose:** Explain how progression drives long-term engagement

---

## Document Purpose

This document explains the **progression philosophy** and how two separate progression tracks (Weapons + Classes) create depth without overwhelming complexity.

**Related Documents:**
- Quick Reference: [Systems Quick Ref](../01-quick-reference/systems-quick-ref.md)
- Technical Specs: [Balance Parameters](../03-technical-specs/balance-parameters.md)

---

## Core Progression Philosophy

**"Unlock guns fast. Master classes slow."**

### The Two-Track System

**Track 1: Weapons (Scrip Currency)**
- Shared across ALL classes
- Fast to unlock (1-5 runs per weapon)
- Provides immediate power gains
- Satisfies "unlocking stuff" itch

**Track 2: Classes (XP + Scrip)**
- Per-class progression
- Slow to max (25-30 runs per class)
- Provides depth and mastery
- Creates "main" identity

**Why two tracks?**
- Fast track (weapons) = short-term goals
- Slow track (classes) = long-term investment
- Always something to work toward
- Never "done" (4 classes × 15 levels = 60 total levels)

---

## Track 1: Weapon Progression

### Design Philosophy

**Problem:**
- If weapons are too expensive: Players frustrated, progression feels slow
- If weapons are too cheap: Players unlock everything in 5 runs, nothing to chase

**Solution:**
- Tier 1 (Free): Immediately accessible, adequate
- Tier 2 (800-1,200 scrip): Unlock in 1-2 runs, noticeable upgrade
- Tier 3 (2,000-3,000 scrip): Unlock in 3-5 runs, significant power spike
- Tier 4 (5,000+ scrip + boss kills): Unlock in 8-10 runs, endgame reward

### Unlock Progression Curve

**Average scrip per run:** 1,000-2,000 (varies by extraction point, contracts)

| Tier | Cost | Runs to Unlock | Hours to Unlock |
|------|------|----------------|-----------------|
| Tier 1 | Free | 0 | 0 |
| Tier 2 | 800-1,200 | 1-2 | 0.5-1 hour |
| Tier 3 | 2,000-3,000 | 2-4 | 1-2 hours |
| Tier 4 | 5,000+ | 5-8 | 3-4 hours |

**Total time to unlock all weapons:** 10-15 hours (reasonable for Early Access)

### Weapon Power Curve

**Tier 1 (Free):**
- DPS: 40-60
- Feel: Adequate, functional
- Role: Learning weapons

**Tier 2 (800-1,200):**
- DPS: 60-80
- Feel: Comfortable, reliable
- Role: Workhorse weapons

**Tier 3 (2,000-3,000):**
- DPS: 80-120
- Feel: Powerful, confident
- Role: Specialist weapons

**Tier 4 (5,000+):**
- DPS: 120-180
- Feel: Dominating, deserved
- Role: Power fantasy, endgame

**Key:** Power increase is noticeable but not essential. Tier 1 weapons are still viable at endgame.

### First-Time Player Experience

**Run 1:**
- Use Tier 1 weapons (M1903, Double-Barrel, .38)
- Earn 500 scrip (died early, learning)
- Progress: 0 weapons unlocked

**Run 2-3:**
- Use Tier 1 weapons
- Earn 1,200 scrip total
- **Unlock Winchester 1897 (pump shotgun)**
- Feels: "I earned this! It's better!"

**Run 4-6:**
- Use Winchester + Tier 1
- Earn 2,500 scrip total
- **Unlock M1911 pistol**
- **Unlock Lee-Enfield rifle**
- Feels: "I have choices now"

**Run 7-10:**
- Use Tier 2 weapons
- Earn 5,000 scrip total
- **Unlock Thompson SMG**
- Feels: "First full-auto! This is awesome!"

**Result:** Always working toward next unlock. Never more than 2-3 runs away from something new.

---

## Track 2: Class Progression

### Design Philosophy

**Problem:**
- If all classes max quickly: No long-term engagement
- If classes take forever to level: Players feel stuck on one class

**Solution:**
- Each class takes 25-30 successful runs to max (Level 15)
- 4 classes total = 100-120 runs for completionist
- Create "main" identity early, explore other classes later

### XP Gain Rates

| Activity | XP Gained | Notes |
|----------|-----------|-------|
| Successful extraction | 200 XP | Base reward |
| Boss kill | +500 XP | Bonus |
| Complete contract | +100 XP | Bonus |
| Failed run (team wipe) | 20 XP | Consolation prize |

**Average XP per successful run:** 300-500 XP (with occasional boss)

### Leveling Curve

| Level | XP Required | Cumulative XP | Runs to Reach |
|-------|-------------|---------------|---------------|
| 1 | 0 | 0 | 0 (start) |
| 2 | 500 | 500 | 2 |
| 3 | 1,000 | 1,500 | 4 |
| 4 | 1,500 | 3,000 | 7 |
| 5 | 2,000 | 5,000 | 12 |
| 6 | 2,500 | 7,500 | 17 |
| 7 | 3,000 | 10,500 | 23 |
| 8 | 3,500 | 14,000 | 30 |
| 9 | 4,000 | 18,000 | 38 |
| 10 | 5,000 | 23,000 | 48 |
| 11-14 | +1,000 each | - | - |
| 15 | 10,000 | 50,000 | ~110 |

**Result:**
- Level 5 (Tier 2 upgrades): 12 runs (~6 hours)
- Level 10 (Tier 3 upgrades): 48 runs (~24 hours)
- Level 15 (Mastery cosmetic): 110 runs (~55 hours)

### Ability Upgrade System

**Three tiers per ability:**

**Tier 1 (Level 1, Free):**
- Base ability
- Immediately functional
- No cost

**Tier 2 (Level 5, 500 scrip):**
- +50% effectiveness
- Noticeable upgrade
- Requires: Level 5 + 500 scrip

**Tier 3 (Level 10, 1,500 scrip):**
- +100% effectiveness or new effect
- Significant power spike
- Requires: Level 10 + 1,500 scrip

**Example: Bulwark's Anchor Point**

| Tier | Unlock | Effect |
|------|--------|--------|
| Tier 1 | Level 1, Free | 200 HP, 20s duration |
| Tier 2 | Level 5, 500 scrip | 300 HP, 25s duration |
| Tier 3 | Level 10, 1,500 scrip | 400 HP, 30s duration, +15% damage reduction for allies |

**Result:** Each upgrade feels meaningful. Tier 3 adds new functionality (not just numbers).

### First-Time Player Experience (Class)

**Runs 1-5 (Bulwark, learning):**
- Level 1 → Level 3
- Base abilities only
- Feels: "I'm learning how to play Bulwark"

**Runs 6-12 (Bulwark, unlocking):**
- Level 3 → Level 5
- **Unlock Tier 2 upgrades (1,000 scrip total)**
- Feels: "My abilities are noticeably better!"

**Runs 13-20 (Bulwark, mastering):**
- Level 5 → Level 7
- Using upgraded abilities
- Feels: "I'm getting good at Bulwark"

**Runs 21-30 (Try new class):**
- Bulwark at Level 7
- **Try Harpooner (Level 1)**
- Feels: "Fresh playstyle! Totally different!"

**Runs 31-50 (Depth):**
- Bulwark Level 7 → 10
- Harpooner Level 1 → 5
- **Unlock Tier 3 upgrades on Bulwark**
- Feels: "I'm a Bulwark main, but I can flex Harpooner"

**Result:** Players develop "main" identity but try other classes naturally.

---

## Player Progression Stages

### Stage 1: Newbie (Runs 1-10)

**Goals:**
- Learn mechanics
- Unlock first Tier 2 weapon
- Reach Class Level 5

**Challenges:**
- Die frequently
- Low scrip earnings (400-800 per run)
- Don't understand saturation yet

**Progression Rewards:**
- âœ… Unlock Winchester shotgun (Run 2-3)
- âœ… Reach Level 5, unlock Tier 2 ability upgrades (Run 12)
- âœ… First boss kill (Run 8-10)

**Psychology:** Frequent unlocks keep newbies engaged. Always 1-2 runs from next goal.

---

### Stage 2: Intermediate (Runs 11-30)

**Goals:**
- Unlock Tier 3 weapons
- Reach Class Level 10
- Master one class

**Challenges:**
- Saturation management
- Boss fights consistent
- Optimal extraction decisions

**Progression Rewards:**
- âœ… Unlock Thompson SMG (Run 15)
- âœ… Unlock BAR (Run 25)
- âœ… Reach Level 10, unlock Tier 3 upgrades (Run 30)

**Psychology:** Players feel competent. Progression slows but still visible. "I'm getting good."

---

### Stage 3: Advanced (Runs 31-60)

**Goals:**
- Unlock Tier 4 weapons
- Max out first class (Level 15)
- Try all 4 classes

**Challenges:**
- Tier 3 contracts
- Cathedral extractions consistent
- Optimize scrip/hour

**Progression Rewards:**
- âœ… Unlock Elephant Gun (Run 40)
- âœ… Reach Level 15 on main class (Run 45)
- âœ… Start leveling second class

**Psychology:** "Main" identity solidified. Experimenting with other classes. Still chasing cosmetics.

---

### Stage 4: Master (Runs 61-120)

**Goals:**
- Max multiple classes
- Perfect builds
- Leaderboards / speedruns

**Challenges:**
- Optimize every decision
- Master all class synergies
- Complete all contracts

**Progression Rewards:**
- âœ… All 4 classes at Level 10+
- âœ… 2+ classes at Level 15 (mastery cosmetics)
- âœ… All weapons unlocked

**Psychology:** Mastery satisfaction. Social prestige ("I'm a Level 15 Occultist"). Engage with meta.

---

## Progression Pacing Principles

### Principle 1: **Never More Than 3 Runs from Something**

**Bad:** "Need 10,000 scrip for next unlock, I have 500" (20 runs away)  
**Good:** "Need 1,200 scrip for Winchester, I have 800" (1 run away)

**Implementation:**
- Tier 2 weapons cheap (1-2 runs)
- Tier 3 weapons moderate (3-5 runs)
- Class levels frequent early (every 2-3 runs until Level 5)

### Principle 2: **Short-Term AND Long-Term Goals**

**Short-term (1-5 runs):**
- âœ… Next weapon unlock
- âœ… Next class level
- âœ… Complete new contract

**Long-term (30-50 runs):**
- âœ… Max out class (Level 15)
- âœ… Unlock Elephant Gun
- âœ… Master all classes

**Result:** Always have immediate goal, but also distant aspiration.

### Principle 3: **Power is Earned, Not Given**

**Bad:** Start with best gun, nothing to chase  
**Good:** Tier 1 weapons adequate, but Tier 3-4 feel EARNED

**Implementation:**
- Tier 1 can beat game (never mandatory to upgrade)
- Tier 4 feels powerful but not essential
- Skill > gear (good players win with Tier 1)

### Principle 4: **Horizontal AND Vertical Progression**

**Vertical (power):**
- Tier 1 → Tier 2 → Tier 3 → Tier 4 weapons
- Level 1 → 5 → 10 → 15 abilities

**Horizontal (variety):**
- 4 classes (totally different playstyles)
- 8 weapons (different roles)
- Contract modifiers (different challenges)

**Result:** Even at "max level," there's variety to explore.

---

## Retention Mechanics

### Daily Engagement (Optional, Not Pushy)

**NO:**
- âŒ Daily login rewards (manipulative)
- âŒ FOMO mechanics (predatory)
- âŒ Streak systems (punish breaks)

**YES:**
- âœ… First win of the day: +50% XP (nice bonus, not mandatory)
- âœ… Weekly featured contract: +100 bonus scrip (optional challenge)

**Philosophy:** Respect players' time. Reward play, don't punish absence.

### Social Engagement

**Friend Referral:**
- Invite friend → They reach Level 5 → You get cosmetic

**Team Achievements:**
- "Flawless Extraction" (no one took damage)
- "Speed Run" (extract under 15 minutes)
- "High Risk, High Reward" (extract above 80% saturation)

**Leaderboards:**
- Fastest Cathedral extraction
- Highest scrip single run
- Most boss kills (career)

**Philosophy:** Encourage co-op, not competition. Celebrate together.

### Cosmetic Progression (Post-Weapon Unlocks)

**Once players have all weapons, chase cosmetics:**

**Class Mastery Skins (Level 15):**
- Bulwark: Gold-plated armor accents
- Lamplighter: Glowing lantern (brighter flare VFX)
- Harpooner: Engraved harpoon gun
- Occultist: Ethereal robes (particle effects)

**Boss Kill Cosmetics:**
- Kill Drowned Priest 10 times → Cathedral-themed weapon skin
- Kill Reef Leviathan 10 times → Barnacle-encrusted weapon skin
- Kill The Color 10 times → Prismatic shimmer weapon effect

**Weapon Charms (Low-Priority):**
- Unlocked from specific achievements
- Hang off weapons (visual only)
- Examples: Anchor charm, Flare charm, Harpoon charm

**Philosophy:** Cosmetics show mastery, not payment. No paid cosmetics at Early Access launch.

---

## Monetization Philosophy (Post-Launch)

**Early Access: No monetization beyond game purchase ($20-25)**

**Full Release (v1.0): Optional cosmetics only**
- âœ… Weapon skins (never gameplay advantage)
- âœ… Class outfit variations
- âœ… VFX customization (ability colors)
- âŒ NO pay-to-win
- âŒ NO loot boxes
- âŒ NO battle passes

**Philosophy:** Fair monetization. Cosmetics only. Never split playerbase (no paid maps/classes).

---

## Comparison to Other Progression Systems

### vs. Deep Rock Galactic

| Deep Rock | Tide's End |
|-----------|------------|
| Per-class weapon unlocks | Shared weapon pool |
| Weapon mods (horizontal) | Ability upgrades (vertical) |
| Slower initial progression | Faster weapon unlocks |
| Cosmetics plentiful | Cosmetics tied to mastery |

**Learning:** DRG's progression is excellent but complex. Simplify by sharing weapons.

### vs. Vermintide 2

| Vermintide 2 | Tide's End |
|--------------|------------|
| Loot box RNG | Deterministic scrip |
| Item power levels | Weapon tiers |
| Talent trees | Ability upgrades |

**Learning:** Remove RNG frustration. Players know exactly what they're working toward.

### vs. Hunt: Showdown

| Hunt | Tide's End |
|------|------------|
| Bloodline rank (account) | Class levels (per class) |
| Weapons unlock via rank | Weapons unlock via currency |
| Permadeath loses loadout | Keep all progression |

**Learning:** Remove permadeath harshness. Players keep progress even on wipes.

---

## New Player Onboarding

### Tutorial Run (AI-Guided)

**Goal:** Teach progression systems without overwhelming

**Run 1 (Tutorial):**
- Start with M1903, Double-Barrel, .38 (Tier 1)
- Dr. Vance (radio): "These are Institute standard issue. Adequate for the job."
- Complete simple extraction
- Earn 500 scrip
- **After run:** "You earned 500 scrip. Use it to unlock better equipment."

**Run 2 (First Unlock):**
- Open armory
- See Winchester (1,000 scrip) highlighted
- Earn 600 scrip (total 1,100)
- **Unlock Winchester**
- Dr. Vance: "The pump shotgun is more reliable than the double-barrel. Solid choice."

**Run 3 (Class Level Up):**
- Bulwark Level 1 → Level 2
- **Level up notification**
- Dr. Vance: "You're improving. Keep deploying those abilities. At Level 5, you'll unlock enhanced versions."

**Result:** Players understand both tracks naturally through play.

---

## Progression Milestones (Player Goals)

**Milestone 1:** "Unlock first Tier 2 weapon" (Runs 2-3, ~1 hour)  
**Milestone 2:** "Reach Class Level 5" (Runs 10-12, ~5 hours)  
**Milestone 3:** "Kill first boss" (Runs 8-10, ~4 hours)  
**Milestone 4:** "Unlock first Tier 3 weapon" (Runs 15-20, ~10 hours)  
**Milestone 5:** "Reach Class Level 10" (Runs 40-50, ~25 hours)  
**Milestone 6:** "Unlock Elephant Gun" (Runs 50-60, ~30 hours)  
**Milestone 7:** "Max out first class (Level 15)" (Runs 100-120, ~60 hours)  
**Milestone 8:** "Max out all 4 classes" (Runs 400-500, ~250 hours)

**Result:** Clear progression path from newbie to master.

---

## Addressing Progression Concerns

### Concern 1: "Progression Too Slow"

**Solution:**
- Increase scrip multipliers (extraction points, contracts)
- Reduce Tier 2-3 costs by 25%
- Increase XP gain by 50%

### Concern 2: "Progression Too Fast"

**Solution:**
- Reduce scrip rewards by 25%
- Increase Tier 3-4 costs
- Add more cosmetic goals (weapon variants, charms)

### Concern 3: "Hitting Max Level Too Early"

**Solution:**
- 4 classes = 60 total levels (Level 15 × 4)
- Add prestige system (Level 15 → reset to 1 with cosmetic badge)
- Add seasonal leaderboards (resets, but permanent rewards)

### Concern 4: "Not Enough Long-Term Goals"

**Solution:**
- Add achievement system (100+ achievements)
- Add class mastery challenges (speed runs, flawless runs)
- Add community goals (global boss kill count, unlock new content)

---

## Success Metrics

**Progression succeeds if:**
- âœ… Players say "just one more run" (chasing next unlock)
- âœ… Average session length 2-3 runs (~1.5 hours)
- âœ… 60%+ players reach Level 10 on one class
- âœ… 20%+ players max out one class (Level 15)
- âœ… 5%+ players max out all 4 classes
- âœ… Players identify as "[Class] main"

**Progression fails if:**
- âŒ Players quit before unlocking Tier 2 weapons (too slow)
- âŒ Players unlock everything in 10 runs (too fast)
- âŒ No one bothers leveling multiple classes (no variety)
- âŒ Grind feels tedious (not fun)

---

## Related Documentation

**For quick reference:**
- [Systems Quick Ref](../01-quick-reference/systems-quick-ref.md)

**For implementation:**
- [Balance Parameters](../03-technical-specs/balance-parameters.md)

**For other systems:**
- [Combat System Design](combat-system-design.md)
- [Extraction Mechanics](extraction-mechanics.md)

---

**Two-track progression creates depth without complexity. Players always have something to unlock, and mastery feels earned.**
