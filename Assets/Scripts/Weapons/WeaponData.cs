using UnityEngine;

namespace TidesEnd.Weapons
{
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "Tide's End/Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string weaponName = "M1903 Springfield";
        public WeaponType weaponType = WeaponType.Rifle;

        [Header("Damage")]
        public float damage = 50f;
        [Tooltip("Damage multiplier for headshots")]
        public float headshotMultiplier = 2f;

        [Header("Fire Rate")]
        [Tooltip("Seconds between shots")]
        public float fireRate = 0.5f;
        public bool isAutomatic = false;

        [Header("Ammo")]
        public int magazineSize = 5;
        public int maxReserveAmmo = 50;
        public float reloadTime = 2.5f;

        [Header("Accuracy")]
        [Tooltip("Bullet spread angle")]
        public float spread = 0.01f;
        [Tooltip("Spread multiplier when aiming")]
        public float aimSpreadMultiplier = 0.5f;
        [Tooltip("Recoil intensity over sustained fire")]
        public AnimationCurve recoilPattern;

        [Header("Recoil Recovery")]
        [Tooltip("If false, no automatic recovery (CS:GO style)")]
        public bool enableRecoilRecovery = true;
        [Tooltip("How fast recoil is applied")]
        public float recoilApplicationSpeed = 10f;
        [Tooltip("How fast recoil returns to zero")]
        public float recoilRecoverySpeed = 5f;
        [Tooltip("Wait time before recovery starts after last shot")]
        public float recoilRecoveryDelay = 0.1f;

        [Header("Recoil Randomness")]
        [Range(0f, 2f)]
        [Tooltip("Minimum vertical recoil multiplier")]
        public float verticalRecoilMin = 0.8f;
        [Range(0f, 2f)]
        [Tooltip("Maximum vertical recoil multiplier")]
        public float verticalRecoilMax = 1.2f;
        [Range(0f, 1f)]
        [Tooltip("Horizontal recoil deviation range (±)")]
        public float horizontalRecoilRange = 0.3f;

        [Header("Screen Shake")]
        [Range(0f, 1f)]
        [Tooltip("Camera shake intensity when firing")]
        public float screenShakeIntensity = 0.1f;
        [Range(0f, 0.5f)]
        [Tooltip("Duration of camera shake effect")]
        public float screenShakeDuration = 0.1f;

        [Header("Range")]
        [Tooltip("Distance where weapon deals full damage (sweet spot)")]
        public float effectiveRange = 100f;
        [Tooltip("Maximum distance where bullets deal damage (hard cutoff)")]
        public float maxRange = 200f;
        [Tooltip("Damage multiplier curve (x=0 at effectiveRange, x=1 at maxRange)")]
        public AnimationCurve damageFalloff;

        [Header("Movement")]
        [Tooltip("Movement speed multiplier when holding this weapon")]
        public float movementSpeedMultiplier = 0.9f;
        [Tooltip("Movement speed multiplier when aiming")]
        public float aimMovementSpeedMultiplier = 0.6f;
    }

    public enum WeaponType
    {
        Rifle,
        Shotgun,
        Pistol,
        SMG,
        Special
    }
}
