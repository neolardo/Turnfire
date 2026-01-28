using System;
using Unity.Netcode;
using UnityEngine;

public class OnlineTerrainExplosionRegistry : NetworkBehaviour,ITerrainExplosionRegistry
{
    public event Action<Vector2, float> ExplosionRegistered;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        var terrain = FindFirstObjectByType<TerrainManager>();
        terrain.SetExplosionRegistry(this);
    }

    public void RegisterExplosion(Vector2 worldPos, float radius)
    {
        if(!IsServer)
        {
            return;
        }
        InvokeExplosionRegisteredClientRpc(worldPos, radius);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeExplosionRegisteredClientRpc(Vector2 worldPos, float radius)
    {
        ExplosionRegistered?.Invoke(worldPos, radius);
    }
}
