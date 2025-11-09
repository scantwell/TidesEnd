# Bullet Collision Fix Summary

**Date**: 2025-10-30
**Issue**: Bullets not registering hits on players consistently

## Root Causes Identified

1. **Layer 8 was undefined** - Player prefab was on layer 8, but TagManager had no name for it
2. **Head collider missing HitZone** - The Cube (head) had a BoxCollider but no HitZone component
3. **Head collider wasn't a trigger** - BoxCollider had `isTrigger: false` and was disabled
4. **Owner reference needed** - HitZone components benefit from explicit owner references

## Existing Setup (Before Fix)

The Player prefab structure was:
```
Player (Layer 8 - undefined!)
├── Model (Body HitZone ✓, CapsuleCollider trigger ✓)
└── Cube (Head - NO HitZone ✗, BoxCollider solid ✗, disabled ✗)
```

The Model already had a properly configured Body HitZone with a trigger collider. The head was partially set up but incomplete.

## Changes Made

### 1. Defined Layer 8 as "Player"
**File**: `ProjectSettings/TagManager.asset`
**Line**: 17
**Change**: Added "Player" as the name for layer 8

```yaml
layers:
  - Default
  - TransparentFX
  - Ignore Raycast
  -
  - Water
  - UI
  - Projectile
  -
  - Player  # ← Added this
```

### 2. Made Head BoxCollider a Trigger
**File**: `Assets/Prefabs/Player.prefab`
**GameObject**: Cube (fileID: 3508016497477997118)
**Component**: BoxCollider (fileID: 4223847613888303480)

**Changes**:
- `m_IsTrigger: 0` → `m_IsTrigger: 1`
- `m_Enabled: 0` → `m_Enabled: 1`

### 3. Added HitZone Component to Head
**File**: `Assets/Prefabs/Player.prefab`
**GameObject**: Cube (fileID: 3508016497477997118)
**New Component**: HitZone (fileID: 8765432109876543210)

**Configuration**:
```yaml
zoneType: 0  # Head (2x damage multiplier)
customMultiplier: 0  # Use default multiplier
ownerObject: {fileID: 2267027562404787343}  # References Player root
```

### 4. Owner References Set
Both the Model (Body) and Cube (Head) HitZones now have explicit owner references pointing to the Player root GameObject, ensuring the Health component is always found.

## Result

**Before**:
- Body shots: Inconsistent (undefined layer)
- Head shots: Never worked (no HitZone component)

**After**:
- Body shots: Should work reliably (defined layer, HitZone configured)
- Head shots: Should work with 2x damage multiplier (HitZone added)

## How the System Works Now

1. **Projectile spawns** on Layer 6 (Projectile)
2. **Projectile trigger collider** moves through space
3. **OnTriggerEnter fires** when hitting:
   - Model CapsuleCollider (trigger, layer 8) → Body HitZone (1x damage)
   - Cube BoxCollider (trigger, layer 8) → Head HitZone (2x damage)
4. **Projectile.cs** (line 148) gets HitZone component
5. **HitZone.Owner** returns the Health component on Player root
6. **Health.TakeDamage()** is called with damage × multiplier
7. **Damage is processed** server-authoritatively via ServerRpc

## Testing Checklist

- [ ] Open Unity Editor
- [ ] Load Player prefab and verify:
  - [ ] Layer 8 shows as "Player" in Inspector
  - [ ] Model has HitZone component (Body, trigger)
  - [ ] Cube has HitZone component (Head, trigger)
  - [ ] Both HitZones reference Player root as owner
- [ ] Enter Play Mode (Host + Client)
- [ ] Fire at another player's body → Should take damage
- [ ] Fire at another player's head → Should take 2x damage
- [ ] Check Console for damage logs

## Related Files

- **Player prefab**: [Assets/Prefabs/Player.prefab](../Assets/Prefabs/Player.prefab)
- **Layer config**: [ProjectSettings/TagManager.asset](../ProjectSettings/TagManager.asset)
- **HitZone system**: [Assets/Scripts/Combat/HitZone.cs](../Assets/Scripts/Combat/HitZone.cs)
- **Projectile collision**: [Assets/Scripts/Weapons/Core/Projectile.cs:135-204](../Assets/Scripts/Weapons/Core/Projectile.cs)
- **Health damage**: [Assets/Scripts/Combat/Health.cs:108-181](../Assets/Scripts/Combat/Health.cs)

## Notes

- The Player prefab already had a working Body HitZone on the Model
- Only the head was incomplete (missing HitZone component)
- No additional HitZones were created - just fixed what was already there
- Layer 8 being undefined was likely causing unpredictable behavior
- Physics collision matrix uses default settings (all layers collide with all)
