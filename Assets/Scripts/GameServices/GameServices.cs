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

    //TODO: event for game services initialized?


    public static void InitializeOffline(Transform containerParent, OfflineGameStateManager gameStateManagerPrefab, OfflineTurnStateManager turnStateManagerPrefab, OfflineTimer timerPrefab, OfflineSceneLoader sceneLoaderPrefab, OfflineDropManager dropManagerPrefab, OfflineIdGenerator idGeneratorPrefab, OfflineExplosionPool explosionPool, OfflineProjectilePool projectilePool, OfflineLaserPool laserPool, ItemDefinitionDatabase itemDatabase, ExplosionDefinitionDatabase explosionDatabase, ProjectileDefinitionDatabase projectileDatabase)
    {
        ClearServices();
        GameStateManager = GameObject.Instantiate(gameStateManagerPrefab, containerParent);
        TurnStateManager = GameObject.Instantiate(turnStateManagerPrefab, containerParent);
        CountdownTimer = GameObject.Instantiate(timerPrefab, containerParent);
        GameplayTimer = GameObject.Instantiate(timerPrefab, containerParent);
        SceneLoader = OfflineSceneLoader.Instance == null ? GameObject.Instantiate(sceneLoaderPrefab, containerParent) : OfflineSceneLoader.Instance;
        DropManager = GameObject.Instantiate(dropManagerPrefab, containerParent);
        ItemInstanceIdGenerator = GameObject.Instantiate(idGeneratorPrefab, containerParent);

        ProjectilePool = GameObject.Instantiate(projectilePool, containerParent);
        ExplosionPool = GameObject.Instantiate(explosionPool, containerParent);
        LaserPool = GameObject.Instantiate(laserPool, containerParent);

        ItemDatabase = itemDatabase;
        ItemDatabase.Initialize();
        ExplosionDatabase = explosionDatabase;
        ExplosionDatabase.Initialize();
        ProjectileDatabase = projectileDatabase;
        ProjectileDatabase.Initialize();
        ConnectServices();
    }

    public static void InitializeOnline(Transform containerParent, OnlineGameStateManager gameStateManagerPrefab, OnlineTurnStateManager turnStateManagerPrefab, OnlineTimer timerPrefab, OnlineSceneLoader sceneLoaderPrefab, OnlineDropManager dropManagerPrefab, OnlineIdGenerator idGeneratorPrefab, OnlineExplosionPool explosionPool, OnlineProjectilePool projectilePool, OnlineLaserPool laserPool, ItemDefinitionDatabase itemDatabase, ExplosionDefinitionDatabase explosionDatabase, ProjectileDefinitionDatabase projectileDatabase)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        ClearServices();
        //TODO: assign them on spawn
        var gameStateManager = GameObject.Instantiate(gameStateManagerPrefab, containerParent);
        gameStateManager.GetComponent<NetworkObject>().Spawn();
        GameStateManager = gameStateManager;

        var turnStateManager = GameObject.Instantiate(turnStateManagerPrefab, containerParent);
        turnStateManager.GetComponent<NetworkObject>().Spawn();
        TurnStateManager = turnStateManager;

        var countdownTimer = GameObject.Instantiate(timerPrefab, containerParent);
        countdownTimer.GetComponent<NetworkObject>().Spawn();
        CountdownTimer = countdownTimer;

        var gameplayTimer = GameObject.Instantiate(timerPrefab, containerParent);
        gameplayTimer.GetComponent<NetworkObject>().Spawn();
        GameplayTimer = gameplayTimer;

        var sceneLoader = OnlineSceneLoader.Instance == null ? GameObject.Instantiate(sceneLoaderPrefab, containerParent) : OnlineSceneLoader.Instance;
        sceneLoader.GetComponent<NetworkObject>().Spawn();
        SceneLoader = sceneLoader;

        var dropManager = GameObject.Instantiate(dropManagerPrefab, containerParent);
        dropManager.GetComponent<NetworkObject>().Spawn();
        DropManager = dropManager;

        var itemInstanceIdGenerator = GameObject.Instantiate(idGeneratorPrefab, containerParent);
        itemInstanceIdGenerator.GetComponent<NetworkObject>().Spawn();
        ItemInstanceIdGenerator = itemInstanceIdGenerator;

        ProjectilePool = GameObject.Instantiate(projectilePool, containerParent);
        ExplosionPool = GameObject.Instantiate(explosionPool, containerParent);
        LaserPool = GameObject.Instantiate(laserPool, containerParent);

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

    private static void ClearServices()
    {
        GameStateManager = null;
        TurnStateManager = null;
        CountdownTimer = null;
        GameplayTimer = null;
        SceneLoader = null;
        DropManager = null; 
        ItemInstanceIdGenerator = null;
        ProjectilePool = null;
        ExplosionPool = null;
        LaserPool = null;
        ItemDatabase = null; 
        ExplosionDatabase = null;
        ProjectileDatabase = null;
    }
}
