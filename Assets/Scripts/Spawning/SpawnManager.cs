using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

// Just MonoBehaviour - NOT NetworkBehaviour
public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Spawning")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private int initialEnemyCount = 5;
    
    private List<SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();
    private List<SpawnPoint> enemySpawnPoints = new List<SpawnPoint>();
    
    private int nextPlayerSpawnIndex = 0;
    
    private static SpawnManager instance;
    public static SpawnManager Instance => instance;

    private bool IsServer => NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        FindSpawnPoints();
    }
    
    private void Start()
    {
        // Subscribe to connection events only if we have NetworkManager
        if (NetworkManager.Singleton != null)
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
        else
        {
            Debug.LogWarning("NetworkManager not found - SpawnManager won't function");
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe
        if (NetworkManager.Singleton != null && IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
    
    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;
        
        Debug.Log($"Client {clientId} connected, will position player at spawn point");

        StartCoroutine(PositionPlayerAfterSpawn(clientId));
        Invoke(nameof(SpawnInitialEnemies), 2f);
    }
    
    private System.Collections.IEnumerator PositionPlayerAfterSpawn(ulong clientId)
    {
        yield return new WaitForSeconds(0.1f);
        
        NetworkObject playerObject = null;
        
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key == clientId && client.Value.PlayerObject != null)
            {
                playerObject = client.Value.PlayerObject;
                break;
            }
        }
        
        if (playerObject != null)
        {
            Vector3 spawnPos = GetPlayerSpawnPosition(nextPlayerSpawnIndex);
            Quaternion spawnRot = GetPlayerSpawnRotation(nextPlayerSpawnIndex);
            
            playerObject.transform.position = spawnPos;
            playerObject.transform.rotation = spawnRot;
            
            Debug.Log($"Positioned player {clientId} at spawn point {nextPlayerSpawnIndex}: {spawnPos}");
            
            nextPlayerSpawnIndex++;
        }
    }
    
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
}