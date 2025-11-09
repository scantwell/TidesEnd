using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Component attached to deployed objects (zones, shields, anchors).
    /// Handles zone-based effect application via trigger colliders.
    /// Automatically added to zone/deployable objects that don't have it.
    /// </summary>
    public class DeployableObject : NetworkBehaviour
    {
        private AbilityData abilityData;
        private AbilityUser owner;
        private List<GameObject> entitiesInZone = new List<GameObject>();
        private SphereCollider triggerCollider;

        [Header("Debug")]
        public bool debugMode = false;

        /// <summary>
        /// Initialize the deployable with ability data and owner.
        /// Called by DeployableAbilityInstance or ZoneAbilityInstance.
        /// </summary>
        public void Initialize(AbilityData data, AbilityUser owner)
        {
            this.abilityData = data;
            this.owner = owner;

            if (debugMode)
                Debug.Log($"[DeployableObject] Initialized: {data.abilityName}");

            // Set up trigger collider for zone effects
            SetupTriggerCollider();
        }

        private void SetupTriggerCollider()
        {
            // Check if collider already exists
            triggerCollider = GetComponent<SphereCollider>();

            if (triggerCollider == null)
            {
                // Create sphere collider for zone
                triggerCollider = gameObject.AddComponent<SphereCollider>();
            }

            triggerCollider.radius = abilityData.radius;
            triggerCollider.isTrigger = true;

            if (debugMode)
                Debug.Log($"[DeployableObject] Set up trigger collider with radius {abilityData.radius}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return; // Only server processes zone effects

            if (debugMode)
                Debug.Log($"[DeployableObject] Entity entered zone: {other.gameObject.name}");

            // Apply all effects to entering entity
            foreach (var effect in abilityData.effects)
            {
                if (effect.ShouldApplyToTarget(owner, other.gameObject))
                {
                    AbilityContext context = new AbilityContext
                    {
                        targetEntity = other.gameObject,
                        targetPosition = other.transform.position
                    };

                    effect.Apply(owner, other.gameObject, context);

                    if (debugMode)
                        Debug.Log($"[DeployableObject] Applied effect {effect.effectType} to {other.gameObject.name}");
                }
            }

            entitiesInZone.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsServer) return;

            if (debugMode)
                Debug.Log($"[DeployableObject] Entity exited zone: {other.gameObject.name}");

            // Remove all effects from exiting entity
            foreach (var effect in abilityData.effects)
            {
                effect.Remove(other.gameObject);
            }

            entitiesInZone.Remove(other.gameObject);
        }

        private void OnDestroy()
        {
            // Remove effects from all entities still in zone
            if (IsServer)
            {
                foreach (var entity in entitiesInZone)
                {
                    if (entity != null)
                    {
                        foreach (var effect in abilityData.effects)
                        {
                            effect.Remove(entity);
                        }
                    }
                }
            }

            if (debugMode)
                Debug.Log($"[DeployableObject] Destroyed: {abilityData?.abilityName}");
        }

        /// <summary>
        /// Damage the deployable object (for destructible anchors, totems, etc.)
        /// </summary>
        public void TakeDamage(float damage)
        {
            // TODO: Implement health system for deployables
            // For now, just log
            if (debugMode)
                Debug.Log($"[DeployableObject] Took {damage} damage");
        }
    }
}