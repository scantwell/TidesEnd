using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TidesEnd.Abilities;
using Unity.Cinemachine;

namespace TidesEnd.Player
{
    /// <summary>
    /// Handles player input for abilities (Q and E keys).
    /// Builds appropriate AbilityContext based on targeting mode and sends ServerRpc.
    /// </summary>
    public class PlayerAbilityInput : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private AbilityUser abilityUser;
        [SerializeField] private Camera playerCamera;

        [Header("Targeting")]
        [SerializeField] private float maxGroundTargetDistance = 50f;
        [SerializeField] private LayerMask groundLayerMask = ~0; // Default: all layers

        [Header("Visual Feedback")]
        [SerializeField] private bool showTargetingIndicators = true;
        [SerializeField] private Color ability1Color = new Color(1f, 0.5f, 0f, 0.7f); // Orange for Q
        [SerializeField] private Color ability2Color = new Color(0.5f, 0f, 1f, 0.7f); // Purple for E

        private void Awake()
        {
            // Find AbilityUser if not assigned
            if (abilityUser == null)
                abilityUser = GetComponent<AbilityUser>();

            // Find camera if not assigned
            if (playerCamera == null)
                playerCamera =  Camera.main;
        }

        private void Update()
        {
            // Only process input for local player
            if (!IsOwner) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // Q key - Ability slot 0
            if (keyboard.qKey.wasPressedThisFrame)
            {
                TryActivateAbility(0);
            }

            // E key - Ability slot 1
            if (keyboard.eKey.wasPressedThisFrame)
            {
                TryActivateAbility(1);
            }
        }

        /// <summary>
        /// Attempt to activate an ability in the specified slot.
        /// Builds appropriate context and sends ServerRpc.
        /// </summary>
        private void TryActivateAbility(int abilityIndex)
        {
            if (abilityUser == null)
            {
                Debug.LogWarning("[PlayerAbilityInput] No AbilityUser component found!");
                return;
            }

            // Get the ability data to determine targeting mode
            AbilityData abilityData = abilityUser.GetAbilityData(abilityIndex);
            if (abilityData == null)
            {
                Debug.LogWarning($"[PlayerAbilityInput] No ability in slot {abilityIndex}");
                return;
            }

            // Build context based on targeting mode
            AbilityContext context = BuildAbilityContext(abilityData);

            // Request activation on server via AbilityUser's ServerRpc
            abilityUser.ActivateAbilityServerRpc(abilityIndex, context);
        }

        /// <summary>
        /// Build AbilityContext based on the ability's targeting mode.
        /// </summary>
        private AbilityContext BuildAbilityContext(AbilityData abilityData)
        {
            switch (abilityData.targetingMode)
            {
                case TargetingMode.Self:
                    return AbilityContext.CreateSelfContext(gameObject);

                case TargetingMode.Ground:
                    return BuildGroundTargetContext(abilityData);

                case TargetingMode.Direction:
                    return BuildDirectionContext();

                case TargetingMode.SingleEnemy:
                case TargetingMode.SingleAlly:
                    return BuildTargetContext(abilityData.targetingMode);

                case TargetingMode.AllEnemies:
                case TargetingMode.AllAllies:
                case TargetingMode.Random:
                    // These modes don't require specific targeting from input
                    return AbilityContext.CreateSelfContext(gameObject);

                default:
                    Debug.LogWarning($"[PlayerAbilityInput] Unhandled targeting mode: {abilityData.targetingMode}");
                    return AbilityContext.CreateSelfContext(gameObject);
            }
        }

        /// <summary>
        /// Build context for ground targeting (raycast to ground).
        /// </summary>
        private AbilityContext BuildGroundTargetContext(AbilityData abilityData)
        {
            if (playerCamera == null)
            {
                Debug.LogWarning("[PlayerAbilityInput] No camera assigned for ground targeting!");
                return AbilityContext.CreateSelfContext(gameObject);
            }

            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, abilityData.range, groundLayerMask))
            {
                return AbilityContext.CreateGroundContext(hit.point);
            }

            // Fallback: Use position in front of player
            Vector3 fallbackPosition = transform.position + transform.forward * 5f;
            return AbilityContext.CreateGroundContext(fallbackPosition);
        }

        /// <summary>
        /// Build context for directional abilities (camera forward direction).
        /// </summary>
        private AbilityContext BuildDirectionContext()
        {
            if (playerCamera == null)
            {
                return new AbilityContext
                {
                    targetDirection = transform.forward
                };
            }

            return new AbilityContext
            {
                targetDirection = playerCamera.transform.forward
            };
        }

        /// <summary>
        /// Build context for targeted abilities (raycast to enemy/ally).
        /// </summary>
        private AbilityContext BuildTargetContext(TargetingMode targetingMode)
        {
            if (playerCamera == null)
            {
                Debug.LogWarning("[PlayerAbilityInput] No camera assigned for target selection!");
                return AbilityContext.CreateSelfContext(gameObject);
            }

            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, maxGroundTargetDistance))
            {
                GameObject targetObject = hit.collider.gameObject;

                // TODO: Add faction/team checking for SingleEnemy vs SingleAlly
                // For now, accept any target
                //return AbilityContext.CreateTargetContext(targetObject, hit.point);
            }

            // No target found - fall back to self
            return AbilityContext.CreateSelfContext(gameObject);
        }

        /// <summary>
        /// Draw runtime targeting indicators for abilities.
        /// Shows where abilities will land/activate during gameplay.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showTargetingIndicators || !Application.isPlaying || !IsOwner)
                return;

            if (abilityUser == null || playerCamera == null)
                return;

            // Draw targeting indicator for ability slot 0 (Q key)
            DrawAbilityTargeting(0, ability1Color);

            // Draw targeting indicator for ability slot 1 (E key)
            DrawAbilityTargeting(1, ability2Color);
        }

        /// <summary>
        /// Draw targeting visualization for a specific ability slot.
        /// </summary>
        private void DrawAbilityTargeting(int abilityIndex, Color color)
        {
            AbilityData ability = abilityUser.GetAbilityData(abilityIndex);
            if (ability == null) return;

            // Check if ability is on cooldown
            bool onCooldown = abilityUser.IsOnCooldown(abilityIndex);
            if (onCooldown)
            {
                // Darken color if on cooldown
                color = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, color.a * 0.5f);
            }

            switch (ability.targetingMode)
            {
                case TargetingMode.Ground:
                    DrawGroundTargeting(ability, color);
                    break;

                case TargetingMode.Direction:
                    DrawDirectionalTargeting(ability, color);
                    break;

                case TargetingMode.Self:
                    DrawSelfTargeting(ability, color);
                    break;

                case TargetingMode.SingleEnemy:
                case TargetingMode.SingleAlly:
                    DrawTargetedTargeting(ability, color);
                    break;
            }
        }

        /// <summary>
        /// Draw ground targeting indicator (circle on ground where ability will land).
        /// </summary>
        private void DrawGroundTargeting(AbilityData ability, Color color)
        {
            //Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            color = Color.red;
            Vector3 targetPosition;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, ability.range, groundLayerMask))
            {
                targetPosition = hit.point;
            }
            else
            {
                // Fallback position
                targetPosition = transform.position + transform.forward * 5f;
            }

            // Draw range circle around player
            if (ability.range > 0)
            {
                GizmoHelpers.DrawWireCircle(transform.position, ability.range, color * 0.5f, 64);
            }

            // Draw radius circle at target position
            if (ability.radius > 0)
            {
                GizmoHelpers.DrawWireCircle(targetPosition, ability.radius, color, 32);
                GizmoHelpers.DrawFilledCircle(targetPosition, ability.radius, color * 0.3f, 32);
            }
            else
            {
                // Draw small crosshair if no radius
                GizmoHelpers.DrawWireSphere(targetPosition, 0.25f, color);
            }
        }

        /// <summary>
        /// Draw directional targeting indicator (arrow showing direction).
        /// </summary>
        private void DrawDirectionalTargeting(AbilityData ability, Color color)
        {
            Vector3 direction = playerCamera.transform.forward;
            float range = ability.range > 0 ? ability.range : 10f;

            // Draw arrow showing direction
            GizmoHelpers.DrawArrow(transform.position + Vector3.up * 1f, direction, range, color, 0.15f);

            // If ability has a radius, draw cylinder along direction
            if (ability.radius > 0)
            {
                Vector3 endPoint = transform.position + direction * range;
                GizmoHelpers.DrawWireSphere(endPoint, ability.radius, color);
            }
        }

        /// <summary>
        /// Draw self-targeting indicator (circle around player).
        /// </summary>
        private void DrawSelfTargeting(AbilityData ability, Color color)
        {
            if (ability.radius > 0)
            {
                // Draw radius around self
                GizmoHelpers.DrawWireCircle(transform.position, ability.radius, color, 48);
                GizmoHelpers.DrawWireSphere(transform.position + Vector3.up * 1f, ability.radius, color * 0.5f);
            }
        }

        /// <summary>
        /// Draw targeted ability indicator (ray to target).
        /// </summary>
        private void DrawTargetedTargeting(AbilityData ability, Color color)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, maxGroundTargetDistance))
            {
                // Draw line from player to target
                Gizmos.color = color;
                Gizmos.DrawLine(transform.position + Vector3.up * 1f, hit.point);

                // Draw sphere at target
                GizmoHelpers.DrawWireSphere(hit.point, 0.5f, color);

                // Draw radius if ability has AOE
                if (ability.radius > 0)
                {
                    GizmoHelpers.DrawWireCircle(hit.point, ability.radius, color, 32);
                }
            }

            // Draw max range circle
            if (ability.range > 0)
            {
                GizmoHelpers.DrawWireCircle(transform.position, ability.range, color * 0.4f, 48);
            }
        }
    }
}
