using Unity.Netcode;
using UnityEngine;
using System;
using TidesEnd.Player;

namespace TidesEnd.Combat {
    public class Health : NetworkBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool regenerateHealth = false;
        [SerializeField] private float regenRate = 5f; // HP per second
        [SerializeField] private float regenDelay = 3f; // Seconds after damage before regen starts
        
        [Header("Death Settings")]
        [SerializeField] private bool destroyOnDeath = false;
        [SerializeField] private float destroyDelay = 5f;
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private bool autoRespawn = true;
        [SerializeField] private float respawnDelay = 5f;
        [SerializeField] private float respawnHealth = 100f;
        
        // Network synchronized health
        [SerializeField] private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
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
        public event Action<float, float, float> OnHealthChanged;
        
        // IDamageable implementation
        public bool IsAlive => !isDead.Value;
        public float CurrentHealth => currentHealth.Value;
        public float MaxHealth => maxHealth;
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // Register listeners FIRST so they catch all future changes
            currentHealth.OnValueChanged += OnHealthValueChanged;
            isDead.OnValueChanged += OnDeathStateChanged;

            // Server initializes health
            if (IsServer)
            {
                currentHealth.Value = maxHealth;
                isDead.Value = false;
                Debug.Log($"[Health.OnNetworkSpawn] Server initialized {gameObject.name}: health={maxHealth}, isDead=false");
            }
            else
            {
                Debug.Log($"[Health.OnNetworkSpawn] Client registered listener for {gameObject.name}. Current health from sync: {currentHealth.Value}");
            }
        }
        
        public override void OnNetworkDespawn()
        {
            currentHealth.OnValueChanged -= OnHealthValueChanged;
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
        
        public void TakeDamage(DamageInfo info)
        {
            // If we're on the server, process directly
            if (!IsServer) return;

            float finalDamage = DamageCalculator.CalculateDamage(info, this);
            float oldHealth = currentHealth.Value;
            currentHealth.Value = Mathf.Max(0, currentHealth.Value - finalDamage);
            lastDamageTime = Time.time;

            Debug.Log($"[Health.TakeDamage] {gameObject.name} took {finalDamage} damage. Health: {oldHealth} -> {currentHealth.Value}");

            // Notify clients of damage
            NotifyDamageClientRpc(info);

            // Check for death
            if (currentHealth.Value <= 0 && !isDead.Value)
            {
                Die(info.AttackerId);
            }
        }

        
        [ClientRpc]
        private void NotifyDamageClientRpc(DamageInfo damageInfo)
        {
            Debug.Log($"[NotifyDamageClientRpc] Called on {gameObject.name}, IsOwner={IsOwner}");
            if (IsOwner) {
                Debug.Log($"[NotifyDamageClientRpc] Invoking OnDamaged event");
                OnDamaged?.Invoke(damageInfo);
            }
            // Play damage effects (blood, sparks, etc.)
            // TODO: Implement damage effect system
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
            var fpsController = GetComponent<PlayerController>();
            if (fpsController != null)
            {
                fpsController.enabled = false;
            }
            
            var combat = GetComponent<PlayerWeaponController>();
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
            
            var fpsController = GetComponent<PlayerController>();
            if (fpsController != null) fpsController.enabled = true;
            
            var combat = GetComponent<PlayerWeaponController>();
            if (combat != null) combat.enabled = true;
            
            Debug.Log($"[Client] {gameObject.name} revived!");
        }
        
        private void OnHealthValueChanged(float previousValue, float newValue)
        {
            OnHealthChanged?.Invoke(previousValue, newValue, maxHealth);
            
            Debug.Log($"Health changed: {previousValue} → {newValue}");
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
    }
}