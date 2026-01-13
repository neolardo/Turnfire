using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameplayerServicesBootstrap : MonoBehaviour
{
    [Header("Offline")]
    [SerializeField] private OfflineGameStateManager _offlineGameStateManagerPrefab;
    [SerializeField] private OfflineTurnStateManager _offlineTurnStateManagerPrefab;
    [SerializeField] private OfflineTimer _offlineCountdownTimerPrefab;
    [SerializeField] private OfflineTimer _offlineGameplayTimerPrefab;
    [SerializeField] private OfflineSceneLoader _offlineSceneLoaderPrefab;
    [SerializeField] private OfflineDropManager _offlineDropManagerPrefab;
    [SerializeField] private OfflineIdGenerator _offlineIdGeneratorPrefab;
    [SerializeField] private OfflineExplosionPool _offlineExplosionPoolPrefab;
    [SerializeField] private OfflineProjectilePool _offlineProjectilePoolPrefab;
    [SerializeField] private OfflineLaserPool _offlineLaserPoolPrefab;
    [Header("Online")]
    [SerializeField] private OnlineGameStateManager _onlineGameStateManagerPrefab;
    [SerializeField] private OnlineTurnStateManager _onlineTurnStateManagerPrefab;
    [SerializeField] private OnlineTimer _onlineCountdownTimerPrefab;
    [SerializeField] private OnlineTimer _onlineGameplayTimerPrefab;
    [SerializeField] private OnlineSceneLoader _onlineSceneLoaderPrefab;
    [SerializeField] private OnlineDropManager _onlineDropManagerPrefab;
    [SerializeField] private OnlineIdGenerator _onlineIdGeneratorPrefab;
    [SerializeField] private OnlineExplosionPool _onlineExplosionPoolPrefab;
    [SerializeField] private OnlineProjectilePool _onlineProjectilePoolPrefab;
    [SerializeField] private OnlineLaserPool _onlineLaserPoolPrefab;
    [Header("Invariant")]
    [SerializeField] private ItemDefinitionDatabase _itemDatabase;
    [SerializeField] private ExplosionDefinitionDatabase _explosionDatabase;
    [SerializeField] private ProjectileDefinitionDatabase _projectileDatabase;

    private void Awake()
    {
        bool isOnlineGame = GameplaySceneSettingsStorage.Current.IsOnlineGame;
        var container = GetComponent<GameServicesContainer>().transform;
        GameServices.ClearServices();
        if (isOnlineGame)
        {
            CreateOnlineServices(container);
        }
        else
        {
            CreateOfflineServices(container);
        }
        RegisterInvariantServices();
    }

    private void CreateOfflineServices(Transform container)
    {
        Instantiate(_offlineProjectilePoolPrefab, container);
        Instantiate(_offlineExplosionPoolPrefab, container);
        Instantiate(_offlineLaserPoolPrefab, container);

        Instantiate(_offlineGameStateManagerPrefab, container);
        Instantiate(_offlineTurnStateManagerPrefab, container);
        Instantiate(_offlineCountdownTimerPrefab, container);
        Instantiate(_offlineGameplayTimerPrefab, container);
        Instantiate(_offlineDropManagerPrefab, container);
        Instantiate(_offlineIdGeneratorPrefab, container);
        if (OfflineSceneLoader.Instance == null)
        {
            Instantiate(_offlineSceneLoaderPrefab, container);
        }
        else
        {
            GameServices.Register(OfflineSceneLoader.Instance);
        }
    }
    private void CreateOnlineServices(Transform container)
    {
        // non-network objects
        Instantiate(_onlineProjectilePoolPrefab, container);
        Instantiate(_onlineExplosionPoolPrefab, container);
        Instantiate(_onlineLaserPoolPrefab, container);
        // network objects
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        var netObjects = new List<NetworkObject>
        {
            Instantiate(_onlineGameStateManagerPrefab, container).GetComponent<NetworkObject>(),
            Instantiate(_onlineTurnStateManagerPrefab, container).GetComponent<NetworkObject>(),
            Instantiate(_onlineCountdownTimerPrefab, container).GetComponent<NetworkObject>(),
            Instantiate(_onlineGameplayTimerPrefab, container).GetComponent<NetworkObject>(),
            Instantiate(_onlineDropManagerPrefab, container).GetComponent<NetworkObject>(),
            Instantiate(_onlineIdGeneratorPrefab, container).GetComponent<NetworkObject>()
        };
        if (OnlineSceneLoader.Instance == null)
        {
            netObjects.Add(Instantiate(_onlineSceneLoaderPrefab, container).GetComponent<NetworkObject>());
        }
        else
        {
            netObjects.Add(OnlineSceneLoader.Instance.GetComponent<NetworkObject>());
            GameServices.Register(OnlineSceneLoader.Instance);
        }
        foreach (var netObject in netObjects)
        {
            netObject.Spawn();
        }
    }

    private void RegisterInvariantServices()
    {
        GameServices.Register(_itemDatabase);
        GameServices.Register(_explosionDatabase);
        GameServices.Register(_projectileDatabase);
    }

}
