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
    public static IItemDatabase ItemDatabase { get; private set; }

    public static void InitializeOffline(OfflineGameStateManager gameStateManagerPrefab, OfflineTurnStateManager turnStateManagerPrefab, OfflineTimer timerPrefab, OfflineSceneLoader sceneLoaderPrefab, OfflineDropManager dropManagerPrefab, ItemDatabase itemDatabase)
    {
        GameStateManager = GameObject.Instantiate(gameStateManagerPrefab);
        TurnStateManager = GameObject.Instantiate(turnStateManagerPrefab);
        CountdownTimer = GameObject.Instantiate(timerPrefab);
        GameplayTimer = GameObject.Instantiate(timerPrefab);
        SceneLoader = OfflineSceneLoader.Instance == null ? GameObject.Instantiate(sceneLoaderPrefab) : OfflineSceneLoader.Instance;
        DropManager = GameObject.Instantiate(dropManagerPrefab);
        ItemDatabase = itemDatabase;
        ItemDatabase.Initialize();
        ConnectServices();
    }

    public static void InitializeOnline(OnlineGameStateManager gameStateManagerPrefab, OnlineTurnStateManager turnStateManagerPrefab, OnlineTimer timerPrefab, OnlineSceneLoader sceneLoaderPrefab, OnlineDropManager dropManagerPrefab, ItemDatabase itemDatabase)
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

        ItemDatabase = itemDatabase;
        ItemDatabase.Initialize();

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
