using UnityEngine;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Handles weapon sound effects
    /// </summary>
    public class WeaponAudio : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip fireSound;
        [SerializeField] private AudioClip reloadSound;
        [SerializeField] private AudioClip emptySound;
        [SerializeField] private AudioClip equipSound;
        
        [Header("Settings")]
        [SerializeField] private float volume = 1f;
        
        private AudioSource audioSource;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            audioSource.volume = volume;
            audioSource.spatialBlend = 0f; // 2D sound (for view model)
        }
        
        public void PlayFireSound()
        {
            audioSource.PlayOneShot(fireSound);
        }
        
        public void PlayReloadSound()
        {
            audioSource.PlayOneShot(reloadSound);
        }
        
        public void PlayEmptySound()
        {
            audioSource.PlayOneShot(emptySound);
        }
        
        public void PlayEquipSound()
        {
            audioSource.PlayOneShot(equipSound);
        }
    }
}