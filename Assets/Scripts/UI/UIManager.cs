using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TidesEnd.UI
{
    /// <summary>
    /// Centralized UI state manager. Controls which menus are visible.
    /// This is the single source of truth for UI transitions.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private enum UIState
        {
            MainMenu,
            Lobby,
            InGame,
            Paused
        }

        public static UIManager Instance { get; private set; }

        public static event Action OnDisconnectRequested;
        [Header("UI Panels")]
        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private HUDManager hudManager;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private ClassSelectionUI lobby;
        
        private UIState currentState = UIState.MainMenu;
        
        void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            // Centralized ESC handling based on state
            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                HandleEscapeKey();
            }
        }
        
        void HandleEscapeKey()
        {
            switch (currentState)
            {
                case UIState.InGame:
                    ShowPauseMenu();
                    break;
                
                case UIState.Paused:
                    ShowInGame();
                    break;
                
                case UIState.Lobby:
                case UIState.MainMenu:
                    // Do nothing or show quit confirmation
                    break;
            }
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Debug.Assert(mainMenuPanel != null, "MainMenuPanel not assigned!", this);
            Debug.Assert(hudManager != null, "HUDManager not assigned!", this);
            Debug.Assert(pauseMenu != null, "PauseMenu not assigned!", this);
        }

        void Start()
        {
            // Initialize to main menu state
            ShowMainMenu();
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        void OnEnable()
        {
            PauseMenu.OnResumeRequested += ShowInGame;
            PauseMenu.OnMainMenuRequested += ReturnToMainMenu;
        }

        void OnDisable()
        {
            PauseMenu.OnResumeRequested -= ShowInGame;
            PauseMenu.OnMainMenuRequested -= ReturnToMainMenu;
        }

        public void ShowLobby()
        {
            currentState = UIState.Lobby;
            if (mainMenuPanel != null) mainMenuPanel.Hide();
            if (hudManager != null) hudManager.Hide();
            if (pauseMenu != null) pauseMenu.Hide();
            if (lobby != null) lobby.Show();
        }

        public void ShowMainMenu()
        {
            currentState = UIState.MainMenu;
            if (hudManager != null) hudManager.Hide();
            if (pauseMenu != null) pauseMenu.Hide();
            if (lobby != null) lobby.Hide();
            if (mainMenuPanel != null) mainMenuPanel.Show();
        }

        public void ShowInGame()
        {            
            // Lock cursor back
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentState = UIState.InGame;
            if (mainMenuPanel != null) mainMenuPanel.Hide();
            if (pauseMenu != null) pauseMenu.Hide();  
            if (lobby != null) lobby.Hide();          
            if (hudManager != null) hudManager.Show();
        }

        public void ShowPauseMenu()
        {
            //Unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            currentState = UIState.Paused;
            if (hudManager != null) hudManager.Hide();
            if (lobby != null) lobby.Hide();
            if (pauseMenu != null) pauseMenu.Show();           
        }

        void ReturnToMainMenu()
        {
            OnDisconnectRequested?.Invoke();
            
            ShowMainMenu();
        }
    }
}
