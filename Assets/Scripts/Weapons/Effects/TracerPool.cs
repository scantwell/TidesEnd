using UnityEngine;
using System.Collections.Generic;

namespace TidesEnd.Weapons
{
    public class TracerPool : MonoBehaviour
    {
    [Header("Pool Settings")]
    [SerializeField] private GameObject tracerPrefab;
    [SerializeField] private int poolSize = 50;
    [SerializeField] private bool expandPool = true;
    
    private Queue<BulletTracer> pool = new Queue<BulletTracer>();
    private List<BulletTracer> activeTracers = new List<BulletTracer>();
    
    private static TracerPool instance;
    public static TracerPool Instance => instance;
    
    private void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        InitializePool();
    }
    
    private void InitializePool()
    {
        if (tracerPrefab == null)
        {
            Debug.LogError("Tracer prefab not assigned to TracerPool!");
            return;
        }
        
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewTracer();
        }
        
        Debug.Log($"TracerPool initialized with {poolSize} tracers");
    }
    
    private BulletTracer CreateNewTracer()
    {
        GameObject obj = Instantiate(tracerPrefab, transform);
        obj.SetActive(false);
        
        BulletTracer tracer = obj.GetComponent<BulletTracer>();
        if (tracer == null)
        {
            Debug.LogError("Tracer prefab doesn't have BulletTracer component!");
            Destroy(obj);
            return null;
        }
        
        pool.Enqueue(tracer);
        return tracer;
    }
    
    public BulletTracer GetTracer()
    {
        BulletTracer tracer;
        
        // Try to get from pool
        if (pool.Count > 0)
        {
            tracer = pool.Dequeue();
        }
        else if (expandPool)
        {
            // Pool empty, create new
            Debug.LogWarning("TracerPool exhausted, creating new tracer");
            tracer = CreateNewTracer();
        }
        else
        {
            Debug.LogWarning("TracerPool exhausted and expansion disabled");
            return null;
        }
        
        if (tracer != null)
        {
            tracer.gameObject.SetActive(true);
            activeTracers.Add(tracer);
        }
        
        return tracer;
    }
    
    public void ReturnTracer(BulletTracer tracer)
    {
        if (tracer == null) return;
        
        tracer.gameObject.SetActive(false);
        activeTracers.Remove(tracer);
        pool.Enqueue(tracer);
    }
    
    public void ClearAllTracers()
    {
        foreach (BulletTracer tracer in activeTracers.ToArray())
        {
            tracer.ForceStop();
        }
        
        activeTracers.Clear();
    }
    
    public int GetActiveCount()
    {
        return activeTracers.Count;
    }
    
    public int GetPooledCount()
    {
        return pool.Count;
    }
    }
}