using UnityEngine;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Rotates the weapon root to match the player's camera aim direction.
    /// This ensures weapon visuals and projectile spawn points match where the player is looking.
    /// </summary>
    public class WeaponRootController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTarget;

        [Header("Settings")]
        [SerializeField] private bool smoothRotation = true;
        [SerializeField] private float rotationSpeed = 20f;

        private Camera playerCamera;
        private Transform cachedTransform;

        private void Awake()
        {
            cachedTransform = transform;

            // Try to find camera target if not assigned
            if (cameraTarget == null)
            {
                // Look for PlayerCameraRoot as sibling
                Transform parent = transform.parent;
                if (parent != null)
                {
                    cameraTarget = parent.Find("PlayerCameraRoot")
                                ?? parent.Find("CameraTarget")
                                ?? parent.Find("Head");
                }
            }
        }

        private void Start()
        {
            // Get main camera (will be set when player spawns)
            playerCamera = Camera.main;
        }

        private void LateUpdate()
        {
            UpdateWeaponRotation();
        }

        private void UpdateWeaponRotation()
        {
            if (cameraTarget == null) return;

            // Get the player's body rotation (yaw only, around Y-axis)
            Transform playerBody = cachedTransform.parent;
            if (playerBody == null) return;

            // Get camera pitch from cameraTarget (X-axis rotation)
            // The cameraTarget has local rotation with pitch on X-axis
            float cameraPitch = cameraTarget.localEulerAngles.x;

            // Normalize pitch to -180 to 180 range
            if (cameraPitch > 180f)
                cameraPitch -= 360f;

            // Create local rotation: pitch on X-axis, no yaw (yaw comes from parent)
            // WeaponRoot should rotate locally to match camera pitch
            Quaternion targetLocalRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

            // Apply rotation (smooth or instant)
            if (smoothRotation)
            {
                cachedTransform.localRotation = Quaternion.Slerp(
                    cachedTransform.localRotation,
                    targetLocalRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            else
            {
                cachedTransform.localRotation = targetLocalRotation;
            }
        }

        /// <summary>
        /// Manually set the camera target reference (useful for networked players)
        /// </summary>
        public void SetCameraTarget(Transform target)
        {
            cameraTarget = target;
        }

        /// <summary>
        /// Manually set the player camera reference (useful for networked players)
        /// </summary>
        public void SetPlayerCamera(Camera camera)
        {
            playerCamera = camera;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            // Draw aim direction
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * 5f);
        }
#endif
    }
}
