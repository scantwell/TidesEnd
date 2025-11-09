using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Netcode.Transports.Facepunch;

public class TransportSwitcher : MonoBehaviour
{
    [Header("Transport Selection")]
    [SerializeField] private bool useSteamInEditor = false; // FALSE for testing
    [SerializeField] private bool useSteamInBuild = true;   // TRUE for release
    
    private UnityTransport unityTransport;
    private NetworkTransport steamTransport;
    private NetworkManager networkManager;
    
    private void Awake()
    {
        // Get NetworkManager from THIS GameObject
        networkManager = GetComponent<NetworkManager>();
        
        if (networkManager == null)
        {
            Debug.LogError("TransportSwitcher must be on the same GameObject as NetworkManager!");
            return;
        }
        
        // Get both transports
        unityTransport = GetComponent<UnityTransport>();
        
        // Find Steam transport - try different possible types
        var transports = GetComponents<NetworkTransport>();
        foreach (var transport in transports)
        {
            // Find the one that's NOT UnityTransport
            if (transport != null && transport.GetType() != typeof(UnityTransport))
            {
                steamTransport = transport;
                break;
            }
        }
        
        if (unityTransport == null)
        {
            Debug.LogError("UnityTransport not found on NetworkManager!");
        }
        
        if (steamTransport == null)
        {
            Debug.LogWarning("Steam transport not found - will use Unity Transport only");
        }
        
        // Decide which to use
        bool useSteam = Application.isEditor ? useSteamInEditor : useSteamInBuild;
        
        SelectTransport(useSteam);
    }
    
    private void SelectTransport(bool useSteam)
    {
        if (networkManager == null) return;
        
        if (useSteam && steamTransport != null)
        {
            Debug.Log("=== USING STEAM P2P TRANSPORT ===");
            
            // Disable Unity Transport
            if (unityTransport != null)
                unityTransport.enabled = false;
            
            // Enable Steam Transport
            steamTransport.enabled = true;
            networkManager.NetworkConfig.NetworkTransport = steamTransport;
        }
        else
        {
            Debug.Log("=== USING UNITY TRANSPORT (Local Testing) ===");
            
            // Enable Unity Transport
            if (unityTransport != null)
            {
                unityTransport.enabled = true;
                networkManager.NetworkConfig.NetworkTransport = unityTransport;
            }
            else
            {
                Debug.LogError("Unity Transport not found!");
            }
            
            // Disable Steam Transport
            if (steamTransport != null)
                steamTransport.enabled = false;
        }
        
        Debug.Log($"Active transport: {networkManager.NetworkConfig.NetworkTransport?.GetType().Name ?? "NONE"}");
    }
}