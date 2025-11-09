using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using TidesEnd.Weapons;

public class NetworkedFPSController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -15f;
    
    [Header("Look")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;
    
    [Header("References")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private WeaponRootController weaponRootController;

    private CharacterController characterController;
    private WeaponManager weaponManager;
    private NetworkTransform networkTransform;
    private Vector3 velocity;
    private float cameraPitch = 0f;
    private bool isGrounded;

    // Network synced camera pitch for remote players to see where we're aiming
    private readonly NetworkVariable<float> networkCameraPitch = new(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        weaponManager = GetComponent<WeaponManager>();
        networkTransform = GetComponent<NetworkTransform>();

        if (cameraTarget == null)
            cameraTarget = virtualCamera?.transform;

        if (virtualCamera == null)
            virtualCamera = GetComponentInChildren<CinemachineCamera>();

        if (audioListener == null)
            audioListener = GetComponentInChildren<AudioListener>();

        if (weaponRootController == null)
            weaponRootController = GetComponentInChildren<WeaponRootController>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Validate NetworkTransform settings
        ValidateNetworkTransformSettings();

        if (IsOwner)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupRemotePlayer();

            // Subscribe to camera pitch changes for remote players
            networkCameraPitch.OnValueChanged += OnRemoteCameraPitchChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsOwner)
        {
            networkCameraPitch.OnValueChanged -= OnRemoteCameraPitchChanged;
        }
    }

    /// <summary>
    /// Called when remote player's camera pitch changes.
    /// Updates the local camera target rotation to match.
    /// </summary>
    private void OnRemoteCameraPitchChanged(float previousValue, float newValue)
    {
        if (cameraTarget != null)
        {
            cameraTarget.localRotation = Quaternion.Euler(newValue, 0f, 0f);
        }
    }

    private void ValidateNetworkTransformSettings()
    {
        if (networkTransform == null)
        {
            Debug.LogWarning($"[NetworkedFPSController] No NetworkTransform found on {gameObject.name}! Movement will not sync.");
            return;
        }

        // Log warnings for potentially problematic settings
        if (!IsOwner && networkTransform.PositionMaxInterpolationTime < 0.15f)
        {
            Debug.LogWarning($"[Remote Player] PositionMaxInterpolationTime is low ({networkTransform.PositionMaxInterpolationTime}s). May cause choppy movement. Recommended: 0.15-0.25s");
        }

        if (!IsOwner && !networkTransform.UseQuaternionSynchronization)
        {
            Debug.LogWarning($"[Remote Player] UseQuaternionSynchronization is disabled. May cause choppy rotation. Recommended: Enable it.");
        }

        if (!IsOwner && !networkTransform.Interpolate)
        {
            Debug.LogWarning($"[Remote Player] Interpolation is disabled! Movement will be very choppy.");
        }
    }
    
    private void SetupLocalPlayer()
    {
        if (virtualCamera != null)
        {
            virtualCamera.Priority = 10;
            virtualCamera.enabled = true;
        }

        if (audioListener != null)
            audioListener.enabled = true;

        // Setup weapon root to follow camera
        if (weaponRootController != null)
        {
            weaponRootController.SetCameraTarget(cameraTarget);
            weaponRootController.SetPlayerCamera(Camera.main);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log($"Local player setup - ClientId: {OwnerClientId}");
    }
    
    private void SetupRemotePlayer()
    {
        if (virtualCamera != null)
        {
            virtualCamera.Priority = 0;
            virtualCamera.enabled = false;
        }

        if (audioListener != null)
            audioListener.enabled = false;

        // Setup weapon root to follow camera target (no camera available for remote players)
        if (weaponRootController != null)
        {
            weaponRootController.SetCameraTarget(cameraTarget);
            weaponRootController.enabled = true; // Enable for remote players too
        }

        // CRITICAL: Disable CharacterController on remote players
        // Let NetworkTransform handle position/rotation interpolation
        if (characterController != null)
            characterController.enabled = false;

        Debug.Log($"Remote player setup - ClientId: {OwnerClientId}");
    }
    
    private void Update()
    {
        // Only the owner controls their character
        if (!IsOwner) return;
        
        HandleMovement();
        HandleLook();
    }
    
    private void HandleMovement()
    {
        if (characterController == null) return;
        
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        isGrounded = characterController.isGrounded;
        
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        
        // Get input
        Vector2 moveInput = Vector2.zero;
        if (keyboard.wKey.isPressed) moveInput.y += 1f;
        if (keyboard.sKey.isPressed) moveInput.y -= 1f;
        if (keyboard.aKey.isPressed) moveInput.x -= 1f;
        if (keyboard.dKey.isPressed) moveInput.x += 1f;
        
        bool sprint = keyboard.leftShiftKey.isPressed;
        bool jump = keyboard.spaceKey.wasPressedThisFrame;
        
        // Calculate movement
        float currentSpeed = sprint ? sprintSpeed : walkSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = move.normalized * currentSpeed;
        
        // Jump
        if (jump && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        
        // Gravity
        velocity.y += gravity * Time.deltaTime;
        move.y = velocity.y;
        
        // Move
        characterController.Move(move * Time.deltaTime);
    }
    
    private void HandleLook()
    {
        var mouse = Mouse.current;
        if (mouse == null || Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 lookInput = mouse.delta.ReadValue();

        // Rotate player body (yaw)
        float yaw = lookInput.x * lookSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * yaw);

        // Rotate camera (pitch)
        float pitch = lookInput.y * lookSensitivity * Time.deltaTime;
        cameraPitch -= pitch;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        if (cameraTarget != null)
            cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        // Sync camera pitch over network so remote players see where we're aiming
        networkCameraPitch.Value = cameraPitch;
    }
}