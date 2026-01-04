using System;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using TidesEnd.Abilities;
using System.Linq;

namespace TidesEnd
{
    /// <summary>
    /// Unified system for Steam lobby management and class selection.
    ///
    /// CONSOLIDATES:
    /// - SteamLobbyManager (Steam API operations: create, join, leave, invite)
    /// - LobbyManager (Class selection and ready state tracking)
    ///
    /// Responsibilities:
    /// 1. Steam Lobby Operations (low-level Steam API)
    /// 2. Class Selection Tracking (game-level lobby logic)
    /// 3. Ready State Management
    /// 4. Event broadcasting for lobby state changes
    /// </summary>
    public class SteamLobbySystem : MonoBehaviour
    {
        public static SteamLobbySystem Instance { get; private set; }

        #region Configuration

        [Header("Lobby Settings")]
        [SerializeField] private int maxPlayers = 4;

        [Header("Class Selection")]
        [Tooltip("Available classes for selection")]
        public List<ClassData> availableClasses = new List<ClassData>();

        [Tooltip("Default class if none selected")]
        public ClassData defaultClass;

        #endregion

        #region State

        // Steam lobby state
        public Lobby? CurrentLobby { get; private set; }
        public bool IsInLobby => CurrentLobby.HasValue;
        public bool IsLobbyOwner => IsInLobby && CurrentLobby.Value.Owner.Id == SteamClient.SteamId;

        // Class selection state
        private Dictionary<ulong, ClassData> classSelections = new Dictionary<ulong, ClassData>();
        private Dictionary<ulong, bool> readyStates = new Dictionary<ulong, bool>();

        // Local player state
        public ClassData LocalPlayerSelectedClass { get; private set; }
        public bool LocalPlayerReady { get; private set; }

        #endregion

        #region Events

        // Steam lobby events
        public static event Action<Lobby> OnLobbyCreated;
        public static event Action<Lobby> OnLobbyEntered;
        public static event Action<ulong> OnJoinRequested; // Sends host Steam ID

        // Class selection events
        public static event Action<ulong, ClassData> OnPlayerClassChanged;
        public static event Action<ulong, bool> OnPlayerReadyChanged;
        public static event Action OnAllPlayersReady;

        #endregion

        #region Unity Lifecycle

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
            // Subscribe to Steam callbacks
            SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
            SteamMatchmaking.OnLobbyEntered += OnLobbyEnteredCallback;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
            SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeaveCallback;
            SteamMatchmaking.OnLobbyMemberDataChanged += OnLobbyMemberDataChanged;
            SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreatedCallback;
            SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequestedCallback;
        }

        private void OnDisable()
        {
            // Unsubscribe from Steam callbacks
            SteamMatchmaking.OnLobbyCreated -= OnLobbyCreatedCallback;
            SteamMatchmaking.OnLobbyEntered -= OnLobbyEnteredCallback;
            SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoinedCallback;
            SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeaveCallback;
            SteamMatchmaking.OnLobbyMemberDataChanged -= OnLobbyMemberDataChanged;
            SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreatedCallback;
            SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequestedCallback;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                LeaveLobby();
                Instance = null;
            }
        }

        #endregion

        #region Steam Lobby Operations

        /// <summary>
        /// Create a new Steam lobby (called when hosting).
        /// </summary>
        public async void CreateLobby()
        {
            if (!SteamClient.IsValid)
            {
                Debug.LogError("[SteamLobbySystem] Cannot create lobby - Steam not initialized!");
                return;
            }

            if (IsInLobby)
            {
                Debug.LogWarning("[SteamLobbySystem] Already in a lobby. Leaving current lobby first.");
                LeaveLobby();
            }

            Debug.Log($"[SteamLobbySystem] Creating Steam lobby with max {maxPlayers} players...");

            try
            {
                var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);

                if (!createLobbyOutput.HasValue)
                {
                    Debug.LogError("[SteamLobbySystem] Failed to create lobby!");
                    return;
                }

                CurrentLobby = createLobbyOutput.Value;

                // Set lobby data
                CurrentLobby.Value.SetPublic();
                CurrentLobby.Value.SetJoinable(true);
                CurrentLobby.Value.SetData("name", $"{SteamClient.Name}'s Game");
                CurrentLobby.Value.SetData("gameVersion", Application.version);

                Debug.Log($"[SteamLobbySystem] ✓ Lobby created! Lobby ID: {CurrentLobby.Value.Id}");
                Debug.Log($"  Owner: {CurrentLobby.Value.Owner.Name} ({CurrentLobby.Value.Owner.Id})");
                Debug.Log($"  Max Players: {CurrentLobby.Value.MaxMembers}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SteamLobbySystem] Exception creating lobby: {e.Message}");
            }
        }

        /// <summary>
        /// Join a Steam lobby by ID.
        /// </summary>
        public async void JoinLobby(SteamId lobbyId)
        {
            if (!SteamClient.IsValid)
            {
                Debug.LogError("[SteamLobbySystem] Cannot join lobby - Steam not initialized!");
                return;
            }

            Debug.Log($"[SteamLobbySystem] Joining lobby: {lobbyId}...");

            try
            {
                Lobby? enterLobbyOutput = await SteamMatchmaking.JoinLobbyAsync(lobbyId);

                if (enterLobbyOutput == null)
                {
                    Debug.LogError($"[SteamLobbySystem] Failed to join lobby!");
                    return;
                }

                Debug.Log($"[SteamLobbySystem] ✓ Successfully joined lobby {lobbyId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SteamLobbySystem] Exception joining lobby: {e.Message}");
            }
        }

        /// <summary>
        /// Leave the current lobby.
        /// </summary>
        public void LeaveLobby()
        {
            if (CurrentLobby.HasValue)
            {
                Debug.Log($"[SteamLobbySystem] Leaving lobby {CurrentLobby.Value.Id}");
                CurrentLobby.Value.Leave();
                CurrentLobby = null;

                // Clear class selection state
                classSelections.Clear();
                readyStates.Clear();
                LocalPlayerSelectedClass = null;
                LocalPlayerReady = false;
            }
        }

        /// <summary>
        /// Open Steam overlay to invite friends to current lobby.
        /// </summary>
        public void OpenInviteDialog()
        {
            if (!IsInLobby)
            {
                Debug.LogWarning("[SteamLobbySystem] Cannot invite - not in a lobby!");
                return;
            }

            if (!IsLobbyOwner)
            {
                Debug.LogWarning("[SteamLobbySystem] Only the lobby owner can invite players!");
                return;
            }

            Debug.Log($"[SteamLobbySystem] Opening Steam invite overlay for lobby {CurrentLobby.Value.Id}...");
            SteamFriends.OpenGameInviteOverlay(CurrentLobby.Value.Id);
        }

        /// <summary>
        /// Get current lobby info for display.
        /// </summary>
        public string GetLobbyInfo()
        {
            if (!IsInLobby) return "Not in lobby";

            var lobby = CurrentLobby.Value;
            return $"{lobby.MemberCount}/{lobby.MaxMembers} players";
        }

        #endregion

        #region Class Selection

        /// <summary>
        /// Select a class for the local player.
        /// </summary>
        public void SelectClass(ClassData classData)
        {
            if (classData == null)
            {
                Debug.LogError("[SteamLobbySystem] Cannot select null class!");
                return;
            }

            LocalPlayerSelectedClass = classData;
            classSelections[SteamClient.SteamId] = classData;

            // Update Steam lobby data
            if (CurrentLobby.HasValue)
            {
                CurrentLobby.Value.SetMemberData("selectedClass", classData.className);
            }

            Debug.Log($"[SteamLobbySystem] Selected class: {classData.className}");
            OnPlayerClassChanged?.Invoke(SteamClient.SteamId, classData);
        }

        /// <summary>
        /// Toggle ready state for local player.
        /// </summary>
        public void SetReady(bool ready)
        {
            LocalPlayerReady = ready;
            readyStates[SteamClient.SteamId] = ready;

            // Update Steam lobby data
            if (CurrentLobby.HasValue)
            {
                CurrentLobby.Value.SetMemberData("ready", ready ? "1" : "0");
            }

            Debug.Log($"[SteamLobbySystem] Ready state: {ready}");
            OnPlayerReadyChanged?.Invoke(SteamClient.SteamId, ready);

            CheckAllPlayersReady();
        }

        /// <summary>
        /// Get class selection for a specific player.
        /// </summary>
        public ClassData GetPlayerClassSelection(ulong steamId)
        {
            return classSelections.TryGetValue(steamId, out ClassData classData) ? classData : defaultClass;
        }

        /// <summary>
        /// Get ready state for a specific player.
        /// </summary>
        public bool GetPlayerReadyState(ulong steamId)
        {
            return readyStates.TryGetValue(steamId, out bool ready) && ready;
        }

        /// <summary>
        /// Get all lobby members.
        /// </summary>
        public List<ulong> GetLobbyMembers()
        {
            List<ulong> members = new List<ulong>();

            if (CurrentLobby.HasValue)
            {
                foreach (var member in CurrentLobby.Value.Members)
                {
                    members.Add(member.Id);
                }
            }

            return members;
        }

        public string GetLobbyMemberName(ulong memberId)
        {
            return CurrentLobby.Value.Members.FirstOrDefault(m => m.Id == memberId).Name;
        }        

        /// <summary>
        /// Get connection data to send during Netcode connection approval.
        /// </summary>
        public byte[] GetConnectionData()
        {
            if (LocalPlayerSelectedClass == null)
            {
                Debug.LogWarning("[SteamLobbySystem] No class selected, using default class");
                LocalPlayerSelectedClass = defaultClass;
            }

            // Send class name as connection data
            // Server will look up ClassData from this name
            string className = LocalPlayerSelectedClass != null ? LocalPlayerSelectedClass.className : "Default";
            return System.Text.Encoding.UTF8.GetBytes(className);
        }

        /// <summary>
        /// Check if all players are ready and trigger connection.
        /// </summary>
        private void CheckAllPlayersReady()
        {
            if (!CurrentLobby.HasValue)
                return;

            bool allReady = true;
            int memberCount = 0;

            foreach (var member in CurrentLobby.Value.Members)
            {
                memberCount++;
                if (!GetPlayerReadyState(member.Id))
                {
                    allReady = false;
                    break;
                }
            }

            if (allReady && memberCount > 0)
            {
                Debug.Log("[SteamLobbySystem] All players ready! Triggering Netcode connection...");
                OnAllPlayersReady?.Invoke();
            }
        }

        #endregion

        #region Steam Callbacks

        private void OnLobbyCreatedCallback(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError($"[SteamLobbySystem] Lobby creation failed: {result}");
                return;
            }

            Debug.Log($"[SteamLobbySystem] === LOBBY CREATED CALLBACK ===");
            Debug.Log($"  Lobby ID: {lobby.Id}");
            Debug.Log($"  Owner: {lobby.Owner.Name}");

            CurrentLobby = lobby;
            OnLobbyCreated?.Invoke(lobby);
        }

        private void OnLobbyEnteredCallback(Lobby lobby)
        {
            Debug.Log($"[SteamLobbySystem] === ENTERED LOBBY ===");
            Debug.Log($"  Lobby ID: {lobby.Id}");
            Debug.Log($"  Owner: {lobby.Owner.Name} ({lobby.Owner.Id})");
            Debug.Log($"  Members: {lobby.MemberCount}/{lobby.MaxMembers}");

            CurrentLobby = lobby;
            OnLobbyEntered?.Invoke(lobby);

            // Initialize class selections for all current members
            foreach (var member in lobby.Members)
            {
                UpdateMemberData(member);
            }

            // If we're not the owner, request to join the host
            if (!IsLobbyOwner)
            {
                ulong hostSteamId = lobby.Owner.Id;
                Debug.Log($"[SteamLobbySystem] Requesting connection to host: {hostSteamId}");
                OnJoinRequested?.Invoke(hostSteamId);
            }
        }

        private void OnLobbyMemberJoinedCallback(Lobby lobby, Friend friend)
        {
            Debug.Log($"[SteamLobbySystem] Player joined lobby: {friend.Name}");
            readyStates[friend.Id] = false;
        }

        private void OnLobbyMemberLeaveCallback(Lobby lobby, Friend friend)
        {
            Debug.Log($"[SteamLobbySystem] Player left lobby: {friend.Name}");
            classSelections.Remove(friend.Id);
            readyStates.Remove(friend.Id);
            CheckAllPlayersReady();
        }

        private void OnLobbyMemberDataChanged(Lobby lobby, Friend friend)
        {
            UpdateMemberData(friend);
            CheckAllPlayersReady();
        }

        private void OnLobbyGameCreatedCallback(Lobby lobby, uint ip, ushort port, SteamId steamId)
        {
            Debug.Log($"[SteamLobbySystem] Lobby game created - connecting to {steamId}");
        }

        private void OnGameLobbyJoinRequestedCallback(Lobby lobby, SteamId steamId)
        {
            Debug.Log($"[SteamLobbySystem] === LOBBY JOIN REQUESTED (via Steam invite) ===");
            Debug.Log($"  Lobby: {lobby.Id}");
            Debug.Log($"  Invited by: {steamId}");

            // Automatically join the lobby when user accepts invite
            JoinLobby(lobby.Id);
        }

        private void UpdateMemberData(Friend member)
        {
            // Update class selection
            string selectedClassName = CurrentLobby?.GetMemberData(member, "selectedClass");
            if (!string.IsNullOrEmpty(selectedClassName))
            {
                ClassData classData = availableClasses.Find(c => c.className == selectedClassName);
                if (classData != null)
                {
                    classSelections[member.Id] = classData;
                    OnPlayerClassChanged?.Invoke(member.Id, classData);
                }
            }

            // Update ready state
            string readyState = CurrentLobby?.GetMemberData(member, "ready");
            bool isReady = readyState == "1";
            readyStates[member.Id] = isReady;
            OnPlayerReadyChanged?.Invoke(member.Id, isReady);
        }

        #endregion
    }
}
