using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineSceneLoader : NetworkBehaviour, ISceneLoader
{
    private readonly HashSet<ulong> _readyClients = new();
    public static OnlineSceneLoader Instance { get; private set; }
    public GameplaySceneSettings CurrentGameplaySceneSettings => GameplaySceneSettingsStorage.Current;

    public bool AllClientsHaveSpawned { get; private set; }

    private MapLocator _mapLocator;

    public override void OnNetworkSpawn()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _mapLocator = FindFirstObjectByType<MapLocator>();
        if (IsClient)
        {
            NotifyServerReadyServerRpc();
        }
        Debug.Log($"{nameof(OnlineSceneLoader)} spawned");
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void NotifyServerReadyServerRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (_readyClients.Add(clientId))
        {
            Debug.Log($"Client {clientId} ready ({_readyClients.Count}/{NetworkManager.ConnectedClients.Count})");
        }

        RefreshClientsHaveSpawned();
    }

    private void RefreshClientsHaveSpawned()
    {
        if(!IsServer)
        {
            return;
        }

        bool value = true;
        foreach (var clientId in NetworkManager.ConnectedClientsIds)
        {
            if (!_readyClients.Contains(clientId))
            {
                value = false;
                break;
            }
        }
        AllClientsHaveSpawned = value;
    }

    public void LoadMenuScene()
    {
        if(!IsServer)
        {
            return;
        }
        SaveGameplaySceneSettingsClientRpc(NetworkGameplaySceneSettingsData.ToNetworkData(null));
        NetworkManager.Singleton.SceneManager.LoadScene(Constants.MenuSceneName, LoadSceneMode.Single);
    }

    public void LoadGameplayScene(GameplaySceneSettings settings)
    {
        if (!IsServer)
        {
            return;
        }
        SaveGameplaySceneSettingsClientRpc(NetworkGameplaySceneSettingsData.ToNetworkData(settings));
        //TODO: ack?
        NetworkManager.Singleton.SceneManager.LoadScene(settings.Map.SceneName, LoadSceneMode.Single);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    void SaveGameplaySceneSettingsClientRpc(NetworkGameplaySceneSettingsData networkSettings)
    {
        GameplaySceneSettingsStorage.Current = networkSettings.ToSceneSettings(_mapLocator);
    }

    public void ReloadScene()
    {
        if (!IsServer)
        {
            return;
        }
        var sceneName = SceneManager.GetActiveScene().name;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
