using UnityEngine;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Pure visual representation of a weapon
    /// Handles animations, audio, and VFX only - NO gameplay state
    /// Both view models and world models use this same component
    /// </summary>
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;

        private WeaponAnimator weaponAnimator;
        private WeaponAudio weaponAudio;
        private WeaponVFX weaponVFX;

        private void Awake()
        {
            weaponAnimator = GetComponent<WeaponAnimator>();
            weaponAudio = GetComponent<WeaponAudio>();
            weaponVFX = GetComponent<WeaponVFX>();
        }

        // Visual methods only - no gameplay logic
        public void PlayFireAnimation()
        {
            weaponAnimator?.PlayFireAnimation();
        }

        public void PlayReloadAnimation()
        {
            weaponAnimator?.PlayReloadAnimation();
        }

        public void PlayInspectAnimation()
        {
            weaponAnimator?.PlayInspectAnimation();
        }

        public void SetAimingState(bool isAiming)
        {
            weaponAnimator?.SetAimingState(isAiming);
        }

        public void PlayFireSound()
        {
            weaponAudio?.PlayFireSound();
        }

        public void PlayReloadSound()
        {
            weaponAudio?.PlayReloadSound();
        }

        public void PlayEmptySound()
        {
            weaponAudio?.PlayEmptySound();
        }

        public void PlayEquipSound()
        {
            weaponAudio?.PlayEquipSound();
        }

        public void PlayMuzzleFlash()
        {
            weaponVFX?.PlayMuzzleFlash();
        }

        public void EjectShell()
        {
            weaponVFX?.EjectShell();
        }

        public void SpawnBulletTracer(Vector3 hitPoint)
        {
            weaponVFX?.SpawnBulletTracer(hitPoint);
        }

        // Read-only data access
        public WeaponData Data => weaponData;
    }
}
