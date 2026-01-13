using System;

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

    public static bool IsInitialized { get; private set; }

    public static event Action Initialized;

    public static void Register(IGameStateManager gameStateManager)
    {
        GameStateManager = gameStateManager;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(ITurnStateManager turnStateManager)
    {
        TurnStateManager = turnStateManager;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void RegisterCountdownTimer(ITimer timer)
    {
        CountdownTimer = timer;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void RegisterGameplayTimer(ITimer timer)
    {
        GameplayTimer = timer;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(ISceneLoader sceneLoader)
    {
        SceneLoader = sceneLoader;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(IDropManager dropManager)
    {
        DropManager = dropManager;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(IIdGenerator itemInstanceIdGenerator)
    {
        ItemInstanceIdGenerator = itemInstanceIdGenerator;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(IPool<IProjectile> projectilePool)
    {
        ProjectilePool = projectilePool;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(IPool<IExplosion> explosionPool)
    {
        ExplosionPool = explosionPool;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(IPool<ILaser> laserPool)
    {
        LaserPool = laserPool;
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(IDatabase<ItemDefinition> itemDatabase)
    {
        ItemDatabase = itemDatabase;
        ItemDatabase.Initialize();
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(IDatabase<ExplosionDefinition> explosionDatabase)
    {
        ExplosionDatabase = explosionDatabase;
        ExplosionDatabase.Initialize();
        ConnectServicesIfAllServicesRegistered();
    }
    public static void Register(IDatabase<ProjectileDefinition> projectileDatabase)
    {
        ProjectileDatabase = projectileDatabase;
        ProjectileDatabase.Initialize();
        ConnectServicesIfAllServicesRegistered();
    }

    private static void ConnectServicesIfAllServicesRegistered()
    {
        if (GameStateManager != null 
            && TurnStateManager != null 
            && CountdownTimer != null 
            && GameplayTimer != null
            && SceneLoader != null
            && DropManager != null
            && ItemInstanceIdGenerator != null 
            && ProjectilePool != null
            && ExplosionPool != null
            && LaserPool != null 
            && ItemDatabase != null
            && ExplosionDatabase != null
            && ProjectileDatabase != null)
        {
            ConnectServices();
        }
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
        IsInitialized = true;
        Initialized?.Invoke();
    }

    public static void ClearServices()
    {
        IsInitialized = false;
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
