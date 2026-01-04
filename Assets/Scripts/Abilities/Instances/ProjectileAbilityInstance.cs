using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Ability instance for projectile-based abilities.
    /// Examples: Lamplighter Flare Shot, Harpooner Whaling Harpoon
    /// Spawns a projectile GameObject that travels and applies effects on hit.
    /// </summary>
    public class ProjectileAbilityInstance : AbilityInstance
    {
        private GameObject projectile;

        public ProjectileAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
            : base(data, caster, context) { }

        public override void Execute()
        {
            // Calculate spawn position (slightly in front of caster)
            Vector3 spawnPosition = caster.transform.position + caster.transform.forward * 1f;

            // Calculate direction
            Vector3 direction = context.targetDirection != Vector3.zero
                ? context.targetDirection
                : caster.transform.forward;

            // Spawn projectile (networked)
            if (abilityData.activeVFXPrefab != null)
            {
                projectile = SpawnNetworkedObject(
                    abilityData.activeVFXPrefab,
                    spawnPosition,
                    Quaternion.LookRotation(direction)
                );
            }
            else
            {
                Debug.LogWarning($"[ProjectileAbilityInstance] No activeVFXPrefab for {abilityData.abilityName}");
                return;
            }

            // Initialize projectile component if exists
            if (projectile.TryGetComponent<AbilityProjectile>(out var proj))
            {
                proj.Initialize(abilityData, caster, context);
            }
            else
            {
                // Add component if not on prefab
                var newProj = projectile.AddComponent<AbilityProjectile>();
                newProj.Initialize(abilityData, caster, context);
            }

            // Play fire audio
            PlayAudio(abilityData.castSound, spawnPosition);
        }

        public override void Update(float deltaTime)
        {
            elapsed += deltaTime;

            // Check if projectile was destroyed (hit something)
            if (projectile == null)
            {
                elapsed = abilityData.duration; // Force expiration
            }
        }

        public override void Cleanup()
        {
            // Destroy projectile if still exists
            if (projectile != null)
            {
                GameObject.Destroy(projectile);
            }
        }
    }
}