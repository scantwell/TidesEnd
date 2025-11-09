using UnityEngine;
using Unity.Netcode;

namespace TidesEnd.Combat {
    public struct DamageInfo : INetworkSerializable
    {
        public float Damage;
        public DamageType Type;
        public ulong AttackerId;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        
        public DamageInfo(float damage, DamageType type = DamageType.Normal, ulong attackerId = 0, Vector3 hitPoint = default, Vector3 hitNormal = default)
        {
            Damage = damage;
            Type = type;
            AttackerId = attackerId;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }
        
        // INetworkSerializable implementation
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Damage);
            serializer.SerializeValue(ref Type);
            serializer.SerializeValue(ref AttackerId);
            serializer.SerializeValue(ref HitPoint);
            serializer.SerializeValue(ref HitNormal);
        }
    }  
}

