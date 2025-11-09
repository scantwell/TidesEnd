using Unity.Netcode;
using UnityEngine;
using System;

namespace TidesEnd.Combat {
    public class Health : NetworkBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool regenerateHealth = false;
        [SerializeField] private float regenRate = 5f; // HP per second
        [SerializeField] private float regenDelay = 3f; // Seconds after damage before regen starts
        
        [Header("Damage Modifiers")]
        [SerializeField] private float normalDamageMultiplier = 1f;
        [SerializeField] private float fireDamageMultiplier = 1f;
        [SerializeField] private float explosiveDamageMultiplier = 1f;
        [SerializeField] private float poisonDamageMultiplier = 1f;
        [SerializeField] private float electricDamageMultiplier = 1f;
        
        [Header("Special Zones")]
        [SerializeField] private bool hasHeadshot = true;
        [SerializeField] private Transform headTransform;
        [SerializeField] private float headshotRadius = 0.3f;
        
        [Header("Death Settings")]
        [SerializeField] private bool destroyOnDeath = false;
        [SerializeField] private float destroyDelay = 5f;
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private bool autoRespawn = true;
        [SerializeField] private float respawnDelay = 5f;
        [SerializeField] private float respawnHealth = 100f;
        
        // Network synchronized health
        private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
            100f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        
        private NetworkVariable<bool> isDead = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        
        private float lastDamageTime;
        
        // Events
        public event Action<DamageInfo> OnDamaged;
        public event Action OnDied;
        public event Action OnRevived;
        
        // IDamageable implementation
        public bool IsAlive => !isDead.Value;
        public float CurrentHealth => currentHealth.Value;
        public float MaxHealth => maxHealth;
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        
        private void Awake()
        {
            // Auto-find head if not assigned
            if (hasHeadshot && headTransform == null)
            {
                headTransform = transform.Find("Head") ?? transform.Find("head");
            }
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            // Server initializes health
            if (IsServer)
            {
                currentHealth.Value = maxHealth;
                isDead.Value = false;
            }
            
            // Listen for changes
            currentHealth.OnValueChanged += OnHealthChanged;
            isDead.OnValueChanged += OnDeathStateChanged;
        }
        
        public override void OnNetworkDespawn()
        {
            currentHealth.OnValueChanged -= OnHealthChanged;
            isDead.OnValueChanged -= OnDeathStateChanged;
            base.OnNetworkDespawn();
        }
        
        private void Update()
        {
            // Only server handles regeneration
            if (!IsServer) return;
            if (!regenerateHealth) return;
            if (isDead.Value) return;
            if (currentHealth.Value >= maxHealth) return;
            
            // Check if enough time has passed since last damage
            if (Time.time - lastDamageTime >= regenDelay)
            {
                RegenerateHealth(regenRate * Time.deltaTime);
            }
        }
        
        public void TakeDamage(float damage, ulong attackerId = 0, Vector3 hitPoint = default, Vector3 hitNormal = default)
        {
            // If we're on the server, process directly
            if (IsServer)
            {
                ProcessDamage(damage, attackerId, hitPoint, hitNormal);
            }
            else
            {
                // If we're on a client, request the server to process damage
                RequestDamageServerRpc(damage, attackerId, hitPoint, hitNormal);
            }
        }

        /// <summary>
        /// ServerRpc called by clients to request damage processing on the server.
        /// This allows client-side projectile hits to be validated and applied by the server.
        /// RequireOwnership = false allows any client to call this on any Health component.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void RequestDamageServerRpc(float damage, ulong attackerId, Vector3 hitPoint, Vector3 hitNormal)
        {
            // Server processes the damage request
            ProcessDamage(damage, attackerId, hitPoint, hitNormal);
        }

        /// <summary>
        /// Server-authoritative damage processing.
        /// Only runs on server, applies damage, checks for death, and notifies clients.
        /// </summary>
        private void ProcessDamage(float damage, ulong attackerId, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (!IsServer) return;
            if (isDead.Value) return;
            if (damage <= 0) return;

            // Check for headshot
            bool isHeadshot = false;
            float damageMultiplier = 1f;

            if (hasHeadshot && headTransform != null && hitPoint != default)
            {
                float distanceToHead = Vector3.Distance(hitPoint, headTransform.position);
                if (distanceToHead <= headshotRadius)
                {
                    isHeadshot = true;
                    // Headshot multiplier would come from weapon, but apply base multiplier here
                    damageMultiplier = 2f;
                    Debug.Log($"[Server] HEADSHOT on {gameObject.name}!");
                }
            }

            // Apply damage type multiplier (would be passed in DamageInfo in full implementation)
            // For now, just apply base multiplier
            float finalDamage = damage * damageMultiplier;

            // Apply damage
            currentHealth.Value = Mathf.Max(0, currentHealth.Value - finalDamage);
            lastDamageTime = Time.time;

            Debug.Log($"[Server] {gameObject.name} took {finalDamage} damage from attacker {attackerId}. Health: {currentHealth.Value}/{maxHealth}");

            // Create damage info
            DamageInfo damageInfo = new DamageInfo(finalDamage, DamageType.Normal, attackerId, hitPoint, hitNormal);

            // Notify clients of damage
            NotifyDamageClientRpc(damageInfo, isHeadshot);

            // Check for death
            if (currentHealth.Value <= 0 && !isDead.Value)
            {
                Die(attackerId);
            }
        }
        
        [ClientRpc]
        private void NotifyDamageClientRpc(DamageInfo damageInfo, bool isHeadshot)
        {
            OnDamaged?.Invoke(damageInfo);
            
            // Play damage effects (blood, sparks, etc.)
            // TODO: Implement damage effect system
            
            if (isHeadshot)
            {
                Debug.Log($"[Client] Headshot effect on {gameObject.name}");
            }
        }
        
        private void Die(ulong killerId)
        {
            if (!IsServer) return;
            if (isDead.Value) return;

            isDead.Value = true;

            Debug.Log($"[Server] {gameObject.name} died! Killed by {killerId}");

            // Notify all clients
            NotifyDeathClientRpc(killerId);

            // Spawn death effect
            if (deathEffectPrefab != null)
            {
                SpawnDeathEffectClientRpc(transform.position, transform.rotation);
            }

            // Auto-respawn after delay if configured
            if (autoRespawn)
            {
                Invoke(nameof(RespawnPlayer), respawnDelay);
            }
            // Otherwise destroy after delay if configured
            else if (destroyOnDeath)
            {
                Invoke(nameof(DestroyEntity), destroyDelay);
            }
        }

        private void RespawnPlayer()
        {
            if (!IsServer) return;

            // Revive the player with specified respawn health
            isDead.Value = false;
            currentHealth.Value = respawnHealth;

            Debug.Log($"[Server] {gameObject.name} respawned with {respawnHealth} HP!");

            // Notify all clients to re-enable components
            NotifyReviveClientRpc();
        }
        
        [ClientRpc]
        private void NotifyDeathClientRpc(ulong killerId)
        {
            OnDied?.Invoke();
            
            // Play death animation, disable components, etc.
            HandleDeath();
        }
        
        [ClientRpc]
        private void SpawnDeathEffectClientRpc(Vector3 position, Quaternion rotation)
        {
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, position, rotation);
            }
        }
        
        private void HandleDeath()
        {
            // Disable character controller to make corpse fall
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
            }
            
            // Disable collider
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            
            // Disable scripts
            var fpsController = GetComponent<NetworkedFPSController>();
            if (fpsController != null)
            {
                fpsController.enabled = false;
            }
            
            var combat = GetComponent<PlayerCombat>();
            if (combat != null)
            {
                combat.enabled = false;
            }
            
            // Play death animation
            var animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            
            Debug.Log($"[Client] {gameObject.name} death handled");
        }
        
        private void DestroyEntity()
        {
            if (!IsServer) return;
            
            if (NetworkObject != null)
            {
                NetworkObject.Despawn();
                Destroy(gameObject);
            }
        }
        
        public void Heal(float amount)
        {
            if (!IsServer) return;
            if (isDead.Value) return;
            if (amount <= 0) return;
            
            float oldHealth = currentHealth.Value;
            currentHealth.Value = Mathf.Min(maxHealth, currentHealth.Value + amount);
            
            Debug.Log($"[Server] {gameObject.name} healed {amount}. Health: {currentHealth.Value}/{maxHealth}");
            
            NotifyHealClientRpc(amount, oldHealth, currentHealth.Value);
        }
        
        [ClientRpc]
        private void NotifyHealClientRpc(float amount, float oldHealth, float newHealth)
        {
            // Play heal effect
            Debug.Log($"[Client] Healed {amount} HP");
        }
        
        private void RegenerateHealth(float amount)
        {
            if (!IsServer) return;
            
            currentHealth.Value = Mathf.Min(maxHealth, currentHealth.Value + amount);
        }
        
        public void Revive()
        {
            if (!IsServer) return;
            if (!isDead.Value) return;
            
            isDead.Value = false;
            currentHealth.Value = maxHealth;
            
            Debug.Log($"[Server] {gameObject.name} revived!");
            
            NotifyReviveClientRpc();
        }
        
        [ClientRpc]
        private void NotifyReviveClientRpc()
        {
            OnRevived?.Invoke();
            
            // Re-enable components
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = true;
            
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = true;
            
            var fpsController = GetComponent<NetworkedFPSController>();
            if (fpsController != null) fpsController.enabled = true;
            
            var combat = GetComponent<PlayerCombat>();
            if (combat != null) combat.enabled = true;
            
            Debug.Log($"[Client] {gameObject.name} revived!");
        }
        
        private void OnHealthChanged(float previousValue, float newValue)
        {
            // Update UI or other systems
            float healthPercent = newValue / maxHealth;
            
            // Visual feedback based on health
            if (healthPercent <= 0.25f)
            {
                // Low health effect (red screen, heavy breathing, etc.)
            }
        }
        
        private void OnDeathStateChanged(bool previousValue, bool newValue)
        {
            if (newValue) // Just died
            {
                Debug.Log($"{gameObject.name} death state changed to dead");
            }
            else // Revived
            {
                Debug.Log($"{gameObject.name} death state changed to alive");
            }
        }
        
        public float GetHealthPercent()
        {
            return currentHealth.Value / maxHealth;
        }
        
        public bool IsCriticalHealth()
        {
            return GetHealthPercent() <= 0.25f;
        }
        
        // Debug visualization
        private void OnDrawGizmosSelected()
        {
            if (!hasHeadshot || headTransform == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(headTransform.position, headshotRadius);
        }
    }
}