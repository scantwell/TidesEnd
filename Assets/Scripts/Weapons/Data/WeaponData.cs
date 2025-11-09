using UnityEngine;
using TidesEnd.Combat;

namespace TidesEnd.Weapons
{
    public enum WeaponType
    {
        Hitscan,    // Instant raycasts (rifles, pistols, SMGs)
        Projectile, // Physics-based projectiles (grenades, rockets)
        Melee       // Future: melee weapons
    }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
    [Header("Weapon Type")]
    public WeaponType weaponType = WeaponType.Hitscan;

    [Header("Weapon Info")]
    public string weaponName = "Rifle";
    [TextArea] public string description;
    
    [Header("Combat Stats")]
    public float damage = 25f;
    public float fireRate = 10f; // Rounds per second
    public float range = 100f;
    public DamageType damageType = DamageType.Normal;
    
    [Header("Ammo")]
    public int magazineSize = 30;
    public int maxAmmo = 300;
    public float reloadTime = 2f;
    
    [Header("Accuracy")]
    public float baseSpread = 0.5f; // Degrees
    public float aimSpread = 0.1f; // Spread while aiming
    public float movementSpreadMultiplier = 2f;
    
    [Header("Firing Behavior")]
    public FiringMode firingMode = FiringMode.Automatic;

    [Header("Hitscan Settings (if weaponType == Hitscan)")]
    [Tooltip("Number of raycasts per shot. 1 for rifles, 8-12 for shotguns")]
    public int pelletsPerShot = 1;

    [Header("Projectile Settings (if weaponType == Projectile)")]
    [Tooltip("Number of projectiles per shot. 1 for rifles, >1 for shotguns")]
    public int projectilesPerShot = 1;

    [Header("Damage Falloff")]
    public float damageFalloffStart = 30f; // Distance where damage starts to fall off
    public float damageFalloffEnd = 80f;   // Distance where damage reaches minimum (20%)

    [Header("Hit Location Multipliers")]
    public float headshotMultiplier = 2f;
    public float limbshotMultiplier = 0.75f;

    [Header("Penetration (Both Hitscan and Projectile)")]
    public bool canPenetrate = false;
    public int maxPenetrations = 0;
    [Tooltip("Damage multiplier after each penetration (0.5 = 50% damage after penetrating)")]
    public float penetrationDamageReduction = 0.5f;

    [Header("Projectile Physics (if weaponType == Projectile)")]
    public GameObject projectilePrefab;
    public float projectileVelocity = 300f; // Units per second
    public float projectileLifetime = 5f;   // Seconds before auto-destroy
    public float projectileGravity = 0f;    // Gravity multiplier (0 = no drop)

    [Header("Hit Detection")]
    public LayerMask hitLayers = -1;

    [Header("Visual Effects")]
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffectPrefab;
    public GameObject tracerPrefab;
    public float tracerSpeed = 300f;
    public bool useTracers = true;
    [Range(1, 100)] public int tracerFrequency = 1; // Every Nth bullet shows tracer (1 = all)

    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    [Range(0f, 1f)] public float volume = 0.7f;
}

    public enum FiringMode
    {
        SemiAutomatic, // One shot per click
        Automatic,     // Continuous fire
        Burst,         // 3-round burst
        Charge         // Hold to charge
    }
}