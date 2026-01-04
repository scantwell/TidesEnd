using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using TidesEnd.UI;
using TidesEnd.Abilities;

namespace TidesEnd
{
    /// <summary>
    /// Unified spawn manager for players and enemies.
    /// CONSOLIDATES: PlayerSpawnManager (class selection + player spawning) + SpawnManager (enemy spawning)
    ///
    /// Responsibilities:
    /// 1. Player spawning with class selection from lobby
    /// 2. Enemy spawning (waves, individual spawns)
    /// 3. Spawn point management using SpawnPoint components
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance { get; private set; }

        [Header("Player Spawning")]
        [SerializeField] private GameObject playerPrefab;

        [Tooltip("Available classes (must match SteamLobbySystem)")]
        public List<ClassData> availableClasses = new List<ClassData>();

        [Tooltip("Default class if none selected or invalid")]
        public ClassData defaultClass;

        [Header("Enemy Spawning")]
        [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
        [SerializeField] private int initialEnemyCount = 5;

        // Spawn points
        private List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();
        private List<SpawnPoint> enemySpawnPoints = new List<SpawnPoint>();

        // Player spawn tracking
        private int nextPlayerSpawnIndex = 0;
        private Dictionary<ulong, ClassData> playerClassSelections = new Dictionary<ulong, ClassData>();
        private Dictionary<ulong, GameObject> spawnedPlayers = new Dictionary<ulong, GameObject>();

    private bool IsServer => NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;

    void OnEnable()
    {
        PauseMenu.OnSpawnEnemyRequested += SpawnInitialEnemies;
    }
    
    void OnDisable()
    {
        PauseMenu.OnSpawnEnemyRequested -= SpawnInitialEnemies;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        FindSpawnPoints();
    }
    
    #region Player Spawning (Class Selection System)

    /// <summary>
    /// Store class selection for a client (called during connection approval by GameBootstrap).
    /// </summary>
    public void RegisterClassSelection(ulong clientId, string className)
    {
        if (!IsServer)
        {
            Debug.LogError("[SpawnManager] RegisterClassSelection can only be called on server!");
            return;
        }

        ClassData selectedClass = availableClasses.Find(c => c.className == className);

        if (selectedClass == null)
        {
            Debug.LogWarning($"[SpawnManager] Invalid class '{className}' for client {clientId}, using default");
            selectedClass = defaultClass;
        }

        playerClassSelections[clientId] = selectedClass;
        Debug.Log($"[SpawnManager] Registered class '{selectedClass.className}' for client {clientId}");
    }

    /// <summary>
    /// Spawn a player for the given client ID with their selected class.
    /// Called by GameBootstrap after connection approval.
    /// </summary>
    public void SpawnPlayer(ulong clientId)
    {
        if (!IsServer)
        {
            Debug.LogError("[SpawnManager] SpawnPlayer can only be called on server!");
            return;
        }

        // Check if player already spawned
        if (spawnedPlayers.ContainsKey(clientId))
        {
            Debug.LogWarning($"[SpawnManager] Player {clientId} already spawned!");
            return;
        }

        // Get class selection
        if (!playerClassSelections.TryGetValue(clientId, out ClassData selectedClass))
        {
            Debug.LogWarning($"[SpawnManager] No class selection for client {clientId}, using default");
            selectedClass = defaultClass;
        }

        // Get spawn position/rotation
        Vector3 spawnPos = GetPlayerSpawnPosition(nextPlayerSpawnIndex);
        Quaternion spawnRot = GetPlayerSpawnRotation(nextPlayerSpawnIndex);

        // Instantiate player prefab
        GameObject playerObject = Instantiate(playerPrefab, spawnPos, spawnRot);
        NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError("[SpawnManager] Player prefab missing NetworkObject component!");
            Destroy(playerObject);
            return;
        }

        // Spawn as player object (assigns ownership to client)
        networkObject.SpawnAsPlayerObject(clientId);

        // Initialize EntityStats with selected class
        if (playerObject.TryGetComponent<EntityStats>(out var entityStats))
        {
            entityStats.InitializeFromClass(selectedClass);
            Debug.Log($"[SpawnManager] Initialized player {clientId} with class '{selectedClass.className}'");
        }
        else
        {
            Debug.LogError("[SpawnManager] Player prefab missing EntityStats component!");
        }

        // Initialize AbilityUser with abilities from class
        if (playerObject.TryGetComponent<AbilityUser>(out var abilityUser))
        {
            abilityUser.LoadAbilities(selectedClass.abilities);
            Debug.Log($"[SpawnManager] Loaded {selectedClass.abilities.Count} abilities for player {clientId}");
        }

        // Track spawned player
        spawnedPlayers[clientId] = playerObject;
        nextPlayerSpawnIndex++;

        Debug.Log($"[SpawnManager] Spawned player {clientId} at {spawnPos} with class '{selectedClass.className}'");
    }

    /// <summary>
    /// Handle player disconnection (cleanup).
    /// </summary>
    public void OnPlayerDisconnected(ulong clientId)
    {
        if (!IsServer)
            return;

        playerClassSelections.Remove(clientId);

        if (spawnedPlayers.TryGetValue(clientId, out GameObject playerObject))
        {
            if (playerObject != null)
            {
                // NetworkObject despawn is handled automatically by Netcode
                // Just clean up our tracking
            }
            spawnedPlayers.Remove(clientId);
        }

        Debug.Log($"[SpawnManager] Cleaned up player {clientId}");
    }

    /// <summary>
    /// Get the class selection for a client.
    /// </summary>
    public ClassData GetPlayerClass(ulong clientId)
    {
        return playerClassSelections.TryGetValue(clientId, out ClassData classData) ? classData : defaultClass;
    }

    #endregion

    #region Spawn Point Management

    private void FindSpawnPoints()
    {
        SpawnPoint[] allSpawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        
        foreach (SpawnPoint point in allSpawnPoints)
        {
            switch (point.Type)
            {
                case SpawnPointType.Player:
                    playerSpawnPoints.Add(point);
                    break;
                case SpawnPointType.Enemy:
                case SpawnPointType.Boss:
                    enemySpawnPoints.Add(point);
                    break;
            }
        }
        
        Debug.Log($"Found {playerSpawnPoints.Count} player spawn points, {enemySpawnPoints.Count} enemy spawn points");
    }
    
    public Vector3 GetPlayerSpawnPosition(int playerIndex)
    {
        if (playerSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No player spawn points found!");
            return new Vector3(0, 1, 0);
        }
        
        int spawnIndex = playerIndex % playerSpawnPoints.Count;
        return playerSpawnPoints[spawnIndex].GetSpawnPosition();
    }
    
    public Quaternion GetPlayerSpawnRotation(int playerIndex)
    {
        if (playerSpawnPoints.Count == 0)
        {
            return Quaternion.identity;
        }
        
        int spawnIndex = playerIndex % playerSpawnPoints.Count;
        return playerSpawnPoints[spawnIndex].GetSpawnRotation();
    }

    #endregion

    #region Enemy Spawning

    public void SpawnInitialEnemies()
    {
        if (!IsServer)
        {
            Debug.LogWarning("SpawnInitialEnemies called on client - ignoring");
            return;
        }
        
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("No enemy prefabs assigned!");
            return;
        }
        
        if (enemySpawnPoints.Count == 0)
        {
            Debug.LogError("No enemy spawn points found!");
            return;
        }
        
        for (int i = 0; i < initialEnemyCount; i++)
        {
            SpawnEnemy();
        }
        
        Debug.Log($"Spawned {initialEnemyCount} enemies");
    }
    
    public void SpawnEnemy()
    {
        if (!IsServer) return;
        
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        SpawnPoint spawnPoint = GetRandomEnemySpawnPoint();
        
        if (spawnPoint == null)
        {
            Debug.LogWarning("No available enemy spawn points!");
            return;
        }
        
        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPoint.GetSpawnPosition(),
            spawnPoint.GetSpawnRotation()
        );
        
        NetworkObject netObj = enemy.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        else
        {
            Debug.LogError($"Enemy prefab missing NetworkObject!");
        }
    }
    
    public void SpawnWave(int enemyCount)
    {
        if (!IsServer) return;
        
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }
        
        Debug.Log($"Spawned wave of {enemyCount} enemies");
    }
    
    private SpawnPoint GetRandomEnemySpawnPoint()
    {
        List<SpawnPoint> availablePoints = enemySpawnPoints.Where(p => p.IsAvailable).ToList();
        
        if (availablePoints.Count == 0)
        {
            availablePoints = enemySpawnPoints;
        }
        
        if (availablePoints.Count == 0)
        {
            return null;
        }
        
        return availablePoints[Random.Range(0, availablePoints.Count)];
    }

    #endregion
    }
}