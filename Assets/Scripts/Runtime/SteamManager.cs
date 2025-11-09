using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    private static SteamManager instance;
    public static SteamManager Instance => instance;
    
    [Header("Steam Configuration")]
    [SerializeField] private uint appId = 480; // Spacewar for testing
    
    public bool IsInitialized { get; private set; }
    public ulong LocalSteamId { get; private set; }
    
    private void Awake()
    {
        // Singleton pattern - prevent duplicates
        if (instance != null && instance != this)
        {
            Debug.LogWarning("SteamManager already exists, destroying duplicate");
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Only initialize if not already initialized
        if (!SteamClient.IsValid)
        {
            InitializeSteam();
        }
        else
        {
            Debug.LogWarning("Steam already initialized elsewhere");
            IsInitialized = true;
            LocalSteamId = SteamClient.SteamId;
        }
    }
    
    private void InitializeSteam()
    {
        try
        {
            // Check if Steam is running
            if (!Steamworks.SteamClient.IsValid)
            {
                // Restart through Steam if necessary
                if (SteamClient.RestartAppIfNecessary(appId))
                {
                    Debug.Log("Restarting through Steam...");
                    Application.Quit();
                    return;
                }
            }
            
            // Initialize Steam (only if not already initialized)
            if (!SteamClient.IsValid)
            {
                SteamClient.Init(appId, true);
            }
            
            if (!SteamClient.IsValid)
            {
                Debug.LogError("Steam client is not valid after initialization!");
                IsInitialized = false;
                return;
            }
            
            IsInitialized = true;
            LocalSteamId = SteamClient.SteamId;
            
            Debug.Log("=== STEAM INITIALIZED ===");
            Debug.Log($"Username: {SteamClient.Name}");
            Debug.Log($"Steam ID: {SteamClient.SteamId}");
            Debug.Log("=========================");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize Steam: {e.Message}");
            IsInitialized = false;
        }
    }
    
    private void Update()
    {
        // Process Steam callbacks every frame
        if (IsInitialized && SteamClient.IsValid)
        {
            SteamClient.RunCallbacks();
        }
    }
    
    private void OnApplicationQuit()
    {
        if (IsInitialized && SteamClient.IsValid)
        {
            SteamClient.Shutdown();
            Debug.Log("Steam shutdown");
        }
    }
    
    private void OnDestroy()
    {
        if (instance == this)
        {
            if (IsInitialized && SteamClient.IsValid)
            {
                SteamClient.Shutdown();
            }
            instance = null;
        }
    }
}