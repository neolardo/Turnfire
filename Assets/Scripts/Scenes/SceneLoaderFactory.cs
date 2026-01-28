using System;
using Unity.Netcode;
using UnityEngine;

public class SceneLoaderFactory : MonoBehaviour
{
    [SerializeField] private OfflineSceneLoader _onlineSceneLoaderPrefab;
    [SerializeField] private OnlineSceneLoader _offlineSceneLoaderPrefab;

    public OfflineSceneLoader TryCreateSceneLoader()
    {
        if (OfflineSceneLoader.Instance != null)
        {
            return OfflineSceneLoader.Instance;
        }

        try
        {
            var sceneLoader = Instantiate(_onlineSceneLoaderPrefab);
            return sceneLoader;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }

    public OnlineSceneLoader TrySpawnNetworkSceneLoader()
    {
        if (OnlineSceneLoader.Instance != null)
        {
            return OnlineSceneLoader.Instance;
        }

        try
        {
            var networkSceneLoader = Instantiate(_offlineSceneLoaderPrefab);
            networkSceneLoader.GetComponent<NetworkObject>().Spawn();
            return networkSceneLoader;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }
}
