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

    public static void Initialize(IGameStateManager gameStateManager, ITurnStateManager turnStateManager, ITimer countdownTimer, ITimer gameplayTimer, ISceneLoader sceneLoader, IDropManager dropManager, IIdGenerator itemInstanceIdGenerator, IPool<IExplosion> explosionPool, IPool<IProjectile> projectilePool, IPool<ILaser> laserPool, ItemDefinitionDatabase itemDatabase, ExplosionDefinitionDatabase explosionDatabase, ProjectileDefinitionDatabase projectileDatabase)
    {
        GameStateManager = gameStateManager;
        TurnStateManager = turnStateManager;
        CountdownTimer = countdownTimer;
        GameplayTimer = gameplayTimer;
        SceneLoader = sceneLoader;
        DropManager = dropManager;
        ItemInstanceIdGenerator = itemInstanceIdGenerator;

        ProjectilePool = projectilePool;
        ExplosionPool = explosionPool;
        LaserPool = laserPool;

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
