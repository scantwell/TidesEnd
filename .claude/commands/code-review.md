# Code Review Command

You are conducting a Unity C# code review following comprehensive best practices.

## Task

Review the code at: **{{input}}**

If the input is:
- A **file path**: Review that specific file
- A **directory**: Review all C# files in that directory
- A **namespace**: Search for and review all files in that namespace
- **Empty/not provided**: Ask the user what they want reviewed

## Review Process

Systematically check the code against these categories:

### 1. Component References & Dependencies
- ❌ Runtime Find/GetComponent calls → ✅ SerializeField
- ❌ Missing RequireComponent → ✅ Add RequireComponent attribute
- ❌ Repeated GetComponent calls → ✅ Cache references

### 2. Unity Lifecycle Methods
- ❌ Wrong initialization order → ✅ Awake for self, Start for others
- ❌ Events in Start/OnDestroy → ✅ OnEnable/OnDisable
- ❌ Missing lifecycle patterns → ✅ Proper initialization

### 3. Unity-Specific Optimizations
- ❌ `tag == "string"` → ✅ `CompareTag("string")`
- ❌ Magic strings → ✅ String constants
- ❌ `new WaitForSeconds()` in loop → ✅ Cache WaitForSeconds
- ❌ `is null` → ✅ `== null` (Unity overload)

### 4. Network Code (Unity Netcode for GameObjects)
- ❌ No ServerRpc validation → ✅ Validate all inputs
- ❌ Plain fields for state → ✅ NetworkVariable<T>
- ❌ Client modifying state → ✅ IsServer checks
- ❌ Missing authority checks → ✅ Proper IsServer/IsClient/IsOwner usage

### 5. ScriptableObject Best Practices
- ❌ Runtime state in SO → ✅ Stateless data only
- ❌ Missing CreateAssetMenu → ✅ Add attribute
- ❌ Hardcoded values → ✅ ScriptableObject data

### 6. Code Organization
- ❌ Implicit access modifiers → ✅ Explicit private/public
- ❌ No structure → ✅ Regions and headers
- ❌ Methods >50 lines → ✅ Single responsibility

### 7. Inspector Enhancement
- ❌ No tooltips/headers → ✅ Tooltip and Header attributes
- ❌ No validation → ✅ Range attributes
- ❌ Hard to test → ✅ ContextMenu for testing

### 8. Debugging Support
- ❌ No visual feedback → ✅ Gizmos (OnDrawGizmosSelected)
- ❌ No assertions → ✅ Debug.Assert for assumptions
- ❌ Poor logging → ✅ Contextual, color-coded logs

### 9. Modularity & Decoupling
- ❌ Singleton dependencies → ✅ Dependency injection
- ❌ Concrete types → ✅ Interface-based design (IDamageable, etc.)
- ❌ Tight coupling → ✅ Events for decoupling

### 10. Event Systems
- ❌ Missing unsubscribe → ✅ OnEnable/OnDisable symmetry
- ❌ `event.Invoke()` → ✅ `event?.Invoke()`
- ❌ Wrong timing → ✅ Invoke after state change
- ❌ Magic strings → ✅ Centralized, type-safe events

### 11. Performance
- ❌ String concatenation in loops → ✅ StringBuilder
- ❌ No pooling for frequent spawns → ✅ Object pooling (>10/sec)
- ❌ Allocations in Update → ✅ Cache/reuse objects

## Severity Levels

**CRITICAL** (Fix Immediately):
- Memory leaks (missing event unsubscribe)
- Network security (no ServerRpc validation)
- Runtime state in ScriptableObjects
- Server authority violations

**HIGH** (Fix Before Commit):
- Performance issues (allocations in Update, uncached GetComponent)
- Missing component serialization (Find in lifecycle)
- Network desync risks

**MEDIUM** (Refactor When Convenient):
- Missing debugging support (no gizmos, assertions)
- Poor organization (no regions/headers)
- Tight coupling (singletons instead of injection)
- Long methods (>50 lines)

**LOW** (Nice to Have):
- Missing tooltips/headers
- Implicit access modifiers
- Missing context menus
- Basic logging improvements

## Output Format

Provide a structured review:

```markdown
## Code Review: [FileName.cs]

### Critical Issues (N)
1. **[Issue Type]** (Line X)
   - Issue: [What's wrong]
   - Impact: [What it causes]
   - Fix: [How to fix]
   ```csharp
   // Corrected code
   ```

### High Priority (N)
2. **[Issue Type]** (Line X)
   - Issue: [What's wrong]
   - Impact: [What it causes]
   - Fix: [How to fix]
   ```csharp
   // Corrected code
   ```

### Medium Priority (N)
[Same format]

### Low Priority (N)
[Same format]

## Summary
- Total Issues: X
- Critical: X | High: X | Medium: X | Low: X
- Estimated Fix Time: X minutes
- Priority: Fix [critical issues] first (X minutes)
```

## Instructions

1. **Read the file(s)** specified in the input
2. **Scan systematically** through all 11 categories
3. **Flag anti-patterns** with specific line numbers
4. **Show corrected code** for every issue found
5. **Prioritize by severity** (Critical → High → Medium → Low)
6. **Estimate fix time** for each issue
7. **Provide actionable feedback** with clear explanations

## Important

- Focus on **actionable, specific feedback**
- Include **code examples** for every suggestion
- Explain **WHY** the pattern is better
- Provide **realistic time estimates**
- Don't overwhelm - **prioritize critical issues**
- If reviewing multiple files, summarize findings across all files at the end

Now proceed with the code review of: **{{input}}**
