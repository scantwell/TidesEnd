using UnityEngine;

namespace TidesEnd.Combat {
    public interface IDamageable
    {
        void TakeDamage(DamageInfo info);
        bool IsAlive { get; }
        float CurrentHealth { get; }
        float MaxHealth { get; }
        GameObject GameObject { get; }
        Transform Transform { get; }
    }
}