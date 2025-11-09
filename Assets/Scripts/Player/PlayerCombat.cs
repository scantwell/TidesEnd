using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TidesEnd.Weapons;

public class PlayerCombat : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Transform cameraTarget;

    private Camera mainCamera;

    private void Awake()
    {
        if (weaponManager == null)
            weaponManager = GetComponent<WeaponManager>();

        if (cameraTarget == null)
        {
            cameraTarget = transform.Find("CameraTarget")
                        ?? transform.Find("PlayerCameraRoot")
                        ?? transform.Find("Head");
        }
    }

    private void Start()
    {
        if (IsOwner)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found!");
            }
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleCombatInput();
        HandleWeaponSwitching();
    }

    private void HandleCombatInput()
    {
        if (weaponManager?.CurrentWeapon == null) return;

        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        if (mouse == null || keyboard == null) return;

        if (mouse.leftButton.isPressed)
        {
            TryFire();
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            TryReload();
        }
    }

    private void HandleWeaponSwitching()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) weaponManager.SwitchToWeapon(0);
        if (keyboard.digit2Key.wasPressedThisFrame) weaponManager.SwitchToWeapon(1);
        if (keyboard.digit3Key.wasPressedThisFrame) weaponManager.SwitchToWeapon(2);
        if (keyboard.digit4Key.wasPressedThisFrame) weaponManager.SwitchToWeapon(3);

        var mouse = Mouse.current;
        if (mouse != null)
        {
            float scroll = mouse.scroll.ReadValue().y;
            if (scroll > 0) weaponManager.NextWeapon();
            else if (scroll < 0) weaponManager.PreviousWeapon();
        }
    }

    private void TryFire()
    {
        Weapon weapon = weaponManager.CurrentWeapon;
        if (weapon == null || !weapon.CanFire()) return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // Setup weapon references
        weapon.SetPlayerCamera(mainCamera);
        weapon.SetOwner(OwnerClientId, gameObject);

        // CLIENT: Fire with immediate visual feedback (no damage)
        weapon.FireClient();

        // SERVER: Request authoritative damage application
        FireServerRpc();
    }

    [ServerRpc]
    private void FireServerRpc()
    {
        Weapon weapon = weaponManager.CurrentWeapon;
        if (weapon == null) return;

        // Ensure weapon knows who the owner is on server
        weapon.SetOwner(OwnerClientId, gameObject);

        // SERVER: Fire with authoritative damage (no visuals)
        // Host doesn't need validation (already did it in FireClient on same frame/instance)
        // Remote clients need validation (server anti-cheat)
        bool isHost = OwnerClientId == NetworkManager.ServerClientId;
        weapon.FireServer(validate: !isHost);

        // Notify all clients (except owner) to play fire effects
        PlayFireEffectsClientRpc();
    }

    [ClientRpc]
    private void PlayFireEffectsClientRpc()
    {
        if (!IsOwner)
        {
            Weapon weapon = weaponManager?.CurrentWeapon;
            if (weapon != null)
            {
                WeaponView view = weapon.GetComponent<WeaponView>();
                if (view != null)
                {
                    view.PlayFireEffects();
                }
            }
        }
    }

    private void TryReload()
    {
        Weapon weapon = weaponManager.CurrentWeapon;
        if (weapon == null) return;

        weapon.Reload();
        ReloadServerRpc();
    }

    [ServerRpc]
    private void ReloadServerRpc()
    {
        Weapon weapon = weaponManager.CurrentWeapon;
        if (weapon != null)
        {
            weapon.Reload();
        }

        ReloadClientRpc();
    }

    [ClientRpc]
    private void ReloadClientRpc()
    {
        if (!IsOwner)
        {
            WeaponView view = weaponManager?.CurrentWeapon?.GetComponent<WeaponView>();
            if (view != null)
            {
                view.PlayReloadEffects();
            }
        }
    }
}
