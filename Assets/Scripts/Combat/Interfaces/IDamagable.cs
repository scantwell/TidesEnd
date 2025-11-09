using UnityEngine;

namespace TidesEnd.Combat {
    public interface IDamageable
    {
        void TakeDamage(float damage, ulong attackerId = 0, Vector3 hitPoint = default, Vector3 hitNormal = default);
        bool IsAlive { get; }
        float CurrentHealth { get; }
        float MaxHealth { get; }
        GameObject GameObject { get; }
        Transform Transform { get; }
    }
}