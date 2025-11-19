// Player.cs - The "root" player component
using System;
using Unity.Netcode;

namespace TidesEnd.Player
{
    public class PlayerRoot : NetworkBehaviour
    {
        public static event Action<PlayerRoot> OnLocalPlayerSpawned;
        public static PlayerRoot LocalInstance { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                LocalInstance = this;
                OnLocalPlayerSpawned?.Invoke(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                LocalInstance = null;
            }
        }
    }
}