using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using TidesEnd.Combat;

namespace TidesEnd.Enemy
{
    /// <summary>
    /// Basic Enemy AI that can idle, chase players, and perform melee attacks.
    /// Uses Unity Netcode for multiplayer synchronization.
    /// </summary>

    public class EnemyAI : NetworkBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float detectionRange = 20f; // How far can see players
        [SerializeField] private float attackRange = 2f; // Melee attack distance
        [SerializeField] private float attackCooldown = 1.5f; // Time between attacks
        [SerializeField] private float attackDamage = 10f;

        [Header("Movement")]
        [SerializeField] private float chaseSpeed = 3.5f;
        [SerializeField] private float patrolSpeed = 2f;

        [Header("References")]
        [SerializeField] private Transform attackPoint; // Where attack originates

        // Components
        private NavMeshAgent agent;
        private Health health;
        private Animator animator; // Optional

        // State
        private AIState currentState = AIState.Idle;
        private Transform targetPlayer;
        private float lastAttackTime;
        private Vector3 startPosition;

        // Network sync
        private NetworkVariable<AIState> networkState = new NetworkVariable<AIState>(
            AIState.Idle,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            animator = GetComponentInChildren<Animator>();

            if (attackPoint == null)
                attackPoint = transform;

            startPosition = transform.position;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (health != null)
            {
                health.OnDied += OnDeath;
            }

            // Listen for state changes
            networkState.OnValueChanged += OnStateChanged;

            // Only server runs AI logic
            if (!IsServer)
            {
                // Clients disable NavMeshAgent (server controls movement)
                if (agent != null)
                    agent.enabled = false;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (health != null)
            {
                health.OnDied -= OnDeath;
            }

            networkState.OnValueChanged -= OnStateChanged;

            base.OnNetworkDespawn();
        }

        private void Update()
        {
            // Only server runs AI
            if (!IsServer) return;
            if (health != null && !health.IsAlive) return;

            // Run current state behavior
            switch (currentState)
            {
                case AIState.Idle:
                    IdleState();
                    break;
                case AIState.Chase:
                    ChaseState();
                    break;
                case AIState.Attack:
                    AttackState();
                    break;
            }
        }

        // ==================== STATE BEHAVIORS ====================

        private void IdleState()
        {
            // Look for players
            FindNearestPlayer();

            if (targetPlayer != null)
            {
                float distance = Vector3.Distance(transform.position, targetPlayer.position);

                if (distance <= detectionRange)
                {
                    // Found player - start chasing
                    TransitionToState(AIState.Chase);
                }
            }

            // Optional: Random patrol/wander
            // TODO: Add patrol behavior here
        }

        private void ChaseState()
        {
            if (targetPlayer == null || !IsPlayerValid(targetPlayer))
            {
                // Lost target
                TransitionToState(AIState.Idle);
                return;
            }

            float distance = Vector3.Distance(transform.position, targetPlayer.position);

            // Check if too far away
            if (distance > detectionRange * 1.5f) // Add hysteresis
            {
                TransitionToState(AIState.Idle);
                return;
            }

            // Check if close enough to attack
            if (distance <= attackRange)
            {
                TransitionToState(AIState.Attack);
                return;
            }

            // Move toward player
            if (agent != null && agent.enabled)
            {
                agent.speed = chaseSpeed;
                agent.SetDestination(targetPlayer.position);
            }
        }

        private void AttackState()
        {
            if (targetPlayer == null || !IsPlayerValid(targetPlayer))
            {
                TransitionToState(AIState.Idle);
                return;
            }

            float distance = Vector3.Distance(transform.position, targetPlayer.position);

            // Player moved away - chase again
            if (distance > attackRange * 1.2f) // Add hysteresis
            {
                TransitionToState(AIState.Chase);
                return;
            }

            // Stop moving
            if (agent != null && agent.enabled)
            {
                agent.SetDestination(transform.position);
            }

            // Face player
            Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
            directionToPlayer.y = 0; // Keep on horizontal plane

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // Attack if cooldown ready
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
        }

        // ==================== ACTIONS ====================

        private void PerformAttack()
        {
            lastAttackTime = Time.time;

            Debug.Log($"[Server] {gameObject.name} attacking {targetPlayer.name}");

            // Play attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // Deal damage to player
            if (targetPlayer != null)
            {
                Health playerHealth = targetPlayer.GetComponent<Health>();
                if (playerHealth != null && playerHealth.IsAlive)
                {
                    playerHealth.TakeDamage(
                        attackDamage,
                        NetworkObjectId,
                        targetPlayer.position,
                        Vector3.up
                    );
                }
            }

            // Notify clients to play attack animation
            PlayAttackAnimationClientRpc();
        }

        [ClientRpc]
        private void PlayAttackAnimationClientRpc()
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }

        // ==================== DETECTION ====================

        private void FindNearestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            float nearestDistance = float.MaxValue;
            Transform nearest = null;

            foreach (GameObject playerObj in players)
            {
                if (!IsPlayerValid(playerObj.transform))
                    continue;

                float distance = Vector3.Distance(transform.position, playerObj.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = playerObj.transform;
                }
            }

            targetPlayer = nearest;
        }

        private bool IsPlayerValid(Transform player)
        {
            if (player == null) return false;

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth == null || !playerHealth.IsAlive)
                return false;

            return true;
        }

        // ==================== STATE MANAGEMENT ====================

        private void TransitionToState(AIState newState)
        {
            if (currentState == newState) return;

            // Exit current state
            OnStateExit(currentState);

            // Enter new state
            currentState = newState;
            networkState.Value = newState;

            OnStateEnter(newState);

            Debug.Log($"[AI] {gameObject.name} → {newState}");
        }

        private void OnStateEnter(AIState state)
        {
            switch (state)
            {
                case AIState.Idle:
                    if (agent != null) agent.speed = patrolSpeed;
                    break;

                case AIState.Chase:
                    if (agent != null) agent.speed = chaseSpeed;
                    break;

                case AIState.Attack:
                    break;
            }
        }

        private void OnStateExit(AIState state)
        {
            // Cleanup when leaving state
        }

        private void OnStateChanged(AIState oldState, AIState newState)
        {
            // Clients sync animation state
            if (animator != null)
            {
                animator.SetInteger("State", (int)newState);
            }
        }

        // ==================== DEATH ====================

        private void OnDeath()
        {
            Debug.Log($"{gameObject.name} died!");

            // Disable AI
            if (agent != null)
                agent.enabled = false;

            this.enabled = false;

            // Transition to dead state
            if (IsServer)
            {
                networkState.Value = AIState.Dead;
            }
        }

        // ==================== DEBUG ====================

        private void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Line to target
            if (targetPlayer != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, targetPlayer.position);
            }
        }
    }

    public enum AIState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }
}
