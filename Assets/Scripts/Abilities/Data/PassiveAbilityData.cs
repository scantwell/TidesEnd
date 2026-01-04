using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Base class for passive abilities that are always active.
    /// Passives provide continuous benefits without player activation.
    /// Examples: faster interaction speed, bonus armor, revealing nearby enemies, etc.
    ///
    /// NOTE: This is a placeholder structure. Full implementation will come later.
    /// </summary>
    [CreateAssetMenu(fileName = "New Passive Ability", menuName = "Tide's End/Abilities/Passive Ability")]
    public class PassiveAbilityData : ScriptableObject
    {
        [Header("Passive Identity")]
        [Tooltip("Display name of the passive ability")]
        public string passiveName = "New Passive";

        [Tooltip("Description for UI")]
        [TextArea(2, 4)]
        public string description = "Passive description here...";

        [Tooltip("Icon for UI display")]
        public Sprite icon;

        [Header("Configuration")]
        [Tooltip("Type of passive effect")]
        public PassiveType passiveType = PassiveType.StatModifier;

        [Tooltip("Always active, or requires specific conditions?")]
        public bool alwaysActive = true;

        // Future: Add specific passive implementation data here
        // For now, this serves as a reference point for ClassData
    }

    /// <summary>
    /// Types of passive abilities.
    /// Will be expanded as we implement specific passive systems.
    /// </summary>
    public enum PassiveType
    {
        StatModifier,      // Modifies base stats (uses EntityStats system)
        Ability,           // Grants additional ability usage (extra charges, reduced cooldown)
        Interaction,       // Faster interactions (revive, loot, hack, etc.)
        Detection,         // Reveals enemies, scans environment
        Movement,          // Unique movement (wall climb, double jump)
        Survival,          // Auto-heal, damage reduction, shield regen
        Utility            // Miscellaneous unique effects
    }
}
