using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [SerializeField] private SpawnPointType spawnType = SpawnPointType.Enemy;
    [SerializeField] private bool oneTimeUse = false;
    [SerializeField] private Color gizmoColor = Color.green;
    
    private bool hasBeenUsed = false;
    
    public SpawnPointType Type => spawnType;
    public bool IsAvailable => !oneTimeUse || !hasBeenUsed;
    
    public Vector3 GetSpawnPosition()
    {
        hasBeenUsed = true;
        return transform.position;
    }
    
    public Quaternion GetSpawnRotation()
    {
        return transform.rotation;
    }
    
    public void Reset()
    {
        hasBeenUsed = false;
    }
    
    // Visualize in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw arrow showing forward direction
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}

public enum SpawnPointType
{
    Player,
    Enemy,
    Boss,
    Item
}