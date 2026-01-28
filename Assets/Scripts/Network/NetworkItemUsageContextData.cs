using Unity.Netcode;
using UnityEngine;

public struct NetworkItemUsageContextData : INetworkSerializable
{
    public Vector2 AimOrigin;
    public Vector2 AimVector;
    // owner does not need to be networked

    public NetworkItemUsageContextData(ItemUsageContext itemUsageContext)
    {
        AimOrigin = itemUsageContext.AimOrigin;
        AimVector = itemUsageContext.AimVector;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref AimOrigin);
        serializer.SerializeValue(ref AimVector);
    }

    public ItemUsageContext ToItemUsageContext(Character owner)
    {
        return new ItemUsageContext(
            this.AimOrigin,
            this.AimVector,
            owner
            );
    }
}
