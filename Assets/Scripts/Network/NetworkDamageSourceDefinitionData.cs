using Unity.Netcode;
using UnityEngine;

public struct NetworkDamageSourceDefinitionData : INetworkSerializable
{
    public int SourceDefinitionId;
    public DamageSourceType Type;

    public NetworkDamageSourceDefinitionData(IDamageSourceDefinition damageSource)
    {
        SourceDefinitionId = damageSource.SourceDefinitionId;
        Type = damageSource.Type;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref SourceDefinitionId);
        serializer.SerializeValue(ref Type);
    }

    public IDamageSourceDefinition ToDamageSource()
    {
        if(Type == DamageSourceType.Weapon)
        {
            return GameServices.ItemDatabase.GetById(SourceDefinitionId) as IDamageSourceDefinition;
        }
        else if(Type == DamageSourceType.Projectile)
        {
            return GameServices.ProjectileDatabase.GetById(SourceDefinitionId);
        }
        else
        {
            Debug.LogError($"Invalid {nameof(DamageSourceType)} when converting network data: {Type}"); 
            return null;
        }
    }
}
