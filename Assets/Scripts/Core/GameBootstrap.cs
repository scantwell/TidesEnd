using UnityEngine;
using Unity.Netcode;
using TidesEnd.UI;
using System.Text;
using Netcode.Transports.Facepunch;

namespace TidesEnd.Core
{
    /// <summary>
    /// Central coordinator for network initialization and game flow.
    /// Handles UI events, Steam lobby integration, connection approval, and player spawning.
    ///
    /// CONSOLIDATES:
    /// - NetworkBootstrap (UI events, Steam lobby triggers)
    /// - NetworkConnectionHandler (connection approval, spawn coordination)
    ///
    /// This is the SINGLE SOURCE OF TRUTH for Netcode connection lifecycle.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        [Header("References")]
        [Tooltip("Reference to SpawnManager (will be found automatically if not set)")]
        public SpawnManager spawnManager;

        private bool isUsingSteam = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            // Subscribe to UI events
            MainMenuPanel.OnHostRequested += HandleHostRequest;
            MainMenuPanel.OnJoinRequested += HandleJoinRequest;
            UIManager.OnDisconnectRequested += HandleDisconnect;

            // Subscribe to Steam lobby events (for logging and flow control)
            SteamLobbySystem.OnLobbyCreated += OnSteamLobbyCreated;
            SteamLobbySystem.OnLobbyEntered += OnSteamLobbyEntered;
            SteamLobbySystem.OnJoinRequested += OnSteamJoinRequested;

            // Subscribe to class selection events
            SteamLobbySystem.OnAllPlayersReady += OnAllPlayersReady;

            // Try to set up network callbacks (will retry in Start() if NetworkManager not ready yet)
            TryRegisterNetworkCallbacks();

            // Find SpawnManager if not set
            if (spawnManager == null)
            {
                spawnManager = FindFirstObjectByType<SpawnManager>();
                if (spawnManager == null)
                {
                    Debug.LogError("[GameBootstrap] SpawnManager not found in scene! Player spawning will fail.");
                }
            }
        }

        private void Start()
        {
            // Retry network callback registration in case NetworkManager wasn't ready during OnEnable
            TryRegisterNetworkCallbacks();
        }

        private bool networkCallbacksRegistered = false;

        private void TryRegisterNetworkCallbacks()
        {
            // Skip if already registered
            if (networkCallbacksRegistered) return;

            // Set up connection approval callback (only if NetworkManager exists)
            if (NetworkManager.Singleton != null)
            {
                DetectTransport();
                NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
                networkCallbacksRegistered = true;
                Debug.Log("[GameBootstrap] Network events subscribed successfully");
            }
            else
            {
                Debug.LogWarning("[GameBootstrap] NetworkManager.Singleton is null. " +
                    "Ensure NetworkManager exists in the scene and is enabled.");
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from UI events
            MainMenuPanel.OnHostRequested -= HandleHostRequest;
            MainMenuPanel.OnJoinRequested -= HandleJoinRequest;
            UIManager.OnDisconnectRequested -= HandleDisconnect;

            // Unsubscribe from Steam lobby events
            SteamLobbySystem.OnLobbyCreated -= OnSteamLobbyCreated;
            SteamLobbySystem.OnLobbyEntered -= OnSteamLobbyEntered;
            SteamLobbySystem.OnJoinRequested -= OnSteamJoinRequested;
            SteamLobbySystem.OnAllPlayersReady -= OnAllPlayersReady;

            // Unsubscribe from network events (only if we registered them and NetworkManager still exists)
            if (networkCallbacksRegistered && NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback = null;
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
                networkCallbacksRegistered = false;
                // NOTE: Do NOT call Shutdown() here - OnDisable is called for many reasons
                // (scene unload, GameObject disabled, domain reload, etc.)
                // Shutdown should only be called intentionally via HandleDisconnect()
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        #region Transport Detection

        private void DetectTransport()
        {
            if (NetworkManager.Singleton == null) return;

            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            isUsingSteam = transport != null && transport is FacepunchTransport;

            Debug.Log($"[GameBootstrap] Transport detected: {transport?.GetType().Name}, Using Steam: {isUsingSteam}");
        }

        #endregion

        #region UI Event Handlers

        private void HandleHostRequest()
        {
            Debug.Log("[GameBootstrap] Host requested");

            // If using Steam, create lobby first
            if (isUsingSteam && SteamLobbySystem.Instance != null)
            {
                SteamLobbySystem.Instance.CreateLobby();
                // Flow continues in OnSteamLobbyCreated → class selection → OnAllPlayersReady
            }
            else
            {
                // For Unity Transport, create a "local" lobby and show class selection immediately
                // (No Steam lobby needed for local play)
                Debug.LogWarning("[GameBootstrap] Local transport hosting - class selection not yet implemented for non-Steam");
                // TODO: Show class selection UI for local play
                NetworkManager.Singleton.StartHost();
                UIManager.Instance?.ShowInGame();
            }
        }

        private void HandleJoinRequest()
        {
            Debug.Log("[GameBootstrap] Join requested");

            if (isUsingSteam)
            {
                // For Steam P2P, users should use Steam invites
                Debug.Log("[GameBootstrap] Please use Steam overlay to accept an invite from a friend!");
                // Flow continues when user accepts invite → OnSteamJoinRequested
            }
            else
            {
                // For Unity Transport, just connect directly
                Debug.LogWarning("[GameBootstrap] Local transport joining - class selection not yet implemented for non-Steam");
                NetworkManager.Singleton.StartClient();
            }
        }

        private void HandleDisconnect()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
            }

            // Leave Steam lobby when disconnecting
            if (isUsingSteam && SteamLobbySystem.Instance != null)
            {
                SteamLobbySystem.Instance.LeaveLobby();
            }
        }

        #endregion

        #region Steam Lobby Callbacks

        private void OnSteamLobbyCreated(Steamworks.Data.Lobby lobby)
        {
            Debug.Log($"[GameBootstrap] Lobby created: {lobby.Id}");
            UIManager.Instance?.ShowLobby();
            // NOTE: We do NOT start Netcode here!
            // Players must select classes first, then OnAllPlayersReady triggers connection
        }

        private void OnSteamLobbyEntered(Steamworks.Data.Lobby lobby)
        {
            Debug.Log($"[GameBootstrap] Entered lobby: {lobby.Id}");
            // Show class selection UI (handled by ClassSelectionUI component)
            UIManager.Instance?.ShowLobby();
        }

        private void OnSteamJoinRequested(ulong hostSteamId)
        {
            Debug.Log($"[GameBootstrap] Join requested to host: {hostSteamId}");

            // Set the target Steam ID on the Facepunch transport
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as FacepunchTransport;
            if (transport != null)
            {
                transport.targetSteamId = hostSteamId;
                Debug.Log($"[GameBootstrap] Set Facepunch transport target to: {hostSteamId}");
            }

            // NOTE: We do NOT start Netcode client here!
            // Wait for class selection, then OnAllPlayersReady triggers connection
        }

        #endregion

        #region Class Selection Flow

        /// <summary>
        /// Called when all players are ready in lobby (triggers Netcode connection).
        /// This is the ONLY place where Netcode connection should be initiated (besides local testing).
        /// </summary>
        private void OnAllPlayersReady()
        {
            Debug.Log("[GameBootstrap] All players ready, starting Netcode connection...");

            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("[GameBootstrap] NetworkManager.Singleton is null!");
                return;
            }

            // Get connection data from SteamLobbySystem
            byte[] connectionData = null;
            if (SteamLobbySystem.Instance != null)
            {
                connectionData = SteamLobbySystem.Instance.GetConnectionData();
            }

            // Set connection data
            if (connectionData != null)
            {
                NetworkManager.Singleton.NetworkConfig.ConnectionData = connectionData;
            }

            // Check if we're the lobby owner (host) or joining client
            bool isLobbyOwner = SteamLobbySystem.Instance != null && SteamLobbySystem.Instance.IsLobbyOwner;

            if (isLobbyOwner)
            {
                Debug.Log("[GameBootstrap] Starting as host...");
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                Debug.Log("[GameBootstrap] Starting as client...");
                NetworkManager.Singleton.StartClient();
            }
        }

        #endregion

        #region Connection Approval

        /// <summary>
        /// Connection approval callback - validates connection and receives class selection data.
        /// </summary>
        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            Debug.Log($"[GameBootstrap] Approval check for client {request.ClientNetworkId}");

            // Parse class selection from connection data
            string selectedClassName = "Default";
            if (request.Payload != null && request.Payload.Length > 0)
            {
                selectedClassName = Encoding.UTF8.GetString(request.Payload);
                Debug.Log($"[GameBootstrap] Client {request.ClientNetworkId} selected class: {selectedClassName}");
            }
            else
            {
                Debug.LogWarning($"[GameBootstrap] Client {request.ClientNetworkId} sent no class data, using default");
            }

            // Register class selection with SpawnManager
            if (spawnManager != null)
            {
                spawnManager.RegisterClassSelection(request.ClientNetworkId, selectedClassName);
            }
            else
            {
                Debug.LogError("[GameBootstrap] SpawnManager is null!");
            }

            // Approve the connection
            response.Approved = true;
            response.CreatePlayerObject = false; // We spawn manually via SpawnManager

            Debug.Log($"[GameBootstrap] Client {request.ClientNetworkId} approved");
        }

        #endregion

        #region Network Event Handlers

        /// <summary>
        /// Called when server starts.
        /// </summary>
        private void OnServerStarted()
        {
            Debug.Log("[GameBootstrap] Server started");

            // Transition to in-game UI
            UIManager.Instance?.ShowInGame();

            // If we're the host (server + client), spawn our own player
            if (NetworkManager.Singleton.IsHost)
            {
                ulong hostClientId = NetworkManager.Singleton.LocalClientId;

                // Register host's class selection
                if (SteamLobbySystem.Instance != null && spawnManager != null)
                {
                    string className = SteamLobbySystem.Instance.LocalPlayerSelectedClass != null
                        ? SteamLobbySystem.Instance.LocalPlayerSelectedClass.className
                        : "Default";

                    spawnManager.RegisterClassSelection(hostClientId, className);
                }

                // Spawn host player
                SpawnPlayerForClient(hostClientId);
            }
        }

        /// <summary>
        /// Called when a client connects (called on server for each client).
        /// </summary>
        private void OnClientConnected(ulong clientId)
        {
            // Only transition UI for the local client
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("[GameBootstrap] Local client connected successfully");
                UIManager.Instance?.ShowInGame();
            }

            // Server-side: spawn player for connecting client
            if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log($"[GameBootstrap] Client {clientId} connected to server");

                // Don't spawn host again (already spawned in OnServerStarted)
                if (NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId)
                {
                    return;
                }

                // Spawn player for this client
                SpawnPlayerForClient(clientId);
            }
        }

        /// <summary>
        /// Called when a client disconnects.
        /// </summary>
        private void OnClientDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            Debug.Log($"[GameBootstrap] Client {clientId} disconnected");

            // Clean up player data
            if (spawnManager != null)
            {
                spawnManager.OnPlayerDisconnected(clientId);
            }
        }

        /// <summary>
        /// Spawn player for a connected client.
        /// </summary>
        private void SpawnPlayerForClient(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogError("[GameBootstrap] SpawnPlayerForClient can only be called on server!");
                return;
            }

            if (spawnManager != null)
            {
                spawnManager.SpawnPlayer(clientId);
            }
            else
            {
                Debug.LogError("[GameBootstrap] SpawnManager is null, cannot spawn player!");
            }
        }

        #endregion
    }
}
