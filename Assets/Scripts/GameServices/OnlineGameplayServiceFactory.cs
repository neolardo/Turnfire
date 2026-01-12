using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineGameplayServiceFactory : NetworkBehaviour, IGameplayServiceFactory
{
    [Header("Online")]
    [SerializeField] private OnlineGameStateManager _gameStateManagerPrefab;
    [SerializeField] private OnlineTurnStateManager _turnStateManagerPrefab;
    [SerializeField] private OnlineTimer _timerPrefab;
    [SerializeField] private OnlineSceneLoader _sceneLoaderPrefab;
    [SerializeField] private OnlineDropManager _dropManagerPrefab;
    [SerializeField] private OnlineIdGenerator _idGeneratorPrefab;
    [SerializeField] private OnlineExplosionPool _explosionPoolPrefab;
    [SerializeField] private OnlineProjectilePool _projectilePoolPrefab;
    [SerializeField] private OnlineLaserPool _laserPoolPrefab;
    [Header("Invariant")]
    [SerializeField] private ItemDefinitionDatabase _itemDatabase;
    [SerializeField] private ExplosionDefinitionDatabase _explosionDatabase;
    [SerializeField] private ProjectileDefinitionDatabase _projectileDatabase;
    
    private NetworkVariable<bool> _allServicesCreated = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _allServicesCreated.OnValueChanged += OnAllServicesCreatedValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        _allServicesCreated.OnValueChanged -= OnAllServicesCreatedValueChanged;
        base.OnNetworkDespawn();
    }

    public void CreateServices(Transform containerParent)
    {
        if(!IsServer)
        {
            return;
        }
        InstantiateServices(containerParent);
    }
    private void InstantiateServices(Transform containerParent)
    {
        if(!IsServer)
        {
            return;
        }
        var networkObjects = new List<NetworkObject>();
        networkObjects.Add(Instantiate(_gameStateManagerPrefab, containerParent).GetComponent<NetworkObject>());
        networkObjects.Add(Instantiate(_turnStateManagerPrefab, containerParent).GetComponent<NetworkObject>());
        networkObjects.Add(Instantiate(_timerPrefab, containerParent).GetComponent<NetworkObject>());
        networkObjects.Add(Instantiate(_timerPrefab, containerParent).GetComponent<NetworkObject>());
        if (OfflineSceneLoader.Instance == null)
        {
            networkObjects.Add(Instantiate(_sceneLoaderPrefab, containerParent).GetComponent<NetworkObject>());
        }
        networkObjects.Add(Instantiate(_dropManagerPrefab, containerParent).GetComponent<NetworkObject>());
        networkObjects.Add(Instantiate(_idGeneratorPrefab, containerParent).GetComponent<NetworkObject>());

        Instantiate(_projectilePoolPrefab, containerParent);
        Instantiate(_explosionPoolPrefab, containerParent);
        Instantiate(_laserPoolPrefab, containerParent);

        SpawnAllServices(networkObjects);
        StartCoroutine(WaitForAllServicesToBeSpawned(networkObjects));
    }

    private void SpawnAllServices(IEnumerable<NetworkObject> networkObjects)
    {
        foreach(var networkObject in networkObjects)
        {
            networkObject.Spawn();
        }
    }

    private IEnumerator WaitForAllServicesToBeSpawned(IEnumerable<NetworkObject> networkObjects)
    {
        foreach (var networkObject in networkObjects)
        {
            yield return new WaitUntil( () => networkObject.IsSpawned);
        }
        _allServicesCreated.Value = true;
    }

    private void OnAllServicesCreatedValueChanged(bool oldValue, bool newValue)
    {
        if(newValue)
        {
            AssignAndInitializeServices();
        }
    }

    private void AssignAndInitializeServices()
    {
        var gameStateManager = FindFirstObjectByType<OnlineGameStateManager>();
        var turnStateManager = FindFirstObjectByType<OnlineTurnStateManager>();
        var timers = FindObjectsByType<OnlineTimer>(FindObjectsSortMode.InstanceID);
        var countdownTimer = timers[0];
        var gameplayTimer = timers[1];
        var sceneLoader = FindFirstObjectByType<OnlineSceneLoader>();
        var dropManager = FindFirstObjectByType<OnlineDropManager>();
        var itemInstanceIdGenerator = FindFirstObjectByType<OnlineIdGenerator>();
        var explosionPool = FindFirstObjectByType<OnlineExplosionPool>();
        var projectilePool = FindFirstObjectByType<OnlineProjectilePool>();
        var laserPool = FindFirstObjectByType<OnlineLaserPool>();
        GameServices.Initialize(gameStateManager, turnStateManager, countdownTimer, gameplayTimer, sceneLoader, dropManager, itemInstanceIdGenerator, explosionPool, projectilePool, laserPool, _itemDatabase, _explosionDatabase, _projectileDatabase);
    }
}
