using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineSceneLoader : NetworkBehaviour, ISceneLoader
{
    private readonly HashSet<ulong> _clientsSpawned = new();
    private readonly HashSet<ulong> _clientsLoadedScene = new();

    private NetworkVariable<bool> _spawnConfirmationRequested= new NetworkVariable<bool>();
    public static OnlineSceneLoader Instance { get; private set; }
    public GameplaySceneSettings CurrentGameplaySceneSettings => GameplaySceneSettingsStorage.Current;
    public bool AllClientsHaveSpawned { get; private set; }

    private MapLocator _mapLocator;
    private const float GameplaySceneLoadTimeoutSeconds = 10f;
    private const float GameplaySceneLoadYieldInterval = .2f;

    public override void OnNetworkSpawn()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _mapLocator = FindFirstObjectByType<MapLocator>();

        if (IsServer)
        {
            NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
        }
        _spawnConfirmationRequested.OnValueChanged += OnSpawnConfirmationRequested;
        if(_spawnConfirmationRequested.Value)
        {
            NotifyServerObjectSpawnedServerRpc();
        }
        GameServices.Register(this);
        Debug.Log($"{nameof(OnlineSceneLoader)} spawned");
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager != null && NetworkManager.SceneManager != null)
        {
            NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
        }
        _spawnConfirmationRequested.OnValueChanged -= OnSpawnConfirmationRequested;
        base.OnNetworkDespawn();
    }

    private void DespawnAllRuntimeSpawnedObjects()
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer)
            return;

        var spawnManager = NetworkManager.Singleton.SpawnManager;

        var spawnedObjects = spawnManager.SpawnedObjectsList.ToArray();
        var sceneLoaderNetObj = GetComponent<NetworkObject>();
        foreach (var obj in spawnedObjects)
        {
            if (obj == null || obj == sceneLoaderNetObj)
                continue;

            // Scene objects should NOT be despawned here (they belong to the scene)
            if (obj.IsSceneObject.Value)
                continue;

            obj.Despawn(true);
        }
    }

    #region Spawn Confirmation

    public void RequestSpawnConfirmation()
    {
        if (!IsServer)
        {
            return;
        }
        StartCoroutine(RequestSpawnConfirmationCoroutine());
    }

    private IEnumerator RequestSpawnConfirmationCoroutine()
    {
        _spawnConfirmationRequested.Value = false;
        _clientsSpawned.Clear();
        AllClientsHaveSpawned = false;
        yield return null;
        _spawnConfirmationRequested.Value = true;
    }

    private void OnSpawnConfirmationRequested(bool previousValue, bool newValue)
    {
        if(newValue)
        {
            NotifyServerObjectSpawnedServerRpc();
        }
    }

    [Rpc(SendTo.Server, InvokePermission =RpcInvokePermission.Everyone)]
    private void NotifyServerObjectSpawnedServerRpc(RpcParams rpcParams = default)
    {
        if(!IsServer)
        {
            return;
        }
        var clientId = rpcParams.Receive.SenderClientId;
        _clientsSpawned.Add(clientId);

        bool allSpawned = true;
        foreach (var id in NetworkManager.ConnectedClientsIds)
        {
            if (!_clientsSpawned.Contains(id))
            {
                allSpawned = false;
                break;
            }
        }

        if (allSpawned)
        {
            Debug.Log("All clients spawned the scene loader.");
            AllClientsHaveSpawned = true;
        }
    }


    #endregion

    #region Menu Scene

    public void LoadMenuScene()
    {
        LoadMenuSceneServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void LoadMenuSceneServerRpc()
    {
        SaveGameplaySceneSettingsClientRpc(NetworkGameplaySceneSettingsData.ToNetworkData(null));
        LoadMenuSceneAndLeaveRoomClientRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void LoadMenuSceneAndLeaveRoomClientRpc()
    {
        SceneManager.LoadScene(Constants.MenuSceneName, LoadSceneMode.Single);
        RoomNetworkManager.LeaveRoom();
    }

    #endregion

    #region Gameplay Scene

    public void LoadGameplayScene(GameplaySceneSettings settings)
    {
        if (!IsServer)
        {
            return;
        }
        SaveGameplaySceneSettingsClientRpc(NetworkGameplaySceneSettingsData.ToNetworkData(settings));
        _clientsLoadedScene.Clear();
        NetworkManager.Singleton.SceneManager.LoadScene(settings.Map.SceneName, LoadSceneMode.Single);
        StartCoroutine(KickClientsIfSceneNotLoadedAfterTimeout());
    }

    private IEnumerator KickClientsIfSceneNotLoadedAfterTimeout()
    {
        float startTime = Time.time;

        while (Time.time - startTime < GameplaySceneLoadTimeoutSeconds)
        {
            bool allLoaded = true;
            foreach (var id in NetworkManager.ConnectedClientsIds)
            {
                if (!_clientsLoadedScene.Contains(id))
                {
                    allLoaded = false;
                    break;
                }
            }

            if (allLoaded)
            {
                Debug.Log("All clients loaded gameplay scene.");
                yield break;
            }

            yield return new WaitForSeconds(GameplaySceneLoadYieldInterval);
        }

        Debug.LogWarning("Timeout waiting for clients to load scene. Disconnecting stuck clients...");

        foreach (var id in NetworkManager.ConnectedClientsIds.ToList())
        {
            if (!_clientsLoadedScene.Contains(id))
            {
                Debug.LogWarning($"Disconnecting client {id} because they never loaded the scene.");
                NetworkManager.DisconnectClient(id);
            }
        }
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (!IsServer)
            return;

        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
        {
            ulong clientId = sceneEvent.ClientId;
            _clientsLoadedScene.Add(clientId);

            Debug.Log($"Client {clientId} finished loading scene ({_clientsLoadedScene.Count}/{NetworkManager.ConnectedClientsIds.Count})");
        }
    }

    #endregion

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void SaveGameplaySceneSettingsClientRpc(NetworkGameplaySceneSettingsData networkSettings)
    {
        Debug.Log("Gameplay scene settings saved");
        GameplaySceneSettingsStorage.Current = networkSettings.ToSceneSettings(_mapLocator);
    }

    public void ReloadScene()
    {
        if (!IsServer)
        {
            return;
        }
        DespawnAllRuntimeSpawnedObjects();
        var sceneName = SceneManager.GetActiveScene().name;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
