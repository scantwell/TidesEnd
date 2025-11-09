using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Factory class for creating ability instances based on ability type.
    /// Uses the factory pattern to instantiate the correct AbilityInstance subclass.
    /// </summary>
    public static class AbilityFactory
    {
        /// <summary>
        /// Create an ability instance based on the ability data type.
        /// </summary>
        /// <param name="data">The ability data (ScriptableObject)</param>
        /// <param name="caster">The entity activating the ability</param>
        /// <param name="context">Context data for targeting/positioning</param>
        /// <returns>Concrete AbilityInstance subclass or null if type not implemented</returns>
        public static AbilityInstance CreateAbilityInstance(AbilityData data, AbilityUser caster, AbilityContext context)
        {
            switch (data.abilityType)
            {
                case AbilityType.Passive:
                    // Passives don't need instances (applied directly on spawn)
                    return null;

                case AbilityType.Deployable:
                    return new DeployableAbilityInstance(data, caster, context);

                case AbilityType.Projectile:
                    return new ProjectileAbilityInstance(data, caster, context);

                case AbilityType.PlacedZone:
                    return new ZoneAbilityInstance(data, caster, context);

                case AbilityType.Buff:
                    return new BuffAbilityInstance(data, caster, context);

                case AbilityType.ChanneledAoE:
                    return new ChanneledAbilityInstance(data, caster, context);

                case AbilityType.SummonMinions:
                    return new SummonAbilityInstance(data, caster, context);

                case AbilityType.Teleport:
                    return new TeleportAbilityInstance(data, caster, context);

                case AbilityType.Possess:
                    // TODO: Implement PossessAbilityInstance
                    Debug.LogWarning($"[AbilityFactory] Possess ability type not yet implemented: {data.abilityName}");
                    return null;

                case AbilityType.Transform:
                    // TODO: Implement TransformAbilityInstance
                    Debug.LogWarning($"[AbilityFactory] Transform ability type not yet implemented: {data.abilityName}");
                    return null;

                default:
                    Debug.LogError($"[AbilityFactory] Unknown ability type: {data.abilityType} for ability {data.abilityName}");
                    return null;
            }
        }
    }
}