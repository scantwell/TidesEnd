using System.Collections.Generic;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Defines a playable character class (e.g., Pathfinder, Wraith, Lifeline, Gibraltar).
    /// Each class has unique abilities, passive, and references base stat values.
    /// Players select a class at match start, and this determines their loadout.
    /// </summary>
    [CreateAssetMenu(fileName = "New Class", menuName = "Tide's End/Character/Class Data")]
    public class ClassData : ScriptableObject
    {
        [Header("Class Identity")]
        [Tooltip("Display name of the class (e.g., 'Pathfinder')")]
        public string className = "New Class";

        [Tooltip("Class description for UI")]
        [TextArea(3, 5)]
        public string description = "Class description here...";

        [Tooltip("Class icon for UI menus")]
        public Sprite icon;

        [Tooltip("Character model prefab (optional, for visual representation)")]
        public GameObject characterModelPrefab;

        [Header("Base Stats")]
        [Tooltip("Reference to base stat values (reusable across classes)")]
        public CharacterStatsData baseStats;

        [Header("Abilities")]
        [Tooltip("Class-specific abilities (typically 2-3 abilities)")]
        public List<AbilityData> abilities = new List<AbilityData>();

        [Tooltip("Maximum number of abilities this class can have equipped")]
        public int maxAbilitySlots = 3;

        [Header("Passive Ability")]
        [Tooltip("Class-specific passive ability (always active)")]
        public PassiveAbilityData passiveAbility;

        [Header("Cosmetics")]
        [Tooltip("Primary color for UI elements")]
        public Color classColor = Color.white;

        /// <summary>
        /// Validate class configuration.
        /// </summary>
        private void OnValidate()
        {
            if (baseStats == null)
            {
                Debug.LogWarning($"[ClassData] '{className}' has no base stats assigned!");
            }

            if (abilities.Count == 0)
            {
                Debug.LogWarning($"[ClassData] '{className}' has no abilities assigned!");
            }

            if (abilities.Count > maxAbilitySlots)
            {
                Debug.LogWarning($"[ClassData] '{className}' has {abilities.Count} abilities but only {maxAbilitySlots} slots!");
            }

            // Remove null abilities
            abilities.RemoveAll(a => a == null);
        }

        /// <summary>
        /// Get a specific ability by index.
        /// </summary>
        public AbilityData GetAbility(int index)
        {
            if (index < 0 || index >= abilities.Count)
            {
                Debug.LogError($"[ClassData] Invalid ability index {index} for class '{className}'");
                return null;
            }

            return abilities[index];
        }

        /// <summary>
        /// Check if class has a specific ability.
        /// </summary>
        public bool HasAbility(AbilityData ability)
        {
            return abilities.Contains(ability);
        }
    }
}
