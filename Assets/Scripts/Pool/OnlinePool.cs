using Unity.Netcode;
using UnityEngine;

public abstract class OnlinePool<T> : PoolBase<T> where T : Component
{
    protected override T CreateInstance()
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError($"Client tried to create poolable item of type {nameof(T)}");
            return null;
        }

        var instance = base.CreateInstance();
        var networkObj = instance.GetComponent<NetworkObject>();
        if(!networkObj.IsSpawned)
        { 
            networkObj.Spawn();
        }
        return instance;
    }

    protected override void CreateInitialItems()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        base.CreateInitialItems();
    }
}
