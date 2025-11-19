using Unity.Netcode;

namespace TidesEnd.Combat {
    public struct DamageInfo : INetworkSerializable
    {
        // Base values
        public float BaseDamage;
        public DamageType DamageType;
        public DamageSource Source;
        
        // Hit context
        public bool IsHeadshot;
        public bool IsCritical;
        public float Distance;
        
        // Source reference
        public ulong AttackerId;
        public int SourceId; // Weapon ID or Ability ID
        
        // Optional: Pre-calculated source properties (for optimization)
        public float HeadshotMultiplier;
        public float EffectiveRange;
        public float MaxRange;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref BaseDamage);
            serializer.SerializeValue(ref DamageType);
            serializer.SerializeValue(ref Source);
            serializer.SerializeValue(ref IsHeadshot);
            serializer.SerializeValue(ref IsCritical);
            serializer.SerializeValue(ref Distance);
            serializer.SerializeValue(ref AttackerId);
            serializer.SerializeValue(ref SourceId);
            serializer.SerializeValue(ref HeadshotMultiplier);
            serializer.SerializeValue(ref EffectiveRange);
            serializer.SerializeValue(ref MaxRange);
        }
    }
}

