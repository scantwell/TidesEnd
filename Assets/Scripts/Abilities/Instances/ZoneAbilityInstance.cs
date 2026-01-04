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
            // Zone abilities MUST have a visual representation (prefab)
            if (abilityData.activeVFXPrefab == null)
            {
                Debug.LogError($"[ZoneAbilityInstance] Zone ability '{abilityData.abilityName}' requires activeVFXPrefab! Players need to see the zone to use it.");
                return;
            }

            // Spawn zone VFX at target position (networked)
            zoneObject = SpawnNetworkedObject(abilityData.activeVFXPrefab, context.targetPosition, Quaternion.identity);

            if (zoneObject == null)
            {
                Debug.LogError($"[ZoneAbilityInstance] Failed to spawn zone object for '{abilityData.abilityName}'");
                return;
            }

            // Add DeployableObject component to handle zone logic if not already present
            if (!zoneObject.TryGetComponent<DeployableObject>(out var deployable))
            {
                deployable = zoneObject.AddComponent<DeployableObject>();
            }
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