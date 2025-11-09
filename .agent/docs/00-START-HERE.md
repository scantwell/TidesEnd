# TIDE'S END: Documentation Master Index

## Your Complete Game Design Reference

**Last Updated:** October 27, 2025  
**Game Version:** Pre-Alpha / Design Phase  
**Status:** Core design locked, ready for implementation

---

## 🚀 NEW TO THE PROJECT? START HERE

**Read these three documents in order (20 minutes total):**

1. **[Game Overview](01-quick-reference/game-overview.md)** (2 min)
   - 30-second pitch, core concept, target audience
2. **[Combat System Design](02-design-docs/combat-system-design.md)** (15 min)
   - Understand the core gameplay loop and philosophy
3. **[Implementation Priorities](03-technical-specs/implementation-priorities.md)** (3 min)
   - Know what to build first

After reading these, you'll understand what Tide's End is and how to start building it.

---

## 📋 QUICK LOOKUPS (Use These Most Often)

**Need fast facts? Use the quick reference docs:**

| What You Need                                   | Go Here                                                      |
| ----------------------------------------------- | ------------------------------------------------------------ |
| Class abilities, passives, cooldowns            | [Classes Quick Ref](01-quick-reference/classes-quick-ref.md) |
| Weapon stats, damage, fire rates                | [Weapons Quick Ref](01-quick-reference/weapons-quick-ref.md) |
| Enemy HP, damage, behaviors                     | [Enemies Quick Ref](01-quick-reference/enemies-quick-ref.md) |
| System overviews (saturation, extraction, etc.) | [Systems Quick Ref](01-quick-reference/systems-quick-ref.md) |

**These are 1-2 page reference tables. Perfect for "What's the cooldown?" questions.**

---

## 🛠️ IMPLEMENTING A SYSTEM? GO TO TECHNICAL SPECS

**Need exact parameters for code? Use the technical specification docs:**

| System to Implement   | Technical Spec Doc                                                   |
| --------------------- | -------------------------------------------------------------------- |
| Classes and abilities | [Class Specifications](03-technical-specs/class-specifications.md)   |
| Weapons and gunplay   | [Weapon Specifications](03-technical-specs/weapon-specifications.md) |
| Enemies and AI        | [Enemy Specifications](03-technical-specs/enemy-specifications.md)   |
| All numerical values  | [Balance Parameters](03-technical-specs/balance-parameters.md)       |

**These contain JSON-ready data structures, exact formulas, and implementation notes.**

---

## 📖 UNDERSTANDING DESIGN DECISIONS? READ DESIGN DOCS

**Want to know WHY systems work this way? Use the design documents:**

| Topic                          | Design Doc                                                       |
| ------------------------------ | ---------------------------------------------------------------- |
| Combat philosophy, scenarios   | [Combat System Design](02-design-docs/combat-system-design.md)   |
| Setting, lore, atmosphere      | [Setting and Narrative](02-design-docs/setting-and-narrative.md) |
| Progression, unlocks, leveling | [Progression System](02-design-docs/progression-system.md)       |
| Breach saturation, extraction  | [Extraction Mechanics](02-design-docs/extraction-mechanics.md)   |
| Contract modifiers             | [Roguelite Contracts](02-design-docs/roguelite-contracts.md)     |

**These explain rationale, provide examples, and show how systems interact.**

---

## 💻 DEVELOPMENT RESOURCES

**Technical setup and coding standards:**

| Resource                  | Document                                                         |
| ------------------------- | ---------------------------------------------------------------- |
| Code structure, patterns  | [Architecture Overview](04-development/architecture-overview.md) |
| Unity project setup       | [Unity Setup Guide](04-development/unity-setup-guide.md)         |
| Naming conventions, style | [Coding Standards](04-development/coding-standards.md)           |
| QA and playtesting        | [Testing Checklist](04-development/testing-checklist.md)         |

---

## 📚 DOCUMENT DEPENDENCY MAP

**Visual guide to which documents reference each other:**

```
START-HERE.md (you are here)
    ↓
game-overview.md ──────────────┐
    ↓                          │
    ├─→ combat-system-design.md ───→ class-specifications.md ──→ CODE
    │                          │
    ├─→ setting-and-narrative.md   │
    │                          │
    ├─→ progression-system.md ─────→ balance-parameters.md ──→ CODE
    │                          │
    └─→ extraction-mechanics.md ───→ enemy-specifications.md ──→ CODE
                               │
                               └─→ weapon-specifications.md ──→ CODE
```

**Flow:**

1. Quick references for fast lookups
2. Design docs to understand systems
3. Technical specs to implement code

---

## 🔍 SEARCH BY TOPIC

### Classes

- **Quick Ref:** [classes-quick-ref.md](01-quick-reference/classes-quick-ref.md)
- **Design:** [combat-system-design.md](02-design-docs/combat-system-design.md) (Class sections)
- **Technical:** [class-specifications.md](03-technical-specs/class-specifications.md)

### Weapons

- **Quick Ref:** [weapons-quick-ref.md](01-quick-reference/weapons-quick-ref.md)
- **Design:** [combat-system-design.md](02-design-docs/combat-system-design.md) (Gunplay section)
- **Technical:** [weapon-specifications.md](03-technical-specs/weapon-specifications.md)

### Enemies

- **Quick Ref:** [enemies-quick-ref.md](01-quick-reference/enemies-quick-ref.md)
- **Design:** [combat-system-design.md](02-design-docs/combat-system-design.md) (Enemy Design section)
- **Technical:** [enemy-specifications.md](03-technical-specs/enemy-specifications.md)

### Breach Saturation

- **Quick Ref:** [systems-quick-ref.md](01-quick-reference/systems-quick-ref.md) (Saturation section)
- **Design:** [extraction-mechanics.md](02-design-docs/extraction-mechanics.md)
- **Technical:** [balance-parameters.md](03-technical-specs/balance-parameters.md) (Saturation parameters)

### Progression

- **Quick Ref:** [systems-quick-ref.md](01-quick-reference/systems-quick-ref.md) (Progression section)
- **Design:** [progression-system.md](02-design-docs/progression-system.md)
- **Technical:** [balance-parameters.md](03-technical-specs/balance-parameters.md) (Unlock costs)

---

## 🎯 COMMON QUESTIONS → WHERE TO LOOK

**"What's the Bulwark's Anchor Point cooldown?"**
→ [classes-quick-ref.md](01-quick-reference/classes-quick-ref.md)

**"How do I implement the Harpooner's pull ability?"**
→ [class-specifications.md](03-technical-specs/class-specifications.md)

**"Why did we choose 60/40 gunplay/abilities split?"**
→ [combat-system-design.md](02-design-docs/combat-system-design.md)

**"What damage does the Thompson SMG do?"**
→ [weapons-quick-ref.md](01-quick-reference/weapons-quick-ref.md) or [weapon-specifications.md](03-technical-specs/weapon-specifications.md)

**"How does Breach Saturation affect enemy spawns?"**
→ [extraction-mechanics.md](02-design-docs/extraction-mechanics.md)

**"What should I build first?"**
→ [implementation-priorities.md](03-technical-specs/implementation-priorities.md)

**"How does class XP work?"**
→ [progression-system.md](02-design-docs/progression-system.md)

**"What's the game's setting and story?"**
→ [setting-and-narrative.md](02-design-docs/setting-and-narrative.md)

---

## 🤖 FOR CLAUDE.AI: USAGE GUIDE

**When the user asks about...**

| Question Type            | Load This Doc                        |
| ------------------------ | ------------------------------------ |
| Quick stat lookup        | Quick reference (1 min read)         |
| How to implement X       | Technical specification (5 min read) |
| Why does X work this way | Design document (10 min read)        |
| Code structure/patterns  | Development docs                     |

**Priority:** Always load the SMALLEST sufficient document. Don't load full design docs for simple stat lookups.

**Example:**

- "What's the Lamplighter's flare cooldown?" → Load `classes-quick-ref.md` only
- "Implement Lamplighter's flare" → Load `class-specifications.md` + `architecture-overview.md`
- "Why does Lamplighter have a flare?" → Load `combat-system-design.md`

---

## 📦 ALL DOCUMENTS BY LAYER

### Layer 1: Quick Reference (Fast Lookups)

- [game-overview.md](01-quick-reference/game-overview.md) - 30-second pitch, core loop
- [classes-quick-ref.md](01-quick-reference/classes-quick-ref.md) - Class stats table
- [weapons-quick-ref.md](01-quick-reference/weapons-quick-ref.md) - Weapon stats table
- [enemies-quick-ref.md](01-quick-reference/enemies-quick-ref.md) - Enemy stats table
- [systems-quick-ref.md](01-quick-reference/systems-quick-ref.md) - All systems overview

### Layer 2: Design Documents (Understanding)

- [combat-system-design.md](02-design-docs/combat-system-design.md) - Combat philosophy
- [setting-and-narrative.md](02-design-docs/setting-and-narrative.md) - Lore and atmosphere
- [progression-system.md](02-design-docs/progression-system.md) - Unlocks and leveling
- [extraction-mechanics.md](02-design-docs/extraction-mechanics.md) - Core loop mechanics
- [roguelite-contracts.md](02-design-docs/roguelite-contracts.md) - Contract modifiers

### Layer 3: Technical Specifications (Implementation)

- [class-specifications.md](03-technical-specs/class-specifications.md) - Exact ability parameters
- [weapon-specifications.md](03-technical-specs/weapon-specifications.md) - Exact weapon stats
- [enemy-specifications.md](03-technical-specs/enemy-specifications.md) - Exact enemy stats
- [balance-parameters.md](03-technical-specs/balance-parameters.md) - All numerical values
- [implementation-priorities.md](03-technical-specs/implementation-priorities.md) - Build order

### Layer 4: Development (Coding)

- [architecture-overview.md](04-development/architecture-overview.md) - Code structure
- [unity-setup-guide.md](04-development/unity-setup-guide.md) - Project setup
- [coding-standards.md](04-development/coding-standards.md) - Style guide
- [testing-checklist.md](04-development/testing-checklist.md) - QA guidelines

### Layer 5: Archive (Context)

- [design-evolution.md](05-archive/design-evolution.md) - How we got here
- [rejected-ideas.md](05-archive/rejected-ideas.md) - What didn't work

---

## 🔄 KEEPING DOCS UPDATED

**When making changes:**

1. **Update technical spec first** (source of truth)
2. **Update quick reference** (sync stats)
3. **Update design doc if needed** (explain new rationale)
4. **Update this index** (if adding/removing docs)

**Version control:**

- Each doc has "Last Updated" date at top
- Track major changes in git commits
- Add changelog section to docs if frequent updates

---

## 📝 DOCUMENT STATUS LEGEND

**Status tags you'll see:**

- ✅ **Locked** - Core design finalized, ready for implementation
- 🚧 **In Progress** - Being written/refined
- 📋 **Needs Review** - Complete but needs validation
- 🔄 **Living Doc** - Frequently updated (like balance parameters)

---

## 🎮 THE GAME AT A GLANCE

**Tide's End** is a 2-4 player co-op extraction shooter set in 1920s Lovecraftian New England.

**Core Loop:**

- Choose your class (Bulwark, Lamplighter, Harpooner, Occultist)
- Deploy into breach zones via rowboat
- Use abilities + period weapons to fight cosmic horrors
- Collect artifacts and currency
- Extract before reality collapses
- Spend currency to unlock better weapons
- Level up your class to upgrade abilities

**Key Differentiators:**

- 60% gunplay, 40% class abilities (hybrid approach)
- Breach Saturation system (dynamic difficulty)
- Maritime Lovecraftian setting (unique IP)
- Pure PvE co-op (no PvP toxicity)
- Team synergy through ability combos

**Target Audience:**

- Deep Rock Galactic players (class-based co-op)
- Hunt: Showdown players (extraction tension)
- Helldivers 2 players (team abilities)
- Bloodborne fans (gothic horror)
- Anyone wanting PvE extraction without cheaters

---

## 🏁 NEXT STEPS

**New developer onboarding:**

1. Read game overview (2 min)
2. Read combat system design (15 min)
3. Set up Unity project using setup guide
4. Build prototype (Week 1-2 priorities in implementation doc)
5. Reference quick refs and specs as you code

**Ready to build?** Start with [Implementation Priorities](03-technical-specs/implementation-priorities.md)

**Have questions?** Check if the answer is in quick references first, then design docs, then ask.

---

**Last Updated:** October 27, 2025  
**Maintained By:** Development Team  
**Feedback:** Submit via issues/discussions
