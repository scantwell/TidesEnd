using UnityEngine;
using TidesEnd.Combat;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Configuration for a single hitscan shot.
    /// Similar to ProjectileConfig but for instant raycasts.
    /// </summary>
    public struct HitscanConfig
    {
        // Damage
        public float baseDamage;
        public DamageType damageType;
        public ulong ownerId;

        // Ray information
        public Vector3 origin;
        public Vector3 direction;
        public float maxRange;

        // Hit location multipliers
        public float headshotMultiplier;
        public float limbshotMultiplier;

        // Penetration
        public bool canPenetrate;
        public int maxPenetrations;
        public float penetrationDamageReduction;

        // Damage falloff
        public float falloffStart;
        public float falloffEnd;

        // Collision
        public LayerMask hitLayers;

        // Visual effects
        public GameObject hitEffectPrefab;
        public bool spawnTracer;

        /// <summary>
        /// Creates a HitscanConfig from WeaponData and shot parameters
        /// </summary>
        public static HitscanConfig FromWeaponData(
            WeaponData weaponData,
            Vector3 origin,
            Vector3 direction,
            ulong owner)
        {
            return new HitscanConfig
            {
                baseDamage = weaponData.damage,
                damageType = weaponData.damageType,
                ownerId = owner,
                origin = origin,
                direction = direction.normalized,
                maxRange = weaponData.range,
                headshotMultiplier = weaponData.headshotMultiplier,
                limbshotMultiplier = weaponData.limbshotMultiplier,
                canPenetrate = weaponData.canPenetrate,
                maxPenetrations = weaponData.maxPenetrations,
                penetrationDamageReduction = weaponData.penetrationDamageReduction,
                falloffStart = weaponData.damageFalloffStart,
                falloffEnd = weaponData.damageFalloffEnd,
                hitLayers = weaponData.hitLayers,
                hitEffectPrefab = weaponData.hitEffectPrefab,
                spawnTracer = weaponData.useTracers
            };
        }
    }

    /// <summary>
    /// Result of a hitscan raycast, can be sent to clients for visual feedback
    /// </summary>
    public struct HitscanHitResult
    {
        public bool didHit;
        public Vector3 hitPoint;
        public Vector3 hitNormal;
        public GameObject hitObject;
        public bool wasHeadshot;
        public float damage;
    }
}
