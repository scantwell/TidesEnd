using UnityEngine;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Static utility class for weapon calculations
    /// Provides pure, stateless functions for spread, recoil, and damage falloff
    /// Can be used by players, AI, turrets, etc.
    /// </summary>
    public static class WeaponLogic
    {
        /// <summary>
        /// Apply bullet spread to a direction vector (stateless)
        /// </summary>
        public static Vector3 ApplySpread(WeaponData weaponData, Vector3 direction)
        {
            if (weaponData == null || weaponData.spread <= 0) return direction;

            float spreadX = Random.Range(-weaponData.spread, weaponData.spread);
            float spreadY = Random.Range(-weaponData.spread, weaponData.spread);

            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation *= Quaternion.Euler(spreadY, spreadX, 0);

            return rotation * Vector3.forward;
        }

        /// <summary>
        /// Get recoil offset for a given shot number (stateless)
        /// Returns x/y camera rotation offsets in degrees
        /// </summary>
        public static Vector2 GetRecoilVector(WeaponData weaponData, int shotNumber)
        {
            if (weaponData == null || weaponData.recoilPattern == null || weaponData.recoilPattern.keys.Length == 0)
                return Vector2.zero;

            float recoilMultiplier = weaponData.recoilPattern.Evaluate(shotNumber);

            // Vertical recoil (pitch) - use weapon-specific randomness
            float verticalRecoil = recoilMultiplier * Random.Range(weaponData.verticalRecoilMin, weaponData.verticalRecoilMax);

            // Horizontal recoil (yaw) - use weapon-specific horizontal range
            float horizontalRecoil = recoilMultiplier * Random.Range(-weaponData.horizontalRecoilRange, weaponData.horizontalRecoilRange);

            return new Vector2(horizontalRecoil, verticalRecoil);
        }

        /// <summary>
        /// Calculate damage falloff based on distance (stateless)
        /// Uses two-stage falloff:
        /// - 0 to effectiveRange: 100% damage (no falloff)
        /// - effectiveRange to maxRange: AnimationCurve falloff
        /// - Beyond maxRange: 0% damage (hard cutoff)
        /// </summary>
        public static float CalculateDamageFalloff(WeaponData weaponData, float distance)
        {
            if (weaponData == null || weaponData.damageFalloff == null || weaponData.damageFalloff.keys.Length == 0)
                return 1f;

            // Within effective range - full damage (sweet spot)
            if (distance <= weaponData.effectiveRange)
                return 1f;

            // Beyond max range - no damage (hard cutoff)
            if (distance >= weaponData.maxRange)
                return 0f;

            // Between effectiveRange and maxRange - apply curve falloff
            // Normalize distance to 0-1 range within the falloff window
            float normalizedDistance = Mathf.InverseLerp(
                weaponData.effectiveRange,
                weaponData.maxRange,
                distance
            );

            return weaponData.damageFalloff.Evaluate(normalizedDistance);
        }

        /// <summary>
        /// Calculate final weapon damage including headshot and distance falloff (stateless)
        /// This calculates weapon-specific damage before armor/resistance modifiers
        /// </summary>
        public static float CalculateWeaponDamage(WeaponData weaponData, float distance, bool isHeadshot)
        {
            if (weaponData == null) return 0f;

            float damage = weaponData.damage;

            if (isHeadshot)
            {
                damage *= weaponData.headshotMultiplier;
            }

            float falloffMultiplier = CalculateDamageFalloff(weaponData, distance);
            damage *= falloffMultiplier;

            return damage;
        }

        /// <summary>
        /// Update recoil state based on weapon data (stateless)
        /// Returns new (currentRecoil, targetRecoil) values
        /// </summary>
        public static (Vector2 newCurrent, Vector2 newTarget) UpdateRecoilState(
            WeaponData weaponData,
            Vector2 currentRecoil,
            Vector2 targetRecoil,
            float lastRecoilTime,
            float currentTime,
            float deltaTime)
        {
            if (weaponData == null)
            {
                return (currentRecoil, targetRecoil);
            }

            Vector2 newCurrent;
            Vector2 newTarget = targetRecoil;

            if (weaponData.enableRecoilRecovery)
            {
                // Recovery enabled: smoothly apply recoil and recover
                newCurrent = Vector2.Lerp(
                    currentRecoil,
                    targetRecoil,
                    deltaTime * weaponData.recoilApplicationSpeed
                );

                // Recover if enough time has passed since last shot
                if (currentTime >= lastRecoilTime + weaponData.recoilRecoveryDelay)
                {
                    newTarget = Vector2.Lerp(
                        targetRecoil,
                        Vector2.zero,
                        deltaTime * weaponData.recoilRecoverySpeed
                    );
                }
            }
            else
            {
                // Recovery disabled (CS:GO style): instant recoil application, no recovery
                // Camera snaps to target immediately, stays there until player manually compensates
                newCurrent = targetRecoil;
            }

            return (newCurrent, newTarget);
        }
    }
}