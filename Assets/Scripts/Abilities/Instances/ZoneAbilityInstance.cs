using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Ability instance for zone-based abilities (ground circles, areas of effect).
    /// Examples: Pathfinder's Mark, Sanctuary Circle, Whirlpool
    /// Creates a zone GameObject with trigger collider to apply effects to entities inside.
    /// </summary>
    public class ZoneAbilityInstance : AbilityInstance
    {
        private GameObject zoneObject;

        public ZoneAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
            : base(data, caster, context) { }

        public override void Execute()
        {
            // Spawn zone VFX at target position
            if (abilityData.activeVFXPrefab != null)
            {
                zoneObject = SpawnVFX(abilityData.activeVFXPrefab, context.targetPosition, Quaternion.identity);
            }
            else
            {
                // Create simple zone GameObject if no VFX prefab specified
                zoneObject = new GameObject($"Zone_{abilityData.abilityName}");
                zoneObject.transform.position = context.targetPosition;
            }

            // Add DeployableObject component to handle zone logic
            var deployable = zoneObject.AddComponent<DeployableObject>();
            deployable.Initialize(abilityData, caster);

            // Play cast audio
            PlayAudio(abilityData.castSound, context.targetPosition);
        }

        public override void Update(float deltaTime)
        {
            elapsed += deltaTime;
        }

        public override void Cleanup()
        {
            // Destroy zone GameObject
            if (zoneObject != null)
            {
                // Play end audio/VFX
                PlayAudio(abilityData.endSound, zoneObject.transform.position);

                GameObject.Destroy(zoneObject);
            }
        }
    }
}