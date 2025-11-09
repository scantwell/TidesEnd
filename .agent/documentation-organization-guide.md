# TIDE'S END: Documentation Organization Guide
## Optimizing Game Design Docs for AI-Assisted Development

**Purpose:** Structure game design documentation so Claude.ai can efficiently use it as context when generating code, answering questions, and making design decisions.

---

## The Problem with Current Organization

**What You Have Now:**
- `lovecraft-setting-redesign.md` (22,000 words, everything)
- `tide-end-quick-reference.md` (6,000 words, overview)
- `design-evolution-summary.md` (5,000 words, rationale)
- `tide-end-combat-system.md` (15,000 words, combat details)

**The Issue:**
- No clear hierarchy (which doc to read first?)
- Mixed concerns (narrative + technical specs in same doc)
- Hard to find specific info ("What's the Bulwark's anchor cooldown?")
- Context window limits (can't load all docs at once)

**The Solution:**
Organize docs into **three layers**: Quick Reference → System Design → Technical Specifications

---

## Recommended Folder Structure

```
tide-end/
├── 00-START-HERE.md                    # Master index, read this first
├── 01-quick-reference/
│   ├── game-overview.md                # 30-second pitch, core loop
│   ├── classes-quick-ref.md            # Class abilities at a glance
│   ├── weapons-quick-ref.md            # Weapon stats table
│   ├── enemies-quick-ref.md            # Enemy types table
│   └── systems-quick-ref.md            # All systems in brief
├── 02-design-docs/
│   ├── setting-and-narrative.md        # Lore, worldbuilding, atmosphere
│   ├── combat-system-design.md         # Combat philosophy, scenarios
│   ├── progression-system.md           # Unlocks, currency, class leveling
│   ├── extraction-mechanics.md         # Breach saturation, extraction points
│   └── roguelite-contracts.md          # Contract modifiers, difficulty
├── 03-technical-specs/
│   ├── class-specifications.md         # Exact ability parameters
│   ├── weapon-specifications.md        # Exact weapon stats
│   ├── enemy-specifications.md         # Exact enemy stats and AI
│   ├── balance-parameters.md           # All numerical values
│   └── implementation-priorities.md    # What to build when
├── 04-development/
│   ├── architecture-overview.md        # Code structure, patterns
│   ├── unity-setup-guide.md            # Project setup, packages
│   ├── coding-standards.md             # Naming conventions, style
│   └── testing-checklist.md            # QA and playtesting
└── 05-archive/
    ├── design-evolution.md             # How we got here
    └── rejected-ideas.md               # What didn't work, why
```

---

## Layer 1: Quick Reference (When to Use)

**Purpose:** Fast lookups, at-a-glance info, no narrative fluff

**Use When:**
- "What's the Lamplighter's flare cooldown?"
- "How much damage does a Thompson do?"
- "What's the core gameplay loop?"
- Claude needs quick facts mid-conversation

**Format Rules:**
- Tables and bullet points ONLY
- No paragraphs or explanations
- Just numbers, names, and brief descriptions
- 1-2 pages max per document

**Example: `classes-quick-ref.md`**
```markdown
# Classes Quick Reference

## Bulwark (Tank)
**Passive:** Steadfast - 130 HP, +15% resist below 50% HP, -50% knockback
**Ability 1:** Anchor Point (60s CD) - 10m zone, 20s duration, stops saturation, slows enemies 30%
**Ability 2:** Bastion Shield (90s CD) - 500 HP shield, blocks front 120°, 30s duration

## Lamplighter (Scout)
**Passive:** Pathfinder - +20% speed, +30% fog vision, -40% footstep noise
**Ability 1:** Flare Shot (45s CD) - 20m radius, 30s duration, reveals enemies through walls
**Ability 2:** Pathfinder's Mark (60s CD) - 15m zone, 20s duration, +40% move speed

[etc...]
```

---

## Layer 2: Design Documents (When to Use)

**Purpose:** Understand WHY systems work this way, design philosophy, examples

**Use When:**
- Implementing a new system from scratch
- Understanding how systems interact
- Seeing practical examples (combat scenarios)
- Making design decisions

**Format Rules:**
- Mix of explanation and examples
- Include rationale ("we chose this because...")
- Show scenarios and use cases
- Reference quick-ref docs for exact numbers
- 3,000-8,000 words per document

**Example: `combat-system-design.md`**
```markdown
# Combat System Design

## Philosophy
Guns deal damage (60%), abilities create opportunities (30%), equipment supplements (10%).

[Explanation of why this split works...]

## The 15-Second Combat Loop
[Detailed breakdown with examples...]

## Example Scenario: Lighthouse Defense
[Full walkthrough showing how systems interact...]

**For exact ability cooldowns and damage numbers, see:** `03-technical-specs/class-specifications.md`
```

---

## Layer 3: Technical Specifications (When to Use)

**Purpose:** Generate code, implement systems, balance tuning

**Use When:**
- Writing actual code
- Implementing abilities, weapons, enemies
- Balancing systems
- Need exact parameters

**Format Rules:**
- ONLY technical specifications
- Zero narrative or design philosophy
- Every number, every formula, every parameter
- Code-ready format (easy to copy-paste)
- Tables and data structures

**Example: `class-specifications.md`**
```markdown
# Class Technical Specifications

## Bulwark

### Passive: Steadfast
```json
{
  "name": "Steadfast",
  "maxHealth": 130,
  "damageResistBelowHalfHP": 0.15,
  "knockbackResistance": 0.50
}
```

### Ability 1: Anchor Point
```json
{
  "name": "Anchor Point",
  "cooldown": 60,
  "castTime": 0.5,
  "throwDistance": 15,
  "radius": 10,
  "duration": 20,
  "effects": {
    "stopsSaturationIncrease": true,
    "enemySlowPercent": 0.30,
    "preventKnockback": true
  },
  "visual": {
    "prefab": "AnchorPointDevice",
    "particleEffect": "GoldenShimmerZone",
    "audioLoop": "RealityStabilization"
  }
}
```

### Implementation Notes
- Anchor device is throwable rigidbody
- Zone is trigger collider (sphere)
- Check enemies in zone every 0.1s, apply slow
- Visual shimmer uses shader "Assets/Shaders/ZoneEffect.shader"

[etc...]
```

---

## The Master Index: `00-START-HERE.md`

This is the single source of truth for navigating all docs.

```markdown
# TIDE'S END: Documentation Master Index

## New to the Project? Start Here:
1. Read `01-quick-reference/game-overview.md` (2 min) - Understand what you're building
2. Read `02-design-docs/combat-system-design.md` (15 min) - Understand core gameplay
3. Read `03-technical-specs/implementation-priorities.md` (5 min) - Know what to build first

## Quick Lookups (Use These Most Often):
- **Class Info:** `01-quick-reference/classes-quick-ref.md`
- **Weapon Stats:** `01-quick-reference/weapons-quick-ref.md`
- **Enemy Stats:** `01-quick-reference/enemies-quick-ref.md`

## Implementing a Specific System? Go To:
- **Classes/Abilities:** `03-technical-specs/class-specifications.md`
- **Weapons:** `03-technical-specs/weapon-specifications.md`
- **Enemies/AI:** `03-technical-specs/enemy-specifications.md`
- **Progression:** `02-design-docs/progression-system.md`

## Understanding Design Decisions? Go To:
- **Why This Combat System:** `02-design-docs/combat-system-design.md`
- **Why This Setting:** `02-design-docs/setting-and-narrative.md`
- **Design Evolution:** `05-archive/design-evolution.md`

## Document Dependency Graph:
```
START-HERE.md
    ↓
game-overview.md (read first)
    ↓
    ├─→ combat-system-design.md (understand design)
    │       ↓
    │       └─→ class-specifications.md (implement code)
    │
    ├─→ setting-and-narrative.md (understand theme)
    │
    └─→ progression-system.md (understand meta-loop)
            ↓
            └─→ balance-parameters.md (tune numbers)
```

## For Claude.ai:
**When Asked About:** → **Load This Doc:**
- Class abilities → `classes-quick-ref.md` OR `class-specifications.md` (depends on detail needed)
- Weapons → `weapons-quick-ref.md` OR `weapon-specifications.md`
- Enemies → `enemies-quick-ref.md` OR `enemy-specifications.md`
- Combat flow → `combat-system-design.md`
- Code architecture → `architecture-overview.md`

## Last Updated: [Date]
```

---

## How to Use This with Claude.ai

### Scenario 1: Quick Question

**User:** "What's the Bulwark's shield cooldown?"

**Claude's Process:**
1. Check `classes-quick-ref.md` (fast lookup)
2. Answer: "90 seconds"
3. Done (no need to load full design doc)

### Scenario 2: Implement a Feature

**User:** "Implement the Bulwark's Anchor Point ability"

**Claude's Process:**
1. Load `class-specifications.md` (exact parameters)
2. Check `architecture-overview.md` (code patterns to follow)
3. Check `coding-standards.md` (naming conventions)
4. Generate code using exact specs

### Scenario 3: Design Discussion

**User:** "Why did we choose 60/40 split for guns vs abilities?"

**Claude's Process:**
1. Load `combat-system-design.md` (has the rationale)
2. Explain philosophy with examples
3. Reference `design-evolution.md` if user wants to know alternatives considered

### Scenario 4: Balance Tuning

**User:** "Is the Thompson too strong?"

**Claude's Process:**
1. Load `weapon-specifications.md` (Thompson stats)
2. Load `balance-parameters.md` (DPS tiers)
3. Compare Thompson DPS to tier expectations
4. Suggest adjustment with reasoning

---

## Document Templates

### Quick Reference Template

```markdown
# [System Name] Quick Reference

**Purpose:** [One sentence - what this system does]

## Key Stats
| Stat | Value |
|------|-------|
| [Name] | [Value] |

## Important Numbers
- **[Parameter]:** [Value]
- **[Parameter]:** [Value]

## Related Docs
- **Design Rationale:** `02-design-docs/[doc].md`
- **Full Specs:** `03-technical-specs/[doc].md`
```

### Design Document Template

```markdown
# [System Name] Design Document

## Overview
[High-level description of what this system is and why it exists]

## Design Goals
- Goal 1: [Why]
- Goal 2: [Why]

## How It Works
[Detailed explanation with examples]

## Example Scenarios
### Scenario 1: [Name]
[Step-by-step walkthrough]

## Design Decisions
**Why We Chose X Over Y:**
[Rationale]

## Related Systems
- **Depends On:** [System A, System B]
- **Affects:** [System C, System D]

## Technical Implementation
**For exact parameters:** See `03-technical-specs/[spec-doc].md`
**For code structure:** See `04-development/architecture-overview.md`
```

### Technical Specification Template

```markdown
# [System Name] Technical Specifications

## Data Structures

### [Structure Name]
```json
{
  "parameter": value,
  "parameter": value
}
```

## Formulas

**[Formula Name]:**
```
result = (parameter1 * parameter2) + parameter3
```

## Implementation Notes
- **File Location:** `Assets/Scripts/[Path]`
- **Dependencies:** [List]
- **Initialization:** [When/how]

## Constants
```csharp
public const float PARAMETER_NAME = 1.5f;
public const int PARAMETER_NAME = 100;
```

## Testing Checklist
- [ ] Test case 1
- [ ] Test case 2
```

---

## Converting Your Current Docs

### Step 1: Create the Structure
Create all the folders and the master index first.

### Step 2: Split `lovecraft-setting-redesign.md`

**Extract to:**
- `game-overview.md` → 30-second pitch, core loop, setting summary (500 words)
- `classes-quick-ref.md` → Just class names, abilities, cooldowns (1 page)
- `weapons-quick-ref.md` → Just weapon names, damage, fire rate (1 page)
- `enemies-quick-ref.md` → Just enemy names, HP, damage, behavior (1 page)
- `setting-and-narrative.md` → All the lore, atmosphere, worldbuilding (5,000 words)
- `combat-system-design.md` → Combat philosophy, scenarios (8,000 words)
- `extraction-mechanics.md` → Breach saturation, extraction points (3,000 words)
- `class-specifications.md` → Exact ability parameters, JSON format (2,000 words)
- `weapon-specifications.md` → Exact weapon stats, tables (1,500 words)
- `enemy-specifications.md` → Exact enemy stats, AI behavior (2,000 words)

### Step 3: Create New Docs

**New docs you need:**
- `balance-parameters.md` → All the numbers in tables (DPS tiers, HP scaling, cooldown tiers)
- `architecture-overview.md` → How code should be structured (MonoBehaviours, ScriptableObjects, patterns)
- `unity-setup-guide.md` → Packages, project settings, folder structure
- `implementation-priorities.md` → What to build when (prototype → EA roadmap)

### Step 4: Cross-Reference Everything

Add "Related Docs" sections to every document pointing to related content.

---

## Practical Example: Full Documentation Set

Let me show you what ONE system looks like across all three layers:

### Example System: Bulwark's Anchor Point

**Layer 1: Quick Reference** (`classes-quick-ref.md`)
```markdown
## Bulwark
**Ability 1:** Anchor Point (60s CD) - 10m zone, stops saturation, slows enemies 30%
```

**Layer 2: Design Doc** (`combat-system-design.md`)
```markdown
### Active Ability 1: "Anchor Point"
**Cooldown:** 60 seconds
**Type:** Deployable zone effect

**Strategic Uses:**
- Drop during extraction channel (stabilize reality)
- Plant at chokepoint before wave (slow enemy push)
- Use mid-boss fight (creates safe damage window)
- Combine with shield for fortress position

**Skill Expression:**
- Placement timing (pre-emptive vs. reactive)
- Reading enemy spawns (where will swarm come from?)

**Example:** In the Lighthouse defense scenario, Bulwark drops Anchor at center platform before lighting beacon, creating a stable zone that slows all approaching enemies by 30% for 20 seconds.

**For exact parameters:** See `class-specifications.md`
```

**Layer 3: Technical Spec** (`class-specifications.md`)
```markdown
### Ability 1: Anchor Point

#### Parameters
```json
{
  "name": "Anchor Point",
  "abilityID": "bulwark_anchor",
  "cooldown": 60.0,
  "castTime": 0.5,
  "throwRange": 15.0,
  "zoneRadius": 10.0,
  "zoneDuration": 20.0,
  "effects": {
    "preventSaturationIncrease": true,
    "enemyMovementSlowPercent": 0.30,
    "allyKnockbackImmunity": true
  }
}
```

#### Implementation
**Script:** `Assets/Scripts/Abilities/BulwarkAnchorPoint.cs`
**Prefab:** `Assets/Prefabs/Abilities/AnchorPointDevice.prefab`

```csharp
public class BulwarkAnchorPoint : Ability
{
    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private float throwRange = 15f;
    [SerializeField] private float zoneRadius = 10f;
    [SerializeField] private float zoneDuration = 20f;
    
    public override void Activate()
    {
        // Calculate throw position
        Vector3 throwTarget = GetThrowPosition(throwRange);
        
        // Instantiate anchor device
        GameObject anchor = Instantiate(anchorPrefab, throwTarget, Quaternion.identity);
        
        // Setup zone effect
        AnchorZone zone = anchor.GetComponent<AnchorZone>();
        zone.Initialize(zoneRadius, zoneDuration);
        
        // Start cooldown
        StartCooldown();
    }
}
```

#### Testing Checklist
- [ ] Anchor throws to max range (15m)
- [ ] Zone radius is 10m (use Debug.DrawSphere)
- [ ] Saturation stops increasing inside zone
- [ ] Enemies slow to 70% speed inside zone
- [ ] Allies cannot be knocked back inside zone
- [ ] Zone expires after 20 seconds
- [ ] Visual FX appear correctly
- [ ] Audio plays on deployment
```

---

## Benefits of This Organization

### For You (Developer)
- **Find info fast:** "Where's the Thompson damage?" → `weapons-quick-ref.md` (5 seconds)
- **Implement faster:** Need exact specs? → `weapon-specifications.md` (copy-paste ready)
- **Understand context:** Why this system? → `combat-system-design.md` (full rationale)

### For Claude.ai
- **Smaller context windows:** Load only relevant docs per question
- **Faster responses:** Quick refs = instant answers
- **Better code generation:** Technical specs are code-ready
- **Consistent answers:** Single source of truth per topic

### For Collaboration
- **Onboard teammates:** New dev? Read `game-overview.md` → done
- **Review systems:** "Is Bulwark balanced?" → Check all three layers
- **Track changes:** Version control each doc separately

---

## Naming Conventions

### File Names
- Use kebab-case: `class-specifications.md`
- Include layer prefix: `01-quick-reference/`
- Be descriptive: `balance-parameters.md` not `numbers.md`

### Document Titles
- Use Title Case: "Bulwark Class Specification"
- Include system name: "Combat System Design" not "Design"

### Section Headers
- Use ## for major sections
- Use ### for subsections
- Use #### for implementation details

### Cross-References
Always use relative paths:
```markdown
**See also:** `03-technical-specs/class-specifications.md`
```

Not:
```markdown
**See also:** The class spec document
```

---

## Maintenance Rules

### When Adding New Content

**Ask yourself:**
1. **Is this a quick lookup?** → Quick reference
2. **Is this explaining WHY?** → Design doc
3. **Is this exact parameters for code?** → Technical spec

**Example:** Adding a new enemy

1. Add row to `enemies-quick-ref.md` (name, HP, damage)
2. Add section to `combat-system-design.md` (role, behavior, counters)
3. Add full spec to `enemy-specifications.md` (JSON, AI state machine, implementation)
4. Update `00-START-HERE.md` if it's a major addition

### When Changing Systems

**Update in order:**
1. Technical spec (source of truth)
2. Quick reference (update stats)
3. Design doc (update examples if needed)
4. Master index (if dependencies changed)

### When Something Gets Too Long

**Split it:**
- `combat-system-design.md` → Split into `combat-core.md` + `combat-scenarios.md`
- `class-specifications.md` → Split into per-class files

**Update master index** to reflect new structure.

---

## Tools to Help

### Document Generation Scripts

You could create scripts to auto-generate quick reference tables from technical specs:

```python
# extract_quick_ref.py
# Reads class-specifications.md JSON blocks
# Generates classes-quick-ref.md table automatically
```

### Validation Scripts

```python
# validate_cross_references.py
# Checks that all cross-references point to existing files
# Warns if a doc references non-existent file
```

### Documentation Linter

```python
# lint_docs.py
# Checks that each doc follows template
# Verifies naming conventions
# Ensures "Related Docs" sections exist
```

---

## Migration Checklist

**Converting your current docs to this structure:**

- [ ] Create folder structure
- [ ] Create `00-START-HERE.md` with index
- [ ] Extract `game-overview.md` from existing docs
- [ ] Create all three quick reference docs (classes, weapons, enemies)
- [ ] Split `lovecraft-setting-redesign.md` into design docs
- [ ] Create all technical specification docs
- [ ] Create `balance-parameters.md` (extract all numbers into tables)
- [ ] Create `architecture-overview.md` (code structure)
- [ ] Create `implementation-priorities.md` (roadmap)
- [ ] Add cross-references to every document
- [ ] Move old docs to `05-archive/`
- [ ] Test by asking Claude a question and seeing if it loads right doc
- [ ] Update this guide as you discover better organization

---

## Example Claude.ai Prompt with New Structure

**Before (inefficient):**
```
User: "Implement the Bulwark class"

Claude: "I'll need to read your design docs first..."
[Loads 22,000-word document]
[Searches for Bulwark info]
[Generates code]
```

**After (efficient):**
```
User: "Implement the Bulwark class"

Claude: "I'll use the technical specifications..."
[Loads class-specifications.md - just Bulwark section, 500 words]
[Loads architecture-overview.md - code patterns, 1,000 words]
[Generates code following exact specs and patterns]
```

**Tokens saved:** ~20,000 tokens = faster response + more context for actual code generation

---

## Final Recommendations

### Start With This
1. Create the folder structure
2. Create `00-START-HERE.md` master index
3. Create the three quick reference docs (classes, weapons, enemies)
4. These alone will make 90% of questions faster

### Then Do This
5. Extract technical specs (classes, weapons, enemies)
6. Create `balance-parameters.md`
7. Now code generation will be much faster

### Finally Do This
8. Split design docs properly
9. Create development docs
10. Archive old docs

### Don't Overthink It
- Start simple, iterate
- If a doc gets confusing, split it
- If you reference something 3+ times, make a quick ref for it
- The goal is **fast lookups**, not perfect organization

---

## Summary

**Three Layers:**
1. **Quick Reference** - Fast lookups, tables only
2. **Design Docs** - Understand why, see examples
3. **Technical Specs** - Code-ready parameters

**Master Index:**
- Single source of truth
- Tells you which doc to load
- Shows dependencies

**Benefits:**
- Find info in seconds
- Claude loads smaller contexts
- Code generation is faster
- Team onboarding is easier

**Next Step:**
Create `00-START-HERE.md` and the three quick reference docs. That alone will make your life easier immediately.

---

**Want me to generate any of these documents for you?** I can create:
- The master index
- Any of the quick reference docs
- The technical specification versions
- The folder structure as a script

Just let me know which you want first!
