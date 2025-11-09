using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Ability instance for channeled AoE abilities.
    /// Examples: Occultist Purge Corruption, boss pulses
    /// Applies effects to all entities in range after channel completes.
    /// </summary>
    public class ChanneledAbilityInstance : AbilityInstance
    {
        public ChanneledAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
            : base(data, caster, context) { }

        public override void Execute()
        {
            // Channel is complete, apply effects in radius
            Collider[] targets = Physics.OverlapSphere(
                caster.transform.position,
                abilityData.radius
            );

            foreach (var collider in targets)
            {
                GameObject target = collider.gameObject;

                // Apply each effect to valid targets
                foreach (var effect in abilityData.effects)
                {
                    if (effect.ShouldApplyToTarget(caster, target))
                    {
                        effect.Apply(caster, target, context);
                    }
                }
            }

            // Play completion VFX/audio
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
            // Channeled abilities are instant after completion, no cleanup needed
        }
    }
}