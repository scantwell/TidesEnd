using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Ability instance for teleport abilities.
    /// Examples: Dimensional Shambler teleport, Reef Leviathan submerge
    /// Instantly moves the caster to a new position.
    /// </summary>
    public class TeleportAbilityInstance : AbilityInstance
    {
        public TeleportAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
            : base(data, caster, context) { }

        public override void Execute()
        {
            // Find teleport effect
            TeleportEffect teleportEffect = System.Array.Find(
                abilityData.effects,
                e => e is TeleportEffect
            ) as TeleportEffect;

            if (teleportEffect == null)
            {
                Debug.LogWarning($"[TeleportAbilityInstance] No TeleportEffect found on {abilityData.abilityName}");
                return;
            }

            // Play teleport out VFX/audio at current position
            PlayAudio(abilityData.castSound, caster.transform.position);
            if (abilityData.castVFXPrefab != null)
            {
                SpawnVFX(abilityData.castVFXPrefab, caster.transform.position, Quaternion.identity);
            }

            // Apply teleport effect (it handles the actual teleportation)
            teleportEffect.Apply(caster, caster.gameObject, context);

            // Play teleport in VFX/audio at new position
            PlayAudio(abilityData.impactSound, caster.transform.position);
            if (abilityData.impactVFXPrefab != null)
            {
                SpawnVFX(abilityData.impactVFXPrefab, caster.transform.position, Quaternion.identity);
            }
        }

        public override void Update(float deltaTime)
        {
            elapsed += deltaTime;
        }

        public override void Cleanup()
        {
            // Teleport is instant, no cleanup needed
        }
    }
}