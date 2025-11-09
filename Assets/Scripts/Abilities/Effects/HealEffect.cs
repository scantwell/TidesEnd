using System;
using UnityEngine;
using TidesEnd.Combat;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Effect that restores health to the target.
    /// Used by healing abilities like Sanctuary Circle, healing items, etc.
    /// </summary>
    [Serializable]
    public class HealEffect : AbilityEffect
    {
        [Header("Heal Parameters")]
        [Tooltip("Is magnitude a percentage of max health? (false = flat amount)")]
        public bool isPercentage = false;

        [Tooltip("Heal over time? (true = HoT, false = instant)")]
        public bool isOverTime = false;

        [Tooltip("If healing over time, tick interval in seconds")]
        public float tickInterval = 1f;

        public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
        {
            if (!ShouldApplyToTarget(caster, target))
                return;

            // Check if target has health component
            if (target.TryGetComponent<Health>(out var health))
            {
                float healAmount = magnitude;

                // Calculate heal amount based on percentage flag
                if (isPercentage)
                {
                    healAmount = health.MaxHealth * (magnitude / 100f);
                }

                // Apply heal
                if (!isOverTime)
                {
                    // Instant heal
                    health.Heal(healAmount);
                }
                else
                {
                    // Heal over time - will be handled by AbilityInstance Update loop
                    // This is just initial tick
                    health.Heal(healAmount);
                }
            }
        }

        public override void Remove(GameObject target)
        {
            // Healing doesn't need removal (already applied)
        }

        /// <summary>
        /// Apply a heal tick (called by AbilityInstance for HoT effects)
        /// </summary>
        public void ApplyTick(GameObject target)
        {
            if (target.TryGetComponent<Health>(out var health))
            {
                float healAmount = magnitude;
                if (isPercentage)
                {
                    healAmount = health.MaxHealth * (magnitude / 100f);
                }
                health.Heal(healAmount);
            }
        }
    }
}
