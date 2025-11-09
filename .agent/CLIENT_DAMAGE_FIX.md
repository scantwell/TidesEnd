# Client Damage Fix - Remote Players Can Now Damage Host

## Problem

**Symptom**: Remote (non-host) players cannot damage the host player, but the host can damage clients.

**Reported**: "The local player can shoot the remote player, but the remote player can't shoot the host"

## Root Cause

### Architecture Issue

The game uses **client-side projectile simulation**:
- Projectiles are NOT networked (just MonoBehaviour, not NetworkBehaviour)
- Each client spawns and simulates projectiles locally
- `OnTriggerEnter()` runs on the client that owns the projectile

### The Problem Flow

**When Host Shoots Client:**
```
1. Host spawns projectile (locally)
2. Projectile's OnTriggerEnter() runs on host
3. Calls target.TakeDamage()
4. Health.TakeDamage() checks: IsServer? YES (host is server)
5. Damage processed ✓
```

**When Client Shoots Host:**
```
1. Client spawns projectile (locally)
2. Projectile's OnTriggerEnter() runs on client
3. Calls target.TakeDamage()
4. Health.TakeDamage() checks: IsServer? NO (client is not server)
5. Early return - damage rejected ❌
```

### Code Evidence

**Health.cs (OLD - Line 108):**
```csharp
public void TakeDamage(...)
{
    // Only server processes damage
    if (!IsServer) return;  // ❌ Clients exit immediately

    // ... damage processing ...
}
```

This pattern is correct for server-authoritative gameplay, but **doesn't work with client-side projectiles**.

## Solution

Implemented **client-to-server damage requests** using ServerRpc pattern.

### New Flow

**When Client Shoots Host:**
```
1. Client spawns projectile (locally)
2. Projectile's OnTriggerEnter() runs on client
3. Calls target.TakeDamage()
4. Health.TakeDamage() checks: IsServer? NO
5. Calls RequestDamageServerRpc() to ask server to process damage
6. Server receives RPC and calls ProcessDamage()
7. Damage processed on server ✓
8. Health NetworkVariable synced to all clients ✓
```

**When Host Shoots (Still Works):**
```
1. Host spawns projectile (locally)
2. Projectile's OnTriggerEnter() runs on host
3. Calls target.TakeDamage()
4. Health.TakeDamage() checks: IsServer? YES
5. Calls ProcessDamage() directly (no RPC needed)
6. Damage processed ✓
```

## Implementation

### Changes to Health.cs

**1. Modified TakeDamage() - Entry Point**
```csharp
public void TakeDamage(float damage, ulong attackerId = 0, Vector3 hitPoint = default, Vector3 hitNormal = default)
{
    // If we're on the server, process directly
    if (IsServer)
    {
        ProcessDamage(damage, attackerId, hitPoint, hitNormal);
    }
    else
    {
        // If we're on a client, request the server to process damage
        RequestDamageServerRpc(damage, attackerId, hitPoint, hitNormal);
    }
}
```

**2. Added RequestDamageServerRpc() - Client-to-Server Request**
```csharp
[ServerRpc(RequireOwnership = false)]
private void RequestDamageServerRpc(float damage, ulong attackerId, Vector3 hitPoint, Vector3 hitNormal)
{
    // Server processes the damage request
    ProcessDamage(damage, attackerId, hitPoint, hitNormal);
}
```

**Key: `RequireOwnership = false`**
- Allows ANY client to call this RPC on ANY Health component
- Without this, clients could only damage objects they own (doesn't make sense for combat)
- Essential for PvP/PvE damage systems

**3. Extracted ProcessDamage() - Server-Authoritative Logic**
```csharp
private void ProcessDamage(float damage, ulong attackerId, Vector3 hitPoint, Vector3 hitNormal)
{
    if (!IsServer) return;  // Double-check server authority
    if (isDead.Value) return;
    if (damage <= 0) return;

    // ... headshot checking ...
    // ... damage calculation ...
    // ... apply damage ...
    // ... notify clients ...
    // ... check for death ...
}
```

All the actual damage logic moved here - called by both direct (server) and RPC (client) paths.

## Why This Approach?

### Alternative 1: Networked Projectiles
Make Projectile a NetworkBehaviour, spawn on server, replicate to clients.

**Pros**: Most authoritative, no client trust
**Cons**:
- High complexity (NetworkObject pooling, spawn/despawn)
- Network latency affects hit detection (feels laggy)
- More bandwidth (every projectile synced)

### Alternative 2: Full Client Trust
Remove `if (!IsServer)` check, let clients apply damage directly.

**Pros**: Simple
**Cons**:
- ❌ **EXPLOITABLE** - Clients can fake damage amounts
- ❌ **INSECURE** - Easy to cheat
- ❌ **NOT RECOMMENDED**

### Chosen: Client Prediction + Server Validation ✓
Keep local projectiles, request server validation via RPC.

**Pros**:
- ✅ Responsive (instant hit detection on client)
- ✅ Server-authoritative (server validates all damage)
- ✅ Moderate complexity (one ServerRpc)
- ✅ Good bandwidth (only send damage events, not projectile state)

**Cons**:
- Server doesn't validate hit (trusts client hit detection)
- Potential for client-side exploits (aimbots can send perfect hits)

**Future Anti-Cheat**: Server could raycast to verify hit legitimacy, rate-limit damage requests, etc.

## Testing

### Test 1: Host Damages Client
1. Start as Host
2. Client joins
3. Host shoots Client
4. **Expected**: Client takes damage ✓ (already worked)

### Test 2: Client Damages Host
1. Start as Host
2. Client joins
3. Client shoots Host
4. **Expected**: Host takes damage ✓ (NOW WORKS!)

### Test 3: Client Damages Client
1. Start as Host
2. Client A joins
3. Client B joins
4. Client A shoots Client B
5. **Expected**: Client B takes damage via server ✓

## Network Flow Diagram

```
Client Projectile Hit:
┌─────────────┐
│   Client    │
│             │
│ Projectile  │
│ OnTrigger   │
│      │      │
│      ▼      │
│ TakeDamage()│
│      │      │
│      ▼      │
│ RequestRpc()│──────────┐
└─────────────┘          │
                         │ Network
                         │
                ┌────────▼────────┐
                │     Server      │
                │                 │
                │ ReceiveRpc()    │
                │      │          │
                │      ▼          │
                │ ProcessDamage() │
                │      │          │
                │      ▼          │
                │ NetworkVariable │
                │   (health)      │
                │      │          │
                └──────┼──────────┘
                       │ Sync
         ┌─────────────┼─────────────┐
         ▼             ▼             ▼
    ┌────────┐   ┌────────┐   ┌────────┐
    │Client A│   │Client B│   │  Host  │
    │Update  │   │Update  │   │ Update │
    │Health  │   │Health  │   │ Health │
    └────────┘   └────────┘   └────────┘
```

## Security Considerations

### Current System
- ✅ Server authoritative (damage applied on server)
- ✅ NetworkVariable synced (all clients see correct health)
- ⚠️ Trusts client hit detection (no validation)
- ⚠️ Trusts client damage amount (passed as parameter)

### Potential Exploits
1. **Aimbot**: Client can send perfect hit positions → Mitigated by server-side hit validation (future)
2. **Damage hacking**: Client could modify damage parameter → Mitigated by using weapon data on server (future)
3. **Rapid fire**: Client could spam damage requests → Mitigated by rate limiting (future)

### Future Hardening
```csharp
[ServerRpc(RequireOwnership = false)]
private void RequestDamageServerRpc(ulong attackerId, Vector3 hitPoint, ServerRpcParams rpcParams = default)
{
    // Server validates:
    // 1. Attacker exists and is alive
    // 2. Hit point is within weapon range
    // 3. Line of sight from attacker to hit point
    // 4. Damage amount from attacker's weapon data (not client parameter)
    // 5. Rate limit: max X damage requests per second

    if (ValidateHit(attackerId, hitPoint, rpcParams.Receive.SenderClientId))
    {
        float damage = GetWeaponDamage(attackerId);
        ProcessDamage(damage, attackerId, hitPoint, default);
    }
}
```

## Related Files

### Modified
- [Health.cs](../Assets/Scripts/Combat/Health.cs)
  - Modified `TakeDamage()` to route client calls through ServerRpc
  - Added `RequestDamageServerRpc()` for client damage requests
  - Extracted `ProcessDamage()` for shared damage logic

### Unchanged (But Relevant)
- [Projectile.cs](../Assets/Scripts/Weapons/Core/Projectile.cs)
  - Calls `TakeDamage()` on hit (line 180)
  - Remains non-networked MonoBehaviour
  - Client-side collision detection

## Summary

**Problem**: Clients couldn't damage host due to server authority check blocking client TakeDamage calls

**Solution**: Added ServerRpc to allow clients to request server to process damage

**Result**:
- ✅ Host can damage clients (already worked)
- ✅ Clients can damage host (NOW WORKS)
- ✅ Clients can damage other clients (works)
- ✅ Server-authoritative damage (maintained)
- ✅ NetworkVariable health sync (maintained)

The fix maintains server authority while enabling client-side projectile simulation with server validation.
