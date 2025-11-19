using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System;

/// <summary>
/// Manages Steam lobby creation, joining, and invite functionality.
/// Works with NetworkBootstrap to handle Steam P2P connections.
/// </summary>
public class SteamLobbyManager : MonoBehaviour
{
    public static SteamLobbyManager Instance { get; private set; }

    // Events
    public static event Action<Lobby> OnLobbyCreated;
    public static event Action<Lobby> OnLobbyEntered;
    public static event Action<ulong> OnJoinRequested; // Sends host Steam ID

    public Lobby? CurrentLobby { get; private set; }
    public bool IsInLobby => CurrentLobby.HasValue;
    public bool IsLobbyOwner => IsInLobby && CurrentLobby.Value.Owner.Id == SteamClient.SteamId;

    [Header("Lobby Settings")]
    [SerializeField] private int maxPlayers = 4;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        // Subscribe to Steam callbacks
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEnteredCallback;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeaveCallback;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreatedCallback;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequestedCallback;
    }

    void OnDisable()
    {
        // Unsubscribe from Steam callbacks
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreatedCallback;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEnteredCallback;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoinedCallback;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeaveCallback;
        SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreatedCallback;
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequestedCallback;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            LeaveLobby();
            Instance = null;
        }
    }

    /// <summary>
    /// Create a new Steam lobby (called when hosting)
    /// </summary>
    public async void CreateLobby()
    {
        if (!SteamClient.IsValid)
        {
            Debug.LogError("Cannot create lobby - Steam not initialized!");
            return;
        }

        if (IsInLobby)
        {
            Debug.LogWarning("Already in a lobby. Leaving current lobby first.");
            LeaveLobby();
        }

        Debug.Log($"Creating Steam lobby with max {maxPlayers} players...");

        try
        {
            var createLobbyOutput = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);

            if (!createLobbyOutput.HasValue)
            {
                Debug.LogError("Failed to create lobby!");
                return;
            }

            CurrentLobby = createLobbyOutput.Value;

            // Set lobby data
            CurrentLobby.Value.SetPublic();
            CurrentLobby.Value.SetJoinable(true);
            CurrentLobby.Value.SetData("name", $"{SteamClient.Name}'s Game");
            CurrentLobby.Value.SetData("gameVersion", Application.version);

            Debug.Log($"✓ Lobby created! Lobby ID: {CurrentLobby.Value.Id}");
            Debug.Log($"  Owner: {CurrentLobby.Value.Owner.Name} ({CurrentLobby.Value.Owner.Id})");
            Debug.Log($"  Max Players: {CurrentLobby.Value.MaxMembers}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception creating lobby: {e.Message}");
        }
    }

    /// <summary>
    /// Join a Steam lobby by ID
    /// </summary>
    public async void JoinLobby(SteamId lobbyId)
    {
        if (!SteamClient.IsValid)
        {
            Debug.LogError("Cannot join lobby - Steam not initialized!");
            return;
        }

        Debug.Log($"Joining lobby: {lobbyId}...");

        try
        {
            Lobby? enterLobbyOutput = await SteamMatchmaking.JoinLobbyAsync(lobbyId);

            if (enterLobbyOutput == null)
            {
                Debug.LogError($"Failed to join lobby! Reason: {enterLobbyOutput}");
                return;
            }

            Debug.Log($"✓ Successfully joined lobby {lobbyId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception joining lobby: {e.Message}");
        }
    }

    /// <summary>
    /// Leave the current lobby
    /// </summary>
    public void LeaveLobby()
    {
        if (CurrentLobby.HasValue)
        {
            Debug.Log($"Leaving lobby {CurrentLobby.Value.Id}");
            CurrentLobby.Value.Leave();
            CurrentLobby = null;
        }
    }

    /// <summary>
    /// Open Steam overlay to invite friends to current lobby
    /// </summary>
    public void OpenInviteDialog()
    {
        Debug.Log($"=== OpenInviteDialog Called ===");
        Debug.Log($"  IsInLobby: {IsInLobby}");
        Debug.Log($"  CurrentLobby.HasValue: {CurrentLobby.HasValue}");

        if (!IsInLobby)
        {
            Debug.LogWarning("Cannot invite - not in a lobby!");
            Debug.LogWarning("Make sure you clicked 'Host' first and lobby was created successfully.");
            return;
        }

        Debug.Log($"  IsLobbyOwner: {IsLobbyOwner}");
        Debug.Log($"  Current SteamId: {SteamClient.SteamId}");
        Debug.Log($"  Lobby Owner: {CurrentLobby.Value.Owner.Id}");

        if (!IsLobbyOwner)
        {
            Debug.LogWarning("Only the lobby owner can invite players!");
            return;
        }

        Debug.Log($"Opening Steam invite overlay for lobby {CurrentLobby.Value.Id}...");
        Debug.Log("If overlay doesn't appear, check:");
        Debug.Log("  1. Steam Settings > In-Game > 'Enable Steam Overlay' is checked");
        Debug.Log("  2. Game is NOT in exclusive fullscreen mode");
        Debug.Log("  3. Try pressing Shift+Tab to manually open overlay");

        SteamFriends.OpenGameInviteOverlay(CurrentLobby.Value.Id);
    }

    #region Steam Callbacks

    private void OnLobbyCreatedCallback(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.LogError($"Lobby creation failed: {result}");
            return;
        }

        Debug.Log($"=== LOBBY CREATED CALLBACK ===");
        Debug.Log($"Lobby ID: {lobby.Id}");
        Debug.Log($"Owner: {lobby.Owner.Name}");
        Debug.Log($"==============================");

        CurrentLobby = lobby;
        OnLobbyCreated?.Invoke(lobby);
    }

    private void OnLobbyEnteredCallback(Lobby lobby)
    {
        Debug.Log($"=== ENTERED LOBBY ===");
        Debug.Log($"Lobby ID: {lobby.Id}");
        Debug.Log($"Owner: {lobby.Owner.Name} ({lobby.Owner.Id})");
        Debug.Log($"Members: {lobby.MemberCount}/{lobby.MaxMembers}");
        Debug.Log($"=====================");

        CurrentLobby = lobby;
        OnLobbyEntered?.Invoke(lobby);

        // If we're not the owner, request to join the host
        if (!IsLobbyOwner)
        {
            ulong hostSteamId = lobby.Owner.Id;
            Debug.Log($"Requesting connection to host: {hostSteamId}");
            OnJoinRequested?.Invoke(hostSteamId);
        }
    }

    private void OnLobbyMemberJoinedCallback(Lobby lobby, Friend friend)
    {
        Debug.Log($"Player joined lobby: {friend.Name}");
    }

    private void OnLobbyMemberLeaveCallback(Lobby lobby, Friend friend)
    {
        Debug.Log($"Player left lobby: {friend.Name}");
    }

    private void OnLobbyGameCreatedCallback(Lobby lobby, uint ip, ushort port, SteamId steamId)
    {
        Debug.Log($"Lobby game created - connecting to {steamId}");
    }

    private void OnGameLobbyJoinRequestedCallback(Lobby lobby, SteamId steamId)
    {
        Debug.Log($"=== LOBBY JOIN REQUESTED (via Steam invite) ===");
        Debug.Log($"Lobby: {lobby.Id}");
        Debug.Log($"Invited by: {steamId}");
        Debug.Log($"===============================================");

        // Automatically join the lobby when user accepts invite
        JoinLobby(lobby.Id);
    }

    #endregion

    /// <summary>
    /// Get current lobby info for display
    /// </summary>
    public string GetLobbyInfo()
    {
        if (!IsInLobby) return "Not in lobby";

        var lobby = CurrentLobby.Value;
        return $"{lobby.MemberCount}/{lobby.MaxMembers} players";
    }
}
