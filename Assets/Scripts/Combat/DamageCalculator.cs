using UnityEngine;

namespace TidesEnd.Combat
{
    /// <summary>
    /// Centralized damage calculation service.
    /// Applies universal damage modifiers (armor, resistances, critical hits).
    /// Weapon-specific calculations (headshot, falloff) are handled by WeaponLogic.
    /// </summary>
    public static class DamageCalculator
    {
        
        /// <summary>
        /// Calculate final damage considering universal modifiers.
        /// BaseDamage should already include weapon-specific modifiers (headshot, falloff).
        /// This applies target-specific modifiers (armor, resistances, critical hits).
        /// </summary>
        public static float CalculateDamage(DamageInfo info, IDamageable target)
        {
            float damage = info.BaseDamage;

            damage = ApplyCriticalModifier(damage, info);
            damage = ApplyTargetModifiers(damage, info, target);

            damage = Mathf.Max(0, damage);

            Debug.Log($"Damage calculated: {info.BaseDamage} → {damage} " +
                    $"(Critical: {info.IsCritical}, Distance: {info.Distance:F1}m)");

            return damage;
        }
        
        /// <summary>
        /// Critical hit multiplier
        /// </summary>
        private static float ApplyCriticalModifier(float damage, DamageInfo info)
        {
            if (!info.IsCritical) return damage;

            float critMultiplier = 1.5f; // Default crit multiplier
            return damage * critMultiplier;
        }

        /// <summary>
        /// Target-specific modifiers (armor, resistances)
        /// </summary>
        private static float ApplyTargetModifiers(float damage, DamageInfo info, IDamageable target)
        {
            if (target is IHasArmor armoredTarget)
            {
                damage = ApplyArmor(damage, armoredTarget.Armor);
            }

            if (target is IHasResistances resistantTarget)
            {
                damage = ApplyResistance(damage, info.DamageType, resistantTarget);
            }

            return damage;
        }
        
        /// <summary>
        /// Armor reduction calculation
        /// </summary>
        private static float ApplyArmor(float damage, float armorValue)
        {
            if (armorValue <= 0) return damage;
            
            // Armor formula: reduction = armor / (armor + 100)
            // 0 armor = 0% reduction
            // 50 armor = 33% reduction
            // 100 armor = 50% reduction
            // 200 armor = 66% reduction
            float armorReduction = armorValue / (armorValue + 100f);
            
            return damage * (1f - armorReduction);
        }
        
        /// <summary>
        /// Damage type resistance
        /// </summary>
        private static float ApplyResistance(float damage, DamageType damageType, IHasResistances target)
        {
            float resistance = target.GetResistance(damageType);
            if (resistance <= 0) return damage;

            return damage * (1f - resistance);
        }
    }
}