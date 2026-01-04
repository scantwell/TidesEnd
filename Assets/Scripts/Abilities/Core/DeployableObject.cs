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

        [Header("Visualization")]
        [SerializeField] private bool showZoneGizmos = true;
        [SerializeField] private Color zoneColor = new Color(0f, 1f, 1f, 0.6f);

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

            // Find the root entity (handles child colliders like Enemy/Capsule)
            GameObject entityRoot = GetEntityRoot(other);
            if (entityRoot == null) return; // Not an entity with AbilityUser

            // Prevent duplicate entries if entity has multiple colliders
            if (entitiesInZone.Contains(entityRoot)) return;

            if (debugMode)
                Debug.Log($"[DeployableObject] Entity entered zone: {entityRoot.name}");

            // Apply all effects to entering entity
            foreach (var effect in abilityData.effects)
            {
                if (effect.ShouldApplyToTarget(owner, entityRoot))
                {
                    AbilityContext context = new AbilityContext
                    {
                        targetEntity = entityRoot,
                        targetPosition = entityRoot.transform.position
                    };

                    effect.Apply(owner, entityRoot, context);

                    if (debugMode)
                        Debug.Log($"[DeployableObject] Applied effect {effect.EffectType} to {entityRoot.name}");
                }
            }

            entitiesInZone.Add(entityRoot);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsServer) return;

            // Find the root entity (handles child colliders)
            GameObject entityRoot = GetEntityRoot(other);
            if (entityRoot == null) return;

            if (debugMode)
                Debug.Log($"[DeployableObject] Entity exited zone: {entityRoot.name}");

            // Remove all effects from exiting entity
            foreach (var effect in abilityData.effects)
            {
                effect.Remove(entityRoot);
            }

            entitiesInZone.Remove(entityRoot);
        }

        /// <summary>
        /// Find the root entity GameObject from a collider.
        /// Handles child colliders by searching up the hierarchy for EntityStats component.
        /// Uses EntityStats instead of AbilityUser since all targetable entities need stats,
        /// but not all need ability casting capabilities.
        /// </summary>
        private GameObject GetEntityRoot(Collider collider)
        {
            // First check if the collider's GameObject has EntityStats
            if (collider.TryGetComponent<EntityStats>(out _))
            {
                return collider.gameObject;
            }

            // If not, search up the parent hierarchy
            EntityStats entityStats = collider.GetComponentInParent<EntityStats>();
            return entityStats != null ? entityStats.gameObject : null;
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
        /// Draw gizmos to visualize the zone radius and effects.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showZoneGizmos) return;

            // Draw zone radius (both in editor and runtime)
            if (abilityData != null && abilityData.radius > 0)
            {
                DrawZoneVisualization();
            }
            else if (triggerCollider != null)
            {
                // Fallback: draw collider radius if ability data not yet initialized
                GizmoHelpers.DrawWireCircle(transform.position, triggerCollider.radius, zoneColor * 0.5f, 32);
            }
        }

        /// <summary>
        /// Draw zone visualization in editor when selected.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!showZoneGizmos) return;

            // Draw detailed visualization when selected
            if (abilityData != null && abilityData.radius > 0)
            {
                DrawZoneVisualization();
                DrawEffectIndicators();
            }
            else if (triggerCollider != null)
            {
                // Fallback: draw collider radius
                GizmoHelpers.DrawWireCircle(transform.position, triggerCollider.radius, zoneColor, 48);
                GizmoHelpers.DrawWireSphere(transform.position, triggerCollider.radius, zoneColor);
            }
        }

        /// <summary>
        /// Draw the zone circle and cylinder visualization.
        /// </summary>
        private void DrawZoneVisualization()
        {
            Vector3 position = transform.position;
            float radius = abilityData.radius;

            // Draw ground circle
            GizmoHelpers.DrawWireCircle(position, radius, zoneColor, 64);

            // Draw filled circle for better visibility
            GizmoHelpers.DrawFilledCircle(position, radius, zoneColor * 0.2f, 32);

            // Draw cylinder to show height
            GizmoHelpers.DrawWireCylinder(position, radius, 2f, zoneColor, 32);

            // Draw center marker
            GizmoHelpers.DrawWireSphere(position, 0.25f, zoneColor);
        }

        /// <summary>
        /// Draw indicators for entities currently in the zone.
        /// </summary>
        private void DrawEffectIndicators()
        {
            if (!Application.isPlaying || entitiesInZone.Count == 0)
                return;

            // Draw lines to each entity in zone
            foreach (var entity in entitiesInZone)
            {
                if (entity != null)
                {
                    Gizmos.color = zoneColor;
                    Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, entity.transform.position + Vector3.up * 1f);

                    // Draw small sphere at entity position
                    GizmoHelpers.DrawWireSphere(entity.transform.position + Vector3.up * 1f, 0.3f, zoneColor);
                }
            }

            // Draw count label (approximate position above deployable)
            if (debugMode)
            {
                Vector3 labelPos = transform.position + Vector3.up * 3f;
                // Unity Gizmos doesn't support text, but we can draw a marker
                GizmoHelpers.DrawWireSphere(labelPos, 0.2f, Color.yellow);
            }
        }
    }
}