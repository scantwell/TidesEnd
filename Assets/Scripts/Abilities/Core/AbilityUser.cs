using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Universal ability component attached to ALL entities that can use abilities.
    /// Works for players, enemies, and bosses through a unified interface.
    /// Server-authoritative: all ability activations are validated and executed on server.
    /// </summary>
    public class AbilityUser : NetworkBehaviour
    {
        [Header("Entity Configuration")]
        [Tooltip("Type of entity (Player, Enemy, or Boss)")]
        public EntityType entityType = EntityType.Player;

        [Tooltip("Unique identifier for this entity (e.g., 'Bulwark', 'DrownedPriest', 'TideTouched')")]
        public string entityID = "";

        [Header("Abilities")]
        [Tooltip("Passive ability (always active, no cooldown)")]
        public AbilityData passiveAbility;

        [Tooltip("Active abilities (can be activated with cooldowns)")]
        public AbilityData[] activeAbilities = new AbilityData[0];

        [Header("Debug")]
        [Tooltip("Show debug logs for ability activation?")]
        public bool debugMode = false;

        // Internal state
        private Dictionary<string, float> cooldowns = new Dictionary<string, float>();
        private Dictionary<string, AbilityInstance> activeAbilityInstances = new Dictionary<string, AbilityInstance>();
        private bool isCasting = false;
        private bool isInterruptible = true;
        private Coroutine currentCastCoroutine = null;

        // Events for UI and AI integration
        public event System.Action<AbilityData> OnAbilityActivated;
        public event System.Action<AbilityData> OnAbilityCooldownStarted;
        public event System.Action<AbilityData> OnAbilityCooldownComplete;
        public event System.Action<AbilityData> OnAbilityCastStarted;
        public event System.Action<AbilityData> OnAbilityCastInterrupted;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (debugMode)
                Debug.Log($"[AbilityUser] {entityID} spawned. Entity type: {entityType}");

            // Apply passive ability on spawn
            if (passiveAbility != null && IsServer)
            {
                ApplyPassiveAbility();
            }
        }

        private void Update()
        {
            // Only server updates cooldowns and active abilities
            if (!IsServer) return;

            UpdateCooldowns();
            UpdateActiveAbilities();
        }

        /// <summary>
        /// Try to activate an ability by index.
        /// Server-authoritative: must be called on server or via ServerRpc.
        /// </summary>
        /// <param name="abilityIndex">Index in activeAbilities array</param>
        /// <param name="context">Context data (target, position, direction)</param>
        /// <returns>True if ability was activated successfully</returns>
        public bool TryActivateAbility(int abilityIndex, AbilityContext context)
        {
            if (!IsServer)
            {
                Debug.LogError($"[AbilityUser] Abilities can only be activated on server! Call via ServerRpc.");
                return false;
            }

            // Validate index
            if (abilityIndex < 0 || abilityIndex >= activeAbilities.Length)
            {
                if (debugMode)
                    Debug.LogWarning($"[AbilityUser] Invalid ability index: {abilityIndex}");
                return false;
            }

            AbilityData ability = activeAbilities[abilityIndex];

            // Validate ability can be used
            if (!CanUseAbility(ability))
            {
                if (debugMode)
                    Debug.Log($"[AbilityUser] Cannot use ability '{ability.abilityName}' (on cooldown or casting)");
                return false;
            }

            // Start cast
            StartAbilityCast(ability, context);
            return true;
        }

        private bool CanUseAbility(AbilityData ability)
        {
            // Check cooldown
            if (IsOnCooldown(ability))
                return false;

            // Check if already casting another ability
            if (isCasting && !ability.canCastWhileCasting)
                return false;

            // TODO: Add more validation
            // - Check resources (mana, energy, etc.)
            // - Check status effects (stunned, silenced, etc.)
            // - Check range to target

            return true;
        }

        private void StartAbilityCast(AbilityData ability, AbilityContext context)
        {
            if (ability.castTime > 0)
            {
                // Channeled/cast time ability
                isCasting = true;
                OnAbilityCastStarted?.Invoke(ability);

                // Start cast coroutine
                currentCastCoroutine = StartCoroutine(CastAbilityCoroutine(ability, context));

                // Notify clients
                NotifyAbilityCastStartedClientRpc(ability.abilityName);
            }
            else
            {
                // Instant cast
                ExecuteAbility(ability, context);
            }
        }

        private System.Collections.IEnumerator CastAbilityCoroutine(AbilityData ability, AbilityContext context)
        {
            float castTimer = 0f;

            while (castTimer < ability.castTime)
            {
                castTimer += Time.deltaTime;

                // Check for interruption
                if (isInterruptible && ShouldInterruptCast())
                {
                    InterruptCast(ability);
                    yield break;
                }

                yield return null;
            }

            // Cast complete
            isCasting = false;
            ExecuteAbility(ability, context);
        }

        private void ExecuteAbility(AbilityData ability, AbilityContext context)
        {
            if (debugMode)
                Debug.Log($"[AbilityUser] {entityID} executing ability: {ability.abilityName}");

            // Create ability instance based on type
            AbilityInstance instance = AbilityFactory.CreateAbilityInstance(ability, this, context);

            if (instance == null)
            {
                Debug.LogError($"[AbilityUser] Failed to create ability instance for {ability.abilityName}");
                return;
            }

            // Execute the ability
            instance.Execute();

            // Track active instances (for duration-based abilities)
            if (ability.duration > 0)
            {
                activeAbilityInstances[ability.abilityName] = instance;
            }

            // Start cooldown
            StartCooldown(ability);

            // Trigger events
            OnAbilityActivated?.Invoke(ability);

            // Sync to clients
            NotifyAbilityActivatedClientRpc(ability.abilityName, context);
        }

        [ClientRpc]
        private void NotifyAbilityCastStartedClientRpc(string abilityName)
        {
            // Play cast VFX/audio on clients
            // Update UI if this is local player
            if (debugMode)
                Debug.Log($"[AbilityUser] Client received cast started: {abilityName}");
        }

        [ClientRpc]
        private void NotifyAbilityActivatedClientRpc(string abilityName, AbilityContext context)
        {
            // Play activation VFX/audio on clients
            // Update UI if this is local player
            if (debugMode)
                Debug.Log($"[AbilityUser] Client received ability activated: {abilityName}");

            // TODO: Spawn VFX, play audio, update UI
        }

        private void StartCooldown(AbilityData ability)
        {
            cooldowns[ability.abilityName] = ability.cooldown;
            OnAbilityCooldownStarted?.Invoke(ability);

            if (debugMode)
                Debug.Log($"[AbilityUser] Started cooldown for {ability.abilityName}: {ability.cooldown}s");
        }

        private void UpdateCooldowns()
        {
            List<string> completedCooldowns = new List<string>();

            foreach (var key in cooldowns.Keys)
            {
                cooldowns[key] -= Time.deltaTime;

                if (cooldowns[key] <= 0)
                {
                    completedCooldowns.Add(key);
                }
            }

            foreach (string abilityName in completedCooldowns)
            {
                cooldowns.Remove(abilityName);

                // Find ability data and trigger event
                AbilityData ability = System.Array.Find(activeAbilities, a => a.abilityName == abilityName);
                if (ability != null)
                {
                    OnAbilityCooldownComplete?.Invoke(ability);

                    if (debugMode)
                        Debug.Log($"[AbilityUser] Cooldown complete: {abilityName}");
                }
            }
        }

        private void UpdateActiveAbilities()
        {
            List<string> expiredAbilities = new List<string>();

            foreach (var key in activeAbilityInstances.Keys)
            {
                activeAbilityInstances[key].Update(Time.deltaTime);

                if (activeAbilityInstances[key].IsExpired())
                {
                    activeAbilityInstances[key].Cleanup();
                    expiredAbilities.Add(key);

                    if (debugMode)
                        Debug.Log($"[AbilityUser] Ability expired: {key}");
                }
            }

            foreach (string abilityName in expiredAbilities)
            {
                activeAbilityInstances.Remove(abilityName);
            }
        }

        public bool IsOnCooldown(AbilityData ability)
        {
            return cooldowns.ContainsKey(ability.abilityName) && cooldowns[ability.abilityName] > 0;
        }

        public float GetCooldownRemaining(AbilityData ability)
        {
            if (!cooldowns.ContainsKey(ability.abilityName))
                return 0f;
            return Mathf.Max(0f, cooldowns[ability.abilityName]);
        }

        private void ApplyPassiveAbility()
        {
            if (passiveAbility == null)
                return;

            if (debugMode)
                Debug.Log($"[AbilityUser] Applying passive ability: {passiveAbility.abilityName}");

            // Create context for self
            AbilityContext context = AbilityContext.CreateSelfContext(gameObject);

            // Apply all passive effects
            foreach (var effect in passiveAbility.effects)
            {
                effect.Apply(this, gameObject, context);
            }
        }

        private bool ShouldInterruptCast()
        {
            // TODO: Implement interruption logic
            // - Check if taken damage
            // - Check if stunned
            // - Check if moved (for channeled abilities that require standing still)
            return false;
        }

        private void InterruptCast(AbilityData ability)
        {
            isCasting = false;

            if (currentCastCoroutine != null)
            {
                StopCoroutine(currentCastCoroutine);
                currentCastCoroutine = null;
            }

            // Trigger cooldown on interruption
            StartCooldown(ability);

            OnAbilityCastInterrupted?.Invoke(ability);

            if (debugMode)
                Debug.Log($"[AbilityUser] Cast interrupted: {ability.abilityName}");
        }

        /// <summary>
        /// Force interrupt current cast (for external stuns, damage, etc.)
        /// </summary>
        public void ForceInterruptCast()
        {
            if (!isCasting)
                return;

            // Find currently casting ability
            // For now, just stop the coroutine
            if (currentCastCoroutine != null)
            {
                StopCoroutine(currentCastCoroutine);
                currentCastCoroutine = null;
            }

            isCasting = false;

            if (debugMode)
                Debug.Log($"[AbilityUser] Cast force interrupted");
        }
    }

    /// <summary>
    /// Entity type determines faction and behavior.
    /// </summary>
    public enum EntityType
    {
        Player,
        Enemy,
        Boss
    }
}