using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TidesEnd.UI
{
    /// <summary>
    /// Main menu UI panel. Handles host/join button clicks.
    /// Fires events that NetworkBootstrap listens to.
    /// </summary>
    public class MainMenuPanel : MonoBehaviour
    {
        public static event Action OnHostRequested;
        public static event Action OnJoinRequested;

        [Header("UI References")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI statusText;

        void Awake()
        {
            // Wire up button listeners
            if (hostButton != null)
                hostButton.onClick.AddListener(HandleHostButton);

            if (joinButton != null)
                joinButton.onClick.AddListener(HandleJoinButton);

            if (quitButton != null)
                quitButton.onClick.AddListener(HandleQuitButton);
        }

        void Start()
        {
            UpdateSteamStatus();
        }

        void Update()
        {
            // Update Steam status every frame (cheap check)
            UpdateSteamStatus();
        }

        private void UpdateSteamStatus()
        {
            if (statusText == null) return;

            var steamManager = SteamManager.Instance;

            if (steamManager == null)
            {
                statusText.text = "<color=yellow>Steam: Initializing...</color>";
                return;
            }

            if (steamManager.IsInitialized)
            {
                string username = steamManager.GetUsername();
                statusText.text = $"<color=green>Steam: Connected as {username}</color>";
            }
            else
            {
                statusText.text = "<color=red>Steam: Not Connected</color>";
            }
        }

        void OnDestroy()
        {
            // Clean up listeners
            if (hostButton != null)
                hostButton.onClick.RemoveListener(HandleHostButton);
            
            if (joinButton != null)
                joinButton.onClick.RemoveListener(HandleJoinButton);
            
            if (quitButton != null)
                quitButton.onClick.RemoveListener(HandleQuitButton);
        }

        void HandleHostButton()
        {
            Debug.Log("Host button clicked");
            OnHostRequested?.Invoke();
        }

        void HandleJoinButton()
        {
            Debug.Log("Join button clicked");
            OnJoinRequested?.Invoke();
        }

        void HandleQuitButton()
        {
            Debug.Log("Quit button clicked");
            Core.GameManager.Instance?.QuitGame();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
