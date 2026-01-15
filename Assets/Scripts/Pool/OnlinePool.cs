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

        return base.CreateInstance();
    }

    public override T Get()
    {
        var item = base.Get();
        if (NetworkManager.Singleton.IsServer)
        {
            var networkObj = item.GetComponent<NetworkObject>();
            if (!networkObj.IsSpawned)
            {
                networkObj.Spawn();
            }
        }
        return item;
    }

    public override void Release(T item)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var networkObj = item.GetComponent<NetworkObject>();
            if (networkObj.IsSpawned)
            {
                networkObj.Despawn();
            }
        }
        base.Release(item);
    }

    protected override void ReparentItemOnRelease(T item) { }

    protected override void CreateInitialItems()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        base.CreateInitialItems();
    }
}
