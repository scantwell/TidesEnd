using UnityEngine;
using Unity.Netcode;
using TidesEnd.UI;
using System;
using Netcode.Transports.Facepunch;

namespace TidesEnd.Core
{
    /// <summary>
    /// Handles network initialization and connection events.
    /// Responds to UI events and triggers network operations.
    /// </summary>
    public class NetworkBootstrap : MonoBehaviour
    {
        private bool isUsingSteam = false;

        void OnEnable()
        {
            DetectTransport();
            // Subscribe to menu events
            MainMenuPanel.OnHostRequested += HandleHostRequest;
            MainMenuPanel.OnJoinRequested += HandleJoinRequest;
            UIManager.OnDisconnectRequested += HandleDisconnect;

            // Subscribe to Steam lobby events
            SteamLobbyManager.OnLobbyCreated += OnSteamLobbyCreated;
            SteamLobbyManager.OnLobbyEntered += OnSteamLobbyEntered;
            SteamLobbyManager.OnJoinRequested += OnSteamJoinRequested;
        }

        private void DetectTransport()
        {
            if (NetworkManager.Singleton == null) return;

            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            isUsingSteam = transport != null && transport is FacepunchTransport;

            Debug.Log($"Transport detected: {transport?.GetType().Name}, Using Steam: {isUsingSteam}");
        }

        private void HandleDisconnect()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
            }

            // Leave Steam lobby when disconnecting
            if (isUsingSteam && SteamLobbyManager.Instance != null)
            {
                SteamLobbyManager.Instance.LeaveLobby();
            }
        }

        // private void DetectTransport()
        // {
        //     var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        //     isUsingSteam = transport.GetType().Name.Contains("Facepunch") || 
        //                 transport.GetType().Name.Contains("Steam");
        // }


        void OnDisable()
        {
            // Unsubscribe from menu events
            MainMenuPanel.OnHostRequested -= HandleHostRequest;
            MainMenuPanel.OnJoinRequested -= HandleJoinRequest;
            UIManager.OnDisconnectRequested -= HandleDisconnect;

            // Unsubscribe from Steam lobby events
            SteamLobbyManager.OnLobbyCreated -= OnSteamLobbyCreated;
            SteamLobbyManager.OnLobbyEntered -= OnSteamLobbyEntered;
            SteamLobbyManager.OnJoinRequested -= OnSteamJoinRequested;

            // Unsubscribe from network events if they exist
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.Shutdown();
            }
        }

        void HandleHostRequest()
        {
            Debug.Log("Starting host...");

            // If using Steam, create lobby first
            if (isUsingSteam && SteamLobbyManager.Instance != null)
            {
                SteamLobbyManager.Instance.CreateLobby();
                // StartHost will be called in OnSteamLobbyCreated callback
            }
            else
            {
                // For Unity Transport, start host directly
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;
                NetworkManager.Singleton.StartHost();
            }
        }

        void HandleJoinRequest()
        {
            Debug.Log("Starting client...");

            if (isUsingSteam)
            {
                // For Steam P2P, users should use Steam invites
                // The OnGameLobbyJoinRequested callback will handle joining via invite
                Debug.Log("Please use Steam overlay to accept an invite from a friend!");
                Debug.Log("Or wait for the host to invite you.");
            }
            else
            {
                // For Unity Transport, just connect directly
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.StartClient();
            }
        }

        void OnServerStarted()
        {
            Debug.Log("Server started successfully");
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            
            // Transition to in-game UI
            UIManager.Instance.ShowInGame();
        }

        void OnClientConnected(ulong clientId)
        {
            // Only transition UI for the local client
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("Client connected successfully");
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

                // Transition to in-game UI
                UIManager.Instance.ShowInGame();
            }
        }

        #region Steam Lobby Callbacks

        void OnSteamLobbyCreated(Steamworks.Data.Lobby lobby)
        {
            Debug.Log($"NetworkBootstrap: Lobby created, starting host...");

            // Now that lobby is created, start the Netcode host
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.StartHost();
        }

        void OnSteamLobbyEntered(Steamworks.Data.Lobby lobby)
        {
            Debug.Log($"NetworkBootstrap: Entered lobby, preparing to join...");
            // Wait for OnSteamJoinRequested to get the host Steam ID
        }

        void OnSteamJoinRequested(ulong hostSteamId)
        {
            Debug.Log($"NetworkBootstrap: Joining host {hostSteamId}...");

            // Set the target Steam ID on the Facepunch transport
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as FacepunchTransport;
            if (transport != null)
            {
                transport.targetSteamId = hostSteamId;
                Debug.Log($"Set Facepunch transport target to: {hostSteamId}");
            }

            // Start Netcode client
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.StartClient();
        }

        #endregion
    }
}
