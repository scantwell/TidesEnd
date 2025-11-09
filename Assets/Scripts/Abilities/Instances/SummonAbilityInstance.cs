using UnityEngine;
using System.Collections.Generic;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Ability instance for summoning minions.
    /// Examples: Drowned Priest Summon Wave, Deep One pack spawns
    /// Spawns multiple entities that persist after ability ends.
    /// </summary>
    public class SummonAbilityInstance : AbilityInstance
    {
        private List<GameObject> summonedEntities = new List<GameObject>();

        public SummonAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
            : base(data, caster, context) { }

        public override void Execute()
        {
            // Find summon effect
            SummonEffect summonEffect = System.Array.Find(
                abilityData.effects,
                e => e is SummonEffect
            ) as SummonEffect;

            if (summonEffect == null)
            {
                Debug.LogWarning($"[SummonAbilityInstance] No SummonEffect found on {abilityData.abilityName}");
                return;
            }

            // Apply the summon effect (it handles spawning)
            summonEffect.Apply(caster, null, context);

            // Play summon audio/VFX
            PlayAudio(abilityData.castSound, context.targetPosition);
            if (abilityData.castVFXPrefab != null)
            {
                SpawnVFX(abilityData.castVFXPrefab, context.targetPosition, Quaternion.identity);
            }
        }

        public override void Update(float deltaTime)
        {
            elapsed += deltaTime;

            // Clean up destroyed summons from list
            summonedEntities.RemoveAll(s => s == null);
        }

        public override void Cleanup()
        {
            // Summons persist after ability ends (unless they have a duration in SummonEffect)
            // No cleanup needed
        }
    }
}