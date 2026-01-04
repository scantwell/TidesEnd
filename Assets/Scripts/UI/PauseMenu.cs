using UnityEngine;
using UnityEngine.InputSystem;
using TidesEnd.Core;
using TidesEnd.Player;
using System;
using UnityEngine.UI;

namespace TidesEnd.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static event Action OnResumeRequested;
        public static event Action OnMainMenuRequested;
        public static event Action OnSpawnEnemyRequested;

        [Header("UI References")]
        [SerializeField] private Button spawnEnemyButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button inviteFriendsButton;

        void Awake()
        {
            // Wire up button listeners
            if (resumeButton != null)
                resumeButton.onClick.AddListener(HandleResumeButton);

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(HandleMainMenuButton);

            if (quitButton != null)
                quitButton.onClick.AddListener(HandleQuitButton);

            if (spawnEnemyButton != null)
                spawnEnemyButton.onClick.AddListener(HandleSpawnEnemyButton); 
            if (inviteFriendsButton != null)
                inviteFriendsButton.onClick.AddListener(HandleInviteFriendsButton);
            // Show invite button only if we're in a Steam lobby as the owner
            bool showInviteButton = SteamLobbySystem.Instance != null &&
                                   SteamLobbySystem.Instance.IsInLobby &&
                                   SteamLobbySystem.Instance.IsLobbyOwner;

            if (inviteFriendsButton != null)
            {
                inviteFriendsButton.gameObject.SetActive(showInviteButton);
            }                              
        }

        void OnDestroy()
        {
            // Clean up listeners
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(HandleResumeButton);

            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(HandleMainMenuButton);

            if (quitButton != null)
                quitButton.onClick.RemoveListener(HandleQuitButton);

            if (spawnEnemyButton != null)
                spawnEnemyButton.onClick.RemoveListener(HandleSpawnEnemyButton);

            if (inviteFriendsButton != null)
                inviteFriendsButton.onClick.RemoveListener(HandleInviteFriendsButton);               
        }

        void HandleResumeButton()
        {
            Debug.Log("Resume button clicked");
            OnResumeRequested?.Invoke();
        }
        void HandleSpawnEnemyButton()
        {
            Debug.Log("Spawn enemy button clicked");
            OnSpawnEnemyRequested?.Invoke();
        }

        void HandleMainMenuButton()
        {
            Debug.Log("Main menu button clicked");
            OnMainMenuRequested?.Invoke();
        }

        void HandleQuitButton()
        {
            Debug.Log("Quit button clicked");
            GameManager.Instance?.QuitGame();
        }

        private void HandleInviteFriendsButton()
        {
            if (SteamLobbySystem.Instance != null)
            {
                SteamLobbySystem.Instance.OpenInviteDialog();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);

            // // Disable player input
            DisablePlayerInput();

            // // Optional: Pause time (only works in single player)
            // // Time.timeScale = 0f;

            Debug.Log("Game Paused");
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            // // Re-enable player input
            EnablePlayerInput();

            // Unpause time
            // Time.timeScale = 1f;

            Debug.Log("Game Resumed");
        }

        private void DisablePlayerInput()
        {
            // Find local player and disable their scripts
            var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

            foreach (var player in players)
            {
                // Only disable local player's input
                if (player.IsOwner)
                {
                    player.enabled = false;

                    // Also disable combat
                    var combat = player.GetComponent<PlayerWeaponController>();
                    if (combat != null)
                    {
                        combat.enabled = false;
                    }
                }
            }
        }

        private void EnablePlayerInput()
        {
            var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

            foreach (var player in players)
            {
                if (player.IsOwner)
                {
                    player.enabled = true;

                    var combat = player.GetComponent<PlayerWeaponController>();
                    if (combat != null)
                    {
                        combat.enabled = true;
                    }
                }
            }
        }
    }
}
