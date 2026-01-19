using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class NetworkObjectPool : INetworkPrefabInstanceHandler
{
    private readonly GameObject _prefab;
    private readonly Stack<NetworkObject> _pool = new();

    public NetworkObjectPool(GameObject prefab, int prewarmCount = 0)
    {
        _prefab = prefab;

        for (int i = 0; i < prewarmCount; i++)
        {
            var obj = CreateInstance();
            _pool.Push(obj);
        }
    }

    private NetworkObject CreateInstance()
    {
        var go = Object.Instantiate(_prefab);
        go.SetActive(false);
        return go.GetComponent<NetworkObject>();
    }

    // Called by NGO on Spawn()
    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        NetworkObject netObj = _pool.Count > 0
            ? _pool.Pop()
            : CreateInstance();

        var go = netObj.gameObject;
        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);

        return netObj;
    }

    // Called by NGO on Despawn()
    public void Destroy(NetworkObject networkObject)
    {
        networkObject.gameObject.SetActive(false);
        _pool.Push(networkObject);
    }
}
