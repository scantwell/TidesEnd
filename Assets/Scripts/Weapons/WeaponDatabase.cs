using UnityEngine;
using System.Collections.Generic;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Database of all weapons in the game
    /// Maps weapon IDs to prefab pairs
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Tide's End/Weapons/Weapon Database")]
    public class WeaponDatabase : ScriptableObject
    {
        [SerializeField] private List<WeaponPrefabPair> weapons = new List<WeaponPrefabPair>();
        
        public WeaponPrefabPair GetWeapon(int weaponID)
        {
            if (weaponID < 0 || weaponID >= weapons.Count)
            {
                Debug.LogError($"Invalid weapon ID: {weaponID}");
                return null;
            }
            
            return weapons[weaponID];
        }
        
        public int GetWeaponCount()
        {
            return weapons.Count;
        }
        
        public int GetWeaponID(string weaponName)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].weaponData.weaponName == weaponName)
                    return i;
            }
            
            return -1;
        }
    }
    
    /// <summary>
    /// Pairs view model and world model prefabs with weapon data
    /// </summary>
    [System.Serializable]
    public class WeaponPrefabPair
    {
        public string weaponName; // For editor reference
        public WeaponData weaponData; // Stats
        public GameObject viewModelPrefab; // First-person (high detail)
        public GameObject worldModelPrefab; // Third-person (low detail)
    }
}