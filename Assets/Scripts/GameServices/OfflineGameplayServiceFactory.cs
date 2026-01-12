using UnityEngine;

public class OfflineGameplayServiceFactory : MonoBehaviour, IGameplayServiceFactory
{
    [Header("Offline")]
    [SerializeField] private OfflineGameStateManager _gameStateManagerPrefab;
    [SerializeField] private OfflineTurnStateManager _turnStateManagerPrefab;
    [SerializeField] private OfflineTimer _timerPrefab;
    [SerializeField] private OfflineSceneLoader _sceneLoaderPrefab;
    [SerializeField] private OfflineDropManager _dropManagerPrefab;
    [SerializeField] private OfflineIdGenerator _idGeneratorPrefab;
    [SerializeField] private OfflineExplosionPool _explosionPoolPrefab;
    [SerializeField] private OfflineProjectilePool _projectilePoolPrefab;
    [SerializeField] private OfflineLaserPool _laserPoolPrefab;
    [Header("Invariant")]
    [SerializeField] private ItemDefinitionDatabase _itemDatabase;
    [SerializeField] private ExplosionDefinitionDatabase _explosionDatabase;
    [SerializeField] private ProjectileDefinitionDatabase _projectileDatabase;

    public void CreateServices(Transform containerParent)
    {
        InstantiateServices(containerParent);
        AssignAndInitializeServices();
    }

    private void InstantiateServices(Transform containerParent)
    {
        GameObject.Instantiate(_gameStateManagerPrefab, containerParent);
        GameObject.Instantiate(_turnStateManagerPrefab, containerParent);
        GameObject.Instantiate(_timerPrefab, containerParent);
        GameObject.Instantiate(_timerPrefab, containerParent);
        if (OfflineSceneLoader.Instance == null)
        {
            GameObject.Instantiate(_sceneLoaderPrefab, containerParent);
        }
        GameObject.Instantiate(_dropManagerPrefab, containerParent);
        GameObject.Instantiate(_idGeneratorPrefab, containerParent);

        GameObject.Instantiate(_projectilePoolPrefab, containerParent);
        GameObject.Instantiate(_explosionPoolPrefab, containerParent);
        GameObject.Instantiate(_laserPoolPrefab, containerParent);
    }

    private void AssignAndInitializeServices()
    {
        var gameStateManager = FindFirstObjectByType<OfflineGameStateManager>();
        var turnStateManager = FindFirstObjectByType<OfflineTurnStateManager>();
        var timers = FindObjectsByType<OfflineTimer>(FindObjectsSortMode.InstanceID);
        var countdownTimer = timers[0];
        var gameplayTimer = timers[1];
        var sceneLoader = FindFirstObjectByType<OfflineSceneLoader>();
        var dropManager = FindFirstObjectByType<OfflineDropManager>();
        var itemInstanceIdGenerator = FindFirstObjectByType<OfflineIdGenerator>();
        var explosionPool = FindFirstObjectByType<OfflineExplosionPool>();
        var projectilePool = FindFirstObjectByType<OfflineProjectilePool>();
        var laserPool = FindFirstObjectByType<OfflineLaserPool>();
        GameServices.Initialize(gameStateManager, turnStateManager, countdownTimer, gameplayTimer, sceneLoader, dropManager, itemInstanceIdGenerator, explosionPool, projectilePool, laserPool, _itemDatabase, _explosionDatabase, _projectileDatabase);
    }
}
