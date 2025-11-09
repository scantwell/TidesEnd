# .agent Documentation Index

**Last Updated**: 2025-10-28

This directory contains all project documentation for **Tide's End**. Documentation is organized into specialized folders based on purpose.

---

## 📖 Master Entry Point

**START HERE**: [docs/00-START-HERE.md](docs/00-START-HERE.md)

This is your complete guide to navigating all game design documentation.

---

## 📁 Current Documentation Structure

### `docs/` - Complete Game Design Documentation

**Purpose**: Everything about WHAT we're building - game mechanics, balance, design philosophy, and technical specifications.

**Owner**: Game Design Team
**Entry Point**: [docs/00-START-HERE.md](docs/00-START-HERE.md)

**Organized in 5 layers:**

| Layer | Folder | Purpose | When to Use |
|-------|--------|---------|-------------|
| **Quick Reference** | `01-quick-reference/` | Fast stat lookups | "What's the cooldown?" |
| **Design Docs** | `02-design-docs/` | Design philosophy | "Why does it work this way?" |
| **Technical Specs** | `03-technical-specs/` | Code-ready parameters | "How do I implement this?" |
| **Development** | `04-development/` | Code architecture, standards | "How should I structure code?" |
| **Archive** | `05-archive/` | Design history | "Why did we change this?" |

---

## 🚀 Quick Navigation

### New to the Project?

1. Read [Game Overview](docs/01-quick-reference/game-overview.md) (2 min) - What is Tide's End?
2. Read [Combat System Design](docs/02-design-docs/combat-system-design.md) (15 min) - Core gameplay
3. Read [Implementation Priorities](docs/03-technical-specs/implementation-priorities.md) (3 min) - What to build first

### Looking Up Stats?

| Need | Go To |
|------|-------|
| Class abilities | [classes-quick-ref.md](docs/01-quick-reference/classes-quick-ref.md) |
| Weapon stats | [weapons-quick-ref.md](docs/01-quick-reference/weapons-quick-ref.md) |
| Enemy stats | [enemies-quick-ref.md](docs/01-quick-reference/enemies-quick-ref.md) |
| System overviews | [systems-quick-ref.md](docs/01-quick-reference/systems-quick-ref.md) |

### Implementing a Feature?

1. **Understand WHAT**: Check `docs/02-design-docs/` for design philosophy
2. **Get SPECS**: Check `docs/03-technical-specs/` for exact parameters
3. **Code HOW**: Check `docs/04-development/` for code patterns
4. **Follow Unity Standards**: Check [CLAUDE.md](../CLAUDE.md) for Unity-specific commands

---

---

## 📂 Active Documentation Folders

### `Tasks/` - Feature Implementation Plans

**Purpose**: PRDs (Product Requirements Documents) and implementation plans for specific features

**When to create**: Before implementing major features

**Current Tasks:**
- [Ability System](Tasks/ability-system/implementation-plan.md) - Universal ability system for players, enemies, and bosses

**Structure**:
```
Tasks/
├── {feature-name}/
│   ├── prd.md              # Requirements (optional)
│   ├── implementation-plan.md   # Technical plan
│   └── testing.md          # Test strategy (optional)
```

---

## 📂 Future Documentation Folders

### `SOP/` *(Planned)*

**Purpose**: Standard Operating Procedures - step-by-step guides for common tasks

**When to create**: After solving tricky problems or establishing patterns

**Examples**:
- How to add a networked component
- How to create a new weapon
- How to debug multiplayer issues
- How to test with Steam locally

---

## 🔍 Finding Information Fast

| I Need To... | Look In... |
|-------------|-----------|
| Understand what Tide's End is | `docs/01-quick-reference/game-overview.md` |
| Look up a specific stat | `docs/01-quick-reference/{classes\|weapons\|enemies}-quick-ref.md` |
| Understand why a system works this way | `docs/02-design-docs/` |
| Get exact parameters for code | `docs/03-technical-specs/` |
| Understand code architecture | `docs/04-development/architecture-overview.md` (if exists) |
| Learn Unity development commands | `../CLAUDE.md` |
| Find what to build first | `docs/03-technical-specs/implementation-priorities.md` |
| Get implementation plan for a feature | `Tasks/{feature-name}/implementation-plan.md` |

---

## 📝 Documentation Maintenance

### When to Update

**Update `docs/` when:**
- Game design changes (balance, mechanics, features)
- Adding/removing/changing classes, weapons, enemies
- Modifying core systems
- Adding new code architecture patterns

**Update this README when:**
- Adding new documentation folders (`Tasks/`, `SOP/`)
- Restructuring documentation
- Adding major new documents

### How to Update

1. Edit the relevant documentation file(s)
2. Update "Last Updated" timestamp in the file
3. If adding new folders or major docs, update this README
4. Update `docs/00-START-HERE.md` if navigation changes
5. Commit with descriptive message

---

## 🤖 For Claude Code / AI Assistants

**When user asks about game mechanics or stats:**
→ Start with `docs/01-quick-reference/`, escalate to design docs if needed

**When user asks about implementation:**
→ Load `docs/03-technical-specs/` for parameters + `docs/04-development/` for patterns

**When user asks "how do I implement X?":**
→ Load: design spec + technical spec + architecture overview

**Priority**: Always load the **smallest sufficient document**. Quick refs for stats, design docs for philosophy, technical specs for implementation.

---

## 📦 Current File Structure

```
.agent/
├── README.md (this file)
├── documentation-organization-guide.md
├── Tasks/
│   └── ability-system/
│       └── implementation-plan.md
└── docs/
    ├── 00-START-HERE.md
    ├── 01-quick-reference/
    │   ├── game-overview.md
    │   ├── classes-quick-ref.md
    │   ├── weapons-quick-ref.md
    │   ├── enemies-quick-ref.md
    │   └── systems-quick-ref.md
    ├── 02-design-docs/
    │   ├── combat-system-design.md
    │   ├── extraction-mechanics.md
    │   ├── progression-system.md
    │   └── roguelite-contracts.md
    ├── 03-technical-specs/
    │   ├── class-specifications.md
    │   ├── weapon-specifications.md
    │   ├── enemy-specifications.md
    │   ├── balance-parameters.md
    │   └── implementation-priorities.md
    ├── 04-development/
    └── 05-archive/
```

---

**Maintained By**: Development Team
**Questions?** Check [docs/00-START-HERE.md](docs/00-START-HERE.md) first, then ask.
