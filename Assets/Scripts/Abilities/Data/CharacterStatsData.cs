using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// ScriptableObject that defines base stat values for a character archetype.
    /// Reusable across multiple classes (e.g., multiple scouts can share ScoutStats.asset).
    /// These are the BASE values - EntityStats applies modifiers on top of these.
    /// </summary>
    [CreateAssetMenu(fileName = "New Character Stats", menuName = "Tide's End/Character/Character Stats")]
    public class CharacterStatsData : ScriptableObject
    {
        [Header("Movement Stats")]
        [Tooltip("Base walking speed in meters per second")]
        public float walkSpeed = 5f;

        [Tooltip("Base sprinting speed in meters per second")]
        public float sprintSpeed = 8f;

        [Tooltip("Base jump height in meters")]
        public float jumpHeight = 2f;

        [Header("Health Stats")]
        [Tooltip("Maximum health points")]
        public float maxHealth = 100f;

        [Tooltip("Health regeneration per second (0 = no regen)")]
        public float regenRate = 0f;

        [Tooltip("Seconds after taking damage before regeneration begins")]
        public float regenDelay = 5f;

        [Header("Combat Stats")]
        [Tooltip("Damage output multiplier (1.0 = normal, 1.5 = +50% damage)")]
        public float damageOutput = 1f;

        [Tooltip("Critical hit chance (0.0 - 1.0, where 0.1 = 10%)")]
        [Range(0f, 1f)]
        public float criticalChance = 0f;

        [Tooltip("Flat damage reduction applied before percentage reductions")]
        public float armor = 0f;

        /// <summary>
        /// Get a stat value by type. Useful for dynamic stat queries.
        /// Note: Weapon stats (FireRate, ReloadSpeed, etc.) are not stored here - they're modifiers in EntityStats.
        /// </summary>
        public float GetStat(StatType statType)
        {
            return statType switch
            {
                StatType.WalkSpeed => walkSpeed,
                StatType.SprintSpeed => sprintSpeed,
                StatType.JumpHeight => jumpHeight,
                StatType.MaxHealth => maxHealth,
                StatType.RegenRate => regenRate,
                StatType.RegenDelay => regenDelay,
                StatType.DamageOutput => damageOutput,
                StatType.CriticalChance => criticalChance,
                StatType.Armor => armor,
                // Weapon stats are modifiers (1.0 default), not character stats
                StatType.FireRate => 1f,
                StatType.ReloadSpeed => 1f,
                StatType.MagazineSize => 1f,
                StatType.Spread => 1f,
                StatType.Recoil => 1f,
                _ => 0f
            };
        }

        /// <summary>
        /// Validation to ensure stats are within reasonable ranges.
        /// </summary>
        private void OnValidate()
        {
            // Clamp stats to reasonable ranges
            walkSpeed = Mathf.Max(0f, walkSpeed);
            sprintSpeed = Mathf.Max(walkSpeed, sprintSpeed); // Sprint should be >= walk
            jumpHeight = Mathf.Max(0f, jumpHeight);
            maxHealth = Mathf.Max(1f, maxHealth);
            regenRate = Mathf.Max(0f, regenRate);
            regenDelay = Mathf.Max(0f, regenDelay);
            damageOutput = Mathf.Max(0f, damageOutput);
            criticalChance = Mathf.Clamp01(criticalChance);
            armor = Mathf.Max(0f, armor);
        }
    }
}
