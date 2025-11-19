using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace TidesEnd.Player
{
    /// <summary>
    /// Manages player's scrip, ammo, and inventory items
    /// Server authoritative
    /// </summary>
    public class PlayerInventory : NetworkBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private int maxInventorySlots = 10;

        // NetworkVariables for synced state
        private NetworkVariable<int> scrip = new NetworkVariable<int>();
        private NetworkList<int> inventoryItems; // Item IDs


        // Server authority - syncs to all clients
        private NetworkList<AmmoEntry> serverAmmo;

        // Owner's local prediction - NOT networked
        private Dictionary<int, int> predictedAmmo = new Dictionary<int, int>();

        public event Action<int> OnScripChanged;
        public event Action<int> OnInventoryChanged; // Item added/removed
        public event Action<int, int> OnAmmoChanged; // WeaponID, count

        private void Awake()
        {
            inventoryItems = new NetworkList<int>();
            serverAmmo = new NetworkList<AmmoEntry>();
        }

        public override void OnNetworkSpawn()
        {
            scrip.OnValueChanged += OnScripValueChanged;
            inventoryItems.OnListChanged += OnInventoryListChanged;
        }

        public override void OnNetworkDespawn()
        {
            scrip.OnValueChanged -= OnScripValueChanged;
            inventoryItems.OnListChanged -= OnInventoryListChanged;
        }

        #region Scrip Management

        /// <summary>
        /// Server-only: Add scrip to player
        /// </summary>
        public void AddScrip(int amount)
        {
            if (!IsServer) return;
            // Add to scrip NetworkVariable
        }

        /// <summary>
        /// Server-only: Remove scrip (for purchases)
        /// </summary>
        public bool TrySpendScrip(int amount)
        {
            if (!IsServer) return false;
            // Check if enough scrip, subtract if valid
            return false;
        }

        #endregion

        #region Inventory Management

        /// <summary>
        /// Request to pick up item (called by client)
        /// </summary>
        [ServerRpc]
        public void PickupItemServerRpc(int itemId)
        {
            // Validate item exists, player is close enough
            // Add to inventoryItems NetworkList
        }

        /// <summary>
        /// Request to drop item (called by client)
        /// </summary>
        [ServerRpc]
        public void DropItemServerRpc(int inventorySlot)
        {
            // Remove from inventoryItems
            // Spawn item in world
        }

        /// <summary>
        /// Check if inventory has space
        /// </summary>
        public bool HasInventorySpace()
        {
            return inventoryItems.Count < maxInventorySlots;
        }

        #endregion

        #region Ammo Management

        /// <summary>
        /// Client predicts ammo usage, server validates
        /// </summary>
        public void ConsumeAmmoPredicted(int weaponId, int amount)
        {
            if (!IsOwner) return;
            // Decrease predicted ammo locally
            // Send ServerRpc to consume on server
            predictedAmmo.TryGetValue(weaponId, out int current);
            predictedAmmo[weaponId] = Math.Max(0, current - amount);
            //ConsumeAmmoServerRpc(weaponId, amount);
        }

        public void ConsumeAmmo(int weaponId, int amount)
        {
            if (!IsServer) return;

            // Server validates and consumes ammo
            // Syncs back to clients
            for (int i = 0; i < serverAmmo.Count; i++)
            {
                var entry = serverAmmo[i];
                if (entry.WeaponId == weaponId)
                {
                    entry.Count = Math.Max(0, entry.Count - amount);
                }
            }
        }

        /// <summary>
        /// Server-only: Add ammo for weapon type
        /// </summary>
        public void AddAmmo(int weaponId, int amount)
        {
            if (!IsServer) return;
            for (int i = 0; i < serverAmmo.Count; i++)
            {
                var entry = serverAmmo[i];
                if (entry.WeaponId == weaponId)
                {
                    entry.Count += amount;
                    serverAmmo[i] = entry;
                    return;
                }
            }
        }

        /// <summary>
        /// Get ammo count (returns predicted for owner, server for others)
        /// </summary>
        public int GetAmmoCount(int weaponId)
        {
            if (IsOwner)
            {
                // Owner uses prediction for instant feedback
                return predictedAmmo.GetValueOrDefault(weaponId, 0);
            }
            else
            {
                // Non-owner reads server value
                return GetServerAmmo(weaponId);
            }
        }

        private int GetServerAmmo(int weaponId)
        {
            foreach (var entry in serverAmmo)
            {
                if (entry.WeaponId == weaponId)
                    return entry.Count;
            }
            return 0;
        }

        #endregion

        private void OnScripValueChanged(int oldValue, int newValue)
        {
            OnScripChanged?.Invoke(newValue);
        }

        private void OnInventoryListChanged(NetworkListEvent<int> changeEvent)
        {
            // Handle item added/removed
            OnInventoryChanged?.Invoke(changeEvent.Value);
        }

        // Public getters
        public int CurrentScrip => scrip.Value;
        public int InventoryCount => inventoryItems.Count;
    }
    
    public struct AmmoEntry : INetworkSerializable, IEquatable<AmmoEntry>
    {
        public int WeaponId;
        public int Count;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref WeaponId);
            serializer.SerializeValue(ref Count);
        }

        public bool Equals(AmmoEntry other)
        {
            return WeaponId == other.WeaponId && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return obj is AmmoEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (WeaponId * 397) ^ Count;
            }
        }
    }
}