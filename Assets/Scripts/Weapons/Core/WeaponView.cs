using UnityEngine;

namespace TidesEnd.Weapons
{
    public class WeaponView : MonoBehaviour
    {
    [Header("Effect Points")]
    [SerializeField] private Transform muzzlePoint;

    [Header("Components")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;

    private WeaponData data;
    private ParticleSystem muzzleFlashInstance;

    private void Awake()
    {
        // Auto-setup audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f; // 3D sound
                audioSource.maxDistance = 50f;
                audioSource.playOnAwake = false;
            }
        }
    }
    
    public void Initialize(WeaponData weaponData)
    {
        data = weaponData;
        
        // Instantiate muzzle flash if we have a prefab
        if (data.muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(data.muzzleFlashPrefab, muzzlePoint);
            flash.transform.localPosition = Vector3.zero;
            flash.transform.localRotation = Quaternion.identity;
            
            muzzleFlashInstance = flash.GetComponent<ParticleSystem>();
            if (muzzleFlashInstance != null)
            {
                muzzleFlashInstance.Stop();
            }
        }
    }
    
    public void PlayFireEffects()
    {
        // Muzzle flash
        if (muzzleFlashInstance != null)
        {
            muzzleFlashInstance.Play();
        }
        
        // Fire sound
        if (audioSource != null && data != null && data.fireSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // Slight variation
            audioSource.PlayOneShot(data.fireSound, data.volume);
        }
        
        // Animation
        if (animator != null)
        {
            animator.SetTrigger("Fire");
        }
    }
    
    public void PlayReloadEffects()
    {
        // Reload sound
        if (audioSource != null && data != null && data.reloadSound != null)
        {
            audioSource.PlayOneShot(data.reloadSound, data.volume);
        }
        
        // Animation
        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }
    }
    
    public void PlayEmptySound()
    {
        if (audioSource != null && data != null && data.emptySound != null)
        {
            audioSource.PlayOneShot(data.emptySound, data.volume * 0.5f);
        }
    }
    }
}