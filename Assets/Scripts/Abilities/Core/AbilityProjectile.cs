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

            // Ignore caster
            if (other.gameObject == caster.gameObject)
                return;

            if (debugMode)
                Debug.Log($"[AbilityProjectile] Hit: {other.gameObject.name}");

            // Apply effects to hit target
            ApplyEffectsToTarget(other.gameObject, other.ClosestPoint(transform.position));

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
                        Debug.Log($"[AbilityProjectile] Applied {effect.effectType} to {target.name}");
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

        private void DestroyProjectile()
        {
            if (debugMode)
                Debug.Log($"[AbilityProjectile] Destroying projectile");

            Destroy(gameObject);
        }
    }

    public enum ProjectileMovementType
    {
        Transform,  // Simple transform movement (no physics)
        Rigidbody   // Physics-based movement
    }
}