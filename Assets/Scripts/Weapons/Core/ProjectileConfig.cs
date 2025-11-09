using UnityEngine;
using TidesEnd.Combat;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Configuration data passed to a projectile when spawned.
    /// Contains all information needed for damage calculation and behavior.
    /// </summary>
    public struct ProjectileConfig
    {
        // Basic damage
        public float baseDamage;
    public DamageType damageType;
    public ulong ownerId;

    // Movement
    public float velocity;
    public Vector3 direction;
    public Vector3 spawnPosition;

    // Damage falloff
    public float falloffStart;
    public float falloffEnd;

    // Hit location multipliers
    public float headshotMultiplier;
    public float limbshotMultiplier;

    // Penetration
    public bool canPenetrate;
    public int maxPenetrations;
    public float penetrationDamageReduction;

    // Lifetime
    public float maxLifetime;
    public float gravityMultiplier;

    // Collision
    public LayerMask hitLayers;

    // Visual
    public bool spawnTracer;
    public GameObject hitEffectPrefab;

    /// <summary>
    /// Creates a default config with sensible values
    /// </summary>
    public static ProjectileConfig Default()
    {
        return new ProjectileConfig
        {
            baseDamage = 25f,
            damageType = DamageType.Normal,
            ownerId = 0,
            velocity = 300f,
            direction = Vector3.forward,
            spawnPosition = Vector3.zero,
            falloffStart = 30f,
            falloffEnd = 80f,
            headshotMultiplier = 2f,
            limbshotMultiplier = 0.75f,
            canPenetrate = false,
            maxPenetrations = 0,
            penetrationDamageReduction = 0.5f,
            maxLifetime = 5f,
            gravityMultiplier = 0f,
            hitLayers = -1,
            spawnTracer = true,
            hitEffectPrefab = null
        };
    }

    /// <summary>
    /// Creates a config from WeaponData
    /// </summary>
    public static ProjectileConfig FromWeaponData(WeaponData weaponData, Vector3 direction, Vector3 spawnPos, ulong owner)
    {
        return new ProjectileConfig
        {
            baseDamage = weaponData.damage,
            damageType = weaponData.damageType,
            ownerId = owner,
            velocity = weaponData.projectileVelocity,
            direction = direction.normalized,
            spawnPosition = spawnPos,
            falloffStart = weaponData.damageFalloffStart,
            falloffEnd = weaponData.damageFalloffEnd,
            headshotMultiplier = weaponData.headshotMultiplier,
            limbshotMultiplier = weaponData.limbshotMultiplier,
            canPenetrate = weaponData.canPenetrate,
            maxPenetrations = weaponData.maxPenetrations,
            penetrationDamageReduction = weaponData.penetrationDamageReduction,
            maxLifetime = weaponData.projectileLifetime,
            gravityMultiplier = weaponData.projectileGravity,
            hitLayers = weaponData.hitLayers,
            spawnTracer = weaponData.useTracers,
            hitEffectPrefab = weaponData.hitEffectPrefab
        };
    }
    }
}
