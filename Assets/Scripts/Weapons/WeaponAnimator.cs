using UnityEngine;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Handles weapon animations (reload, fire, etc.)
    /// </summary>
    public class WeaponAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }
        
        public void PlayFireAnimation()
        {
            animator?.SetTrigger("Fire");
        }
        
        public void PlayReloadAnimation()
        {
            animator?.SetTrigger("Reload");
        }
        
        public void PlayInspectAnimation()
        {
            animator?.SetTrigger("Inspect");
        }
        
        public void SetAimingState(bool isAiming)
        {
            animator?.SetBool("IsAiming", isAiming);
        }
        
        /// <summary>
        /// Called by animation event when reload completes
        /// </summary>
        public void OnReloadComplete()
        {
            // Notify WeaponBase that reload finished
        }
    }
}