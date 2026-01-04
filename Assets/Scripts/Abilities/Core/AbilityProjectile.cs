using Unity.Netcode;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Component attached to projectile objects (flares, harpoons, etc.)
    /// Handles projectile movement and hit detection.
    /// Can use physics (Rigidbody) or simple transform movement.
    /// </summary>
    public class AbilityProjectile : NetworkBehaviour
    {
        private AbilityData abilityData;
        private AbilityUser caster;
        private AbilityContext context;

        [Header("Movement")]
        public ProjectileMovementType movementType = ProjectileMovementType.Transform;
        public float projectileSpeed = 20f;
        public bool useGravity = false;
        public float gravityMultiplier = 1f;

        [Header("Lifetime")]
        public float maxLifetime = 10f;
        private float lifetime = 0f;

        [Header("Hit Detection")]
        public LayerMask hitLayers;
        public bool destroyOnHit = true;

        [Header("Debug")]
        public bool debugMode = false;

        [Header("Visualization")]
        [SerializeField] private bool showTrajectory = true;
        [SerializeField] private Color trajectoryColor = new Color(1f, 0.5f, 0f, 0.8f);
        [SerializeField] private int trajectorySegments = 30;

        private Rigidbody rb;
        private Vector3 velocity;
        private bool hasHit = false;

        /// <summary>
        /// Initialize the projectile with ability data.
        /// Called by ProjectileAbilityInstance.
        /// </summary>
        public void Initialize(AbilityData data, AbilityUser caster, AbilityContext context)
        {
            this.abilityData = data;
            this.caster = caster;
            this.context = context;

            if (debugMode)
                Debug.Log($"[AbilityProjectile] Initialized: {data.abilityName}");

            // Set up movement based on type
            SetupMovement();
        }

        private void SetupMovement()
        {
            if (movementType == ProjectileMovementType.Rigidbody)
            {
                // Use physics-based movement
                rb = GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                }

                rb.useGravity = useGravity;
                rb.linearVelocity = transform.forward * projectileSpeed;
            }
            else
            {
                // Use simple transform movement
                velocity = transform.forward * projectileSpeed;
            }
        }

        private void Update()
        {
            if (hasHit) return;

            lifetime += Time.deltaTime;

            // Destroy after max lifetime
            if (lifetime >= maxLifetime)
            {
                DestroyProjectile();
                return;
            }

            // Update movement if using transform
            if (movementType == ProjectileMovementType.Transform)
            {
                // Apply gravity if enabled
                if (useGravity)
                {
                    velocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
                }

                // Move projectile
                transform.position += velocity * Time.deltaTime;

                // Rotate to face direction of travel
                if (velocity != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(velocity);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            if (!IsServer) return; // Only server processes hits

            // Find the root entity (handles child colliders like Enemy/Capsule)
            GameObject entityRoot = GetEntityRoot(other);

            // Ignore if no entity found or if it's the caster
            if (entityRoot == null || entityRoot == caster.gameObject)
                return;

            if (debugMode)
                Debug.Log($"[AbilityProjectile] Hit: {entityRoot.name}");

            // Apply effects to hit target (use entityRoot, not collider's GameObject)
            ApplyEffectsToTarget(entityRoot, other.ClosestPoint(transform.position));

            hasHit = true;

            if (destroyOnHit)
            {
                DestroyProjectile();
            }
        }

        private void ApplyEffectsToTarget(GameObject target, Vector3 hitPoint)
        {
            // Create context for hit
            AbilityContext hitContext = new AbilityContext
            {
                targetEntity = target,
                targetPosition = target.transform.position,
                hitPosition = hitPoint
            };

            // Apply all effects
            foreach (var effect in abilityData.effects)
            {
                if (effect.ShouldApplyToTarget(caster, target))
                {
                    effect.Apply(caster, target, hitContext);

                    if (debugMode)
                        Debug.Log($"[AbilityProjectile] Applied {effect.EffectType} to {target.name}");
                }
            }

            // Spawn impact VFX
            if (abilityData.impactVFXPrefab != null)
            {
                GameObject.Instantiate(abilityData.impactVFXPrefab, hitPoint, Quaternion.identity);
            }

            // Play impact audio
            if (abilityData.impactSound != null)
            {
                AudioSource.PlayClipAtPoint(abilityData.impactSound, hitPoint);
            }
        }

        /// <summary>
        /// Find the root entity GameObject from a collider.
        /// Handles child colliders by searching up the hierarchy for AbilityUser component.
        /// </summary>
        private GameObject GetEntityRoot(Collider collider)
        {
            // First check if the collider's GameObject has AbilityUser
            if (collider.TryGetComponent<AbilityUser>(out _))
            {
                return collider.gameObject;
            }

            // If not, search up the parent hierarchy
            AbilityUser abilityUser = collider.GetComponentInParent<AbilityUser>();
            return abilityUser != null ? abilityUser.gameObject : null;
        }

        private void DestroyProjectile()
        {
            if (debugMode)
                Debug.Log($"[AbilityProjectile] Destroying projectile");

            Destroy(gameObject);
        }

        /// <summary>
        /// Draw trajectory visualization during gameplay.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showTrajectory || hasHit) return;

            // Draw current position marker
            GizmoHelpers.DrawWireSphere(transform.position, 0.2f, trajectoryColor);

            // Draw velocity arrow
            if (Application.isPlaying && velocity != Vector3.zero)
            {
                GizmoHelpers.DrawArrow(transform.position, velocity.normalized, 2f, trajectoryColor, 0.3f);
            }
            else if (!Application.isPlaying)
            {
                // In editor, show forward direction
                GizmoHelpers.DrawArrow(transform.position, transform.forward, 2f, trajectoryColor * 0.5f, 0.3f);
            }
        }

        /// <summary>
        /// Draw detailed trajectory prediction when selected.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!showTrajectory) return;

            // Draw predicted trajectory
            if (Application.isPlaying && !hasHit)
            {
                DrawRuntimeTrajectory();
            }
            else if (!Application.isPlaying)
            {
                DrawEditorTrajectory();
            }

            // Draw hit radius if ability has one
            if (abilityData != null && abilityData.radius > 0)
            {
                GizmoHelpers.DrawWireSphere(transform.position, abilityData.radius, trajectoryColor * 0.6f);
            }
        }

        /// <summary>
        /// Draw trajectory prediction during runtime based on current velocity.
        /// </summary>
        private void DrawRuntimeTrajectory()
        {
            Vector3 currentPos = transform.position;
            Vector3 currentVel = velocity;

            if (movementType == ProjectileMovementType.Rigidbody && rb != null)
            {
                currentVel = rb.linearVelocity;
            }

            float timeStep = (maxLifetime - lifetime) / trajectorySegments;
            Vector3 prevPoint = currentPos;

            for (int i = 1; i <= trajectorySegments; i++)
            {
                float time = timeStep * i;
                Vector3 newPoint = currentPos + currentVel * time;

                // Apply gravity if enabled
                if (useGravity)
                {
                    newPoint += Physics.gravity * gravityMultiplier * (0.5f * time * time);
                }

                // Draw line segment
                Gizmos.color = Color.Lerp(trajectoryColor, trajectoryColor * 0.3f, (float)i / trajectorySegments);
                Gizmos.DrawLine(prevPoint, newPoint);

                prevPoint = newPoint;
            }
        }

        /// <summary>
        /// Draw trajectory prediction in editor mode.
        /// </summary>
        private void DrawEditorTrajectory()
        {
            Vector3 startPos = transform.position;
            Vector3 initialVelocity = transform.forward * projectileSpeed;
            float duration = maxLifetime;

            if (useGravity)
            {
                // Draw parabolic trajectory with gravity
                GizmoHelpers.DrawTrajectory(startPos, initialVelocity, duration, trajectoryColor, trajectorySegments);
            }
            else
            {
                // Draw straight line trajectory
                Vector3 endPos = startPos + initialVelocity * duration;
                Gizmos.color = trajectoryColor;
                Gizmos.DrawLine(startPos, endPos);

                // Draw arrow at end
                GizmoHelpers.DrawArrow(endPos - initialVelocity.normalized * 2f, initialVelocity.normalized, 2f, trajectoryColor, 0.3f);
            }

            // Draw markers along the path
            for (int i = 1; i <= 5; i++)
            {
                float time = (duration / 5f) * i;
                Vector3 markerPos = startPos + initialVelocity * time;

                if (useGravity)
                {
                    markerPos += Physics.gravity * gravityMultiplier * (0.5f * time * time);
                }

                GizmoHelpers.DrawWireSphere(markerPos, 0.15f, trajectoryColor * 0.6f);
            }
        }
    }

    public enum ProjectileMovementType
    {
        Transform,  // Simple transform movement (no physics)
        Rigidbody   // Physics-based movement
    }
}