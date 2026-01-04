using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Centralized stat management component for all entities (Players, Enemies).
    /// Handles stat modifications from abilities, buffs, and debuffs.
    /// Server-authoritative with automatic NetworkVariable synchronization to clients.
    /// </summary>
    public class EntityStats : NetworkBehaviour
    {
        [Header("Character Configuration")]
        [SerializeField] private CharacterStatsData characterStats;

        [Header("Entity Type")]
        [Tooltip("Type of entity (Player, Enemy, Boss) - used for ability targeting")]
        [SerializeField] private EntityType entityType = EntityType.Player;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        /// <summary>
        /// Get the entity type (Player, Enemy, Boss).
        /// Used by ability targeting system to determine allies/enemies.
        /// </summary>
        public EntityType EntityType => entityType;

        // Networked final stat values (auto-synced to all clients)
        // Movement
        public NetworkVariable<float> WalkSpeed = new NetworkVariable<float>();
        public NetworkVariable<float> SprintSpeed = new NetworkVariable<float>();
        public NetworkVariable<float> JumpHeight = new NetworkVariable<float>();

        // Health
        public NetworkVariable<float> MaxHealth = new NetworkVariable<float>();
        public NetworkVariable<float> RegenRate = new NetworkVariable<float>();
        public NetworkVariable<float> RegenDelay = new NetworkVariable<float>();

        // Weapon
        public NetworkVariable<float> FireRate = new NetworkVariable<float>();
        public NetworkVariable<float> ReloadSpeed = new NetworkVariable<float>();
        public NetworkVariable<float> MagazineSize = new NetworkVariable<float>();
        public NetworkVariable<float> Spread = new NetworkVariable<float>();
        public NetworkVariable<float> Recoil = new NetworkVariable<float>();

        // Combat
        public NetworkVariable<float> DamageOutput = new NetworkVariable<float>();
        public NetworkVariable<float> CriticalChance = new NetworkVariable<float>();
        public NetworkVariable<float> Armor = new NetworkVariable<float>();

        // Status Effects
        public NetworkVariable<StatusEffects> ActiveStatuses = new NetworkVariable<StatusEffects>(StatusEffects.None);

        // Server-only modifier tracking (doesn't need to be networked)
        private Dictionary<string, StatModifier> activeModifiers = new Dictionary<string, StatModifier>();

        /// <summary>
        /// Initialize stats from a ClassData configuration.
        /// Used when a player selects a class at spawn.
        /// Server-only.
        /// </summary>
        public void InitializeFromClass(ClassData classData)
        {
            if (!IsServer)
            {
                Debug.LogWarning("[EntityStats] InitializeFromClass should only be called on server!");
                return;
            }

            if (classData == null || classData.baseStats == null)
            {
                Debug.LogError("[EntityStats] Cannot initialize from null ClassData or missing baseStats!");
                return;
            }

            // Set the character stats reference
            characterStats = classData.baseStats;

            // Set entity type (players should always be EntityType.Player)
            entityType = EntityType.Player;

            // Initialize character stats
            WalkSpeed.Value = characterStats.walkSpeed;
            SprintSpeed.Value = characterStats.sprintSpeed;
            JumpHeight.Value = characterStats.jumpHeight;

            MaxHealth.Value = characterStats.maxHealth;
            RegenRate.Value = characterStats.regenRate;
            RegenDelay.Value = characterStats.regenDelay;

            DamageOutput.Value = characterStats.damageOutput;
            CriticalChance.Value = characterStats.criticalChance;
            Armor.Value = characterStats.armor;

            // Weapon stats are MODIFIERS (1.0 = neutral)
            FireRate.Value = 1f;
            ReloadSpeed.Value = 1f;
            MagazineSize.Value = 1f;
            Spread.Value = 1f;
            Recoil.Value = 1f;

            if (debugMode)
                Debug.Log($"[EntityStats] Initialized from class '{classData.className}' with stats '{characterStats.name}'");
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Initialize all NetworkVariables
                // If CharacterStatsData assigned, use those values; otherwise keep prefab/existing values

                if (characterStats != null)
                {
                    // Initialize from CharacterStatsData
                    WalkSpeed.Value = characterStats.walkSpeed;
                    SprintSpeed.Value = characterStats.sprintSpeed;
                    JumpHeight.Value = characterStats.jumpHeight;

                    MaxHealth.Value = characterStats.maxHealth;
                    RegenRate.Value = characterStats.regenRate;
                    RegenDelay.Value = characterStats.regenDelay;

                    DamageOutput.Value = characterStats.damageOutput;
                    CriticalChance.Value = characterStats.criticalChance;
                    Armor.Value = characterStats.armor;

                    if (debugMode)
                        Debug.Log($"[EntityStats] Initialized {gameObject.name} from CharacterStatsData '{characterStats.name}'");
                }
                else
                {
                    // No CharacterStatsData - keep existing NetworkVariable values from prefab
                    // Only set defaults if values are still 0 (not configured in prefab)
                    if (WalkSpeed.Value == 0f) WalkSpeed.Value = 5f;
                    if (SprintSpeed.Value == 0f) SprintSpeed.Value = 8f;
                    if (JumpHeight.Value == 0f) JumpHeight.Value = 2f;
                    if (MaxHealth.Value == 0f) MaxHealth.Value = 100f;
                    if (RegenDelay.Value == 0f) RegenDelay.Value = 5f;
                    if (DamageOutput.Value == 0f) DamageOutput.Value = 1f;

                    if (debugMode)
                        Debug.Log($"[EntityStats] Using prefab values for {gameObject.name} (WalkSpeed={WalkSpeed.Value}, SprintSpeed={SprintSpeed.Value})");
                }

                // Weapon stats are MODIFIERS (1.0 = neutral), not base values
                // Actual weapon values come from WeaponData
                if (FireRate.Value == 0f) FireRate.Value = 1f;
                if (ReloadSpeed.Value == 0f) ReloadSpeed.Value = 1f;
                if (MagazineSize.Value == 0f) MagazineSize.Value = 1f;
                if (Spread.Value == 0f) Spread.Value = 1f;
                if (Recoil.Value == 0f) Recoil.Value = 1f;
            }

            // Clients can subscribe to value changes for UI updates
            if (IsClient)
            {
                WalkSpeed.OnValueChanged += OnMovementStatChanged;
                SprintSpeed.OnValueChanged += OnMovementStatChanged;
                MaxHealth.OnValueChanged += OnHealthStatChanged;
                RegenRate.OnValueChanged += OnHealthStatChanged;
                // Add more subscriptions as needed for UI updates
            }
        }

        /// <summary>
        /// Add a stat modifier from an ability/buff/debuff.
        /// Server-only. Automatically recalculates affected stat and syncs to clients.
        /// </summary>
        /// <param name="modifierId">Unique ID for this modifier (e.g., "ability_heal_player_123")</param>
        /// <param name="statType">Which stat to modify</param>
        /// <param name="value">Modification value (flat bonus or multiplier)</param>
        /// <param name="isMultiplier">If true, value is multiplier (1.5 = +50%). If false, flat bonus.</param>
        public void AddModifier(string modifierId, StatType statType, float value, bool isMultiplier)
        {
            if (!IsServer)
            {
                Debug.LogWarning("[EntityStats] AddModifier called on client! Only server can modify stats.");
                return;
            }

            // Add or update modifier
            activeModifiers[modifierId] = new StatModifier(statType, value, isMultiplier);

            if (debugMode)
                Debug.Log($"[EntityStats] Added modifier '{modifierId}': {statType} {(isMultiplier ? "x" : "+")}{value}");

            // Recalculate the affected stat
            RecalculateStat(statType);
        }

        /// <summary>
        /// Remove a stat modifier.
        /// Server-only. Automatically recalculates affected stat and syncs to clients.
        /// </summary>
        /// <param name="modifierId">The unique ID of the modifier to remove</param>
        public void RemoveModifier(string modifierId)
        {
            if (!IsServer)
            {
                Debug.LogWarning("[EntityStats] RemoveModifier called on client! Only server can modify stats.");
                return;
            }

            if (activeModifiers.TryGetValue(modifierId, out StatModifier modifier))
            {
                StatType affectedStat = modifier.StatType;
                activeModifiers.Remove(modifierId);

                if (debugMode)
                    Debug.Log($"[EntityStats] Removed modifier '{modifierId}'");

                // Recalculate the affected stat
                RecalculateStat(affectedStat);
            }
        }

        /// <summary>
        /// Recalculate a specific stat based on all active modifiers.
        /// Server-only. Updates NetworkVariable which auto-syncs to clients.
        /// </summary>
        private void RecalculateStat(StatType statType)
        {
            if (!IsServer) return;

            // Get base value for this stat
            float baseValue = GetBaseStat(statType);

            // Calculate final value with all modifiers
            float finalValue = CalculateFinalValue(statType, baseValue);

            // Update the appropriate NetworkVariable
            SetNetworkStat(statType, finalValue);

            if (debugMode)
                Debug.Log($"[EntityStats] Recalculated {statType}: {baseValue} → {finalValue}");
        }

        /// <summary>
        /// Calculate final stat value from base value and all active modifiers.
        /// Formula: (Base + ΣFlat) * ΠMultipliers
        /// </summary>
        private float CalculateFinalValue(StatType statType, float baseValue)
        {
            // Get all modifiers for this stat type
            var relevantModifiers = activeModifiers.Values.Where(m => m.StatType == statType).ToList();

            if (relevantModifiers.Count == 0)
                return baseValue;

            // Step 1: Sum all flat bonuses
            float flatBonus = relevantModifiers
                .Where(m => !m.IsMultiplier)
                .Sum(m => m.Value);

            // Step 2: Multiply all multipliers
            float totalMultiplier = relevantModifiers
                .Where(m => m.IsMultiplier)
                .Aggregate(1f, (acc, m) => acc * m.Value);

            // Final calculation
            return (baseValue + flatBonus) * totalMultiplier;
        }

        /// <summary>
        /// Get the base value for a stat type.
        /// If CharacterStatsData is assigned, uses those values.
        /// Otherwise returns sensible defaults.
        /// </summary>
        private float GetBaseStat(StatType statType)
        {
            if (characterStats != null)
            {
                return characterStats.GetStat(statType);
            }

            // Default values when no CharacterStatsData is assigned
            return statType switch
            {
                StatType.WalkSpeed => 5f,
                StatType.SprintSpeed => 8f,
                StatType.JumpHeight => 2f,
                StatType.MaxHealth => 100f,
                StatType.RegenRate => 0f,
                StatType.RegenDelay => 5f,
                StatType.DamageOutput => 1f,
                StatType.CriticalChance => 0f,
                StatType.Armor => 0f,
                // Weapon stats are modifiers (1.0 = neutral)
                StatType.FireRate => 1f,
                StatType.ReloadSpeed => 1f,
                StatType.MagazineSize => 1f,
                StatType.Spread => 1f,
                StatType.Recoil => 1f,
                _ => 0f
            };
        }

        /// <summary>
        /// Set the NetworkVariable for a stat type.
        /// Server-only. NetworkVariable handles synchronization to clients.
        /// </summary>
        private void SetNetworkStat(StatType statType, float value)
        {
            if (!IsServer) return;

            switch (statType)
            {
                case StatType.WalkSpeed:
                    WalkSpeed.Value = value;
                    break;
                case StatType.SprintSpeed:
                    SprintSpeed.Value = value;
                    break;
                case StatType.JumpHeight:
                    JumpHeight.Value = value;
                    break;
                case StatType.MaxHealth:
                    MaxHealth.Value = value;
                    break;
                case StatType.RegenRate:
                    RegenRate.Value = value;
                    break;
                case StatType.RegenDelay:
                    RegenDelay.Value = value;
                    break;
                case StatType.FireRate:
                    FireRate.Value = value;
                    break;
                case StatType.ReloadSpeed:
                    ReloadSpeed.Value = value;
                    break;
                case StatType.MagazineSize:
                    MagazineSize.Value = value;
                    break;
                case StatType.Spread:
                    Spread.Value = value;
                    break;
                case StatType.Recoil:
                    Recoil.Value = value;
                    break;
                case StatType.DamageOutput:
                    DamageOutput.Value = value;
                    break;
                case StatType.CriticalChance:
                    CriticalChance.Value = value;
                    break;
                case StatType.Armor:
                    Armor.Value = value;
                    break;
            }
        }

        // Client-side callbacks for stat changes (useful for UI updates)
        private void OnMovementStatChanged(float oldValue, float newValue)
        {
            if (debugMode)
                Debug.Log($"[EntityStats] Movement stat changed: {oldValue} → {newValue}");
            // UI can hook into this to update movement speed display
        }

        private void OnHealthStatChanged(float oldValue, float newValue)
        {
            if (debugMode)
                Debug.Log($"[EntityStats] Health stat changed: {oldValue} → {newValue}");
            // UI can hook into this to update health bar, regen display, etc.
        }

        /// <summary>
        /// Add a status effect to the entity.
        /// Server-only. Automatically syncs to clients via NetworkVariable.
        /// </summary>
        /// <param name="effect">The status effect to add</param>
        public void AddStatusEffect(StatusEffects effect)
        {
            if (!IsServer)
            {
                Debug.LogWarning("[EntityStats] AddStatusEffect called on client! Only server can modify status effects.");
                return;
            }

            var previous = ActiveStatuses.Value;
            ActiveStatuses.Value |= effect;

            if (debugMode && previous != ActiveStatuses.Value)
                Debug.Log($"[EntityStats] Added status effect: {effect}. Active statuses: {ActiveStatuses.Value}");
        }

        /// <summary>
        /// Remove a status effect from the entity.
        /// Server-only. Automatically syncs to clients via NetworkVariable.
        /// </summary>
        /// <param name="effect">The status effect to remove</param>
        public void RemoveStatusEffect(StatusEffects effect)
        {
            if (!IsServer)
            {
                Debug.LogWarning("[EntityStats] RemoveStatusEffect called on client! Only server can modify status effects.");
                return;
            }

            var previous = ActiveStatuses.Value;
            ActiveStatuses.Value &= ~effect;

            if (debugMode && previous != ActiveStatuses.Value)
                Debug.Log($"[EntityStats] Removed status effect: {effect}. Active statuses: {ActiveStatuses.Value}");
        }

        /// <summary>
        /// Check if the entity has a specific status effect.
        /// Can be called on both server and client.
        /// </summary>
        /// <param name="effect">The status effect to check</param>
        /// <returns>True if the effect is active</returns>
        public bool HasStatus(StatusEffects effect)
        {
            return ActiveStatuses.Value.HasFlag(effect);
        }

        /// <summary>
        /// Clear all status effects.
        /// Server-only.
        /// </summary>
        public void ClearAllStatusEffects()
        {
            if (!IsServer)
            {
                Debug.LogWarning("[EntityStats] ClearAllStatusEffects called on client! Only server can modify status effects.");
                return;
            }

            ActiveStatuses.Value = StatusEffects.None;

            if (debugMode)
                Debug.Log("[EntityStats] Cleared all status effects");
        }

        /// <summary>
        /// Debug method to list all active modifiers.
        /// </summary>
        [ContextMenu("Debug: List Active Modifiers")]
        private void DebugListModifiers()
        {
            if (activeModifiers.Count == 0)
            {
                Debug.Log("[EntityStats] No active modifiers");
                return;
            }

            Debug.Log($"[EntityStats] Active Modifiers ({activeModifiers.Count}):");
            foreach (var kvp in activeModifiers)
            {
                var mod = kvp.Value;
                Debug.Log($"  {kvp.Key}: {mod.StatType} {(mod.IsMultiplier ? "x" : "+")}{mod.Value}");
            }
        }
    }
}
