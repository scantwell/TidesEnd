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
        public override EffectType EffectType => EffectType.Heal;

        [Header("Heal Parameters")]
        [Tooltip("Amount of health to restore")]
        public float healAmount = 20f;

        [Tooltip("Is healAmount a percentage of max health? (false = flat amount)")]
        public bool isPercentage = false;

        [Tooltip("Heal over time? (true = HoT, false = instant)")]
        public bool isOverTime = false;

        [Tooltip("If healing over time, tick interval in seconds")]
        public float tickInterval = 1f;

        [Tooltip("Duration of the heal over time effect")]
        public float duration = 0f;

        public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
        {
            if (!ShouldApplyToTarget(caster, target))
                return;

            // Check if target has health component
            if (target.TryGetComponent<Health>(out var health))
            {
                float actualHealAmount = healAmount;

                // Calculate heal amount based on percentage flag
                if (isPercentage)
                {
                    actualHealAmount = health.MaxHealth * (healAmount / 100f);
                }

                // Apply heal
                if (!isOverTime)
                {
                    // Instant heal
                    health.Heal(actualHealAmount);
                }
                else
                {
                    // Heal over time - will be handled by AbilityInstance Update loop
                    // This is just initial tick
                    health.Heal(actualHealAmount);
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
                float actualHealAmount = healAmount;
                if (isPercentage)
                {
                    actualHealAmount = health.MaxHealth * (healAmount / 100f);
                }
                health.Heal(actualHealAmount);
            }
        }
    }
}
