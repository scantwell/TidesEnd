# TIDE'S END: Implementation Priorities
## Build Order Roadmap for 18-Month Development

**Version:** 1.0  
**Last Updated:** October 27, 2025  
**Purpose:** Detailed development roadmap with concrete milestones

---

## Document Purpose

This document provides a **step-by-step implementation order** for building Tide's End from scratch to Early Access launch. Organized by priority and dependencies.

**Timeline:** 18 months, part-time (10-15 hours/week)  
**Target:** Playable Early Access build  
**Approach:** Iterate on fun, not features

---

## PHASE 1: PROTOTYPE (Months 1-3)
**Goal:** Prove core combat loop feels good

### Month 1: Foundation

**Week 1-2: Project Setup**
- [ ] Create Unity 6 project with URP
- [ ] Install Netcode for GameObjects
- [ ] Set up version control (Git)
- [ ] Create project folder structure
- [ ] Install essential packages (Cinemachine, Input System)
- [ ] Configure build settings (PC, Steam)

**Week 3-4: Player Controller**
- [ ] Implement first-person movement (WASD)
- [ ] Add sprint and crouch
- [ ] Implement jumping
- [ ] Add camera look (mouse)
- [ ] Create player capsule collider
- [ ] Test movement feel (speed, acceleration)

**Success Metric:** Player can move, jump, and look around smoothly

---

### Month 2: Core Combat

**Week 5-6: First Weapon (M1903 Rifle)**
- [ ] Create weapon base class
- [ ] Implement M1903 ScriptableObject
- [ ] Add firing logic (raycast hitscan)
- [ ] Create muzzle flash VFX
- [ ] Add fire sound
- [ ] Implement recoil (camera kick)
- [ ] Add ammo tracking and UI
- [ ] Create reload system with animation lock
- [ ] Test weapon feel (satisfying shots)

**Week 7-8: Damage System**
- [ ] Create damage dealer interface
- [ ] Implement health system
- [ ] Add hit detection (body, head, limbs)
- [ ] Create damage multipliers (headshot 2×)
- [ ] Add hit feedback VFX (blood spray)
- [ ] Add hit feedback audio (impact sound)
- [ ] Create death system
- [ ] Test damage numbers match spec

**Success Metric:** Can shoot target dummy, see hit feedback, ammo decreases, reload works

---

### Month 3: First Enemy & Ability

**Week 9-10: Hollow-Eyed Enemy**
- [ ] Create enemy base class
- [ ] Model basic humanoid (placeholder art OK)
- [ ] Implement Hollow-Eyed ScriptableObject
- [ ] Create 3-state AI (Idle, Alert, Combat)
- [ ] Add NavMeshAgent pathfinding
- [ ] Implement melee attack (lunge)
- [ ] Add enemy health and damage
- [ ] Create death animation/ragdoll
- [ ] Test enemy behavior

**Week 11-12: First Ability (Bulwark's Anchor Point)**
- [ ] Create ability base class
- [ ] Implement Anchor Point ScriptableObject
- [ ] Add throwable projectile system
- [ ] Create zone effect system (sphere trigger)
- [ ] Implement zone effects (slow enemies)
- [ ] Add ability cooldown UI
- [ ] Create VFX (golden glow)
- [ ] Add audio (deploy, active hum)
- [ ] Test ability impact on gameplay

**Success Metric:** Can throw anchor, enemies slow inside zone, cooldown works, feels impactful

---

### PHASE 1 CHECKPOINT

**Playable Demo:**
- Player can move and shoot M1903 rifle
- Hollow-Eyed enemies pathfind and attack
- Bulwark's Anchor Point ability creates tactical advantage
- Basic UI (health, ammo, cooldown)

**Testing Question:** *"Does the core 15-second loop (position, shoot, adapt) feel fun?"*
- If YES → Continue to Phase 2
- If NO → Iterate until fun before proceeding

---

## PHASE 2: VERTICAL SLICE (Months 4-6)
**Goal:** Demo-able build showing full vision

### Month 4: Content Expansion

**Week 13-14: Second Class (Harpooner)**
- [ ] Implement Harpooner ScriptableObject
- [ ] Create Whaling Harpoon ability
- [ ] Add harpoon projectile physics
- [ ] Implement pull mechanic (enemy force)
- [ ] Create Reel & Frenzy buff system
- [ ] Add stat modifiers (fire rate, reload, damage)
- [ ] Test class differentiation

**Week 15-16: Two More Weapons**
- [ ] Implement Double-Barrel Shotgun
  - [ ] Pellet spread system
  - [ ] Two-shot mechanic
  - [ ] Heavy recoil
- [ ] Implement .38 Revolver (sidearm)
  - [ ] Weapon swap system
  - [ ] Faster handling than primary
- [ ] Test weapon variety

**Success Metric:** Two classes feel distinctly different, three weapons have unique roles

---

### Month 5: Enemy Variety & Environment

**Week 17-18: Second Enemy (Reef Walker)**
- [ ] Create ranged enemy AI
- [ ] Implement projectile system
- [ ] Add distance-keeping behavior
- [ ] Create strafe movement
- [ ] Add backpedal logic (if player too close)
- [ ] Test mixed enemy encounters (rusher + ranged)

**Week 19-20: First Map Zone**
- [ ] Create lighthouse exterior (ProBuilder/asset pack)
- [ ] Build interior (spiral staircase)
- [ ] Add extraction point at top
- [ ] Place spawn points for enemies
- [ ] Add lighting (moody atmosphere)
- [ ] Create fog system (Unity's volumetric fog)
- [ ] Test player flow through space

**Success Metric:** Lighthouse area feels atmospheric, combat has variety (melee + ranged)

---

### Month 6: Systems Integration

**Week 21-22: Breach Saturation System**
- [ ] Create saturation manager script
- [ ] Implement saturation UI (percentage bar)
- [ ] Add saturation gain triggers (time, kills)
- [ ] Connect saturation to spawn rate
- [ ] Add visual effects per tier (fog density)
- [ ] Add audio changes per tier (ambient layers)
- [ ] Test escalating tension

**Week 23-24: First Boss (Drowned Priest - Simplified)**
- [ ] Create boss arena (flooded cathedral interior)
- [ ] Implement boss AI (single phase for now)
- [ ] Add summon wave ability
- [ ] Create ritual pulse attack
- [ ] Add boss health bar UI
- [ ] Create 3 ritual totems (destructible)
- [ ] Test boss encounter flow (3-5 minute fight)

**Success Metric:** Boss fight feels climactic, requires team coordination

---

### PHASE 2 CHECKPOINT

**Vertical Slice Demo:**
- Two playable classes (Bulwark, Harpooner)
- Three weapons (rifle, shotgun, pistol)
- Two enemy types + one boss
- One complete map zone (lighthouse)
- Breach Saturation system functional
- Basic extraction system

**Testing Questions:**
1. *"Does class choice matter?"* → YES
2. *"Are weapons satisfying?"* → YES
3. *"Is boss fight fun?"* → YES
4. *"Do we want to play another round?"* → YES

**If all YES → Continue to Phase 3**  
**If any NO → Polish that system before proceeding**

---

## PHASE 3: CONTENT EXPANSION (Months 7-12)
**Goal:** Full Early Access content pipeline

### Month 7-8: Complete Class Roster

**Lamplighter Class:**
- [ ] Implement Lamplighter ScriptableObject
- [ ] Create Flare Shot ability (projectile + light)
- [ ] Implement enemy reveal system (outline shader)
- [ ] Create Pathfinder's Mark ability (speed zone)
- [ ] Add speed boost logic
- [ ] Test scout playstyle

**Occultist Class:**
- [ ] Implement Occultist ScriptableObject
- [ ] Create Sanctuary Circle ability (healing zone)
- [ ] Implement regeneration system (3% HP/s)
- [ ] Create Purge Corruption ability (channeled)
- [ ] Add shield grant system (temporary HP)
- [ ] Test support playstyle

**Ability Upgrade System:**
- [ ] Create upgrade UI (3 tiers per ability)
- [ ] Implement upgrade unlock logic (level + scrip)
- [ ] Connect upgrades to ability parameters
- [ ] Test progression feel

**Success Metric:** All 4 classes playable, team comp matters, abilities synergize

---

### Month 9-10: Complete Weapon Arsenal

**Tier 2 Weapons:**
- [ ] Winchester 1897 (pump shotgun)
- [ ] M1911 Pistol (semi-auto sidearm)
- [ ] Lee-Enfield (fast bolt rifle)

**Tier 3 Weapons:**
- [ ] Thompson SMG (full-auto)
  - [ ] Implement sustained fire recoil
  - [ ] Add recoil per-shot increase
- [ ] BAR (automatic rifle)
  - [ ] Heavy recoil pattern
  - [ ] Higher damage than Thompson

**Blessed Rounds:**
- [ ] Create special ammo type
- [ ] Implement damage modifiers (vs eldritch/corrupted)
- [ ] Add blessed VFX (golden tracer)
- [ ] Add blessed audio (holy impact)
- [ ] Test strategic ammo use

**Weapon Progression:**
- [ ] Create weapon unlock UI
- [ ] Implement scrip cost system
- [ ] Test unlock progression curve

**Success Metric:** All 8 weapons feel distinct, progression curve smooth

---

### Month 11: Complete Enemy Roster

**Tier 2 Enemies:**
- [ ] Deep One (pack hunter)
  - [ ] Implement pack behavior script
  - [ ] Add flanking logic
  - [ ] Create leap attack
  - [ ] Add water emergence spawn
- [ ] Tide Touched (mutant)
  - [ ] Implement phase ability (invulnerable)
  - [ ] Add wall-phasing logic
  - [ ] Create unpredictable movement

**Tier 3 Enemies:**
- [ ] Shoggoth (Mini)
  - [ ] Implement split mechanic
  - [ ] Add corruption trail system
  - [ ] Create ooze movement
- [ ] Dimensional Shambler (hunter)
  - [ ] Implement teleport system
  - [ ] Add invisibility shader
  - [ ] Create hit-and-run behavior

**Spawn Director AI:**
- [ ] Create spawn manager script
- [ ] Implement saturation-based spawning
- [ ] Add ambush spawn logic (25%+)
- [ ] Create director AI (50%+, tactical spawns)
- [ ] Test spawn escalation

**Success Metric:** Enemy variety creates different tactical challenges, spawning feels intelligent

---

### Month 12: Complete Boss Encounters

**Drowned Priest (Full Implementation):**
- [ ] Add Phase 2 (corruption zones)
- [ ] Add Phase 3 (enraged)
- [ ] Polish boss abilities
- [ ] Add weak point system (totems)
- [ ] Test multi-phase fight

**Reef Leviathan:**
- [ ] Create coastal boss arena
- [ ] Implement tentacle sweep attack
- [ ] Add whirlpool ability
- [ ] Create submerge/reemerge mechanic
- [ ] Add barnacle weak points
- [ ] Test arena hazards

**The Color:**
- [ ] Create grove arena
- [ ] Implement possession mechanic
- [ ] Add color drain passive
- [ ] Create reality distortion ability
- [ ] Add darkness weakness
- [ ] Test mechanic-heavy fight

**Boss Rewards:**
- [ ] Create boss token system
- [ ] Implement trophy drops
- [ ] Add guaranteed loot (500-1200 scrip)
- [ ] Test reward satisfaction

**Success Metric:** All 3 bosses offer unique challenges, rewards feel earned

---

### PHASE 3 CHECKPOINT

**Content Complete:**
- 4 classes fully implemented with 2 abilities each
- 8 weapons + blessed rounds
- 6 regular enemy types
- 3 boss encounters
- Spawn director AI functional
- Ability upgrade system working
- Weapon progression complete

**Estimated Gameplay:** 30+ hours before content exhaustion

---

## PHASE 4: POLISH & BALANCE (Months 13-15)
**Goal:** Early Access quality

### Month 13: Balance Pass

**Weapon Balance:**
- [ ] Gather playtest data (weapon usage, TTK)
- [ ] Adjust damage values per tier
- [ ] Tune reload times
- [ ] Balance recoil patterns
- [ ] Test DPS targets (40-180 range)

**Enemy Balance:**
- [ ] Analyze player death data
- [ ] Adjust enemy HP values
- [ ] Tune enemy damage
- [ ] Balance spawn rates
- [ ] Test difficulty curve

**Ability Balance:**
- [ ] Review ability usage data
- [ ] Adjust cooldowns
- [ ] Tune effect strength
- [ ] Balance upgrade tiers
- [ ] Test class power levels

**Progression Balance:**
- [ ] Track scrip earn rates
- [ ] Adjust unlock costs
- [ ] Tune XP gain rates
- [ ] Balance class leveling curve
- [ ] Test time-to-unlock weapons

**Success Metric:** All systems balanced, no dominant strategies, progression feels good

---

### Month 14: Systems Polish

**Combat Feel:**
- [ ] Polish weapon audio (layers, variations)
- [ ] Improve weapon VFX (muzzle flash, tracers)
- [ ] Enhance hit feedback (screen shake options)
- [ ] Add haptic feedback (controller rumble)
- [ ] Test "juiciness" factor

**AI Polish:**
- [ ] Improve pathfinding (smoother movement)
- [ ] Add animation blending
- [ ] Enhance telegraphs (clearer attack warnings)
- [ ] Add enemy VO (grunts, death sounds)
- [ ] Test enemy readability

**Ability Polish:**
- [ ] Upgrade ability VFX (particle systems)
- [ ] Improve ability audio (spatial, layers)
- [ ] Add ability telegraphs (range indicators)
- [ ] Polish UI (cooldown animations)
- [ ] Test ability clarity

**Environment Polish:**
- [ ] Improve lighting (atmospheric)
- [ ] Add detail props
- [ ] Enhance fog system (density, color)
- [ ] Add ambient audio (waves, wind, creaks)
- [ ] Test immersion

**Success Metric:** Game feels polished, not janky

---

### Month 15: Performance & QA

**Performance Optimization:**
- [ ] Profile FPS (target 60 FPS on mid-range PC)
- [ ] Optimize draw calls (batching, occlusion culling)
- [ ] Reduce garbage collection (object pooling)
- [ ] Optimize navmesh (reduce complexity)
- [ ] Implement LOD system for enemies
- [ ] Test on minimum spec hardware

**Bug Fixing:**
- [ ] Fix critical bugs (crashes, soft-locks)
- [ ] Fix major bugs (broken abilities, wrong damage)
- [ ] Fix minor bugs (UI glitches, audio issues)
- [ ] Create bug tracking system
- [ ] Conduct playtest marathon

**Networking Polish:**
- [ ] Fix desync issues
- [ ] Improve latency handling
- [ ] Add reconnect logic
- [ ] Polish lobby system
- [ ] Test co-op stability

**UI/UX Polish:**
- [ ] Improve menu navigation
- [ ] Add keybind customization
- [ ] Polish HUD layout
- [ ] Add settings (audio, graphics)
- [ ] Create tutorial/onboarding

**Success Metric:** Stable 60 FPS, no critical bugs, smooth co-op experience

---

### PHASE 4 CHECKPOINT

**Early Access Ready:**
- All systems balanced and tuned
- 60 FPS stable performance
- No critical bugs
- Co-op networking stable
- Polished feel (audio, VFX, UI)

**Quality Bar:** "Would I pay $20 for this?"
- If YES → Proceed to Phase 5
- If NO → Iterate polish until yes

---

## PHASE 5: LAUNCH PREP (Months 16-18)
**Goal:** Ship Early Access successfully

### Month 16: Marketing Assets

**Steam Page:**
- [ ] Write game description
- [ ] Create capsule art (main image, header)
- [ ] Record gameplay trailer (60-90 seconds)
- [ ] Capture screenshots (6-10 high quality)
- [ ] Write feature list
- [ ] Set price ($20-25 recommended)
- [ ] Enable wishlist

**Press Kit:**
- [ ] Write press release
- [ ] Create fact sheet
- [ ] Compile media assets
- [ ] Prepare dev interview Q&A
- [ ] Contact indie game press

**Community Building:**
- [ ] Create Discord server
- [ ] Post devlog on social media (Twitter, Reddit)
- [ ] Share GIFs of cool moments
- [ ] Engage with followers
- [ ] Build hype

**Success Metric:** 500+ wishlists before launch

---

### Month 17: Beta Testing

**Closed Beta:**
- [ ] Recruit 20-50 playtesters
- [ ] Create feedback form (Google Forms)
- [ ] Collect gameplay data (telemetry)
- [ ] Monitor crash reports
- [ ] Fix critical issues found

**Balance Refinement:**
- [ ] Analyze beta player data
- [ ] Adjust difficulty if needed
- [ ] Tweak progression pace
- [ ] Fix exploits
- [ ] Test changes with beta group

**Content Polish:**
- [ ] Add missing audio cues (beta feedback)
- [ ] Improve unclear UI (beta feedback)
- [ ] Fix map flow issues (beta feedback)
- [ ] Enhance tutorial (beta feedback)

**Success Metric:** 70%+ positive beta feedback, <5 critical bugs

---

### Month 18: Launch Month

**Week 73-74: Final Polish**
- [ ] Final bug fixing pass
- [ ] Polish launch trailer
- [ ] Update Steam page with beta feedback
- [ ] Prepare launch announcement
- [ ] Test build on fresh PC (no dev tools)

**Week 75: Soft Launch**
- [ ] Release to closed beta (final test)
- [ ] Monitor for critical issues
- [ ] Fix any show-stoppers
- [ ] Prepare day-one patch if needed

**Week 76: LAUNCH DAY**
- [ ] Release Early Access on Steam
- [ ] Post launch announcement (Discord, social)
- [ ] Monitor reviews and feedback
- [ ] Respond to critical bugs immediately
- [ ] Celebrate! 🎉

**Success Metrics:**
- 1,000 sales Week 1
- 70%+ positive reviews
- Active Discord community (100+ members)
- No critical bugs reported

---

## CRITICAL PATH DEPENDENCIES

**These must be completed in order:**

```
Player Controller → Weapon System → Enemy AI → Ability System → Class System
        ↓               ↓              ↓            ↓              ↓
   Movement Feel    Gun Feel      Combat Loop   Team Synergy   Replayability
```

**Cannot proceed to next phase without:**
- Phase 1 → Core combat feels fun
- Phase 2 → Vertical slice demonstrates full vision
- Phase 3 → All content playable (rough)
- Phase 4 → Game feels polished, balanced
- Phase 5 → Marketing ready, bug-free

---

## RISK MITIGATION

### High-Risk Areas

**1. Multiplayer Networking**
- Risk: Desync, latency issues
- Mitigation: Start testing co-op early (Month 4)
- Fallback: Solo mode always functional

**2. AI Pathfinding**
- Risk: Enemies get stuck, break maps
- Mitigation: Use Unity NavMesh (proven system)
- Fallback: Simpler AI if needed

**3. Performance**
- Risk: Game runs poorly on target hardware
- Mitigation: Profile early and often
- Fallback: Reduce enemy count, simplify VFX

**4. Scope Creep**
- Risk: Adding too many features, never shipping
- Mitigation: Strict "no new features" rule after Month 12
- Fallback: Cut non-essential content

### Low-Risk Areas

**1. Gunplay** - Well-documented Unity patterns
**2. Abilities** - Relatively simple systems (zones, projectiles)
**3. Art** - Low-poly aesthetic is achievable
**4. Audio** - Asset packs available

---

## PARALLEL WORK STREAMS

**These can be worked on simultaneously:**

**Stream A (Core Gameplay):**
- Player controller, weapons, enemies, abilities

**Stream B (Systems):**
- Breach Saturation, progression, UI

**Stream C (Content):**
- Maps, props, bosses, contracts

**Stream D (Polish):**
- Audio, VFX, balance tuning

**Recommended Split:**
- Solo dev: Focus on Stream A (months 1-6), then B (7-9), then C (10-12), then D (13-15)
- Two devs: One on A/B, one on C/D starting month 7
- Three devs: One per stream starting month 4

---

## WEEKLY SCHEDULE TEMPLATE

**Assuming 10-15 hours/week:**

**Monday (2-3 hours):**
- Planning: Review weekly goals
- Implementation: Core feature work

**Tuesday (Skip or 1 hour):**
- Bug fixing or testing

**Wednesday (2-3 hours):**
- Implementation: Continue feature work

**Thursday (2-3 hours):**
- Implementation: Finish feature
- Testing: Playtest

**Friday (Skip or 1 hour):**
- Polish or bug fixing

**Saturday (3-4 hours):**
- Implementation: New feature or big task
- Testing: Playtest with friends

**Sunday (2-3 hours):**
- Polish and bug fixing
- Plan next week

**Total: 12-15 hours/week**

---

## SUCCESS CRITERIA PER PHASE

### Phase 1 Success (Prototype):
- âœ… Can shoot one gun at one enemy
- âœ… One ability feels impactful
- âœ… Core loop is fun (15-second cycle)
- âœ… Friends want to play again

### Phase 2 Success (Vertical Slice):
- âœ… Two classes feel different
- âœ… Three weapons have distinct roles
- âœ… Boss fight is exciting
- âœ… Demo-able to external playtesters

### Phase 3 Success (Content Complete):
- âœ… All 4 classes implemented
- âœ… All 8 weapons functional
- âœ… All enemies/bosses in game
- âœ… 30+ hours of gameplay content

### Phase 4 Success (Polish):
- âœ… 60 FPS stable
- âœ… All systems balanced
- âœ… Game feels polished, not janky
- âœ… <10 known bugs

### Phase 5 Success (Launch):
- âœ… 1,000+ sales Week 1
- âœ… 70%+ positive reviews
- âœ… No critical bugs
- âœ… Sustainable development path forward

---

## MOTIVATION MAINTENANCE

**How to stay motivated for 18 months:**

1. **Celebrate small wins** - Finished a weapon? Take a break
2. **Share progress publicly** - Devlog every 2 weeks
3. **Playtest with friends regularly** - Social feedback energizes
4. **Don't compare to AAA** - You're one person making something cool
5. **Take breaks** - Burnout kills projects
6. **Remember the vision** - This is YOUR game
7. **Join indie communities** - Others doing the same thing
8. **Track progress visually** - Checklist completion feels good

**Warning Signs You Need a Break:**
- Haven't coded in 2 weeks
- Everything feels bad
- Comparing to Hunt: Showdown and feeling hopeless
- Not having fun playing your own game

**What to do:** Take 1-2 weeks off completely. Come back fresh.

---

## EMERGENCY "SHIP SOMETHING" PLAN

**If you reach Month 18 and it's not done:**

**Minimum Viable Product:**
- 2 classes (Bulwark, Harpooner)
- 4 weapons (M1903, Double-Barrel, .38, Winchester)
- 3 enemies (Hollow-Eyed, Reef Walker, Deep One)
- 1 boss (Drowned Priest)
- 1 map zone (Lighthouse)
- Basic progression (weapon unlocks only)

**Ship this as "Early Access v0.1"**
- Be transparent about content plans
- Price at $15 instead of $20-25
- Update monthly with new content
- Build community trust

**Better to ship something than nothing.**

---

## POST-LAUNCH ROADMAP (Month 19+)

**Month 19-21: Stabilization**
- Fix critical bugs from player reports
- Balance based on player data
- QoL improvements from feedback

**Month 22-24: Content Update 1**
- Add 5th class
- Add 2 more weapons
- Add new map zone
- Add new boss

**Month 25-30: Content Update 2**
- Add 6th class (if needed)
- Add new game mode (e.g., endless survival)
- Add weapon variants/skins
- Add progression systems (achievements)

**Month 31+: Full Release v1.0**
- Polish all content
- Add final missing features
- Increase price to $25-30
- Full marketing push

---

## FINAL ADVICE

**Don't:**
- âŒ Add features not in this plan without removing others
- âŒ Start over when things get hard
- âŒ Try to compete with Hunt: Showdown's gun feel
- âŒ Perfectionism (done is better than perfect)

**Do:**
- âœ… Follow this roadmap (it's battle-tested)
- âœ… Playtest constantly (fun > features)
- âœ… Cut scope if falling behind (better small and polished)
- âœ… Ship something (even if not perfect)

**Remember:** You're building a class-based co-op extraction game with Lovecraftian horror. Not Hunt: Showdown. Not Deep Rock Galactic. Your game.

**Now go build it.** 🚀

---

## Related Documentation

**For technical implementation:**
- [Class Specifications](class-specifications.md)
- [Weapon Specifications](weapon-specifications.md)
- [Enemy Specifications](enemy-specifications.md)
- [Balance Parameters](balance-parameters.md)

**For design context:**
- [Combat System Design](../02-design-docs/combat-system-design.md)
- [Game Overview](../01-quick-reference/game-overview.md)
- [00-START-HERE](../00-START-HERE.md)

---

**This roadmap is realistic and achievable. Follow it, and in 18 months you'll have a shippable game.**
