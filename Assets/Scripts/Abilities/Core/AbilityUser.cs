using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Component for entities that can CAST abilities (players, ability-using enemies, bosses).
    /// If an entity only needs to be AFFECTED BY abilities (basic enemies, props), use EntityStats instead.
    ///
    /// Responsibility:
    /// - Ability casting: cooldowns, casting, passive abilities
    /// - NOT required for targeting: EntityStats component is used for ability targeting
    ///
    /// Server-authoritative: all ability activations are validated and executed on server.
    /// </summary>
    public class AbilityUser : NetworkBehaviour
    {
        [Header("Entity Configuration")]
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

        // Internal state - Array-based cooldowns (thread-safe, network-ready)
        private float[] cooldownTimers; // Index matches activeAbilities[] array
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

            // Initialize cooldown timers array
            cooldownTimers = new float[activeAbilities.Length];
            for (int i = 0; i < cooldownTimers.Length; i++)
            {
                cooldownTimers[i] = 0f;
            }

            if (debugMode)
                Debug.Log($"[AbilityUser] {entityID} spawned");

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
        /// ServerRpc for clients to request ability activation.
        /// RequireOwnership = false allows AI entities to call this locally on server.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ActivateAbilityServerRpc(int abilityIndex, AbilityContext context)
        {
            TryActivateAbility(abilityIndex, context);
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
            if (!CanUseAbility(abilityIndex, ability))
            {
                if (debugMode)
                    Debug.Log($"[AbilityUser] Cannot use ability '{ability.abilityName}' (on cooldown or casting)");
                return false;
            }

            // Start cast
            StartAbilityCast(abilityIndex, ability, context);
            return true;
        }

        private bool CanUseAbility(int abilityIndex, AbilityData ability)
        {
            // Check cooldown
            if (IsOnCooldown(abilityIndex))
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

        private void StartAbilityCast(int abilityIndex, AbilityData ability, AbilityContext context)
        {
            if (ability.castTime > 0)
            {
                // Channeled/cast time ability
                isCasting = true;
                OnAbilityCastStarted?.Invoke(ability);

                // Start cast coroutine
                currentCastCoroutine = StartCoroutine(CastAbilityCoroutine(abilityIndex, ability, context));

                // Notify clients
                NotifyAbilityCastStartedClientRpc(ability.abilityName);
            }
            else
            {
                // Instant cast
                ExecuteAbility(abilityIndex, ability, context);
            }
        }

        private System.Collections.IEnumerator CastAbilityCoroutine(int abilityIndex, AbilityData ability, AbilityContext context)
        {
            float castTimer = 0f;

            while (castTimer < ability.castTime)
            {
                castTimer += Time.deltaTime;

                // Check for interruption
                if (isInterruptible && ShouldInterruptCast())
                {
                    InterruptCast(abilityIndex, ability);
                    yield break;
                }

                yield return null;
            }

            // Cast complete
            isCasting = false;
            ExecuteAbility(abilityIndex, ability, context);
        }

        private void ExecuteAbility(int abilityIndex, AbilityData ability, AbilityContext context)
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
            StartCooldown(abilityIndex, ability);

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

        private void StartCooldown(int abilityIndex, AbilityData ability)
        {
            if (abilityIndex >= 0 && abilityIndex < cooldownTimers.Length)
            {
                cooldownTimers[abilityIndex] = ability.cooldown;
                OnAbilityCooldownStarted?.Invoke(ability);

                if (debugMode)
                    Debug.Log($"[AbilityUser] Started cooldown for {ability.abilityName}: {ability.cooldown}s");
            }
        }

        private void UpdateCooldowns()
        {
            // Thread-safe: Simple array iteration, no collection modification during iteration
            for (int i = 0; i < cooldownTimers.Length; i++)
            {
                if (cooldownTimers[i] > 0)
                {
                    cooldownTimers[i] -= Time.deltaTime;

                    // Check if cooldown just completed
                    if (cooldownTimers[i] <= 0)
                    {
                        cooldownTimers[i] = 0; // Clamp to zero

                        // Trigger event for UI/AI
                        AbilityData ability = activeAbilities[i];
                        OnAbilityCooldownComplete?.Invoke(ability);

                        if (debugMode)
                            Debug.Log($"[AbilityUser] Cooldown complete: {ability.abilityName}");
                    }
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

        public bool IsOnCooldown(int abilityIndex)
        {
            if (abilityIndex < 0 || abilityIndex >= cooldownTimers.Length)
                return false;

            return cooldownTimers[abilityIndex] > 0;
        }

        public float GetCooldownRemaining(int abilityIndex)
        {
            if (abilityIndex < 0 || abilityIndex >= cooldownTimers.Length)
                return 0f;

            return Mathf.Max(0f, cooldownTimers[abilityIndex]);
        }

        // Overload for backward compatibility with AbilityData parameter
        public bool IsOnCooldown(AbilityData ability)
        {
            int index = System.Array.IndexOf(activeAbilities, ability);
            return IsOnCooldown(index);
        }

        public float GetCooldownRemaining(AbilityData ability)
        {
            int index = System.Array.IndexOf(activeAbilities, ability);
            return GetCooldownRemaining(index);
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

        private void InterruptCast(int abilityIndex, AbilityData ability)
        {
            isCasting = false;

            if (currentCastCoroutine != null)
            {
                StopCoroutine(currentCastCoroutine);
                currentCastCoroutine = null;
            }

            // Trigger cooldown on interruption
            StartCooldown(abilityIndex, ability);

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

        internal AbilityData GetAbilityData(int abilityIndex)
        {
            return (abilityIndex >= 0 && abilityIndex < activeAbilities.Length) ? activeAbilities[abilityIndex] : null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw gizmos when this entity is selected in the editor.
        /// Shows ability ranges, radii, and targeting modes.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Draw passive ability visualization
            if (passiveAbility != null && passiveAbility.range > 0)
            {
                GizmoHelpers.DrawWireCircle(transform.position, passiveAbility.range, new Color(0f, 1f, 1f, 0.5f), 32);
            }

            // Draw active abilities
            for (int i = 0; i < activeAbilities.Length; i++)
            {
                AbilityData ability = activeAbilities[i];
                if (ability == null) continue;

                // Choose color based on ability index
                Color abilityColor = i == 0 ? new Color(1f, 0.5f, 0f, 0.6f) : new Color(0.5f, 0f, 1f, 0.6f);

                // Draw range circle
                if (ability.range > 0)
                {
                    GizmoHelpers.DrawWireCircle(transform.position, ability.range, abilityColor, 48);
                }

                // Draw radius/AOE circle
                if (ability.radius > 0)
                {
                    // Draw a sphere to show AOE size
                    Vector3 previewPosition = transform.position + transform.forward * Mathf.Min(ability.range, 5f);
                    GizmoHelpers.DrawWireSphere(previewPosition, ability.radius, abilityColor);
                }

                // Draw targeting mode visualization
                switch (ability.targetingMode)
                {
                    case TargetingMode.Direction:
                        // Draw forward arrow
                        GizmoHelpers.DrawArrow(transform.position, transform.forward,
                            Mathf.Min(ability.range, 10f), abilityColor, 0.2f);
                        break;

                    case TargetingMode.Self:
                        // Draw sphere around self
                        if (ability.radius > 0)
                        {
                            GizmoHelpers.DrawWireSphere(transform.position, ability.radius, abilityColor);
                        }
                        break;

                    case TargetingMode.Ground:
                        // Draw circle on ground at preview position
                        Vector3 groundPreview = transform.position + transform.forward * Mathf.Min(ability.range * 0.5f, 5f);
                        if (ability.radius > 0)
                        {
                            GizmoHelpers.DrawWireCircle(groundPreview, ability.radius, abilityColor, 32);
                        }
                        break;
                }
            }

            // Draw cooldown indicators (only in Play mode)
            if (Application.isPlaying && cooldownTimers != null)
            {
                for (int i = 0; i < Mathf.Min(cooldownTimers.Length, activeAbilities.Length); i++)
                {
                    if (cooldownTimers[i] > 0 && activeAbilities[i] != null)
                    {
                        // Draw a small indicator above the entity
                        Vector3 labelPos = transform.position + Vector3.up * 2.5f + Vector3.right * (i * 0.5f - 0.25f);

                        // Draw cooldown as a colored sphere (red = on cooldown)
                        float cooldownPercent = cooldownTimers[i] / activeAbilities[i].cooldown;
                        Color cooldownColor = Color.Lerp(Color.green, Color.red, cooldownPercent);
                        GizmoHelpers.DrawWireSphere(labelPos, 0.1f, cooldownColor);
                    }
                }
            }
        }
#endif
        internal void LoadAbilities(List<AbilityData> abilities)
        {
            for (int i = 0; i < Mathf.Min(abilities.Count, activeAbilities.Length); i++)
            {
                activeAbilities[i] = abilities[i];
            }
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