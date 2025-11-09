using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject inGameUI;
    
    private bool isPaused = false;
    
    private void Awake()
    {
        // Make sure pause menu starts hidden
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    private void Update()
    {
        // Check for ESC key
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    
    public void Pause()
    {
        isPaused = true;
        
        // Show pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        // Hide in-game UI (optional)
        if (inGameUI != null)
        {
            inGameUI.SetActive(false);
        }
        
        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Disable player input
        DisablePlayerInput();
        
        // Optional: Pause time (only works in single player)
        // Time.timeScale = 0f;
        
        Debug.Log("Game Paused");
    }
    
    public void Resume()
    {
        isPaused = false;
        
        // Hide pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Show in-game UI
        if (inGameUI != null)
        {
            inGameUI.SetActive(true);
        }
        
        // Lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Re-enable player input
        EnablePlayerInput();
        
        // Unpause time
        // Time.timeScale = 1f;
        
        Debug.Log("Game Resumed");
    }
    
    private void DisablePlayerInput()
    {
        // Find local player and disable their scripts
        var players = FindObjectsByType<NetworkedFPSController>(FindObjectsSortMode.None);
        
        foreach (var player in players)
        {
            // Only disable local player's input
            if (player.IsOwner)
            {
                player.enabled = false;
                
                // Also disable combat
                var combat = player.GetComponent<PlayerCombat>();
                if (combat != null)
                {
                    combat.enabled = false;
                }
            }
        }
    }
    
    private void EnablePlayerInput()
    {
        var players = FindObjectsByType<NetworkedFPSController>(FindObjectsSortMode.None);
        
        foreach (var player in players)
        {
            if (player.IsOwner)
            {
                player.enabled = true;
                
                var combat = player.GetComponent<PlayerCombat>();
                if (combat != null)
                {
                    combat.enabled = true;
                }
            }
        }
    }
    
    // Button callbacks
    public void OnResumeClicked()
    {
        Resume();
    }
    
    public void OnDisconnectClicked()
    {
        Resume(); // Unpause first
        
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
        
        Debug.Log("Disconnected");
    }
    
    public void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}