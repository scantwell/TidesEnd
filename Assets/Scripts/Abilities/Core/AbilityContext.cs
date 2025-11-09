using UnityEngine;
using Unity.Netcode;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Context data passed when activating an ability.
    /// Contains targeting information, positions, and references needed for ability execution.
    /// Networked via INetworkSerializable for client-server synchronization.
    /// </summary>
    [System.Serializable]
    public struct AbilityContext : INetworkSerializable
    {
        /// <summary>
        /// The targeted entity (can be enemy, ally, or null for ground-targeted abilities)
        /// </summary>
        public GameObject targetEntity;

        /// <summary>
        /// Target position in world space (for ground-targeted abilities, projectile destinations, etc.)
        /// </summary>
        public Vector3 targetPosition;

        /// <summary>
        /// Direction vector from caster (for directional abilities like projectiles)
        /// </summary>
        public Vector3 targetDirection;

        /// <summary>
        /// Exact hit position (for damage effects, impact VFX spawning, etc.)
        /// </summary>
        public Vector3 hitPosition;

        /// <summary>
        /// Optional VFX prefab to spawn (can be overridden per-context)
        /// </summary>
        [System.NonSerialized]
        public GameObject vfxPrefab;

        /// <summary>
        /// Network object ID of target entity (for network serialization)
        /// </summary>
        private ulong targetNetworkObjectId;

        /// <summary>
        /// Serialize/deserialize for network transmission.
        /// GameObject references are converted to NetworkObject IDs.
        /// </summary>
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // Serialize positions and directions
            serializer.SerializeValue(ref targetPosition);
            serializer.SerializeValue(ref targetDirection);
            serializer.SerializeValue(ref hitPosition);

            // Serialize target entity as NetworkObject ID
            if (serializer.IsWriter)
            {
                // Writing: Convert GameObject to NetworkObject ID
                if (targetEntity != null && targetEntity.TryGetComponent<NetworkObject>(out var netObj))
                {
                    targetNetworkObjectId = netObj.NetworkObjectId;
                }
                else
                {
                    targetNetworkObjectId = 0; // 0 = no target
                }
                serializer.SerializeValue(ref targetNetworkObjectId);
            }
            else
            {
                // Reading: Convert NetworkObject ID back to GameObject
                serializer.SerializeValue(ref targetNetworkObjectId);
                if (targetNetworkObjectId != 0)
                {
                    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(
                        targetNetworkObjectId, out var netObj))
                    {
                        targetEntity = netObj.gameObject;
                    }
                }
            }
        }

        /// <summary>
        /// Create a simple context for self-targeted abilities.
        /// </summary>
        public static AbilityContext CreateSelfContext(GameObject caster)
        {
            return new AbilityContext
            {
                targetEntity = caster,
                targetPosition = caster.transform.position,
                targetDirection = caster.transform.forward,
                hitPosition = caster.transform.position
            };
        }

        /// <summary>
        /// Create a context for ground-targeted abilities.
        /// </summary>
        public static AbilityContext CreateGroundContext(Vector3 groundPosition)
        {
            return new AbilityContext
            {
                targetEntity = null,
                targetPosition = groundPosition,
                targetDirection = Vector3.forward,
                hitPosition = groundPosition
            };
        }

        /// <summary>
        /// Create a context for entity-targeted abilities.
        /// </summary>
        public static AbilityContext CreateTargetContext(GameObject target)
        {
            return new AbilityContext
            {
                targetEntity = target,
                targetPosition = target.transform.position,
                targetDirection = Vector3.forward,
                hitPosition = target.transform.position
            };
        }
    }
}