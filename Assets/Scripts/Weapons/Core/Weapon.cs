using UnityEngine;
using TidesEnd.Combat;
using TidesEnd.Core;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Main weapon component handling state, visuals, and gameplay.
    /// Uses explicit FireClient/FireServer methods for clear separation of client visuals and server authority.
    ///
    /// Layer Setup:
    /// - Configure hitLayers in WeaponData to use GameLayers constants (e.g., GameLayers.HitscanTargets)
    /// - Physics collision matrix should be configured in Project Settings > Physics
    /// - Use GameLayers.EnemiesOnly for player weapons to prevent friendly fire
    /// </summary>
    [RequireComponent(typeof(WeaponView))]
    public class Weapon : MonoBehaviour, IWeapon
    {
        [Header("Configuration")]
        [SerializeField] private WeaponData data;
        [SerializeField] private Transform fireOrigin;

        [Header("References")]
        [SerializeField] private Camera playerCamera;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        private WeaponView view;
        private float nextFireTime;
        private int currentAmmo;
        private int reserveAmmo;
        private bool isReloading;
        private int tracerCounter = 0;

        // Owner tracking
        private ulong ownerId = 0;
        private GameObject ownerGameObject = null;

        // Public properties
        public WeaponData Data => data;
        public int CurrentAmmo => currentAmmo;
        public int ReserveAmmo => reserveAmmo;
        public int MaxAmmo => data.magazineSize;
        public bool IsReloading => isReloading;

        private void Awake()
        {
            view = GetComponent<WeaponView>();

            if (fireOrigin == null)
                fireOrigin = transform;

            if (data == null)
            {
                Debug.LogError($"WeaponData not assigned on {name}");
                enabled = false;
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            if (view != null && data != null)
            {
                view.Initialize(data);
            }
        }

        public void Initialize()
        {
            currentAmmo = data.magazineSize;
            reserveAmmo = data.maxAmmo;
            isReloading = false;
            nextFireTime = 0f;
        }

        public bool CanFire()
        {
            return !isReloading
                && Time.time >= nextFireTime
                && currentAmmo > 0;
        }

        /// <summary>
        /// Fires the weapon on client side (visual effects only, no damage)
        /// Shows muzzle flash, plays sounds, spawns tracers
        /// </summary>
        public void FireClient()
        {
            if (!CanFire()) return;

            // Update weapon state
            nextFireTime = Time.time + (1f / data.fireRate);
            currentAmmo--;

            // Play visual/audio effects
            if (view != null)
            {
                view.PlayFireEffects();
            }

            // Fire based on weapon type
            switch (data.weaponType)
            {
                case WeaponType.Hitscan:
                    FireHitscanVisuals();
                    break;

                case WeaponType.Projectile:
                    FireProjectileClient();
                    break;

                case WeaponType.Melee:
                    Debug.LogWarning("Melee weapons not yet implemented");
                    break;
            }

            if (showDebugInfo)
            {
                Debug.Log($"{data.weaponName} ({data.weaponType}) fired [Client]. Ammo: {currentAmmo}/{data.magazineSize}");
            }
        }

        /// <summary>
        /// Fires the weapon on server side (authoritative damage application)
        /// Performs raycasts, applies damage, no visual effects
        /// </summary>
        /// <param name="validate">If true, validates CanFire and updates state (for remote clients). If false, skips validation (for host who already validated).</param>
        public void FireServer(bool validate = true)
        {
            if (validate)
            {
                // Server-side validation (anti-cheat for remote clients)
                if (!CanFire()) return;

                // Update weapon state on server
                nextFireTime = Time.time + (1f / data.fireRate);
                currentAmmo--;
            }

            // Fire based on weapon type
            switch (data.weaponType)
            {
                case WeaponType.Hitscan:
                    FireHitscanServer();
                    break;

                case WeaponType.Projectile:
                    FireProjectileServer();
                    break;

                case WeaponType.Melee:
                    Debug.LogWarning("Melee weapons not yet implemented");
                    break;
            }

            if (showDebugInfo)
            {
                string validationNote = validate ? " (validated)" : " (host - no validation)";
                Debug.Log($"{data.weaponName} ({data.weaponType}) fired [Server]{validationNote}. Ammo: {currentAmmo}/{data.magazineSize}");
            }
        }

        #region Hitscan Client (Visuals)

        /// <summary>
        /// Client-side hitscan: spawns tracers and hit effects (no damage)
        /// </summary>
        private void FireHitscanVisuals()
        {
            Ray aimRay = GetAimRay();
            Vector3 aimPoint = GetAimPoint(aimRay, out bool raycastHit);

            int pelletCount = data.pelletsPerShot;

            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 baseDirection = (aimPoint - fireOrigin.position).normalized;
                Vector3 direction = ApplySpread(baseDirection);

                // Show tracer and hit effects
                Ray ray = new Ray(fireOrigin.position, direction);
                if (Physics.Raycast(ray, out RaycastHit hit, data.range, data.hitLayers))
                {
                    // Show tracer to hit point
                    if (ShouldSpawnTracer())
                    {
                        SpawnHitscanTracer(ray.origin, hit.point);
                    }

                    // Show hit effect
                    SpawnHitEffect(hit.point, hit.normal);
                }
                else
                {
                    // Miss - show tracer to max range
                    if (ShouldSpawnTracer())
                    {
                        Vector3 maxRangePoint = ray.origin + ray.direction * data.range;
                        SpawnHitscanTracer(ray.origin, maxRangePoint);
                    }
                }
            }
        }

        #endregion

        #region Hitscan Server (Damage)

        /// <summary>
        /// Server-side hitscan: performs raycasts and applies damage (no visuals)
        /// </summary>
        private void FireHitscanServer()
        {
            Ray aimRay = GetAimRay();
            Vector3 aimPoint = GetAimPoint(aimRay, out bool raycastHit);

            int pelletCount = data.pelletsPerShot;

            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 baseDirection = (aimPoint - fireOrigin.position).normalized;
                Vector3 direction = ApplySpread(baseDirection);

                PerformHitscan(fireOrigin.position, direction);
            }
        }

        private void PerformHitscan(Vector3 origin, Vector3 direction)
        {
            Ray ray = new Ray(origin, direction);

            if (data.canPenetrate && data.maxPenetrations > 0)
            {
                PerformHitscanWithPenetration(ray);
            }
            else
            {
                PerformSingleHitscan(ray);
            }
        }

        private void PerformSingleHitscan(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, data.range, data.hitLayers))
            {
                // Skip if owner (prevent self-damage)
                if (IsOwnerOrChild(hit.collider.gameObject))
                {
                    if (showDebugInfo)
                        Debug.Log("<color=yellow>Hitscan hit owner, skipping</color>");
                    return;
                }

                ProcessHitscanHit(hit, data.damage);

                if (showDebugInfo)
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
                }
            }
        }

        private void PerformHitscanWithPenetration(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, data.range, data.hitLayers);

            if (hits.Length == 0) return;

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            int penetrationsRemaining = data.maxPenetrations;
            float currentDamage = data.damage;

            foreach (RaycastHit hit in hits)
            {
                if (IsOwnerOrChild(hit.collider.gameObject))
                    continue;

                ProcessHitscanHit(hit, currentDamage);

                if (showDebugInfo)
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
                }

                if (penetrationsRemaining > 0)
                {
                    penetrationsRemaining--;
                    currentDamage *= (1f - data.penetrationDamageReduction);

                    if (showDebugInfo)
                    {
                        Debug.Log($"<color=cyan>Penetration:</color> {penetrationsRemaining} remaining, damage reduced to {currentDamage}");
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void ProcessHitscanHit(RaycastHit hit, float baseDamage)
        {
            float distance = hit.distance;
            float damageWithFalloff = CalculateDamageFalloff(baseDamage, distance);

            HitZone hitZone = hit.collider.GetComponent<HitZone>();
            float hitLocationMultiplier = GetHitLocationMultiplier(hitZone);
            float finalDamage = damageWithFalloff * hitLocationMultiplier;

            IDamageable target;
            if (hitZone != null && hitZone.Owner != null)
            {
                target = hitZone.Owner;
            }
            else
            {
                target = hit.collider.GetComponent<IDamageable>();
            }

            if (target != null && target.IsAlive)
            {
                target.TakeDamage(finalDamage, ownerId, hit.point, hit.normal);

                if (showDebugInfo)
                {
                    string hitZoneName = hitZone != null ? $" ({hitZone.Type})" : "";
                    Debug.Log($"<color=green>[Server] Hitscan hit {target.GameObject.name}{hitZoneName} for {finalDamage:F1} damage at {distance:F1}m</color>");
                }
            }
        }

        private float CalculateDamageFalloff(float baseDamage, float distance)
        {
            if (distance <= data.damageFalloffStart)
            {
                return baseDamage;
            }
            else if (distance >= data.damageFalloffEnd)
            {
                return baseDamage * 0.2f;
            }
            else
            {
                float falloffRange = data.damageFalloffEnd - data.damageFalloffStart;
                float distanceInFalloff = distance - data.damageFalloffStart;
                float falloffPercent = distanceInFalloff / falloffRange;
                return Mathf.Lerp(baseDamage, baseDamage * 0.2f, falloffPercent);
            }
        }

        private float GetHitLocationMultiplier(HitZone hitZone)
        {
            if (hitZone == null)
                return 1f;

            if (hitZone.Multiplier > 0f)
                return hitZone.Multiplier;

            switch (hitZone.Type)
            {
                case HitZone.ZoneType.Head:
                    return data.headshotMultiplier;
                case HitZone.ZoneType.Limb:
                    return data.limbshotMultiplier;
                case HitZone.ZoneType.Body:
                default:
                    return 1f;
            }
        }

        #endregion

        #region Projectile

        private void FireProjectileClient()
        {
            Ray aimRay = GetAimRay();
            Vector3 aimPoint = GetAimPoint(aimRay, out bool raycastHit);

            for (int i = 0; i < data.projectilesPerShot; i++)
            {
                Vector3 baseDirection = (aimPoint - fireOrigin.position).normalized;
                Vector3 direction = ApplySpread(baseDirection);

                SpawnProjectile(fireOrigin.position, direction);
            }
        }

        private void FireProjectileServer()
        {
            Ray aimRay = GetAimRay();
            Vector3 aimPoint = GetAimPoint(aimRay, out bool raycastHit);

            for (int i = 0; i < data.projectilesPerShot; i++)
            {
                Vector3 baseDirection = (aimPoint - fireOrigin.position).normalized;
                Vector3 direction = ApplySpread(baseDirection);

                SpawnProjectile(fireOrigin.position, direction);
            }
        }

        private void SpawnProjectile(Vector3 origin, Vector3 direction)
        {
            if (ProjectilePool.Instance == null)
            {
                Debug.LogError("ProjectilePool not found in scene!");
                return;
            }

            if (data.projectilePrefab == null)
            {
                Debug.LogError($"No projectile prefab assigned to {data.weaponName}!");
                return;
            }

            Projectile projectile = ProjectilePool.Instance.GetProjectile(data.projectilePrefab);
            if (projectile == null)
            {
                Debug.LogWarning($"Failed to get projectile from pool for {data.weaponName}");
                return;
            }

            bool spawnTracer = ShouldSpawnTracer();
            ProjectileConfig config = ProjectileConfig.FromWeaponData(data, direction, origin, ownerId);
            config.spawnTracer = spawnTracer;

            GameObject owner = ownerGameObject != null ? ownerGameObject : gameObject;
            projectile.Initialize(config, owner);

            if (showDebugInfo)
            {
                Debug.Log($"<color=cyan>Spawned projectile:</color> {data.weaponName}, Owner: {owner.name}");
            }
        }

        #endregion

        #region Utilities

        private Ray GetAimRay()
        {
            if (playerCamera != null)
            {
                return playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            }
            else
            {
                return new Ray(fireOrigin.position, fireOrigin.forward);
            }
        }

        private Vector3 GetAimPoint(Ray aimRay, out bool didHit)
        {
            if (Physics.Raycast(aimRay, out RaycastHit hit, data.range, data.hitLayers))
            {
                didHit = true;

                if (showDebugInfo)
                {
                    Debug.DrawLine(aimRay.origin, hit.point, Color.red, 0.1f);
                }
                return hit.point;
            }
            else
            {
                didHit = false;
                Vector3 maxRangePoint = aimRay.origin + (aimRay.direction * data.range);

                if (showDebugInfo)
                {
                    Debug.DrawLine(aimRay.origin, maxRangePoint, Color.yellow, 0.1f);
                }
                return maxRangePoint;
            }
        }

        private Vector3 ApplySpread(Vector3 baseDirection)
        {
            if (data.baseSpread <= 0f)
                return baseDirection;

            float spreadAngle = data.baseSpread;
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);

            Quaternion spreadRotation = Quaternion.Euler(randomX, randomY, 0);
            return spreadRotation * baseDirection;
        }

        private bool ShouldSpawnTracer()
        {
            if (!data.useTracers)
                return false;

            tracerCounter++;
            if (tracerCounter >= data.tracerFrequency)
            {
                tracerCounter = 0;
                return true;
            }

            return false;
        }

        private void SpawnHitscanTracer(Vector3 start, Vector3 end)
        {
            if (TracerPool.Instance == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning("TracerPool not found in scene");
                return;
            }

            BulletTracer tracer = TracerPool.Instance.GetTracer();
            if (tracer == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning("Failed to get tracer from pool");
                return;
            }

            tracer.InitializeWithHit(start, end);
        }

        private void SpawnHitEffect(Vector3 point, Vector3 normal)
        {
            if (data.hitEffectPrefab == null)
                return;

            Quaternion rotation = Quaternion.LookRotation(normal);
            GameObject effect = Instantiate(data.hitEffectPrefab, point, rotation);
            Destroy(effect, 2f);
        }

        private bool IsOwnerOrChild(GameObject obj)
        {
            if (ownerGameObject == null)
                return false;

            return obj == ownerGameObject || obj.transform.IsChildOf(ownerGameObject.transform);
        }

        #endregion

        #region Reload

        public void Reload()
        {
            if (isReloading) return;
            if (currentAmmo >= data.magazineSize) return;
            if (reserveAmmo <= 0) return;

            isReloading = true;
            view?.PlayReloadEffects();

            Invoke(nameof(CompleteReload), data.reloadTime);

            if (showDebugInfo)
            {
                Debug.Log($"{data.weaponName} started reload. Time: {data.reloadTime}s");
            }
        }

        private void CompleteReload()
        {
            int ammoNeeded = data.magazineSize - currentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

            currentAmmo += ammoToReload;
            reserveAmmo -= ammoToReload;
            isReloading = false;

            if (showDebugInfo)
            {
                Debug.Log($"{data.weaponName} reloaded. Ammo: {currentAmmo}/{data.magazineSize}, Reserve: {reserveAmmo}");
            }
        }

        #endregion

        #region Public API

        public void SetOwner(ulong networkOwnerId, GameObject playerGameObject = null)
        {
            ownerId = networkOwnerId;
            ownerGameObject = playerGameObject;

            if (showDebugInfo)
            {
                string ownerName = ownerGameObject != null ? ownerGameObject.name : "None";
                Debug.Log($"<color=yellow>Weapon owner set:</color> {data?.weaponName}, Owner: {ownerName}, ClientId: {ownerId}");
            }
        }

        public void SetPlayerCamera(Camera camera)
        {
            playerCamera = camera;
        }

        public void SetAmmo(int ammo, int reserve)
        {
            currentAmmo = Mathf.Clamp(ammo, 0, data.magazineSize);
            reserveAmmo = Mathf.Clamp(reserve, 0, data.maxAmmo);
        }

        public void AddAmmo(int amount)
        {
            reserveAmmo = Mathf.Min(reserveAmmo + amount, data.maxAmmo);
        }

        #endregion
    }
}
