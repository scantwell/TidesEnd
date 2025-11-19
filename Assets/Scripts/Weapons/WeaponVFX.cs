using UnityEngine;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Handles weapon visual effects (muzzle flash, shell ejection, etc.)
    /// </summary>
    public class WeaponVFX : MonoBehaviour
    {
        [Header("VFX References")]
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private Transform shellEjectionPoint;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject shellCasingPrefab;
        [SerializeField] private GameObject bulletTracerPrefab;
        
        [Header("Settings")]
        [SerializeField] private float shellEjectionForce = 3f;
        
        public void PlayMuzzleFlash()
        {
            if (muzzleFlash == null) return;
            muzzleFlash?.Play();
        }
        
        public void EjectShell()
        {
            if (shellCasingPrefab == null || shellEjectionPoint == null) return;
            
            // Instantiate shell casing
            GameObject shell = Instantiate(shellCasingPrefab, shellEjectionPoint.position, shellEjectionPoint.rotation);
            
            // Add force to shell
            Rigidbody rb = shell.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(shellEjectionPoint.right * shellEjectionForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * shellEjectionForce);
            }
            
            // Destroy shell after a few seconds
            Destroy(shell, 5f);
        }
        
        public void SpawnBulletTracer(Vector3 hitPoint)
        {
            if (bulletTracerPrefab == null || muzzlePoint == null) return;
            
            // Instantiate tracer from muzzle to hit point
            GameObject tracer = Instantiate(bulletTracerPrefab, muzzlePoint.position, Quaternion.identity);
            
            // Point tracer toward hit point
            tracer.transform.LookAt(hitPoint);
            
            // Destroy after brief duration
            Destroy(tracer, 0.1f);
        }
    }
}