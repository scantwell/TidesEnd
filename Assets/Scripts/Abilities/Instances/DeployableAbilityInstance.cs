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
            // Spawn deployable object at target position
            if (abilityData.activeVFXPrefab != null)
            {
                deployedObject = SpawnVFX(
                    abilityData.activeVFXPrefab,
                    context.targetPosition,
                    Quaternion.identity
                );
            }
            else
            {
                Debug.LogWarning($"[DeployableAbilityInstance] No activeVFXPrefab set for {abilityData.abilityName}");
                return;
            }

            // Initialize deployable component if it exists
            if (deployedObject.TryGetComponent<DeployableObject>(out var deployable))
            {
                deployable.Initialize(abilityData, caster);
            }
            else
            {
                // Add component if prefab doesn't have one
                var newDeployable = deployedObject.AddComponent<DeployableObject>();
                newDeployable.Initialize(abilityData, caster);
            }

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