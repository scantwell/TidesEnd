using System;
using System.Collections.Generic;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Applies status effects to entities (Stunned, Disarmed, Silenced, Rooted, etc.).
    /// Status effects are binary on/off conditions that affect entity capabilities.
    /// Automatically removes status when effect expires or is manually removed.
    /// </summary>
    [Serializable]
    public class ApplyStatusEffect : AbilityEffect
    {
        public override EffectType EffectType => EffectType.ApplyStatus;

        [Header("Status Configuration")]
        [Tooltip("Which status effect to apply")]
        public StatusEffects statusToApply = StatusEffects.Stunned;

        [Header("Duration")]
        [Tooltip("Duration of the status effect in seconds. 0 = permanent (until manually removed).")]
        public float duration = 0f;

        // Track which entities have this status applied
        private HashSet<GameObject> affectedEntities = new HashSet<GameObject>();

        public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
        {
            // Get EntityStats component
            if (!target.TryGetComponent<EntityStats>(out var entityStats))
            {
                Debug.LogError($"[ApplyStatusEffect] Target '{target.name}' does not have EntityStats component!");
                return;
            }

            // Apply status effect
            entityStats.AddStatusEffect(statusToApply);

            // Track affected entity
            affectedEntities.Add(target);

            if (statusToApply.HasFlag(StatusEffects.Slowed))
            {
                Debug.Log($"[ApplyStatusEffect] Note: Slowed status should be combined with ModifyMovementEffect for actual speed reduction");
            }
        }

        public override void Remove(GameObject target)
        {
            // Get EntityStats component
            if (!target.TryGetComponent<EntityStats>(out var entityStats))
            {
                return;
            }

            // Remove status effect
            entityStats.RemoveStatusEffect(statusToApply);

            // Untrack entity
            affectedEntities.Remove(target);
        }

        /// <summary>
        /// Remove status from all affected entities.
        /// Useful for cleanup when ability ends.
        /// </summary>
        public void RemoveFromAll()
        {
            foreach (var entity in new List<GameObject>(affectedEntities))
            {
                Remove(entity);
            }
        }
    }
}
