using System;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Modifies weapon stat MULTIPLIERS (fire rate, reload speed, magazine size, spread, recoil).
    /// IMPORTANT: These modify the multiplier applied to weapon base stats from WeaponData.
    /// Base weapon stats come from WeaponData (e.g., AK47.fireRate = 600 RPM).
    /// This effect modifies EntityStats multipliers (starts at 1.0 = neutral).
    /// Final value: weaponData.stat * entityStats.stat
    /// Example: AK47 (600 RPM) with 1.5x fireRate multiplier = 900 RPM
    /// </summary>
    [Serializable]
    public class ModifyWeaponStatsEffect : AbilityEffect
    {
        public override EffectType EffectType => EffectType.ModifyWeaponStats;

        [Header("Weapon Stat Configuration")]
        [Tooltip("Which weapon stat to modify")]
        public WeaponStatType statToModify = WeaponStatType.FireRate;

        [Tooltip("The modification value (e.g., +100 RPM for flat, 1.5 for 50% increase)")]
        public float value = 1.5f;

        [Tooltip("If true, value is a multiplier (1.5 = 150% = +50%). If false, value is a flat bonus.")]
        public bool isMultiplier = true;

        [Header("Duration")]
        [Tooltip("Duration of the effect in seconds. 0 = permanent (until manually removed).")]
        public float duration = 0f;

        // Track modifier IDs for removal
        private System.Collections.Generic.Dictionary<GameObject, string> activeModifierIds
            = new System.Collections.Generic.Dictionary<GameObject, string>();

        public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
        {
            // Get EntityStats component
            if (!target.TryGetComponent<EntityStats>(out var entityStats))
            {
                Debug.LogError($"[ModifyWeaponStatsEffect] Target '{target.name}' does not have EntityStats component!");
                return;
            }

            // Generate unique modifier ID
            string modifierId = $"weapon_{statToModify}_{caster.GetInstanceID()}_{target.GetInstanceID()}_{Time.time}";

            // Map WeaponStatType to StatType
            StatType statType = statToModify switch
            {
                WeaponStatType.FireRate => StatType.FireRate,
                WeaponStatType.ReloadSpeed => StatType.ReloadSpeed,
                WeaponStatType.MagazineSize => StatType.MagazineSize,
                WeaponStatType.Spread => StatType.Spread,
                WeaponStatType.Recoil => StatType.Recoil,
                _ => StatType.FireRate
            };

            // Apply modifier
            entityStats.AddModifier(modifierId, statType, value, isMultiplier);

            // Track modifier ID for later removal
            activeModifierIds[target] = modifierId;

            // Schedule automatic removal if duration > 0
            if (duration > 0f)
            {
                // Note: In a real implementation, you'd use a coroutine or timer system
                // For now, effects will be removed when the ability expires or manually via Remove()
            }
        }

        public override void Remove(GameObject target)
        {
            // Get EntityStats component
            if (!target.TryGetComponent<EntityStats>(out var entityStats))
            {
                return;
            }

            // Remove the modifier if we have tracked ID
            if (activeModifierIds.TryGetValue(target, out string modifierId))
            {
                entityStats.RemoveModifier(modifierId);
                activeModifierIds.Remove(target);
            }
        }
    }

    /// <summary>
    /// Weapon stats that can be modified by abilities.
    /// </summary>
    public enum WeaponStatType
    {
        FireRate,       // Rounds per minute
        ReloadSpeed,    // Reload speed multiplier (1.0 = normal, 2.0 = 2x faster)
        MagazineSize,   // Ammo capacity
        Spread,         // Bullet spread/accuracy (lower = more accurate)
        Recoil          // Recoil intensity (lower = less recoil)
    }
}
