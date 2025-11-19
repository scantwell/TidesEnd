using Unity.Netcode;
using UnityEngine;
using TidesEnd.Weapons;
using UnityEngine.InputSystem;
using TidesEnd.Combat;
using System;

namespace TidesEnd.Player
{
    /// <summary>
    /// Manages equipped weapons, switching, and firing
    /// Holds all weapon state, delegates visuals to WeaponView
    /// </summary>
    public class PlayerWeaponController : NetworkBehaviour
    {
        // Events for UI
        public event Action<WeaponData> OnWeaponChanged;
        public event Action<int, int> OnAmmoChanged; // current, reserve
        public event Action<bool> OnReloadStateChanged; // isReloading
        public event Action<float> OnCooldownChanged; // for fire rate cooldown

        [SerializeField] private PlayerInventory PlayerInventory;
        [SerializeField] private Health PlayerHealth;
        [SerializeField] private PlayerController playerController;

        [Header("Weapon Attachment Points")]
        [SerializeField] private Transform viewWeaponHolder;
        [SerializeField] private Transform worldWeaponHolder;

        [Header("Weapon Database")]
        [SerializeField] private WeaponDatabase weaponDatabase;

        [Header("Settings")]
        [SerializeField] private LayerMask weaponHitLayers;

        [Header("Debug")]
        [SerializeField] private bool showDebugRays = true;

        // Networked weapon state
        private NetworkVariable<int> equippedWeaponID = new NetworkVariable<int>(-1);
        private NetworkVariable<int> currentMagazineAmmo = new NetworkVariable<int>(0);
        private NetworkVariable<bool> isReloading = new NetworkVariable<bool>(false);

        // Local weapon state
        private float lastFireTime;
        private int currentShotCount; // For recoil pattern tracking
        private WeaponData currentWeaponData;

        // Weapon view (view model for owner, world model for non-owners)
        private WeaponView currentWeaponView;   // Presentation (animations, audio, VFX)
        private Camera playerCamera;

        public override void OnNetworkSpawn()
        {
            equippedWeaponID.OnValueChanged += OnEquippedWeaponChanged;
            currentMagazineAmmo.OnValueChanged += UpdateAmmoDisplay;
            isReloading.OnValueChanged += (prev, current) =>
            {
                OnReloadStateChanged?.Invoke(current);
            };  

            if (IsOwner)
            {
                // Local player: set up camera and request starting weapon from server
                playerCamera = Camera.main;

                // Auto-discover PlayerController if not assigned
                if (playerController == null)
                    playerController = GetComponent<PlayerController>();

                EquipWeaponServerRpc(0); // Default weapon
            }
            else
            {
                // Non-owner: manually instantiate current weapon
                // (OnValueChanged doesn't fire for initial NetworkVariable sync)
                if (equippedWeaponID.Value >= 0)
                {
                    InstantiateNewWeapon(equippedWeaponID.Value);

                    // Update weapon data reference
                    WeaponPrefabPair weaponPair = weaponDatabase.GetWeapon(equippedWeaponID.Value);
                    if (weaponPair != null)
                    {
                        currentWeaponData = weaponPair.weaponData;
                    }
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            equippedWeaponID.OnValueChanged -= OnEquippedWeaponChanged;
        }

        private void Update()
        {
            // Only the owner with a view weapon and an assigned camera should process input
            if (!IsOwner || currentWeaponView == null || playerCamera == null) return;

            HandleFireInput();
            HandleReloadInput();
            HandleWeaponSwitchInput();

            // Reset shot counter and accumulated recoil after a break in firing (for spray pattern reset)
            if (currentWeaponData != null && Time.time - lastFireTime > currentWeaponData.recoilRecoveryDelay)
            {
                if (currentShotCount > 0) // Only reset if we were actually firing
                {
                    currentShotCount = 0;
                    playerController?.ResetRecoil();
                }
            }
        }

        #region Input Handling

        private void HandleFireInput()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;
            if (mouse == null || keyboard == null) return;

            if (mouse.leftButton.isPressed)
            {
                TryFire();
            }
        }

        private void HandleReloadInput()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                TryReload();
            }
        }

        private void HandleWeaponSwitchInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // Number keys to switch weapons
            for (int i = 0; i < 4; i++)
            {
                // UnityEngine.InputSystem.Key.A is 0x04, Key.Digit1 is 0x1E
                var key = UnityEngine.InputSystem.Key.Digit1 + i;
                if (keyboard[key].wasPressedThisFrame)
                {
                    EquipWeaponServerRpc(i);
                }
            }
        }

        #endregion

        #region Weapon Actions

        private void TryFire()
        {
            if (!CanFire()) return;

            // Apply weapon logic: spread
            Vector3 direction = playerCamera.transform.forward;
            direction = WeaponLogic.ApplySpread(currentWeaponData, direction);

            // Apply weapon logic: recoil and screen shake
            ApplyCameraRecoil();
            ApplyCameraScreenShake();

            // Client prediction: immediate visual feedback
            currentWeaponView.PlayFireAnimation();
            currentWeaponView.PlayFireSound();
            currentWeaponView.PlayMuzzleFlash();
            currentWeaponView.EjectShell();

            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, direction, out hit, currentWeaponData.maxRange, weaponHitLayers))
            {
                // Spawn immediate impact effect (client-side prediction)
                SpawnImpactEffect(hit.point, hit.normal, isPrediction: true);

                // Optional: Spawn bullet tracer
                currentWeaponView.SpawnBulletTracer(hit.point);
            }

            // Debug: Draw where raycast goes
            if (showDebugRays)
            {
                Debug.DrawRay(
                    playerCamera.transform.position,
                    direction * currentWeaponData.maxRange,
                    Color.red,
                    10f
                );
            }

            // Send to server for validation
            FireServerRpc(playerCamera.transform.position, direction);

            // Update local state for prediction
            lastFireTime = Time.time;
            currentShotCount++;
        }

        /// <summary>
        /// Apply camera recoil using weapon's recoil pattern
        /// </summary>
        private void ApplyCameraRecoil()
        {
            if (playerController == null) return;

            Vector2 recoilOffset = WeaponLogic.GetRecoilVector(currentWeaponData, currentShotCount);

            // Apply recoil through the player controller
            // Positive Y = upward recoil (cameraPitch decreases)
            playerController.ApplyRecoil(recoilOffset);
        }

        /// <summary>
        /// Apply screen shake effect when firing
        /// </summary>
        private void ApplyCameraScreenShake()
        {
            if (playerController == null || currentWeaponData == null) return;

            // Apply screen shake using weapon's settings
            playerController.ApplyScreenShake(
                currentWeaponData.screenShakeIntensity,
                currentWeaponData.screenShakeDuration
            );
        }

        [ServerRpc]
        private void FireServerRpc(Vector3 origin, Vector3 direction)
        {
            // Server validates can fire (ammo, fire rate, not reloading)
            if (!CanFire()) return;

            // Update server weapon state
            currentMagazineAmmo.Value--;
            lastFireTime = Time.time;

            // Server-authoritative raycast
            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, currentWeaponData.maxRange, weaponHitLayers))
            {
                ApplyDamage(hit);
                FireClientRpc(hit.point, hit.normal, true);
            }
            else
            {
                Vector3 endPoint = origin + direction * currentWeaponData.maxRange;
                // Missed shot - still notify clients to play effects
                FireClientRpc(endPoint, Vector3.up, false);
            }
        }

        [ClientRpc]
        private void FireClientRpc(Vector3 hitPoint, Vector3 hitNormal, bool didHit)
        {
            if (IsOwner) return; // Owner already played prediction

            if (didHit)
            {
                SpawnImpactEffect(hitPoint, hitNormal, isPrediction: false);
            }

            // Show shooter's world weapon firing
            if (currentWeaponView != null)
            {
                currentWeaponView.PlayFireAnimation();
                currentWeaponView.PlayFireSound();
                currentWeaponView.PlayMuzzleFlash();
                currentWeaponView.EjectShell();
            }
        }

        private void ApplyDamage(RaycastHit hit)
        {
            HitZone hitZone = hit.collider.GetComponent<HitZone>();
            if (hitZone != null)
            {
                var pair = weaponDatabase.GetWeapon(equippedWeaponID.Value);
                IDamageable target = hitZone.Owner;
                if (target != null && target.IsAlive)
                {
                    // Calculate distance from shooter to hit point
                    float distance = Vector3.Distance(playerCamera.transform.position, hit.point);
                    bool isHeadshot = hitZone.Type == HitZone.ZoneType.Head;

                    // Calculate final weapon damage (includes headshot multiplier and distance falloff)
                    float weaponDamage = WeaponLogic.CalculateWeaponDamage(
                        pair.weaponData,
                        distance,
                        isHeadshot
                    );

                    DamageInfo info = new DamageInfo
                    {
                        BaseDamage = weaponDamage,  // Pre-calculated with weapon-specific modifiers
                        DamageType = DamageType.Physical,
                        Source = DamageSource.Weapon,
                        AttackerId = OwnerClientId,
                        IsHeadshot = isHeadshot,
                        Distance = distance  // For logging/UI purposes
                    };
                    target.TakeDamage(info);
                }
            }
        }

        private bool CanFire()
        {
            if (PlayerHealth == null || !PlayerHealth.IsAlive) return false;
            if (currentWeaponView == null) return false;
            if (isReloading.Value) return false;
            if (currentMagazineAmmo.Value <= 0) return false;
            if (Time.time < lastFireTime + (currentWeaponData != null ? currentWeaponData.fireRate : 0.1f)) return false;

            // Reset shot count if enough time has passed (recoil recovery)
            if (Time.time > lastFireTime + 0.5f)
            {
                currentShotCount = 0;
            }

            return true;
        }

        private void TryReload()
        {
            if (isReloading.Value) return;
            if (currentMagazineAmmo.Value >= (currentWeaponData?.magazineSize ?? 0)) return;

            // Client prediction: start reload animation
            currentWeaponView.PlayReloadAnimation();
            currentWeaponView.PlayReloadSound();

            // Reset shot count and recoil on reload (clean spray pattern reset)
            currentShotCount = 0;
            if (playerController != null)
            {
                playerController.ResetRecoil();
            }

            OnReloadStateChanged?.Invoke(true);

            // Tell server
            ReloadServerRpc();
        }

        private void UpdateAmmoDisplay(int prev, int current)
        {
            if (currentWeaponView == null) return;

            int reserveAmmo = PlayerInventory.GetAmmoCount(equippedWeaponID.Value);

            OnAmmoChanged?.Invoke(current, reserveAmmo);
        }

        [ServerRpc]
        private void ReloadServerRpc()
        {
            if (isReloading.Value) return;
            if (currentWeaponData == null) return;

            // Check if player has reserve ammo
            int reserveAmmo = 100;//PlayerInventory.GetAmmoCount(equippedWeaponID.Value);
            if (reserveAmmo <= 0) return;

            // Start reload
            isReloading.Value = true;

            // Reset shot counter on server (for spray pattern consistency)
            currentShotCount = 0;

            // Wait for reload time, then complete reload
            StartCoroutine(CompleteReloadAfterDelay(currentWeaponData.reloadTime));
        }

        private System.Collections.IEnumerator CompleteReloadAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Calculate how much ammo to reload
            int reserveAmmo = 100;//PlayerInventory.GetAmmoCount(equippedWeaponID.Value);
            int ammoNeeded = currentWeaponData.magazineSize - currentMagazineAmmo.Value;
            int ammoToAdd = Mathf.Min(ammoNeeded, reserveAmmo);

            // Update ammo
            currentMagazineAmmo.Value += ammoToAdd;

            // Consume from reserves (would call PlayerInventory.ConsumeAmmo)
            // PlayerInventory.ConsumeAmmo(equippedWeaponID.Value, ammoToAdd);

            // End reload
            isReloading.Value = false;
        }

        /// <summary>
        /// Spawn impact effect (bullet hole, sparks, etc.)
        /// </summary>
        private void SpawnImpactEffect(Vector3 position, Vector3 normal, bool isPrediction)
        {
            // Spawn particles
            // Spawn decal
            // Play sound

            if (isPrediction)
            {
                Debug.Log($"Client predicted hit at {position}");
            }
            else
            {
                Debug.Log($"Server confirmed hit at {position}");
            }
        }

        #endregion

        #region Weapon Switching

        [ServerRpc]
        private void EquipWeaponServerRpc(int weaponID)
        {
            // Validate player owns weapon
            if (!PlayerOwnsWeapon(weaponID)) return;

            // Update NetworkVariable
            equippedWeaponID.Value = weaponID;
        }

        private void OnEquippedWeaponChanged(int oldID, int newID)
        {
            // Called on all clients when weapon changes
            DestroyCurrentWeapon();
            InstantiateNewWeapon(newID);

            // Update weapon data reference
            WeaponPrefabPair weaponPair = weaponDatabase.GetWeapon(newID);
            if (weaponPair != null)
            {
                currentWeaponData = weaponPair.weaponData;

                // Initialize ammo on server
                if (IsServer)
                {
                    currentMagazineAmmo.Value = currentWeaponData.magazineSize;
                    isReloading.Value = false;
                }

                // Update weapon data and reset recoil for owner
                if (IsOwner && playerController != null)
                {
                    playerController.SetCurrentWeapon(currentWeaponData);
                    playerController.ResetRecoil();
                }
            }

            // Reset shot counter on weapon change
            currentShotCount = 0;

            if (IsOwner && currentWeaponView != null)
            {
                OnWeaponChanged?.Invoke(currentWeaponView.Data);
            }
        }

        private void DestroyCurrentWeapon()
        {
            if (currentWeaponView != null)
                Destroy(currentWeaponView.gameObject);

            currentWeaponView = null;
        }

        private void InstantiateNewWeapon(int weaponID)
        {
            WeaponPrefabPair weaponPair = weaponDatabase.GetWeapon(weaponID);
            if (weaponPair == null) return;

            GameObject weaponObj;

            if (IsOwner)
            {
                // Owner: instantiate view model only
                weaponObj = Instantiate(weaponPair.viewModelPrefab, viewWeaponHolder);
            }
            else
            {
                // Non-owners: instantiate world model only
                weaponObj = Instantiate(weaponPair.worldModelPrefab, worldWeaponHolder);
            }

            // Get WeaponView component from the weapon prefab
            currentWeaponView = weaponObj.GetComponent<WeaponView>();
        }

        private bool PlayerOwnsWeapon(int weaponID)
        {
            // Check if player has unlocked this weapon
            return true; // TODO: implement unlock system
        }

        #endregion

        public WeaponData GetCurrentWeaponData()
        {
            return currentWeaponData;
        }
    }
}