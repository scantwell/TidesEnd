using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using TidesEnd.Weapons;
using TidesEnd.Abilities;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    [Tooltip("Gravity force applied to player")]
    [SerializeField] private float gravity = -15f;

    [Header("Look")]
    [Tooltip("Mouse sensitivity for camera rotation")]
    [SerializeField] private float lookSensitivity = 0.02f;
    [Tooltip("Maximum vertical look angle in degrees")]
    [SerializeField] private float maxLookAngle = 90f;

    [Header("References")]
    [Tooltip("Transform used as camera pivot point")]
    [SerializeField] private Transform cameraTarget;
    [Tooltip("Cinemachine virtual camera component")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [Tooltip("Audio listener component for local player")]
    [SerializeField] private AudioListener audioListener;

    private CharacterController characterController;
    private NetworkTransform networkTransform;
    private EntityStats entityStats;
    private Vector3 velocity;
    private float cameraPitch = 0f;
    private bool isGrounded;

    // Recoil state
    private Vector2 currentRecoil = Vector2.zero;  // Current recoil offset (yaw, pitch)
    private Vector2 targetRecoil = Vector2.zero;   // Target recoil to apply
    private float lastRecoilTime = 0f;             // When was the last recoil applied

    // Screen shake state
    private Vector3 screenShakeOffset = Vector3.zero;  // Current shake offset
    private float screenShakeIntensity = 0f;           // Current shake intensity
    private float screenShakeStartTime = 0f;           // When shake started
    private float screenShakeDuration = 0f;            // How long shake lasts
    private Vector3 originalCameraPosition = Vector3.zero; // Store original camera position

    // Current weapon data (set by PlayerWeaponController)
    private WeaponData currentWeaponData;

    // Network synced camera pitch for remote players to see where we're aiming
    private readonly NetworkVariable<float> networkCameraPitch = new(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        networkTransform = GetComponent<NetworkTransform>();
        entityStats = GetComponent<EntityStats>();

        if (cameraTarget == null)
            cameraTarget = virtualCamera?.transform;

        if (virtualCamera == null)
            virtualCamera = GetComponentInChildren<CinemachineCamera>();

        if (audioListener == null)
            audioListener = GetComponentInChildren<AudioListener>();
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Store original camera position for screen shake
        if (cameraTarget != null)
        {
            originalCameraPosition = cameraTarget.localPosition;
        }

        // Ensure AudioListener is enabled for local player
        if (audioListener != null)
            audioListener.enabled = true;

        // Ensure virtual camera has high priority for local player
        if (virtualCamera != null)
            virtualCamera.Priority = 10;

        Debug.Log($"Local player setup - ClientId: {OwnerClientId}");
    }
    
    private void SetupRemotePlayer()
    {
        // CRITICAL: Disable CharacterController on remote players
        // Let NetworkTransform handle position/rotation interpolation
        if (characterController != null)
            characterController.enabled = false;

        // Disable AudioListener for remote players (only local player should hear)
        if (audioListener != null)
            audioListener.enabled = false;

        // Disable virtual camera for remote players (only local player needs active camera)
        if (virtualCamera != null)
            virtualCamera.Priority = 0;

        Debug.Log($"Remote player setup - ClientId: {OwnerClientId}");
    }
    
    private void Update()
    {
        // Only the owner controls their character
        if (!IsOwner) return;

        HandleMovement();
        HandleLook();
        UpdateRecoil();
        UpdateScreenShake();
    }

    /// <summary>
    /// Apply weapon recoil to the camera
    /// Call this from PlayerWeaponController when firing
    /// </summary>
    public void ApplyRecoil(Vector2 recoilAmount)
    {
        targetRecoil += recoilAmount;
        lastRecoilTime = Time.time;
    }

    /// <summary>
    /// Set current weapon data for recoil calculations
    /// Call this when weapon changes
    /// </summary>
    public void SetCurrentWeapon(WeaponData weaponData)
    {
        currentWeaponData = weaponData;
    }

    /// <summary>
    /// Reset recoil state (e.g., when switching weapons)
    /// </summary>
    public void ResetRecoil()
    {
        currentRecoil = Vector2.zero;
        targetRecoil = Vector2.zero;
        lastRecoilTime = 0f;
    }

    /// <summary>
    /// Apply screen shake effect when firing
    /// Call this from PlayerWeaponController when firing
    /// </summary>
    public void ApplyScreenShake(float intensity, float duration)
    {
        screenShakeIntensity = intensity;
        screenShakeDuration = duration;
        screenShakeStartTime = Time.time;
    }

    /// <summary>
    /// Update screen shake effect with decay over time
    /// </summary>
    private void UpdateScreenShake()
    {
        if (screenShakeIntensity <= 0f)
        {
            screenShakeOffset = Vector3.zero;
            return;
        }

        float elapsed = Time.time - screenShakeStartTime;

        if (elapsed >= screenShakeDuration)
        {
            // Shake expired
            screenShakeIntensity = 0f;
            screenShakeOffset = Vector3.zero;
        }
        else
        {
            // Calculate falloff (shake diminishes over time)
            float falloff = 1f - (elapsed / screenShakeDuration);
            float currentIntensity = screenShakeIntensity * falloff;

            // Generate random shake offset
            screenShakeOffset = new Vector3(
                UnityEngine.Random.Range(-currentIntensity, currentIntensity),
                UnityEngine.Random.Range(-currentIntensity, currentIntensity),
                0f
            );
        }
    }

    /// <summary>
    /// Smoothly apply and recover from recoil
    /// </summary>
    private void UpdateRecoil()
    {
        // Use WeaponLogic to calculate new recoil state
        var (newCurrent, newTarget) = WeaponLogic.UpdateRecoilState(
            currentWeaponData,
            currentRecoil,
            targetRecoil,
            lastRecoilTime,
            Time.time,
            Time.deltaTime
        );

        currentRecoil = newCurrent;
        targetRecoil = newTarget;
    }
    
    private void HandleMovement()
    {
        if (characterController == null || entityStats == null) return;

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

        // Get current speed from EntityStats (already includes all modifiers)
        float currentSpeed = sprint ? entityStats.SprintSpeed.Value : entityStats.WalkSpeed.Value;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = move.normalized * currentSpeed;

        // Jump - use EntityStats jump height
        if (jump && isGrounded)
            velocity.y = Mathf.Sqrt(entityStats.JumpHeight.Value * -2f * gravity);

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

        // Rotate player body (yaw + recoil horizontal)
        // Mouse delta is already per-frame, so no Time.deltaTime needed
        float yaw = lookInput.x * lookSensitivity;
        transform.Rotate(Vector3.up * (yaw + currentRecoil.x));

        // Rotate camera (pitch + recoil vertical)
        // Mouse delta is already per-frame, so no Time.deltaTime needed
        float pitch = lookInput.y * lookSensitivity;
        cameraPitch -= pitch;
        cameraPitch -= currentRecoil.y; // Apply recoil to pitch
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        // Apply screen shake to camera position (add offset to original position)
        cameraTarget.localPosition = originalCameraPosition + screenShakeOffset;

        // Sync camera pitch over network so remote players see where we're aiming
        networkCameraPitch.Value = cameraPitch;
    }
}