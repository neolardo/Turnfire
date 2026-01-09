using Unity.Netcode;
using UnityEngine;

public class OnlineIdGenerator : NetworkBehaviour, IIdGenerator
{
    private NetworkVariable<int> _lastId = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _lastId.Value = IIdGenerator.InitialId;
    }
    public int GenerateId()
    {
        if(!IsServer)
        {
            Debug.LogWarning("Client tried to generate id.");
            return IIdGenerator.InvalidId; 
        }
        _lastId.Value++;
        return _lastId.Value;
    }
}
