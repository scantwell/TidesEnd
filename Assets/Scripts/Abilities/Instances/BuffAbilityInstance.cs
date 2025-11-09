using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Ability instance for self-buff abilities.
    /// Examples: Harpooner Reel & Frenzy, boss enrage phases
    /// Applies effects to the caster for a duration, then removes them.
    /// </summary>
    public class BuffAbilityInstance : AbilityInstance
    {
        private bool effectsApplied = false;

        public BuffAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
            : base(data, caster, context) { }

        public override void Execute()
        {
            // Apply all effects to caster
            foreach (var effect in abilityData.effects)
            {
                if (effect.affectsSelf)
                {
                    effect.Apply(caster, caster.gameObject, context);
                }
            }

            effectsApplied = true;

            // Play buff audio/VFX
            PlayAudio(abilityData.castSound, caster.transform.position);
            if (abilityData.castVFXPrefab != null)
            {
                SpawnVFX(abilityData.castVFXPrefab, caster.transform.position, Quaternion.identity);
            }
        }

        public override void Update(float deltaTime)
        {
            elapsed += deltaTime;
        }

        public override void Cleanup()
        {
            if (effectsApplied)
            {
                // Remove all effects from caster
                foreach (var effect in abilityData.effects)
                {
                    effect.Remove(caster.gameObject);
                }

                // Play end audio/VFX
                PlayAudio(abilityData.endSound, caster.transform.position);
            }
        }
    }
}