using System;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Modifies health-related stats (max health, regen rate, regen delay).
    /// Can apply flat bonuses (+50 HP) or multipliers (1.5x = +50%).
    /// Supports temporary buffs/debuffs or permanent modifications.
    /// </summary>
    [Serializable]
    public class ModifyHealthStatsEffect : AbilityEffect
    {
        public override EffectType EffectType => EffectType.ModifyHealthStats;

        [Header("Health Stat Configuration")]
        [Tooltip("Which health stat to modify")]
        public HealthStatType statToModify = HealthStatType.MaxHealth;

        [Tooltip("The modification value (e.g., +50 for flat HP, 1.5 for 50% increase)")]
        public float value = 50f;

        [Tooltip("If true, value is a multiplier (1.5 = 150% = +50%). If false, value is a flat bonus (+50 HP).")]
        public bool isMultiplier = false;

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
                Debug.LogError($"[ModifyHealthStatsEffect] Target '{target.name}' does not have EntityStats component!");
                return;
            }

            // Generate unique modifier ID
            string modifierId = $"health_{statToModify}_{caster.GetInstanceID()}_{target.GetInstanceID()}_{Time.time}";

            // Map HealthStatType to StatType
            StatType statType = statToModify switch
            {
                HealthStatType.MaxHealth => StatType.MaxHealth,
                HealthStatType.RegenRate => StatType.RegenRate,
                HealthStatType.RegenDelay => StatType.RegenDelay,
                _ => StatType.MaxHealth
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
    /// Health stats that can be modified by abilities.
    /// </summary>
    public enum HealthStatType
    {
        MaxHealth,   // Maximum health pool
        RegenRate,   // Health regeneration per second
        RegenDelay   // Delay after taking damage before regen starts
    }
}
