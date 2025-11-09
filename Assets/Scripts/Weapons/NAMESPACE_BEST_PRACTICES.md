# Namespace Best Practices for Tide's End

## Namespace Structure

The codebase uses **logical namespaces** based on feature areas, not folder structure.

### Root Namespace
```csharp
TidesEnd
```

### Feature Namespaces
```csharp
TidesEnd.Combat      // Combat system, damage, health, interfaces
TidesEnd.Weapons     // Weapon system, projectiles, firing
TidesEnd.Abilities   // Ability system, effects, deployables
TidesEnd.Enemy       // Enemy AI and behaviors
TidesEnd.Editor      // Editor tools and custom inspectors
```

## Why Logical Namespaces?

**Logical namespaces** (feature-based) vs **Folder-based namespaces**:

### ✅ Logical Namespaces (Our Approach)
```
namespace TidesEnd.Weapons
```
- Groups related functionality
- Easier refactoring (move files without changing namespace)
- Clearer API surface
- Follows Unity best practices

### ❌ Folder-based Namespaces
```
namespace Assets.Scripts.Weapons.Core
```
- Verbose and cluttered
- Breaks when files move
- Exposes internal structure
- Not recommended for Unity projects

## IDE Warnings

You may see warnings like:
```
Namespace "TidesEnd.Weapons" does not match folder structure, expected "Assets.Scripts.Weapons.Core"
```

**This is intentional and can be safely ignored.** We're using logical namespaces for better code organization.

## Namespace Usage Guide

### When to Use Namespaces

**Always use namespaces** for:
- ✅ Core game systems (Combat, Weapons, Abilities)
- ✅ Reusable components
- ✅ Data structures and interfaces
- ✅ Editor tools

**Don't use namespaces** for:
- ❌ MonoBehaviour scripts that are only used in specific scenes
- ❌ One-off utility scripts (though a `TidesEnd.Utilities` namespace is fine)

### Namespace Naming

```csharp
// Root namespace for project
namespace TidesEnd
{
    // Simple class in root namespace
    public class GameManager { }
}

// Feature namespace
namespace TidesEnd.Weapons
{
    // Weapon-related classes
    public class Weapon { }
    public class Projectile { }
}

// Sub-feature namespace (if needed)
namespace TidesEnd.Weapons.Effects
{
    // Specialized classes
    public class BulletTracer { }
}
```

### Accessing Classes from Other Namespaces

```csharp
using UnityEngine;
using TidesEnd.Combat;   // Import Combat namespace
using TidesEnd.Weapons;  // Import Weapons namespace

namespace TidesEnd.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Weapon weapon;           // From TidesEnd.Weapons
        private IDamageable damageable;  // From TidesEnd.Combat
    }
}
```

## Current Namespace Map

```
TidesEnd
├── Combat
│   ├── IDamageable
│   ├── IWeapon
│   ├── DamageInfo
│   ├── DamageType
│   ├── Health
│   ├── HitZone
│   └── DamageNotifier
│
├── Weapons
│   ├── Weapon
│   ├── WeaponData
│   ├── WeaponManager
│   ├── WeaponView
│   ├── Projectile
│   ├── ProjectileConfig
│   ├── ProjectilePool
│   ├── BulletTracer
│   ├── TracerPool
│   └── FiringMode (enum)
│
├── Abilities
│   ├── AbilityData
│   ├── AbilityInstance
│   ├── AbilityUser
│   ├── AbilityFactory
│   ├── AbilityContext
│   ├── IMovable
│   ├── DeployableObject
│   ├── AbilityProjectile
│   └── (various AbilityInstance types)
│
├── Enemy
│   └── EnemyAI
│
└── Editor
    └── AbilityDataEditor
```

## File Organization

Even though we use logical namespaces, keep files organized in folders:

```
Assets/Scripts/
├── Combat/              → TidesEnd.Combat
│   ├── Health.cs
│   ├── HitZone.cs
│   └── Interfaces/
│       ├── IDamageable.cs
│       └── IWeapon.cs
│
├── Weapons/             → TidesEnd.Weapons
│   ├── Core/
│   │   ├── Weapon.cs
│   │   ├── Projectile.cs
│   │   └── ProjectileConfig.cs
│   ├── Data/
│   │   └── WeaponData.cs
│   └── Effects/
│       ├── BulletTracer.cs
│       └── ProjectilePool.cs
│
└── Abilities/           → TidesEnd.Abilities
    ├── Core/
    └── Effects/
```

**Key Point:** Folder structure is for **file organization**, namespace is for **code organization**.

## Adding New Classes

When creating new classes:

1. **Determine logical feature area**: Combat? Weapons? Abilities? Player? Enemy?

2. **Use appropriate namespace**:
```csharp
using UnityEngine;

namespace TidesEnd.{Feature}
{
    public class MyNewClass : MonoBehaviour
    {
        // Implementation
    }
}
```

3. **Add using statements** in files that need it:
```csharp
using TidesEnd.{Feature};
```

4. **Place file in appropriate folder** (for organization, not namespace)

## Unity-Specific Considerations

### ScriptableObjects
```csharp
using UnityEngine;

namespace TidesEnd.Weapons
{
    [CreateAssetMenu(...)]  // Works fine inside namespace
    public class WeaponData : ScriptableObject
    {
    }
}
```

### MonoBehaviours
```csharp
using UnityEngine;

namespace TidesEnd.Weapons
{
    [RequireComponent(typeof(WeaponView))]  // Works fine
    public class Weapon : MonoBehaviour
    {
    }
}
```

### Editor Scripts
```csharp
using UnityEditor;

namespace TidesEnd.Editor  // Separate namespace for editor
{
    [CustomEditor(typeof(WeaponData))]
    public class WeaponDataEditor : Editor
    {
    }
}
```

## Common Issues

### Issue: "Type not found"
**Solution**: Add `using TidesEnd.{Feature};` at top of file

### Issue: "Namespace does not match folder structure" warning
**Solution**: Ignore it - this is intentional

### Issue: Circular dependencies
**Solution**: Move shared interfaces to a common namespace like `TidesEnd.Combat`

## Benefits of This Approach

1. **Clear API boundaries** - Know what belongs to each system
2. **Better IntelliSense** - Easier to find related classes
3. **Refactoring safety** - Can move files without breaking references
4. **Unity-friendly** - Follows Unity's own namespace conventions
5. **Team collaboration** - Clear ownership of code areas

## References

- Unity's own code uses logical namespaces (UnityEngine, UnityEditor)
- Microsoft C# guidelines recommend feature-based namespaces
- Most Unity Asset Store packages use logical namespaces

---

**Summary**: Use `TidesEnd.{Feature}` namespaces based on what the code does, not where the file lives.
