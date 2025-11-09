using System;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Effect that modifies movement speed of the target.
    /// Used by buff/debuff abilities like Pathfinder's Mark, Anchor Point slows, etc.
    /// </summary>
    [Serializable]
    public class ModifySpeedEffect : AbilityEffect
    {
        [Header("Speed Modification")]
        [Tooltip("Is magnitude a multiplier (true) or flat bonus (false)?")]
        public bool isMultiplier = true;

        [Tooltip("Unique identifier for this speed modifier (for stacking/removal)")]
        public string modifierID = "SpeedModifier";

        public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
        {
            if (!ShouldApplyToTarget(caster, target))
                return;

            // Try to find IMovable interface on target
            if (target.TryGetComponent<IMovable>(out var movable))
            {
                if (isMultiplier)
                {
                    movable.AddSpeedMultiplier(modifierID, magnitude);
                }
                else
                {
                    movable.AddSpeedBonus(modifierID, magnitude);
                }
            }
            else
            {
                Debug.LogWarning($"Target {target.name} does not implement IMovable, cannot modify speed");
            }
        }

        public override void Remove(GameObject target)
        {
            // Remove the speed modifier
            if (target != null && target.TryGetComponent<IMovable>(out var movable))
            {
                movable.RemoveSpeedModifier(modifierID);
            }
        }
    }
}
