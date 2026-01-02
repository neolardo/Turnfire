using System;
using Unity.Netcode;
using UnityEngine;

public class SceneLoaderFactory : MonoBehaviour
{
    [SerializeField] private SceneLoader _sceneLoaderPrefab;
    [SerializeField] private NetworkSceneLoader _networkSceneLoaderPrefab;

    public SceneLoader TryCreateSceneLoader()
    {
        if (SceneLoader.Instance != null)
        {
            return SceneLoader.Instance;
        }

        try
        {
            var sceneLoader = Instantiate(_sceneLoaderPrefab);
            return sceneLoader;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }

    public NetworkSceneLoader TrySpawnNetworkSceneLoader()
    {
        if (NetworkSceneLoader.Instance != null)
        {
            return NetworkSceneLoader.Instance;
        }

        try
        {
            var networkSceneLoader = Instantiate(_networkSceneLoaderPrefab);
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
