using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneLoader : NetworkBehaviour
{
    private readonly HashSet<ulong> _readyClients = new();
    public static NetworkSceneLoader Instance { get; private set; }
    public GameplaySceneSettings CurrentGameplaySceneSettings
    {
        get 
        {
            return _networkSceneSettings.Value.IsValid ? _networkSceneSettings.Value.ToSceneSettings(_mapLocator) : null;
        }
        set
        {
            if(!IsServer)
            {
                return;
            }
            _networkSceneSettings.Value = NetworkGameplaySceneSettingsData.ToNetworkData(value);
        }
    }

    public bool AllClientsHaveSpawned { get; private set; }

    private NetworkVariable<NetworkGameplaySceneSettingsData> _networkSceneSettings = new (
                 new NetworkGameplaySceneSettingsData() { IsValid = false},
                 NetworkVariableReadPermission.Everyone,
                 NetworkVariableWritePermission.Server
            );

    private LoadingTextUI _loadingText;
    private MapLocator _mapLocator;


    public override void OnNetworkSpawn()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
        NetworkManager.Singleton.SceneManager.OnLoad += OnSceneLoadStarted;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
        if (IsClient)
        {
            NotifyServerReadyServerRpc();
        }
        Debug.Log($"{nameof(NetworkSceneLoader)} spawned");
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

    private void OnSceneLoadStarted(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
    {
        _loadingText.gameObject.SetActive(true);
        Debug.Log($"Started loading scene: {sceneName}");
    }

    private void OnSceneLoadCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        _loadingText = FindFirstObjectByType<LoadingTextUI>(FindObjectsInactive.Include);
        if(sceneName != Constants.MenuSceneName)
        {
            _mapLocator = FindFirstObjectByType<MapLocator>();
        }
        Debug.Log($"Finished loading scene: {sceneName}");
    }


    public void LoadMenuScene()
    {
        if(!IsServer)
        {
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene(Constants.MenuSceneName, LoadSceneMode.Single);
    }

    public void LoadGameplayScene(GameplaySceneSettings settings)
    {
        if (!IsServer)
        {
            return;
        }
        CurrentGameplaySceneSettings = settings;
        NetworkManager.Singleton.SceneManager.LoadScene(settings.Map.SceneName, LoadSceneMode.Single);
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
