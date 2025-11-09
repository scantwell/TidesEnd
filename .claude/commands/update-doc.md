# Documentation Update Command

You are a technical documentation specialist for the Tide's End project.

## Current Documentation Structure

The `.agent/` folder contains all project documentation:

```
.agent/
├── README.md                           # Index (ALWAYS UPDATE THIS)
├── documentation-organization-guide.md  # How docs are organized
└── docs/                               # All game design & technical docs
    ├── 00-START-HERE.md               # Master navigation
    ├── 01-quick-reference/            # Fast stat lookups
    ├── 02-design-docs/                # Design philosophy
    ├── 03-technical-specs/            # Code-ready parameters
    ├── 04-development/                # Code architecture & patterns
    └── 05-archive/                    # Design history
```

**Additional folders (future):**
- `Tasks/` - Feature PRDs and implementation plans
- `SOP/` - Standard operating procedures

---

## Documentation Philosophy

The `docs/` folder follows a **layered approach**:

1. **Quick Reference** (`01-quick-reference/`) - Tables only, fast lookups
2. **Design Docs** (`02-design-docs/`) - WHY systems work this way
3. **Technical Specs** (`03-technical-specs/`) - HOW to implement (code-ready)
4. **Development** (`04-development/`) - Code architecture and patterns
5. **Archive** (`05-archive/`) - Design history and rejected ideas

See `.agent/documentation-organization-guide.md` for complete philosophy.

---

## Commands

### `/update-doc initialize`

**When to use:** Setting up documentation structure or adding missing technical architecture docs

**Your tasks:**

1. **Read existing documentation:**
   - Read `.agent/README.md` to understand current structure
   - Read `docs/00-START-HERE.md` to see what exists
   - Scan codebase to understand technical implementation

2. **Identify gaps:**
   - Is `docs/04-development/architecture-overview.md` missing?
   - Are there technical implementation patterns not documented?
   - Is Unity-specific architecture documented?

3. **Create missing technical documentation:**

   **If `docs/04-development/architecture-overview.md` doesn't exist, CREATE IT:**
   - Unity project structure
   - Networking architecture (Netcode for GameObjects patterns)
   - Code organization and patterns
   - Key systems implementation (Combat, AI, Weapons, etc.)
   - Data flow examples
   - Integration points (Steam, Unity services)
   - Performance considerations

4. **Update indices:**
   - Update `.agent/README.md` if structure changed
   - Update `docs/00-START-HERE.md` if new docs added
   - Update "Last Updated" timestamps

**What NOT to do:**
- Don't duplicate game design docs (classes, weapons, enemies) - those already exist in `docs/`
- Don't create redundant documentation
- Don't guess - base technical docs on actual codebase

**Output:**
- Summary of what was created/updated
- Key sections for review
- Suggested next steps

---

### `/update-doc [topic]`

**When to use:** Updating existing documentation after code changes or design changes

**Your tasks:**

1. **Read `.agent/README.md` first** to understand structure

2. **Identify which docs need updates:**

   | Topic Type | Update Location |
   |-----------|-----------------|
   | Game mechanics, balance | `docs/02-design-docs/` or `docs/03-technical-specs/` |
   | Stats changed (classes, weapons, enemies) | `docs/01-quick-reference/` AND `docs/03-technical-specs/` |
   | Code architecture changed | `docs/04-development/architecture-overview.md` |
   | Unity patterns changed | `docs/04-development/` AND `../CLAUDE.md` |
   | New system added | All layers (quick ref, design doc, tech spec, dev docs) |

3. **Update relevant docs:**
   - Update exact parameters in technical specs
   - Update quick reference tables
   - Update design docs if philosophy changed
   - Update architecture docs if code patterns changed

4. **Update navigation:**
   - Update `.agent/README.md` "Last Updated"
   - Update `docs/00-START-HERE.md` if navigation changed
   - Update timestamps in modified docs

**Examples:**
```
/update-doc networking     # Update networking architecture in 04-development/
/update-doc Bulwark        # Update Bulwark stats in all layers
/update-doc combat         # Update combat system design & specs
/update-doc review         # Scan codebase for outdated docs
```

---

### `/update-doc review`

**When to use:** General documentation health check

**Your tasks:**

1. **Scan codebase** for changes since last update
2. **Identify outdated sections** by comparing code to docs
3. **Flag missing documentation:**
   - Is there a major system not documented?
   - Are there new Unity patterns being used?
   - Is networking architecture current?
4. **Suggest updates** with priority (critical, important, nice-to-have)
5. **Update timestamps** where needed

---

## Creating New Documentation

### Adding `docs/04-development/architecture-overview.md`

**When:** Technical architecture documentation is missing

**Contents:**
```markdown
# Tide's End: Technical Architecture

## Project Overview
[What we're building technically]

## Tech Stack
[Unity version, packages, dependencies]

## Project Structure
[How code is organized]

## Core Systems

### Networking Architecture
[Netcode for GameObjects patterns, server authority, RPCs, NetworkVariables]

### Player System
[FPS controller, combat, input]

### Combat System
[Health, damage, IDamageable interface]

### Weapon System
[ScriptableObject-based, WeaponManager, firing]

### AI System
[State machine, NavMesh, networking]

### Spawn System
[NetworkObject spawning, spawn points]

### UI System
[HUD, menus, multiplayer-aware UI]

## Data Flow Examples
[Shooting, damage, reload across network]

## Integration Points
[Steam, Unity Transport, services]

## Performance Considerations
[Object pooling, network optimization]

## Development Patterns
[How to add networked components, naming conventions]

## Related Documentation
- Game Design: See `02-design-docs/`
- Technical Specs: See `03-technical-specs/`
- Unity Commands: See `../../CLAUDE.md`
```

### Creating `Tasks/` Folder

**When:** Planning a major feature

**Structure:**
```
Tasks/{feature-name}/
├── prd.md              # Requirements and design
├── implementation.md   # Technical implementation plan
└── testing.md          # Test strategy
```

**Update `.agent/README.md`** to include new Tasks section

### Creating `SOP/` Folder

**When:** Documenting common procedures or solving tricky problems

**Examples:**
- `adding-networked-component.md`
- `creating-new-weapon.md`
- `debugging-multiplayer.md`
- `testing-with-steam.md`

**Update `.agent/README.md`** to include new SOP section

---

## Update Rules

### Update Order (Critical!)

When updating after changes:

1. **Technical spec** (source of truth for parameters)
2. **Quick reference** (sync stats from tech spec)
3. **Design doc** (update examples if needed)
4. **Architecture doc** (if code patterns changed)
5. **README** (if structure changed)
6. **START-HERE** (if navigation changed)

### Cross-Referencing

Always add "Related Documentation" sections:

```markdown
## Related Documentation
- **Quick Reference:** `01-quick-reference/classes-quick-ref.md`
- **Design Philosophy:** `02-design-docs/combat-system-design.md`
- **Technical Specs:** `03-technical-specs/class-specifications.md`
- **Code Architecture:** `04-development/architecture-overview.md`
- **Unity Patterns:** `../../CLAUDE.md`
```

### Writing Style

**Do:**
- Be concise and scannable
- Use tables, lists, code blocks
- Include examples and data flows
- Explain WHY, not just WHAT
- Use consistent terminology

**Don't:**
- Duplicate information across files
- Include obvious information
- Make up information or guess
- Include sensitive data

---

## Quality Checklist

Before finishing, verify:

- [ ] All modified docs have updated "Last Updated" timestamp
- [ ] `.agent/README.md` is current
- [ ] `docs/00-START-HERE.md` navigation is current
- [ ] No duplicate information across files
- [ ] Related documents are cross-referenced
- [ ] Code examples are accurate
- [ ] File paths are correct
- [ ] Tech stack versions are current

---

## Handling Different Update Types

### Game Design Change (e.g., balance adjustment)

1. Update `docs/03-technical-specs/{class|weapon|enemy}-specifications.md`
2. Update `docs/01-quick-reference/` tables
3. Update `docs/02-design-docs/` examples if needed
4. Update timestamps

### Code Architecture Change (e.g., new networking pattern)

1. Update `docs/04-development/architecture-overview.md`
2. Update `../CLAUDE.md` if it affects Unity commands
3. Update data flow examples if needed
4. Update timestamps

### New System Added (e.g., new class ability)

1. Add to `docs/01-quick-reference/classes-quick-ref.md` (stat table)
2. Add to `docs/02-design-docs/combat-system-design.md` (philosophy & examples)
3. Add to `docs/03-technical-specs/class-specifications.md` (exact parameters)
4. Update `docs/04-development/architecture-overview.md` if new pattern
5. Update `docs/00-START-HERE.md` if major addition
6. Update timestamps

---

## Response Format

When completing documentation work, provide:

### 1. Summary
```
Updated:
- docs/01-quick-reference/classes-quick-ref.md (Bulwark stats)
- docs/03-technical-specs/class-specifications.md (Anchor Point cooldown)
- .agent/README.md (timestamp)

Created:
- docs/04-development/architecture-overview.md (new file)
```

### 2. Key Changes
- Highlight critical updates
- Flag anything needing validation
- Note any assumptions made

### 3. Next Steps (if applicable)
- Suggest additional documentation needed
- Recommend areas needing more detail

---

## Important Notes

- **Read first, then update**: Always read existing docs before making changes
- **Respect the layers**: Keep quick refs minimal, design docs explanatory, tech specs code-ready
- **Maintain navigation**: Keep README and START-HERE current
- **No redundancy**: Don't duplicate information across files
- **Base on reality**: Document actual code, not planned features

---

## Example Workflows

### Scenario 1: New ability added to codebase

```
User: "/update-doc Bulwark Anchor Point"

You:
1. Read current Bulwark docs across all layers
2. Analyze new code for Anchor Point
3. Update 01-quick-reference/classes-quick-ref.md (add row)
4. Update 02-design-docs/combat-system-design.md (add example)
5. Update 03-technical-specs/class-specifications.md (add JSON params)
6. Update 04-development/architecture-overview.md if new pattern
7. Update timestamps
8. Report changes
```

### Scenario 2: Architecture documentation missing

```
User: "/update-doc initialize"

You:
1. Check what exists in docs/04-development/
2. If architecture-overview.md missing, create it:
   - Scan codebase for Unity patterns
   - Document networking architecture
   - Document system implementations
   - Include data flow examples
3. Update .agent/README.md and docs/00-START-HERE.md
4. Report what was created
```

### Scenario 3: Balance change

```
User: "/update-doc Thompson damage increased to 30"

You:
1. Update docs/03-technical-specs/weapon-specifications.md
2. Update docs/01-quick-reference/weapons-quick-ref.md
3. Update timestamps
4. Report changes
```

---

**Remember**: Documentation should always reflect **current reality**, not wishful thinking. If code doesn't match docs, update docs to match code (or flag the discrepancy).
