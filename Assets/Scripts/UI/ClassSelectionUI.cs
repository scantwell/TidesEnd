using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TidesEnd;
using TidesEnd.Abilities;

namespace TidesEnd.UI
{
    /// <summary>
    /// UI for selecting a character class in the lobby before match start.
    /// Displays available classes, their info, and allows player to select and ready up.
    /// </summary>
    public class ClassSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Parent container for class selection buttons")]
        public Transform classButtonContainer;

        [Tooltip("Prefab for class selection button")]
        public GameObject classButtonPrefab;

        [Tooltip("Text displaying selected class name")]
        public TextMeshProUGUI selectedClassNameText;

        [Tooltip("Text displaying class description")]
        public TextMeshProUGUI classDescriptionText;

        [Tooltip("Image displaying class icon")]
        public Image classIconImage;

        [Tooltip("Parent container for ability icons")]
        public Transform abilityIconContainer;

        [Tooltip("Prefab for ability icon display")]
        public GameObject abilityIconPrefab;

        [Tooltip("Ready button")]
        public Button readyButton;

        [Tooltip("Ready button text")]
        public TextMeshProUGUI readyButtonText;

        [Tooltip("Lobby members list container")]
        public Transform lobbyMembersContainer;

        [Tooltip("Prefab for lobby member entry")]
        public GameObject lobbyMemberPrefab;

        [Header("Configuration")]
        [Tooltip("Canvas group for showing/hiding UI")]
        public CanvasGroup canvasGroup;

        private ClassData selectedClass;
        private bool isReady = false;
        private List<GameObject> classButtons = new List<GameObject>();
        private List<GameObject> lobbyMemberEntries = new List<GameObject>();

        private void Start()
        {
            // Subscribe to SteamLobbySystem events
            if (SteamLobbySystem.Instance != null)
            {
                SteamLobbySystem.OnPlayerClassChanged += OnPlayerClassChanged;
                SteamLobbySystem.OnPlayerReadyChanged += OnPlayerReadyChanged;
                SteamLobbySystem.OnAllPlayersReady += OnAllPlayersReady;

                // Populate class buttons
                PopulateClassButtons();

                // Update lobby members list
                RefreshLobbyMembers();
            }
            else
            {
                Debug.LogError("[ClassSelectionUI] SteamLobbySystem.Instance is null!");
            }

            // Setup ready button
            if (readyButton != null)
            {
                readyButton.onClick.AddListener(ToggleReady);
                UpdateReadyButton();
            }

            Show();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            SteamLobbySystem.OnPlayerClassChanged -= OnPlayerClassChanged;
            SteamLobbySystem.OnPlayerReadyChanged -= OnPlayerReadyChanged;
            SteamLobbySystem.OnAllPlayersReady -= OnAllPlayersReady;

            if (readyButton != null)
            {
                readyButton.onClick.RemoveListener(ToggleReady);
            }
        }

        /// <summary>
        /// Populate class selection buttons from available classes.
        /// </summary>
        private void PopulateClassButtons()
        {
            if (SteamLobbySystem.Instance == null || classButtonContainer == null || classButtonPrefab == null)
                return;

            // Clear existing buttons
            foreach (var button in classButtons)
            {
                Destroy(button);
            }
            classButtons.Clear();

            // Create button for each available class
            foreach (var classData in SteamLobbySystem.Instance.availableClasses)
            {
                GameObject buttonObj = Instantiate(classButtonPrefab, classButtonContainer);
                Button button = buttonObj.GetComponent<Button>();

                // Set button text
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = classData.className;
                }

                // Set button icon
                Image buttonIcon = buttonObj.transform.Find("Icon")?.GetComponent<Image>();
                if (buttonIcon != null && classData.icon != null)
                {
                    buttonIcon.sprite = classData.icon;
                }

                // Add click listener
                ClassData capturedClassData = classData; // Capture for closure
                button.onClick.AddListener(() => SelectClass(capturedClassData));

                classButtons.Add(buttonObj);
            }

            // Select default class if available
            if (SteamLobbySystem.Instance.availableClasses.Count > 0)
            {
                SelectClass(SteamLobbySystem.Instance.availableClasses[0]);
            }
        }

        /// <summary>
        /// Select a class.
        /// </summary>
        private void SelectClass(ClassData classData)
        {
            if (isReady)
            {
                Debug.LogWarning("[ClassSelectionUI] Cannot change class while ready!");
                return;
            }

            selectedClass = classData;
            SteamLobbySystem.Instance?.SelectClass(classData);

            UpdateClassDisplay();
        }

        /// <summary>
        /// Update the class info display.
        /// </summary>
        private void UpdateClassDisplay()
        {
            if (selectedClass == null)
                return;

            // Update class name
            if (selectedClassNameText != null)
            {
                selectedClassNameText.text = selectedClass.className;
            }

            // Update description
            if (classDescriptionText != null)
            {
                classDescriptionText.text = selectedClass.description;
            }

            // Update icon
            if (classIconImage != null && selectedClass.icon != null)
            {
                classIconImage.sprite = selectedClass.icon;
                classIconImage.enabled = true;
            }

            // Update abilities
            UpdateAbilityDisplay();
        }

        /// <summary>
        /// Update the ability icons display.
        /// </summary>
        private void UpdateAbilityDisplay()
        {
            if (abilityIconContainer == null || abilityIconPrefab == null)
                return;

            // Clear existing ability icons
            foreach (Transform child in abilityIconContainer)
            {
                Destroy(child.gameObject);
            }

            if (selectedClass == null || selectedClass.abilities == null)
                return;

            // Create icon for each ability
            foreach (var ability in selectedClass.abilities)
            {
                if (ability == null)
                    continue;

                GameObject iconObj = Instantiate(abilityIconPrefab, abilityIconContainer);
                Image icon = iconObj.GetComponent<Image>();

                if (icon != null && ability.icon != null)
                {
                    icon.sprite = ability.icon;
                }

                // Optional: Add tooltip with ability name/description
                TextMeshProUGUI tooltip = iconObj.GetComponentInChildren<TextMeshProUGUI>();
                if (tooltip != null)
                {
                    tooltip.text = ability.abilityName;
                }
            }
        }

        /// <summary>
        /// Toggle ready state.
        /// </summary>
        private void ToggleReady()
        {
            if (selectedClass == null)
            {
                Debug.LogWarning("[ClassSelectionUI] Cannot ready up without selecting a class!");
                return;
            }

            isReady = !isReady;
            SteamLobbySystem.Instance?.SetReady(isReady);

            UpdateReadyButton();
        }

        /// <summary>
        /// Update ready button appearance.
        /// </summary>
        private void UpdateReadyButton()
        {
            if (readyButton == null || readyButtonText == null)
                return;

            readyButtonText.text = isReady ? "Unready" : "Ready";
            readyButton.interactable = selectedClass != null;
        }

        /// <summary>
        /// Refresh the lobby members list.
        /// </summary>
        private void RefreshLobbyMembers()
        {
            if (lobbyMembersContainer == null || lobbyMemberPrefab == null)
                return;

            // Clear existing entries
            foreach (var entry in lobbyMemberEntries)
            {
                Destroy(entry);
            }
            lobbyMemberEntries.Clear();

            if (SteamLobbySystem.Instance == null)
                return;

            // Create entry for each lobby member
            foreach (var memberId in SteamLobbySystem.Instance.GetLobbyMembers())
            {
                GameObject entryObj = Instantiate(lobbyMemberPrefab, lobbyMembersContainer);

                // Update member name
                TextMeshProUGUI nameText = entryObj.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                {
                    nameText.text = $"{SteamLobbySystem.Instance.GetLobbyMemberName(memberId)}"; // Could use Steam name here
                }

                // Update class selection
                ClassData memberClass = SteamLobbySystem.Instance.GetPlayerClassSelection(memberId);
                TextMeshProUGUI classText = entryObj.transform.Find("Class")?.GetComponent<TextMeshProUGUI>();
                if (classText != null)
                {
                    classText.text = memberClass != null ? memberClass.className : "None";
                }

                // Update ready state
                bool memberReady = SteamLobbySystem.Instance.GetPlayerReadyState(memberId);
                TextMeshProUGUI readyText = entryObj.transform.Find("Ready")?.GetComponent<TextMeshProUGUI>();
                if (readyText != null)
                {
                    readyText.text = memberReady ? "Ready" : "Not Ready";
                    readyText.color = memberReady ? Color.green : Color.red;
                }

                lobbyMemberEntries.Add(entryObj);
            }
        }

        #region Event Handlers

        private void OnPlayerClassChanged(ulong playerId, ClassData classData)
        {
            RefreshLobbyMembers();
        }

        private void OnPlayerReadyChanged(ulong playerId, bool ready)
        {
            RefreshLobbyMembers();
        }

        private void OnAllPlayersReady()
        {
            Debug.Log("[ClassSelectionUI] All players ready, starting match...");
            Hide();
        }

        #endregion

        #region Show/Hide

        public void Show()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
