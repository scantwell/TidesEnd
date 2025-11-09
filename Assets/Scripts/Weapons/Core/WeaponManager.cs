using System.Collections.Generic;
using UnityEngine;

namespace TidesEnd.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
    [Header("Setup")]
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private List<GameObject> weaponPrefabs = new List<GameObject>();

    [Header("Debug")]
    [SerializeField] private WeaponDebugUI debugUI;
    [SerializeField] private WeaponDebugVisualizer debugVisualizer;

    private List<GameObject> spawnedWeapons = new List<GameObject>();
    private int currentWeaponIndex = 0;

    public Weapon CurrentWeapon { get; private set; }
    public int CurrentWeaponIndex => currentWeaponIndex;
    public int WeaponCount => spawnedWeapons.Count;
    
    private void Awake()
    {
        // Create weapon root if needed
        if (weaponRoot == null)
        {
            GameObject root = new GameObject("WeaponRoot");
            root.transform.SetParent(transform);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            weaponRoot = root.transform;
        }

        // Find debug components if not assigned
        if (debugUI == null)
        {
            debugUI = GetComponent<WeaponDebugUI>();
        }

        if (debugVisualizer == null)
        {
            debugVisualizer = GetComponent<WeaponDebugVisualizer>();
        }
    }
    
    private void Start()
    {
        SpawnWeapons();
        
        if (spawnedWeapons.Count > 0)
        {
            SwitchToWeapon(0);
        }
    }
    
    private void SpawnWeapons()
    {
        foreach (GameObject prefab in weaponPrefabs)
        {
            if (prefab == null) continue;

            GameObject weapon = Instantiate(prefab, weaponRoot);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.SetActive(false);

            // Link debug UI to weapon
            SetupWeaponDebug(weapon);

            spawnedWeapons.Add(weapon);
        }

        Debug.Log($"Spawned {spawnedWeapons.Count} weapons");
    }

    /// <summary>
    /// Automatically set up debug UI reference for a weapon.
    /// Called when weapon is spawned from prefab.
    /// </summary>
    private void SetupWeaponDebug(GameObject weaponObject)
    {
        if (weaponObject == null) return;

        Weapon weapon = weaponObject.GetComponent<Weapon>();
        if (weapon == null) return;

        // TODO: Setup debug UI if needed
        if (debugUI != null)
        {
            // debugUI integration can be added later
            Debug.Log($"Weapon spawned: {weapon.Data?.weaponName ?? weaponObject.name}");
        }
    }
    
    public void SwitchToWeapon(int index)
    {
        if (index < 0 || index >= spawnedWeapons.Count) return;
        if (index == currentWeaponIndex && CurrentWeapon != null) return;

        // Deactivate all weapons
        foreach (GameObject weapon in spawnedWeapons)
        {
            weapon.SetActive(false);
        }

        // Activate selected weapon
        currentWeaponIndex = index;
        GameObject activeWeapon = spawnedWeapons[index];
        activeWeapon.SetActive(true);

        // Get weapon component
        CurrentWeapon = activeWeapon.GetComponent<Weapon>();

        Debug.Log($"Switched to weapon {index}: {CurrentWeapon?.Data?.weaponName ?? "Unknown"}");
    }
    
    public void NextWeapon()
    {
        int next = (currentWeaponIndex + 1) % spawnedWeapons.Count;
        SwitchToWeapon(next);
    }
    
    public void PreviousWeapon()
    {
        int prev = (currentWeaponIndex - 1 + spawnedWeapons.Count) % spawnedWeapons.Count;
        SwitchToWeapon(prev);
    }
    }
}