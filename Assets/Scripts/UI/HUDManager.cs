using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TidesEnd.Combat;
using TidesEnd.Weapons;

public class HUDManager : MonoBehaviour
{
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
    
    private Health playerHealth;
    private WeaponManager weaponManager;
    private float crosshairResetTime;
    
    private void Update()
    {
        UpdateHUD();
    }
    
    private void UpdateHUD()
    {
        // Find local player if needed
        if (playerHealth == null || weaponManager == null)
        {
            FindLocalPlayer();
            return;
        }
        
        UpdateHealth();
        UpdateWeapon();
        UpdateCrosshair();
    }
    
    private void FindLocalPlayer()
    {
        var players = FindObjectsByType<NetworkedFPSController>(FindObjectsSortMode.None);
        
        foreach (var player in players)
        {
            if (player.IsOwner)
            {
                playerHealth = player.GetComponent<Health>();
                weaponManager = player.GetComponent<WeaponManager>();
                break;
            }
        }
    }
    
    private void UpdateHealth()
    {
        if (playerHealth == null) return;
        
        float healthPercent = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        
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
            healthText.text = Mathf.CeilToInt(playerHealth.CurrentHealth).ToString();
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
    
    private void UpdateWeapon()
    {
        if (weaponManager == null || weaponManager.CurrentWeapon == null)
        {
            if (weaponNameText != null) weaponNameText.text = "NO WEAPON";
            if (ammoText != null) ammoText.text = "-- / --";
            return;
        }
        
        Weapon weapon = weaponManager.CurrentWeapon;
        
        // Weapon name
        if (weaponNameText != null)
        {
            weaponNameText.text = weapon.Data.weaponName.ToUpper();
        }
        
        // Ammo display
        if (ammoText != null)
        {
            ammoText.text = $"{weapon.CurrentAmmo} / {weapon.ReserveAmmo}";
            
            // Color based on ammo
            if (weapon.CurrentAmmo == 0)
                ammoText.color = Color.red;
            else if (weapon.CurrentAmmo <= weapon.MaxAmmo * 0.25f)
                ammoText.color = Color.yellow;
            else
                ammoText.color = Color.white;
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
}