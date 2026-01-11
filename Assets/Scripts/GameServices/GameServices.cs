using Unity.Netcode;
using UnityEngine;

public static class GameServices
{
    public static IGameStateManager GameStateManager { get; private set; }
    public static ITurnStateManager TurnStateManager { get; private set; }
    public static ITimer CountdownTimer { get; private set; }
    public static ITimer GameplayTimer { get; private set; }
    public static ISceneLoader SceneLoader { get; private set; }
    public static IDropManager DropManager { get; private set; }
    public static IIdGenerator ItemInstanceIdGenerator { get; private set; }
    public static IPool<IProjectile> ProjectilePool { get; private set; }
    public static IPool<IExplosion> ExplosionPool { get; private set; }
    public static IPool<ILaser> LaserPool { get; private set; }
    public static IDatabase<ItemDefinition> ItemDatabase { get; private set; }
    public static IDatabase<ExplosionDefinition> ExplosionDatabase { get; private set; }
    public static IDatabase<ProjectileDefinition> ProjectileDatabase { get; private set; }


    public static void InitializeOffline(OfflineGameStateManager gameStateManagerPrefab, OfflineTurnStateManager turnStateManagerPrefab, OfflineTimer timerPrefab, OfflineSceneLoader sceneLoaderPrefab, OfflineDropManager dropManagerPrefab, OfflineIdGenerator idGeneratorPrefab, OfflineExplosionPool explosionPool, OfflineProjectilePool projectilePool, OfflineLaserPool laserPool, ItemDefinitionDatabase itemDatabase, ExplosionDefinitionDatabase explosionDatabase, ProjectileDefinitionDatabase projectileDatabase)
    {
        GameStateManager = GameObject.Instantiate(gameStateManagerPrefab);
        TurnStateManager = GameObject.Instantiate(turnStateManagerPrefab);
        CountdownTimer = GameObject.Instantiate(timerPrefab);
        GameplayTimer = GameObject.Instantiate(timerPrefab);
        SceneLoader = OfflineSceneLoader.Instance == null ? GameObject.Instantiate(sceneLoaderPrefab) : OfflineSceneLoader.Instance;
        DropManager = GameObject.Instantiate(dropManagerPrefab);
        ItemInstanceIdGenerator = GameObject.Instantiate(idGeneratorPrefab);

        ProjectilePool = GameObject.Instantiate(projectilePool);
        ExplosionPool = GameObject.Instantiate(explosionPool);
        LaserPool = GameObject.Instantiate(laserPool);

        ItemDatabase = itemDatabase;
        ItemDatabase.Initialize();
        ExplosionDatabase = explosionDatabase;
        ExplosionDatabase.Initialize();
        ProjectileDatabase = projectileDatabase;
        ProjectileDatabase.Initialize();
        ConnectServices();
    }

    public static void InitializeOnline(OnlineGameStateManager gameStateManagerPrefab, OnlineTurnStateManager turnStateManagerPrefab, OnlineTimer timerPrefab, OnlineSceneLoader sceneLoaderPrefab, OnlineDropManager dropManagerPrefab, OnlineIdGenerator idGeneratorPrefab, OnlineExplosionPool explosionPool, OnlineProjectilePool projectilePool, OnlineLaserPool laserPool, ItemDefinitionDatabase itemDatabase, ExplosionDefinitionDatabase explosionDatabase, ProjectileDefinitionDatabase projectileDatabase)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        var gameStateManager = GameObject.Instantiate(gameStateManagerPrefab);
        gameStateManager.GetComponent<NetworkObject>().Spawn();
        GameStateManager = gameStateManager;

        var turnStateManager = GameObject.Instantiate(turnStateManagerPrefab);
        turnStateManager.GetComponent<NetworkObject>().Spawn();
        TurnStateManager = turnStateManager;

        var countdownTimer = GameObject.Instantiate(timerPrefab);
        countdownTimer.GetComponent<NetworkObject>().Spawn();
        CountdownTimer = countdownTimer;

        var gameplayTimer = GameObject.Instantiate(timerPrefab);
        gameplayTimer.GetComponent<NetworkObject>().Spawn();
        GameplayTimer = gameplayTimer;

        var sceneLoader = OnlineSceneLoader.Instance == null ? GameObject.Instantiate(sceneLoaderPrefab) : OnlineSceneLoader.Instance;
        sceneLoader.GetComponent<NetworkObject>().Spawn();
        SceneLoader = sceneLoader;

        var dropManager = GameObject.Instantiate(dropManagerPrefab);
        dropManager.GetComponent<NetworkObject>().Spawn();
        DropManager = dropManager;

        var itemInstanceIdGenerator = GameObject.Instantiate(idGeneratorPrefab);
        itemInstanceIdGenerator.GetComponent<NetworkObject>().Spawn();
        ItemInstanceIdGenerator = itemInstanceIdGenerator;

        ProjectilePool = GameObject.Instantiate(projectilePool);
        ExplosionPool = GameObject.Instantiate(explosionPool);
        LaserPool = GameObject.Instantiate(laserPool);

        ItemDatabase = itemDatabase;
        ItemDatabase.Initialize();
        ExplosionDatabase = explosionDatabase;
        ExplosionDatabase.Initialize();
        ProjectileDatabase = projectileDatabase;
        ProjectileDatabase.Initialize();

        ConnectServices();
    }

    private static void ConnectServices()
    {
        var useGameplayTimer = GameplaySceneSettingsStorage.Current.UseTimer;
        GameplayTimer.CanRestart = () => (GameStateManager.CurrentState == GameStateType.Playing && useGameplayTimer);
        GameplayTimer.CanResume = () => (GameStateManager.CurrentState == GameStateType.Playing && useGameplayTimer);
        GameplayTimer.CanPause = () => useGameplayTimer;

        if (useGameplayTimer)
        {
            GameStateManager.StateChanged += (state) =>
            {
                if (state == GameStateType.Paused)
                    GameplayTimer.Pause();
                else
                    GameplayTimer.Resume();
            };
        }
    }
}
