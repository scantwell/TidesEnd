using System;
using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// Effect that teleports the caster to a new position.
    /// Used by Dimensional Shambler teleport, Reef Leviathan submerge, etc.
    /// </summary>
    [Serializable]
    public class TeleportEffect : AbilityEffect
    {
        public override EffectType EffectType => EffectType.Teleport;

        [Header("Teleport Parameters")]
        [Tooltip("How to determine the teleport destination")]
        public TeleportMode mode = TeleportMode.ToPosition;

        [Tooltip("Maximum teleport range")]
        public float teleportRange = 20f;

        [Tooltip("For BehindTarget mode: distance behind target")]
        public float behindDistance = 10f;

        [Tooltip("Make caster invisible during teleport?")]
        public bool invisibleDuringTeleport = false;

        [Tooltip("Duration of invisibility (if enabled)")]
        public float invisibilityDuration = 0.5f;

        public override void Apply(AbilityUser caster, GameObject target, AbilityContext context)
        {
            Vector3 destination = CalculateTeleportDestination(caster, context);

            // Validate destination (check if on NavMesh, not inside walls, etc.)
            if (!IsValidTeleportDestination(destination))
            {
                Debug.LogWarning($"Invalid teleport destination: {destination}");
                return;
            }

            // Make invisible if configured
            if (invisibleDuringTeleport && caster.TryGetComponent<Renderer>(out var renderer))
            {
                renderer.enabled = false;
                // Re-enable after duration
                caster.StartCoroutine(ReenableRendererAfterDelay(renderer, invisibilityDuration));
            }

            // Actually teleport
            caster.transform.position = destination;
        }

        public override void Remove(GameObject target)
        {
            // Teleport is instant, no removal needed
        }

        private Vector3 CalculateTeleportDestination(AbilityUser caster, AbilityContext context)
        {
            switch (mode)
            {
                case TeleportMode.ToPosition:
                    return context.targetPosition;

                case TeleportMode.ToTarget:
                    if (context.targetEntity != null)
                    {
                        return context.targetEntity.transform.position;
                    }
                    break;

                case TeleportMode.BehindTarget:
                    if (context.targetEntity != null)
                    {
                        Vector3 targetForward = context.targetEntity.transform.forward;
                        return context.targetEntity.transform.position - targetForward * behindDistance;
                    }
                    break;

                case TeleportMode.RandomNearby:
                    Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * teleportRange;
                    return caster.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            }

            // Fallback to caster's current position
            return caster.transform.position;
        }

        private bool IsValidTeleportDestination(Vector3 position)
        {
            // TODO: Add validation
            // - Check if on NavMesh
            // - Check if not inside colliders
            // - Check if within map bounds
            return true;
        }

        private System.Collections.IEnumerator ReenableRendererAfterDelay(Renderer renderer, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (renderer != null)
            {
                renderer.enabled = true;
            }
        }
    }

    /// <summary>
    /// Determines how the teleport destination is calculated.
    /// </summary>
    public enum TeleportMode
    {
        ToPosition,     // Teleport to context.targetPosition
        ToTarget,       // Teleport to context.targetEntity position
        BehindTarget,   // Teleport behind context.targetEntity
        RandomNearby    // Random position within range
    }
}