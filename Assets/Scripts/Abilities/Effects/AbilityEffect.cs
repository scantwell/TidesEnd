using System;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Base class for all ability effects.
    /// Effects are modular building blocks that can be composed to create complex abilities.
    /// Each effect defines what happens when an ability is activated.
    /// </summary>
    [Serializable]
    public abstract class AbilityEffect
    {
        [Header("Effect Configuration")]
        [Tooltip("Type of effect this applies")]
        public EffectType effectType;

        [Tooltip("Magnitude/strength of the effect (damage, heal amount, multiplier, etc.)")]
        public float magnitude;

        [Tooltip("Duration the effect lasts (0 = instant/permanent)")]
        public float duration;

        [Header("Targeting Filters")]
        [Tooltip("Can this effect apply to allies?")]
        public bool affectsAllies = true;

        [Tooltip("Can this effect apply to enemies?")]
        public bool affectsEnemies = true;

        [Tooltip("Can this effect apply to the caster?")]
        public bool affectsSelf = true;

        /// <summary>
        /// Apply the effect to the target entity.
        /// Called when ability activates or when entity enters effect zone.
        /// </summary>
        /// <param name="caster">The entity that cast the ability</param>
        /// <param name="target">The entity to apply the effect to</param>
        /// <param name="context">Additional context data (position, direction, etc.)</param>
        public abstract void Apply(AbilityUser caster, GameObject target, AbilityContext context);

        /// <summary>
        /// Remove the effect from the target entity.
        /// Called when ability expires or entity exits effect zone.
        /// </summary>
        /// <param name="target">The entity to remove the effect from</param>
        public abstract void Remove(GameObject target);

        /// <summary>
        /// Helper method to determine if this effect should apply to a target.
        /// Checks ally/enemy flags and caster relationship.
        /// </summary>
        /// <param name="caster">The entity that cast the ability</param>
        /// <param name="target">The potential target</param>
        /// <returns>True if effect should be applied</returns>
        public virtual bool ShouldApplyToTarget(AbilityUser caster, GameObject target)
        {
            // Check if target is self
            if (caster.gameObject == target)
            {
                return affectsSelf;
            }

            // Determine if target is ally or enemy
            bool isAlly = IsAlly(caster, target);

            if (isAlly && !affectsAllies) return false;
            if (!isAlly && !affectsEnemies) return false;

            return true;
        }

        /// <summary>
        /// Determine if target is an ally of the caster.
        /// Players are allies with players, enemies are allies with enemies.
        /// </summary>
        protected virtual bool IsAlly(AbilityUser caster, GameObject target)
        {
            // Check if target has AbilityUser component
            if (target.TryGetComponent<AbilityUser>(out var targetAbilityUser))
            {
                return caster.entityType == targetAbilityUser.entityType;
            }

            // Fallback: check tags
            bool casterIsPlayer = caster.entityType == EntityType.Player;
            bool targetIsPlayer = target.CompareTag("Player");
            return casterIsPlayer == targetIsPlayer;
        }
    }

    /// <summary>
    /// All possible effect types that can be applied by abilities.
    /// Each type should have a corresponding concrete implementation.
    /// </summary>
    public enum EffectType
    {
        Damage,             // Deal damage to target
        Heal,               // Restore health to target
        ModifySpeed,        // Change movement speed (buff/debuff)
        ModifyDamage,       // Change damage output (buff/debuff)
        Knockback,          // Apply knockback force
        Pull,               // Pull target toward caster
        Stun,               // Disable entity for duration
        Root,               // Prevent movement for duration
        Shield,             // Grant temporary HP
        Reveal,             // Make enemies visible through walls/fog
        Summon,             // Spawn minions
        Teleport,           // Move caster to new position
        Possess,            // Take control of target entity
        Transform,          // Change entity state (phase, invulnerable, etc.)
        AreaDenial,         // Create damaging ground zone
        ModifySaturation,   // Change breach saturation gain rate
        ModifyFireRate,     // Change weapon fire rate
        ModifyReloadSpeed,  // Change weapon reload speed
        CreateDeployable    // Spawn deployable object (shield, totem, etc.)
    }
}
