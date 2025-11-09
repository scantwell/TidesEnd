using UnityEngine;

namespace TidesEnd.Core
{
    /// <summary>
    /// Centralized layer management for consistent layer usage across the game.
    /// Provides layer indices, common layer masks, and helper methods for layer-based queries.
    /// </summary>
    public static class GameLayers
    {
        // Layer indices (set in Edit > Project Settings > Tags and Layers)
        public static readonly int Default = 0;
        public static readonly int TransparentFX = 1;
        public static readonly int IgnoreRaycast = 2;
        public static readonly int Water = 4;
        public static readonly int UI = 5;
        public static readonly int Projectile = 6;
        public static readonly int Environment = 7;
        public static readonly int Player = 8;
        public static readonly int PlayerHitbox = 9;
        public static readonly int Enemy = 10;
        public static readonly int EnemyHitbox = 11;
        public static readonly int Neutral = 12;
        public static readonly int Boss = 13;
        public static readonly int Interactable = 14;
        public static readonly int DynamicObstacle = 15;

        // Common layer masks (use these in weapon configurations and raycasts)

        /// <summary>
        /// All entities that can take damage (players, enemies, neutral objects, bosses)
        /// </summary>
        public static readonly LayerMask AllDamageable =
            (1 << Player) | (1 << PlayerHitbox) |
            (1 << Enemy) | (1 << EnemyHitbox) |
            (1 << Neutral) | (1 << Boss);

        /// <summary>
        /// Only enemy entities (for player weapons with friendly fire OFF)
        /// </summary>
        public static readonly LayerMask EnemiesOnly =
            (1 << Enemy) | (1 << EnemyHitbox) | (1 << Boss);

        /// <summary>
        /// Only player entities (for enemy weapons)
        /// </summary>
        public static readonly LayerMask PlayersOnly =
            (1 << Player) | (1 << PlayerHitbox);

        /// <summary>
        /// All valid targets for hitscan weapons (damageable + environment for bullet holes)
        /// </summary>
        public static readonly LayerMask HitscanTargets =
            AllDamageable | (1 << Environment);

        /// <summary>
        /// What projectiles should collide with (enemies + environment, no friendly fire)
        /// </summary>
        public static readonly LayerMask ProjectileColliders =
            EnemiesOnly | (1 << Environment) | (1 << Neutral);

        /// <summary>
        /// Layers that block player movement (environment + dynamic obstacles)
        /// </summary>
        public static readonly LayerMask PlayerMovementObstacles =
            (1 << Environment) | (1 << DynamicObstacle);

        /// <summary>
        /// Layers to check for ground detection
        /// </summary>
        public static readonly LayerMask GroundLayers =
            (1 << Environment) | (1 << DynamicObstacle);

        // Helper Methods

        /// <summary>
        /// Check if a GameObject is on a player-related layer
        /// </summary>
        public static bool IsPlayer(GameObject obj)
        {
            if (obj == null) return false;
            return obj.layer == Player || obj.layer == PlayerHitbox;
        }

        /// <summary>
        /// Check if a GameObject is on an enemy-related layer
        /// </summary>
        public static bool IsEnemy(GameObject obj)
        {
            if (obj == null) return false;
            return obj.layer == Enemy || obj.layer == EnemyHitbox || obj.layer == Boss;
        }

        /// <summary>
        /// Check if a GameObject is on a damageable layer
        /// </summary>
        public static bool IsDamageable(GameObject obj)
        {
            if (obj == null) return false;
            return ((1 << obj.layer) & AllDamageable) != 0;
        }

        /// <summary>
        /// Check if a GameObject is on a hitbox layer (PlayerHitbox or EnemyHitbox)
        /// </summary>
        public static bool IsHitbox(GameObject obj)
        {
            if (obj == null) return false;
            return obj.layer == PlayerHitbox || obj.layer == EnemyHitbox;
        }

        /// <summary>
        /// Check if two GameObjects are on the same team (both players or both enemies)
        /// </summary>
        public static bool IsSameTeam(GameObject objA, GameObject objB)
        {
            if (objA == null || objB == null) return false;

            bool aIsPlayer = IsPlayer(objA);
            bool bIsPlayer = IsPlayer(objB);
            bool aIsEnemy = IsEnemy(objA);
            bool bIsEnemy = IsEnemy(objB);

            return (aIsPlayer && bIsPlayer) || (aIsEnemy && bIsEnemy);
        }

        /// <summary>
        /// Set all child objects to a specific layer recursively
        /// </summary>
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            if (obj == null) return;

            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        /// <summary>
        /// Set hitbox children to hitbox layer based on parent layer
        /// </summary>
        public static void SetupHitboxLayers(GameObject rootObject)
        {
            if (rootObject == null) return;

            int hitboxLayer = -1;

            if (rootObject.layer == Player)
            {
                hitboxLayer = PlayerHitbox;
            }
            else if (rootObject.layer == Enemy || rootObject.layer == Boss)
            {
                hitboxLayer = EnemyHitbox;
            }

            if (hitboxLayer == -1) return;

            // Find all HitZone components and set their layer
            var hitZones = rootObject.GetComponentsInChildren<TidesEnd.Combat.HitZone>(true);
            foreach (var hitZone in hitZones)
            {
                hitZone.gameObject.layer = hitboxLayer;
            }
        }

        /// <summary>
        /// Debug helper: Get layer name from layer index
        /// </summary>
        public static string GetLayerName(int layer)
        {
            return LayerMask.LayerToName(layer);
        }

        /// <summary>
        /// Debug helper: Log all layers of GameObject and its children
        /// </summary>
        public static void DebugLogLayers(GameObject obj, bool includeChildren = false)
        {
            if (obj == null) return;

            Debug.Log($"<color=cyan>{obj.name}</color> layer: {GetLayerName(obj.layer)} ({obj.layer})");

            if (includeChildren)
            {
                foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
                {
                    if (child != obj.transform)
                    {
                        Debug.Log($"  └─ <color=yellow>{child.name}</color> layer: {GetLayerName(child.gameObject.layer)} ({child.gameObject.layer})");
                    }
                }
            }
        }
    }
}
