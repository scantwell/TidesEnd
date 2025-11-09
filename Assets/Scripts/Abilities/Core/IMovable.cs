namespace TidesEnd.Abilities
{
    /// <summary>
    /// Interface for entities that can have their movement speed modified.
    /// Implemented by NetworkedFPSController (players) and EnemyAI (enemies).
    /// Allows abilities to apply speed buffs/debuffs.
    /// </summary>
    public interface IMovable
    {
        /// <summary>
        /// Add a speed multiplier (multiplicative modifier).
        /// Example: multiplier of 0.7 = 70% speed (slow), 1.5 = 150% speed (fast)
        /// </summary>
        /// <param name="source">Unique identifier for this modifier (ability name, effect ID, etc.)</param>
        /// <param name="multiplier">Speed multiplier (1.0 = no change)</param>
        void AddSpeedMultiplier(string source, float multiplier);

        /// <summary>
        /// Add a flat speed bonus (additive modifier).
        /// Example: bonus of 2.0 = +2 units/sec, -1.0 = -1 units/sec
        /// </summary>
        /// <param name="source">Unique identifier for this modifier</param>
        /// <param name="bonus">Flat speed bonus in units/sec</param>
        void AddSpeedBonus(string source, float bonus);

        /// <summary>
        /// Remove a speed modifier by its source identifier.
        /// Removes both multipliers and bonuses with this source.
        /// </summary>
        /// <param name="source">Identifier of the modifier to remove</param>
        void RemoveSpeedModifier(string source);

        /// <summary>
        /// Get the current effective movement speed after all modifiers.
        /// </summary>
        float GetCurrentSpeed();
    }
}