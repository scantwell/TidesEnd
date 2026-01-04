using System;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Modifies combat-related stats (damage output, critical chance, armor).
    /// Can apply flat bonuses (+0.1 crit chance) or multipliers (1.5x damage = +50%).
    /// Supports temporary buffs/debuffs or permanent modifications.
    /// </summary>
    [Serializable]
    public class ModifyCombatStatsEffect : AbilityEffect
    {
        public override EffectType EffectType => EffectType.ModifyCombatStats;

        [Header("Combat Stat Configuration")]
        [Tooltip("Which combat stat to modify")]
        public CombatStatType statToModify = CombatStatType.DamageOutput;

        [Tooltip("The modification value (e.g., +0.1 for 10% crit chance, 1.5 for 50% damage increase)")]
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
                Debug.LogError($"[ModifyCombatStatsEffect] Target '{target.name}' does not have EntityStats component!");
                return;
            }

            // Generate unique modifier ID
            string modifierId = $"combat_{statToModify}_{caster.GetInstanceID()}_{target.GetInstanceID()}_{Time.time}";

            // Map CombatStatType to StatType
            StatType statType = statToModify switch
            {
                CombatStatType.DamageOutput => StatType.DamageOutput,
                CombatStatType.CriticalChance => StatType.CriticalChance,
                CombatStatType.Armor => StatType.Armor,
                _ => StatType.DamageOutput
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
    /// Combat stats that can be modified by abilities.
    /// </summary>
    public enum CombatStatType
    {
        DamageOutput,    // Damage multiplier for all attacks (1.5 = +50% damage)
        CriticalChance,  // Critical hit chance (0.1 = 10% chance)
        Armor            // Flat damage reduction
    }
}
