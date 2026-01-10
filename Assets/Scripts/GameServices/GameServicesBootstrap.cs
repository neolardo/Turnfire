using UnityEngine;

public class GameplayerServicesBootstrap : MonoBehaviour
{
    [Header("Online")]
    [SerializeField] private OnlineGameStateManager _onlineGameStateManagerPrefab;
    [SerializeField] private OnlineTurnStateManager _onlineTurnStateManagerPrefab;
    [SerializeField] private OnlineTimer _onlineTimerPrefab;
    [SerializeField] private OnlineSceneLoader _onlineSceneLoaderPrefab;
    [SerializeField] private OnlineDropManager _onlineDropManagerPrefab;
    [SerializeField] private OnlineIdGenerator _onlineIdGeneratorPrefab;
    [SerializeField] private OnlineExplosionPool _onlineExplosionPoolPrefab;
    [SerializeField] private OnlineProjectilePool _onlineProjectilePoolPrefab;
    [Header("Offline")]
    [SerializeField] private OfflineGameStateManager _offlineGameStateManagerPrefab;
    [SerializeField] private OfflineTurnStateManager _offlineTurnStateManagerPrefab;
    [SerializeField] private OfflineTimer _offlineTimerPrefab;
    [SerializeField] private OfflineSceneLoader _offlineSceneLoaderPrefab;
    [SerializeField] private OfflineDropManager _offlineDropManagerPrefab;
    [SerializeField] private OfflineIdGenerator _offlineIdGeneratorPrefab;
    [SerializeField] private OfflineExplosionPool _offlineExplosionPoolPrefab;
    [SerializeField] private OfflineProjectilePool _offlineProjectilePoolPrefab;
    [Header("Invariant")]
    [SerializeField] ItemDefinitionDatabase _itemDatabase;
    [SerializeField] ExplosionDefinitionDatabase _explosionDatabase;
    [SerializeField] ProjectileDefinitionDatabase _projectileDatabase;

    private void Awake()
    {
        bool isOnlineGame = GameplaySceneSettingsStorage.Current.IsOnlineGame;
        if(isOnlineGame)
        {
            GameServices.InitializeOnline(_onlineGameStateManagerPrefab, _onlineTurnStateManagerPrefab, _onlineTimerPrefab, _onlineSceneLoaderPrefab, _onlineDropManagerPrefab, _onlineIdGeneratorPrefab, _onlineExplosionPoolPrefab, _onlineProjectilePoolPrefab, _itemDatabase, _explosionDatabase, _projectileDatabase);
        }
        else
        {
            GameServices.InitializeOffline(_offlineGameStateManagerPrefab, _offlineTurnStateManagerPrefab, _offlineTimerPrefab, _offlineSceneLoaderPrefab, _offlineDropManagerPrefab, _offlineIdGeneratorPrefab, _offlineExplosionPoolPrefab, _offlineProjectilePoolPrefab, _itemDatabase, _explosionDatabase, _projectileDatabase);
        }
    }
}
