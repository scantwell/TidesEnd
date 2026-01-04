using System;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Represents a temporary or permanent modification to an entity's stat.
    /// Used by EntityStats to track individual buffs/debuffs and calculate final stat values.
    /// </summary>
    [Serializable]
    public struct StatModifier
    {
        /// <summary>
        /// Which stat this modifier affects
        /// </summary>
        public StatType StatType;

        /// <summary>
        /// The modification value (e.g., +50 for flat, 1.5 for 50% multiplier)
        /// </summary>
        public float Value;

        /// <summary>
        /// If true, Value is a multiplier (1.5 = 150% = +50%).
        /// If false, Value is a flat bonus (+50 health).
        /// </summary>
        public bool IsMultiplier;

        public StatModifier(StatType statType, float value, bool isMultiplier)
        {
            StatType = statType;
            Value = value;
            IsMultiplier = isMultiplier;
        }
    }

    /// <summary>
    /// All modifiable stats in the game.
    /// Organized by system for clarity.
    /// </summary>
    public enum StatType
    {
        // Movement Stats
        WalkSpeed,
        SprintSpeed,
        JumpHeight,

        // Health Stats
        MaxHealth,
        RegenRate,
        RegenDelay,

        // Weapon Stats
        FireRate,
        ReloadSpeed,
        MagazineSize,
        Spread,
        Recoil,

        // Combat Stats
        DamageOutput,
        CriticalChance,
        Armor
    }

    /// <summary>
    /// Status effects that can be applied to entities.
    /// These are binary on/off conditions, not stat modifiers.
    /// Uses [Flags] to allow multiple effects simultaneously.
    /// </summary>
    [Flags]
    public enum StatusEffects
    {
        None = 0,               // No status effects
        Stunned = 1 << 0,       // Cannot move, attack, or use abilities
        Disarmed = 1 << 1,      // Cannot use weapons
        Silenced = 1 << 2,      // Cannot use abilities
        Rooted = 1 << 3,        // Cannot move but can still attack/use abilities
        Slowed = 1 << 4,        // Movement speed reduced (via stat modifier)
        Blinded = 1 << 5,       // Reduced vision/FOV
        Revealed = 1 << 6,      // Visible to enemies through walls
        Invulnerable = 1 << 7,  // Cannot take damage
        Invisible = 1 << 8      // Not visible to enemies
    }
}
