using UnityEngine;

namespace TidesEnd.Combat {
    public class DamageNotifier : MonoBehaviour
    {
    // In another component
        private void Start()
        {
            Health health = GetComponent<Health>();

            health.OnDamaged += HandleDamage;
            health.OnDied += HandleDeath;
            health.OnRevived += HandleRevive;
        }

        private void HandleDamage(DamageInfo info)
        {
            Debug.Log($"Took {info.BaseDamage} damage from attacker {info.AttackerId}");
            
            // Play damage sound, screen shake, etc.
        }

        private void HandleDeath()
        {
            Debug.Log("Player died!");
            
            // Show death screen, respawn menu, etc.
        }

        private void HandleRevive()
        {
            Debug.Log("Player revived!");

            // Show revive effect, enable HUD, etc.
        }
    }
}

