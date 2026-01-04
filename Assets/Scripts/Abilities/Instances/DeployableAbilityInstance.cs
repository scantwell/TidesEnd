using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Ability instance for deployable objects (anchors, shields, totems).
    /// Examples: Bulwark Anchor Point, Bastion Shield, Ritual Totems
    /// Spawns a physical GameObject that can be destroyed or interacted with.
    /// </summary>
    public class DeployableAbilityInstance : AbilityInstance
    {
        private GameObject deployedObject;

        public DeployableAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
            : base(data, caster, context) { }

        public override void Execute()
        {
            // Deployable abilities MUST have a prefab with visual representation
            if (abilityData.activeVFXPrefab == null)
            {
                Debug.LogError($"[DeployableAbilityInstance] Deployable ability '{abilityData.abilityName}' requires activeVFXPrefab!");
                return;
            }

            // Spawn deployable object at target position (networked)
            deployedObject = SpawnNetworkedObject(
                abilityData.activeVFXPrefab,
                context.targetPosition,
                Quaternion.identity
            );

            if (deployedObject == null)
            {
                Debug.LogError($"[DeployableAbilityInstance] Failed to spawn deployable object for '{abilityData.abilityName}'");
                return;
            }

            // Initialize deployable component (add if not present on prefab)
            if (!deployedObject.TryGetComponent<DeployableObject>(out var deployable))
            {
                deployable = deployedObject.AddComponent<DeployableObject>();
            }
            deployable.Initialize(abilityData, caster);

            // Play deploy audio
            PlayAudio(abilityData.castSound, context.targetPosition);
        }

        public override void Update(float deltaTime)
        {
            elapsed += deltaTime;

            // Check if deployed object was destroyed externally
            if (deployedObject == null)
            {
                elapsed = abilityData.duration; // Force expiration
            }
        }

        public override void Cleanup()
        {
            // Destroy deployed object if still exists
            if (deployedObject != null)
            {
                PlayAudio(abilityData.endSound, deployedObject.transform.position);
                GameObject.Destroy(deployedObject);
            }
        }
    }
}