using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TidesEnd.Combat;
using TidesEnd.Weapons;
using TidesEnd.Player;
using System;

namespace TidesEnd.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HUDManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup canvasGroup;
        [Header("Health")]
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image lowHealthVignette;
        
        [Header("Weapon")]
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI ammoText;
        
        [Header("Crosshair")]
        [SerializeField] private Image crosshair;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitFeedbackDuration = 0.1f;

        [Header("Multiplayer")]
        [SerializeField] private TextMeshProUGUI lobbyInfoText;

        private Health playerHealth;
        private PlayerWeaponController weaponController;
        private float crosshairResetTime;

        void OnEnable()
        {
            PlayerRoot.OnLocalPlayerSpawned += HandleLocalPlayerSpawn;
            Hide();
        }

        void OnDisable()
        {
            PlayerRoot.OnLocalPlayerSpawned -= HandleLocalPlayerSpawn;
        }

        private void HandleLocalPlayerSpawn(PlayerRoot playerRoot)
        {
            playerHealth = playerRoot.GetComponent<Health>();
            playerHealth.OnHealthChanged += HandleLocalPlayerHealthChanged;

            weaponController = playerRoot.GetComponent<PlayerWeaponController>();
            weaponController.OnWeaponChanged += HandleWeaponChanged;
            weaponController.OnAmmoChanged += HandleAmmoChanged;
            weaponController.OnReloadStateChanged += HandleReloadStateChanged;
            var wd = weaponController.GetCurrentWeaponData();
            if (wd != null)
                HandleWeaponChanged(wd);

            // Show the HUD when the local player spawns
            Show();
        }

        private void OnDestroy()
        {
            // Unsubscribe from health changes
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= HandleLocalPlayerHealthChanged;
            }
            if (weaponController != null)
            {
                weaponController.OnWeaponChanged -= HandleWeaponChanged;
                weaponController.OnAmmoChanged -= HandleAmmoChanged;
                weaponController.OnReloadStateChanged -= HandleReloadStateChanged;
            }
        }
        
        private void HandleWeaponChanged(WeaponData weaponData)
        {
            weaponNameText.text = weaponData.weaponName;
            //weaponIcon.sprite = weaponData.weaponIcon; // Add to WeaponData
        }
        
        private void HandleAmmoChanged(int current, int reserve)
        {
            ammoText.text = $"{current} / {reserve}";

            // Color based on ammo
            if (current == 0)
                ammoText.color = Color.red;
            else if (current <= reserve * 0.25f)
                ammoText.color = Color.yellow;
            else
                ammoText.color = Color.white;
        }
        
        private void HandleReloadStateChanged(bool isReloading)
        {
            //reloadingIndicator.SetActive(isReloading);
        }

        private void Update()
        {
            UpdateCrosshair();
            UpdateMultiplayerUI();
        }

        private void UpdateMultiplayerUI()
        {
            // Update lobby info text
            if (lobbyInfoText != null && SteamLobbySystem.Instance != null)
            {
                if (SteamLobbySystem.Instance.IsInLobby)
                {
                    lobbyInfoText.text = SteamLobbySystem.Instance.GetLobbyInfo();
                    lobbyInfoText.gameObject.SetActive(true);
                }
                else
                {
                    lobbyInfoText.gameObject.SetActive(false);
                }
            }
        }

        private void HandleLocalPlayerHealthChanged(float previous, float current, float maxHealth)
        {
            if (playerHealth == null) return;

            float healthPercent = current / maxHealth;

            // Health bar fill
            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = healthPercent;

                // Color gradient
                if (healthPercent > 0.5f)
                    healthBarFill.color = Color.Lerp(Color.yellow, Color.green, (healthPercent - 0.5f) * 2f);
                else
                    healthBarFill.color = Color.Lerp(Color.red, Color.yellow, healthPercent * 2f);
            }

            // Health number
            if (healthText != null)
            {
                healthText.text = Mathf.CeilToInt(current).ToString();
            }

            // Low health vignette
            if (lowHealthVignette != null)
            {
                if (healthPercent <= 0.3f)
                {
                    // Pulse when low
                    float pulse = Mathf.Lerp(0.2f, 0.5f, Mathf.PingPong(Time.time * 2f, 1f));
                    float alpha = Mathf.Lerp(0f, pulse, (0.3f - healthPercent) / 0.3f);

                    Color c = lowHealthVignette.color;
                    c.a = alpha;
                    lowHealthVignette.color = c;
                }
                else
                {
                    // Fade out
                    Color c = lowHealthVignette.color;
                    c.a = 0f;
                    lowHealthVignette.color = c;
                }
            }
        }

        private void UpdateCrosshair()
        {
            if (crosshair == null) return;
            
            // Reset after hit feedback
            if (Time.time > crosshairResetTime)
            {
                crosshair.color = normalColor;
            }
        }
        
        public void ShowHitMarker()
        {
            if (crosshair != null)
            {
                crosshair.color = hitColor;
                crosshairResetTime = Time.time + hitFeedbackDuration;
            }
        }

        public void Show()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                // Debug crosshair visibility
                if (crosshair != null)
                {
                    Debug.Log($"[HUDManager] Show() called - Crosshair assigned, Sprite: {(crosshair.sprite != null ? crosshair.sprite.name : "NULL")}, Active: {crosshair.gameObject.activeSelf}, Color: {crosshair.color}");
                }
                else
                {
                    Debug.LogWarning("[HUDManager] Show() called but Crosshair is NULL! Please assign it in the Inspector.");
                }
            }
        }

        public void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}