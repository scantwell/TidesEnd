using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Base class for runtime ability instances.
    /// Each ability type (Projectile, Zone, Buff, etc.) has a corresponding instance implementation.
    /// Instances are created when an ability is activated and handle the execution logic.
    /// </summary>
    public abstract class AbilityInstance
    {
        protected AbilityData abilityData;
        protected AbilityUser caster;
        protected AbilityContext context;
        protected float elapsed = 0f;

        public AbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        {
            this.abilityData = data;
            this.caster = caster;
            this.context = context;
        }

        /// <summary>
        /// Execute the ability. Called once when ability is activated.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Update the ability state. Called every frame for duration-based abilities.
        /// </summary>
        /// <param name="deltaTime">Time since last update</param>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Cleanup when ability expires or is removed.
        /// </summary>
        public abstract void Cleanup();

        /// <summary>
        /// Check if this ability instance has expired based on duration.
        /// </summary>
        /// <returns>True if ability should be removed</returns>
        public virtual bool IsExpired()
        {
            return abilityData.duration > 0 && elapsed >= abilityData.duration;
        }

        /// <summary>
        /// Helper to spawn VFX at a position.
        /// For non-networked VFX only (particles, visual effects).
        /// </summary>
        protected GameObject SpawnVFX(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
                return null;

            return GameObject.Instantiate(prefab, position, rotation);
        }

        /// <summary>
        /// Helper to spawn a networked object (projectile, deployable, zone).
        /// Automatically spawns as NetworkObject if component exists.
        /// </summary>
        protected GameObject SpawnNetworkedObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
                return null;

            GameObject obj = GameObject.Instantiate(prefab, position, rotation);

            // Check if this is a NetworkObject and spawn it
            if (obj.TryGetComponent<Unity.Netcode.NetworkObject>(out var netObj))
            {
                if (!netObj.IsSpawned)
                {
                    netObj.Spawn();
                }
            }
            else
            {
                Debug.LogWarning($"[AbilityInstance] Prefab '{prefab.name}' has no NetworkObject component! Will not sync in multiplayer.");
            }

            return obj;
        }

        /// <summary>
        /// Helper to play audio at a position.
        /// </summary>
        protected void PlayAudio(AudioClip clip, Vector3 position)
        {
            if (clip == null)
                return;

            AudioSource.PlayClipAtPoint(clip, position);
        }
    }
}