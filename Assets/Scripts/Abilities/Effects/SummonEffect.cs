using System;
using UnityEngine;
using Unity.Netcode;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Effect that spawns minions/entities.
    /// Used by boss summon abilities like Drowned Priest's Summon Wave.
    /// </summary>
    [Serializable]
    public class SummonEffect : AbilityEffect
    {
        [Header("Summon Parameters")]
        [Tooltip("Prefab to spawn (must have NetworkObject component)")]
        public GameObject summonPrefab;

        [Tooltip("Number of entities to summon")]
        public int summonCount = 1;

        [Tooltip("Radius around spawn position to scatter summons")]
        public float summonRadius = 5f;

        [Tooltip("If > 0, summons despawn after this duration")]
        public float summonDuration = 0f;

        [Tooltip("Should summons be positioned in a circle pattern?")]
        public bool circleFormation = true;

        public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
        {
            if (summonPrefab == null)
            {
                Debug.LogError("SummonEffect: summonPrefab is null!");
                return;
            }

            // Spawn minions around the target position
            for (int i = 0; i < summonCount; i++)
            {
                Vector3 spawnPos = CalculateSpawnPosition(context.targetPosition, i);
                GameObject summon = GameObject.Instantiate(summonPrefab, spawnPos, Quaternion.identity);

                // Network spawn if using Netcode
                if (summon.TryGetComponent<NetworkObject>(out var netObj))
                {
                    netObj.Spawn();
                }

                // Set up summon behavior (target assignment, etc.)
                ConfigureSummon(summon, caster);

                // Despawn after duration if specified
                if (summonDuration > 0)
                {
                    GameObject.Destroy(summon, summonDuration);
                }
            }
        }

        public override void Remove(GameObject target)
        {
            // Summons persist after ability ends (unless summonDuration is set)
        }

        private Vector3 CalculateSpawnPosition(Vector3 center, int index)
        {
            if (circleFormation)
            {
                // Spawn in circle pattern around center
                float angle = (360f / summonCount) * index * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * summonRadius;
                return center + offset;
            }
            else
            {
                // Random position within radius
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * summonRadius;
                return center + new Vector3(randomCircle.x, 0, randomCircle.y);
            }
        }

        private void ConfigureSummon(GameObject summon, AbilityUser caster)
        {
            // Try to set target for enemy AI
            if (summon.TryGetComponent<Enemy.EnemyAI>(out var enemyAI))
            {
                // Find nearest player to attack
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    // enemyAI.SetTarget(player.transform);
                }
            }
        }
    }
}
