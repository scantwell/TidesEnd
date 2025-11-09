using UnityEngine;
using System.Collections;
using TidesEnd.Combat;
using TidesEnd.Core;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Physical projectile that travels through the world and applies damage on hit.
    /// Supports damage falloff, penetration, and hit location multipliers.
    ///
    /// Layer Setup:
    /// - Projectile GameObject should be on "Projectile" layer (Layer 6)
    /// - Configure Physics collision matrix to control what projectiles can hit
    /// - Projectiles typically collide with: Enemy, EnemyHitbox, Environment
    /// - Projectiles typically ignore: UI, Player (friendly fire), other Projectiles
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        // Configuration (set on Initialize)
        private ProjectileConfig config;

        // Current state
        private float currentDamage;
        private float distanceTraveled;
        private int penetrationsRemaining;
        private bool isActive;
        private float spawnTime;
        private Vector3 lastPosition;

        // Components
        private Rigidbody rb;
        private Collider col;
        private BulletTracer tracer;

        // Owner tracking (to avoid self-damage)
        private GameObject ownerObject;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            // Configure rigidbody
            rb.useGravity = false; // We'll apply custom gravity
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // Ensure collider is trigger
            col.isTrigger = true;
        }

        /// <summary>
        /// Initialize and launch the projectile
        /// </summary>
        public void Initialize(ProjectileConfig configuration, GameObject owner = null)
        {
            config = configuration;
            ownerObject = owner;

            // Set initial state
            transform.position = config.spawnPosition;
            transform.rotation = Quaternion.LookRotation(config.direction);

            currentDamage = config.baseDamage;
            distanceTraveled = 0f;
            penetrationsRemaining = config.maxPenetrations;
            isActive = true;
            spawnTime = Time.time;
            lastPosition = transform.position;

            // Launch projectile
            rb.linearVelocity = config.direction * config.velocity;

            // Spawn tracer if configured
            if (config.spawnTracer)
            {
                SpawnTracer();
            }

            // Start lifetime countdown
            StartCoroutine(LifetimeCountdown());

            if (showDebugInfo)
            {
                Debug.Log($"Projectile launched: Damage={currentDamage}, Velocity={config.velocity}, Direction={config.direction}");
            }
        }

        private void FixedUpdate()
        {
            if (!isActive) return;

            // Track distance traveled
            float frameDist = Vector3.Distance(transform.position, lastPosition);
            distanceTraveled += frameDist;
            lastPosition = transform.position;

            // Apply custom gravity
            if (config.gravityMultiplier > 0f)
            {
                rb.linearVelocity += Physics.gravity * config.gravityMultiplier * Time.fixedDeltaTime;
            }

            // Update rotation to face movement direction
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
            }

            // Calculate damage falloff based on distance
            UpdateDamageFalloff();
        }

        private void UpdateDamageFalloff()
        {
            if (distanceTraveled <= config.falloffStart)
            {
                // Within effective range - full damage
                currentDamage = config.baseDamage;
            }
            else if (distanceTraveled >= config.falloffEnd)
            {
                // Beyond max range - minimum damage (20%)
                currentDamage = config.baseDamage * 0.2f;
            }
            else
            {
                // Linear falloff between start and end
                float falloffRange = config.falloffEnd - config.falloffStart;
                float distanceInFalloff = distanceTraveled - config.falloffStart;
                float falloffPercent = distanceInFalloff / falloffRange;

                currentDamage = Mathf.Lerp(config.baseDamage, config.baseDamage * 0.2f, falloffPercent);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;

            // Ignore owner and any children of owner
            if (ownerObject != null)
            {
                // Check if the hit object is the owner or a child of the owner
                if (other.gameObject == ownerObject || other.transform.IsChildOf(ownerObject.transform))
                {
                    if (showDebugInfo)
                        Debug.Log($"Projectile ignored owner collision: {other.gameObject.name}");
                    return;
                }
            }

            // Check if this layer should be hit
            if (config.hitLayers != -1 && ((1 << other.gameObject.layer) & config.hitLayers) == 0)
                return;

            // Try to get hit zone for location-based damage
            HitZone hitZone = other.GetComponent<HitZone>();
            IDamageable target = hitZone?.Owner ?? other.GetComponent<IDamageable>();

            // Additional check: Don't damage ourselves through HitZone
            if (target != null && ownerObject != null)
            {
                // Check if the target is the owner or a child of the owner
                if (target.GameObject == ownerObject || target.Transform.IsChildOf(ownerObject.transform))
                {
                    if (showDebugInfo)
                        Debug.Log($"Projectile ignored owner damage via HitZone: {target.GameObject.name}");
                    return;
                }
            }

            bool hitSomething = false;

            if (target != null && target.IsAlive)
            {
                // Calculate final damage with hit location multiplier
                float damageMultiplier = GetHitLocationMultiplier(hitZone);
                float finalDamage = currentDamage * damageMultiplier;

                // Apply damage
                target.TakeDamage(finalDamage, config.ownerId, transform.position, -transform.forward);

                hitSomething = true;

                if (showDebugInfo)
                {
                    string zoneName = hitZone != null ? hitZone.Type.ToString() : "Unknown";
                    Debug.Log($"Projectile hit {target.GameObject.name} in {zoneName} for {finalDamage} damage (base: {currentDamage}, mult: {damageMultiplier})");
                }
            }
            else
            {
                // Hit environment/non-damageable
                hitSomething = true;
            }

            if (hitSomething)
            {
                // Spawn hit effect
                SpawnHitEffect(other);

                // Handle penetration or destroy
                if (config.canPenetrate && penetrationsRemaining > 0)
                {
                    penetrationsRemaining--;

                    // Reduce damage for next hit
                    float damageRetention = 1f - config.penetrationDamageReduction;
                    config.baseDamage *= damageRetention;
                    currentDamage *= damageRetention;

                    if (showDebugInfo)
                    {
                        Debug.Log($"Projectile penetrated! Remaining penetrations: {penetrationsRemaining}, New damage: {currentDamage}");
                    }

                    // Continue through
                }
                else
                {
                    // Destroy projectile
                    ReturnToPool();
                }
            }
        }

        private float GetHitLocationMultiplier(HitZone hitZone)
        {
            if (hitZone == null)
                return 1f; // Body shot default

            // Use custom multiplier from hit zone if set, otherwise use config defaults
            if (hitZone.Multiplier > 0f)
                return hitZone.Multiplier;

            switch (hitZone.Type)
            {
                case HitZone.ZoneType.Head:
                    return config.headshotMultiplier;
                case HitZone.ZoneType.Limb:
                    return config.limbshotMultiplier;
                case HitZone.ZoneType.Body:
                default:
                    return 1f;
            }
        }

        private void SpawnTracer()
        {
            // Get tracer from pool
            if (TracerPool.Instance == null)
            {
                Debug.LogWarning("TracerPool not found in scene!");
                return;
            }

            tracer = TracerPool.Instance.GetTracer();
            if (tracer == null) return;

            // Attach tracer as child
            tracer.transform.SetParent(transform);
            tracer.transform.localPosition = Vector3.zero;
            tracer.transform.localRotation = Quaternion.identity;

            // Initialize tracer to follow projectile
            // The tracer will automatically follow since it's a child
            tracer.SetSpeed(config.velocity);
        }

        private void SpawnHitEffect(Collider hitCollider)
        {
            if (config.hitEffectPrefab == null) return;

            // Spawn impact effect at hit point
            GameObject effect = Instantiate(config.hitEffectPrefab, transform.position, Quaternion.LookRotation(-transform.forward));

            // Auto-destroy after a delay
            Destroy(effect, 2f);
        }

        private IEnumerator LifetimeCountdown()
        {
            yield return new WaitForSeconds(config.maxLifetime);

            if (isActive)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"Projectile expired after {config.maxLifetime} seconds. Distance traveled: {distanceTraveled}");
                }
                ReturnToPool();
            }
        }

        /// <summary>
        /// Force stop and return projectile to pool
        /// </summary>
        public void ForceStop()
        {
            if (isActive)
            {
                StopAllCoroutines();
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            isActive = false;

            // Stop rigidbody
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Return tracer to pool if it exists
            if (tracer != null)
            {
                tracer.transform.SetParent(null);
                tracer.ForceStop();
                tracer = null;
            }

            // Return projectile to pool
            ProjectilePool pool = FindAnyObjectByType<ProjectilePool>();
            if (pool != null)
            {
                pool.ReturnProjectile(this);
            }
            else
            {
                // No pool found, just destroy
                Destroy(gameObject);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!isActive || !showDebugInfo) return;

            // Draw velocity vector
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + rb.linearVelocity.normalized * 2f);

            // Draw falloff ranges
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(config.spawnPosition, config.falloffStart);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(config.spawnPosition, config.falloffEnd);
        }
#endif
    }
}