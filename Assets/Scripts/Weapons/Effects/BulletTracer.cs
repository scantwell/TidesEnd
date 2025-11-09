using UnityEngine;
using System.Collections;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Visual tracer effect for bullets. Can operate in two modes:
    /// 1. Standalone: Moves independently from start to target
    /// 2. Attached: Follows a parent projectile (set as child)
    /// </summary>
    public class BulletTracer : MonoBehaviour
    {
        [Header("Tracer Settings")]
    [SerializeField] private float speed = 300f; // Units per second
    [SerializeField] private float maxLifetime = 2f; // Max time before auto-destroy

    [Header("Visual")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Light tracerLight;
    [SerializeField] private float lightIntensity = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float distanceToTravel;
    private float travelledDistance;
    private bool isActive;
    private bool isAttachedToProjectile;
    
    private void Awake()
    {
        if (trail == null)
            trail = GetComponent<TrailRenderer>();
        
        if (tracerLight == null)
            tracerLight = GetComponent<Light>();
    }
    
    /// <summary>
    /// Initialize tracer for standalone movement from start to direction
    /// </summary>
    public void Initialize(Vector3 start, Vector3 direction, float range)
    {
        startPosition = start;
        targetPosition = start + (direction.normalized * range);
        distanceToTravel = Vector3.Distance(startPosition, targetPosition);
        travelledDistance = 0f;
        isActive = true;
        isAttachedToProjectile = false;

        transform.position = startPosition;
        transform.rotation = Quaternion.LookRotation(direction);

        ResetVisuals();
        StartCoroutine(MoveTracer());
    }

    /// <summary>
    /// Initialize tracer for standalone movement from start to specific hit point
    /// </summary>
    public void InitializeWithHit(Vector3 start, Vector3 hitPoint)
    {
        startPosition = start;
        targetPosition = hitPoint;
        distanceToTravel = Vector3.Distance(startPosition, targetPosition);
        travelledDistance = 0f;
        isActive = true;
        isAttachedToProjectile = false;

        transform.position = startPosition;
        transform.rotation = Quaternion.LookRotation(targetPosition - startPosition);

        ResetVisuals();
        StartCoroutine(MoveTracer());
    }

    /// <summary>
    /// Initialize tracer as child of projectile (no movement needed, follows parent)
    /// </summary>
    public void InitializeAsChild()
    {
        isActive = true;
        isAttachedToProjectile = true;

        ResetVisuals();

        // No movement coroutine needed - follows parent transform
        // Will be stopped when projectile returns to pool
    }

    private void ResetVisuals()
    {
        // Reset trail
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }

        // Enable light
        if (tracerLight != null)
        {
            tracerLight.enabled = true;
            tracerLight.intensity = lightIntensity;
        }
    }
    
    private IEnumerator MoveTracer()
    {
        float startTime = Time.time;

        while (isActive && travelledDistance < distanceToTravel)
        {
            // Check if exceeded max lifetime
            if (Time.time - startTime > maxLifetime)
            {
                ReturnToPool();
                yield break;
            }

            // Only move if not attached to projectile
            if (!isAttachedToProjectile)
            {
                // Move tracer
                float moveDistance = speed * Time.deltaTime;
                travelledDistance += moveDistance;

                float t = Mathf.Clamp01(travelledDistance / distanceToTravel);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            }

            yield return null;
        }

        // Reached destination
        if (trail != null)
        {
            trail.emitting = false;
        }

        if (tracerLight != null)
        {
            tracerLight.enabled = false;
        }

        // Wait for trail to fade
        yield return new WaitForSeconds(trail != null ? trail.time : 0.5f);

        ReturnToPool();
    }
    
    private void ReturnToPool()
    {
        isActive = false;
        
        // Return to pool or destroy
        TracerPool pool = FindAnyObjectByType<TracerPool>();
        if (pool != null)
        {
            pool.ReturnTracer(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    public void ForceStop()
    {
        StopAllCoroutines();
        ReturnToPool();
    }
    }
}