using Unity.Netcode;
using UnityEngine;

public abstract class OnlinePool<T> : PoolBase<T> where T : Component, IPoolable
{
    protected override void Awake()
    {
        var pool = new NetworkObjectPool(_prefab.gameObject, _initialSize);
        NetworkManager.Singleton.PrefabHandler.AddHandler(
            _prefab.gameObject,
            pool
        );
        base.Awake();
    }

    private void OnDestroy()
    {
        if(_prefab != null)
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(_prefab.gameObject);
        }
    }

    protected override T CreateInstance()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError($"Client tried to create poolable item of type {nameof(T)}");
            return null;
        }
        return base.CreateInstance();
    }

    public override T Get()
    {
        var item = base.Get();
        var networkObj = item.GetComponent<NetworkObject>();
        item.gameObject.SetActive(true);
        if (!networkObj.IsSpawned)
        {
            networkObj.Spawn();
        }
        return item;
    }

    public override T GetAndPlace(Vector2 position)
    {
        var item = base.Get();
        var networkObj = item.GetComponent<NetworkObject>();
        item.transform.position = position;
        item.gameObject.SetActive(true);
        if (!networkObj.IsSpawned)
        {
            networkObj.Spawn();
        }
        return item;
    }

    public override void Release(T item)
    {
        var networkObj = item.GetComponent<NetworkObject>();
        if (networkObj.IsSpawned)
        {
            networkObj.Despawn();
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
