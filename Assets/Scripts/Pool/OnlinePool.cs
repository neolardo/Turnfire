using Unity.Netcode;
using UnityEngine;

public abstract class OnlinePool<T> : PoolBase<T> where T : Component
{
    protected override T CreateInstance()
    {
        var instance = base.CreateInstance();
        var networkObj = instance.GetComponent<NetworkObject>();
        if(!networkObj.IsSpawned)
        { 
            networkObj.Spawn();
        }
        return instance;
    }
}
