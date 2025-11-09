using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class SteamNetworkManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button disconnectButton;
    [SerializeField] private Button spawnEnemyButton;
    [SerializeField] private TextMeshProUGUI statusText;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject inGamePanel; // Optional: for disconnect button
    
    private bool isUsingSteam = false;
    
    private void Start()
    {
        hostButton.onClick.AddListener(OnHostClicked);
        clientButton.onClick.AddListener(OnClientClicked);
        disconnectButton.onClick.AddListener(OnDisconnectClicked);
        spawnEnemyButton.onClick.AddListener(OnSpawnEnemiesButtonClicked);
        
        DetectTransport();
        UpdateUI();
    }
    
    private void Update()
    {
        UpdateUI();
    }
    
    private void DetectTransport()
    {
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        isUsingSteam = transport.GetType().Name.Contains("Facepunch") || 
                       transport.GetType().Name.Contains("Steam");
    }
    
    private void OnHostClicked()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    private void OnClientClicked()
    {
        if (isUsingSteam)
        {
            statusText.text = "Join via Steam invite (coming soon)";
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    private void OnDisconnectClicked()
    {
        NetworkManager.Singleton.Shutdown();
    }
    
    public void OnSpawnEnemiesButtonClicked()
    {
        Debug.Log("Spawn Enemies Button Clicked");
        if (NetworkManager.Singleton.IsServer && SpawnManager.Instance != null)
        {
            SpawnManager.Instance.SpawnInitialEnemies();
        }
    }
    
    private void UpdateUI()
    {
        if (NetworkManager.Singleton == null) return;
        
        bool isInGame = NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient;
        
        // Show/hide panels
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(!isInGame);
        }
        
        if (inGamePanel != null)
        {
            inGamePanel.SetActive(isInGame);
        }
        
        // Update UI state
        if (isInGame)
        {
            disconnectButton.gameObject.SetActive(true);
            spawnEnemyButton.gameObject.SetActive(true);
        }
        else
        {
            string transportName = isUsingSteam ? "Steam P2P" : "Local Network";
            statusText.text = $"Ready ({transportName})";
            
            hostButton.interactable = true;
            clientButton.interactable = true;
            disconnectButton.gameObject.SetActive(false);
            
            // Show cursor in menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}