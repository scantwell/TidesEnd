using System;
using UnityEngine;
using TidesEnd.Combat;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Effect that applies damage to the target.
    /// Used by offensive abilities like harpoon, boss attacks, etc.
    /// </summary>
    [Serializable]
    public class DamageEffect : AbilityEffect
    {
        [Header("Damage Parameters")]
        [Tooltip("Type of damage dealt")]
        public DamageType damageType = DamageType.Normal;

        [Tooltip("Ignore armor/resistances?")]
        public bool ignoreArmor = false;

        [Tooltip("Can this damage crit/headshot?")]
        public bool canCrit = false;

        public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
        {
            if (!ShouldApplyToTarget(caster, target))
                return;

            // Check if target can take damage
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(magnitude,
                    caster.NetworkObjectId,
                    context.hitPosition != Vector3.zero ? context.hitPosition : target.transform.position,
                    context.targetDirection
                );
            }
        }

        public override void Remove(GameObject target)
        {
            // Damage is instant, no removal needed
        }
    }
}
