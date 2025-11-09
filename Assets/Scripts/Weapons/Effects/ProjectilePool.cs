using UnityEngine;
using System.Collections.Generic;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Object pool for projectiles to avoid instantiation overhead.
    /// Supports multiple projectile types with separate pools.
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        [Header("Pool Configuration")]
    [SerializeField] private int defaultPoolSize = 100;
    [SerializeField] private bool expandPool = true;
    [SerializeField] private int maxPoolSize = 500;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    // Pools organized by prefab (allows different projectile types)
    private Dictionary<GameObject, Queue<Projectile>> pools = new Dictionary<GameObject, Queue<Projectile>>();
    private Dictionary<GameObject, List<Projectile>> activePools = new Dictionary<GameObject, List<Projectile>>();
    private Dictionary<GameObject, Transform> poolParents = new Dictionary<GameObject, Transform>();

    private static ProjectilePool instance;
    public static ProjectilePool Instance => instance;

    private void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Pre-warm a pool for a specific projectile prefab
    /// </summary>
    public void InitializePool(GameObject projectilePrefab, int size)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Cannot initialize pool with null prefab!");
            return;
        }

        if (pools.ContainsKey(projectilePrefab))
        {
            if (showDebugInfo)
                Debug.Log($"Pool for {projectilePrefab.name} already exists with {pools[projectilePrefab].Count} projectiles");
            return;
        }

        // Create pool structures
        pools[projectilePrefab] = new Queue<Projectile>();
        activePools[projectilePrefab] = new List<Projectile>();

        // Create parent transform for organization
        GameObject parent = new GameObject($"Pool_{projectilePrefab.name}");
        parent.transform.SetParent(transform);
        poolParents[projectilePrefab] = parent.transform;

        // Pre-spawn projectiles
        for (int i = 0; i < size; i++)
        {
            CreateProjectile(projectilePrefab);
        }

        if (showDebugInfo)
            Debug.Log($"Initialized pool for {projectilePrefab.name} with {size} projectiles");
    }

    /// <summary>
    /// Get a projectile from the pool
    /// </summary>
    public Projectile GetProjectile(GameObject projectilePrefab)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Cannot get projectile with null prefab!");
            return null;
        }

        // Initialize pool if it doesn't exist
        if (!pools.ContainsKey(projectilePrefab))
        {
            InitializePool(projectilePrefab, defaultPoolSize);
        }

        Projectile projectile;

        // Try to get from pool
        if (pools[projectilePrefab].Count > 0)
        {
            projectile = pools[projectilePrefab].Dequeue();
        }
        else if (expandPool && GetTotalProjectileCount() < maxPoolSize)
        {
            // Pool empty, create new
            if (showDebugInfo)
                Debug.LogWarning($"Pool for {projectilePrefab.name} exhausted, creating new projectile");

            projectile = CreateProjectile(projectilePrefab);
        }
        else
        {
            Debug.LogWarning($"Pool for {projectilePrefab.name} exhausted and expansion disabled/max reached!");
            return null;
        }

        if (projectile != null)
        {
            projectile.gameObject.SetActive(true);
            activePools[projectilePrefab].Add(projectile);
        }

        return projectile;
    }

    /// <summary>
    /// Return a projectile to the pool
    /// </summary>
    public void ReturnProjectile(Projectile projectile)
    {
        if (projectile == null) return;

        // Find which pool this belongs to
        GameObject prefabKey = null;
        foreach (var kvp in activePools)
        {
            if (kvp.Value.Contains(projectile))
            {
                prefabKey = kvp.Key;
                break;
            }
        }

        if (prefabKey == null)
        {
            // Not in any active pool, just destroy it
            if (showDebugInfo)
                Debug.LogWarning($"Projectile {projectile.name} not found in any active pool, destroying");
            Destroy(projectile.gameObject);
            return;
        }

        // Return to pool
        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(poolParents[prefabKey]);
        projectile.transform.localPosition = Vector3.zero;
        projectile.transform.localRotation = Quaternion.identity;

        activePools[prefabKey].Remove(projectile);
        pools[prefabKey].Enqueue(projectile);
    }

    /// <summary>
    /// Clear all active projectiles (e.g., on scene change)
    /// </summary>
    public void ClearAllProjectiles()
    {
        foreach (var activeList in activePools.Values)
        {
            foreach (Projectile projectile in activeList.ToArray())
            {
                projectile.ForceStop();
            }
        }

        if (showDebugInfo)
            Debug.Log("All active projectiles cleared");
    }

    /// <summary>
    /// Get statistics for a specific projectile type
    /// </summary>
    public (int pooled, int active) GetPoolStats(GameObject projectilePrefab)
    {
        if (projectilePrefab == null || !pools.ContainsKey(projectilePrefab))
            return (0, 0);

        return (pools[projectilePrefab].Count, activePools[projectilePrefab].Count);
    }

    /// <summary>
    /// Get total active projectile count across all pools
    /// </summary>
    public int GetTotalActiveCount()
    {
        int total = 0;
        foreach (var activeList in activePools.Values)
        {
            total += activeList.Count;
        }
        return total;
    }

    /// <summary>
    /// Get total projectile count (pooled + active)
    /// </summary>
    public int GetTotalProjectileCount()
    {
        int total = 0;
        foreach (var pool in pools.Values)
        {
            total += pool.Count;
        }
        foreach (var activeList in activePools.Values)
        {
            total += activeList.Count;
        }
        return total;
    }

    private Projectile CreateProjectile(GameObject projectilePrefab)
    {
        Transform parent = poolParents.ContainsKey(projectilePrefab) ? poolParents[projectilePrefab] : transform;
        GameObject obj = Instantiate(projectilePrefab, parent);
        obj.SetActive(false);

        Projectile projectile = obj.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError($"Projectile prefab {projectilePrefab.name} doesn't have Projectile component!");
            Destroy(obj);
            return null;
        }

        pools[projectilePrefab].Enqueue(projectile);
        return projectile;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 200, 300, 200));
        GUILayout.Label($"<b>Projectile Pool Stats</b>", new GUIStyle(GUI.skin.label) { richText = true });

        foreach (var kvp in pools)
        {
            string prefabName = kvp.Key.name;
            int pooled = kvp.Value.Count;
            int active = activePools[kvp.Key].Count;
            GUILayout.Label($"{prefabName}: {pooled} pooled, {active} active");
        }

        GUILayout.Label($"<b>Total: {GetTotalProjectileCount()}</b>", new GUIStyle(GUI.skin.label) { richText = true });
        GUILayout.EndArea();
    }
#endif
    }
}
