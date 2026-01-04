using System;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Modifies movement-related stats (walk speed, sprint speed, jump height).
    /// Can apply flat bonuses (+2 m/s) or multipliers (1.5x = +50%).
    /// Supports temporary buffs/debuffs or permanent modifications.
    /// </summary>
    [Serializable]
    public class ModifyMovementEffect : AbilityEffect
    {
        public override EffectType EffectType => EffectType.ModifyMovement;

        [Header("Movement Stat Configuration")]
        [Tooltip("Which movement stat to modify")]
        public MovementStatType statToModify = MovementStatType.WalkSpeed;

        [Tooltip("The modification value (e.g., +2 for flat, 1.5 for 50% increase)")]
        public float value = 1.5f;

        [Tooltip("If true, value is a multiplier (1.5 = 150% = +50%). If false, value is a flat bonus (+2 m/s).")]
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
                Debug.LogError($"[ModifyMovementEffect] Target '{target.name}' does not have EntityStats component!");
                return;
            }

            if (statToModify == MovementStatType.All)
            {
                // Apply to all movement stats
                string baseModifierId = $"movement_all_{caster.GetInstanceID()}_{target.GetInstanceID()}_{Time.time}";

                entityStats.AddModifier($"{baseModifierId}_walk", StatType.WalkSpeed, value, isMultiplier);
                entityStats.AddModifier($"{baseModifierId}_sprint", StatType.SprintSpeed, value, isMultiplier);
                entityStats.AddModifier($"{baseModifierId}_jump", StatType.JumpHeight, value, isMultiplier);

                // Track the base modifier ID for later removal
                activeModifierIds[target] = baseModifierId;
            }
            else
            {
                // Apply to single stat
                string modifierId = $"movement_{statToModify}_{caster.GetInstanceID()}_{target.GetInstanceID()}_{Time.time}";

                // Map MovementStatType to StatType
                StatType statType = statToModify switch
                {
                    MovementStatType.WalkSpeed => StatType.WalkSpeed,
                    MovementStatType.SprintSpeed => StatType.SprintSpeed,
                    MovementStatType.JumpHeight => StatType.JumpHeight,
                    _ => StatType.WalkSpeed
                };

                // Apply modifier
                entityStats.AddModifier(modifierId, statType, value, isMultiplier);

                // Track modifier ID for later removal
                activeModifierIds[target] = modifierId;
            }

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
                // Check if this was an "All" modifier (contains "_all_")
                if (modifierId.Contains("_all_"))
                {
                    // Remove all three modifiers
                    entityStats.RemoveModifier($"{modifierId}_walk");
                    entityStats.RemoveModifier($"{modifierId}_sprint");
                    entityStats.RemoveModifier($"{modifierId}_jump");
                }
                else
                {
                    // Remove single modifier
                    entityStats.RemoveModifier(modifierId);
                }

                activeModifierIds.Remove(target);
            }
        }
    }

    /// <summary>
    /// Movement stats that can be modified by abilities.
    /// </summary>
    public enum MovementStatType
    {
        WalkSpeed,
        SprintSpeed,
        JumpHeight,
        All  // Apply to all movement stats (WalkSpeed, SprintSpeed, JumpHeight)
    }
}
