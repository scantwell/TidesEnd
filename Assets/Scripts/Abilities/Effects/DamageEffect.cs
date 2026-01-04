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
        public override EffectType EffectType => EffectType.Damage;

        [Header("Damage Parameters")]
        [Tooltip("Amount of damage to deal")]
        public float damageAmount = 10f;

        [Tooltip("Type of damage dealt")]
        public DamageType damageType = DamageType.Physical;

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
                // Create DamageInfo struct for ability damage
                var damageInfo = new DamageInfo
                {
                    BaseDamage = damageAmount,
                    DamageType = damageType,
                    Source = DamageSource.Ability,
                    IsCritical = canCrit,
                    IsHeadshot = false, // Abilities don't use headshots by default
                    AttackerId = caster.NetworkObjectId,
                    SourceId = 0, // TODO: Pass ability ID from AbilityData
                    Distance = Vector3.Distance(caster.transform.position, target.transform.position)
                };

                damageable.TakeDamage(damageInfo);
            }
        }

        public override void Remove(GameObject target)
        {
            // Damage is instant, no removal needed
        }
    }
}
