using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraSetup : NetworkBehaviour
{
    [Header("Virtual Camera")]
    [SerializeField] private CinemachineCamera virtualCamera;
    
    [Header("Camera Target")]
    [SerializeField] private Transform cameraTarget;
    
    [Header("Priority")]
    [SerializeField] private int ownerPriority = 10;
    [SerializeField] private int nonOwnerPriority = 0;
    
    private void Awake()
    {
        // Auto-find if not assigned
        if (virtualCamera == null)
        {
            virtualCamera = GetComponentInChildren<CinemachineCamera>();
        }
    }
    
    public override void OnNetworkSpawn()
    {
        ConfigureCamera();
    }
    
    private void ConfigureCamera()
    {
        if (IsOwner)
        {
            // Owner: Camera follows this player
            virtualCamera.Priority.Value = ownerPriority;
            virtualCamera.enabled = true;
            
            // Lock cursor for FPS
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            int cullingMask = Camera.main.cullingMask;
        
            // Hide own world weapon
            int firstPersonHideLayer = LayerMask.NameToLayer("FirstPersonHide");
            cullingMask &= ~(1 << firstPersonHideLayer);
            
            Camera.main.cullingMask = cullingMask;
            Debug.Log($"[Player {OwnerClientId}] Virtual camera enabled (Owner)");
        }
        else
        {
            // Non-owner: Camera ignores this player
            virtualCamera.Priority.Value = nonOwnerPriority;
            virtualCamera.enabled = false;
            
            Debug.Log($"[Player {OwnerClientId}] Virtual camera disabled (Non-owner)");
        }
    }
    
    public Transform CameraTarget => cameraTarget;
}