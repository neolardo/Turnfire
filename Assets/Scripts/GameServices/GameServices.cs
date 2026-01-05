using Unity.Netcode;
using UnityEngine;

public static class GameServices
{
    public static IGameStateManager GameStateManager { get; private set; }
    public static ITurnStateManager TurnStateManager { get; private set; }
    public static ITimer CountdownTimer { get; private set; }
    public static ITimer GameplayTimer { get; private set; }

    public static void InitializeOffline(OfflineGameStateManager gameStateManagerPrefab, OfflineTurnStateManager turnStateManagerPrefab, OfflineTimer timerPrefab)
    {
        GameStateManager = GameObject.Instantiate(gameStateManagerPrefab);
        TurnStateManager = GameObject.Instantiate(turnStateManagerPrefab);
        CountdownTimer = GameObject.Instantiate(timerPrefab);
        GameplayTimer = GameObject.Instantiate(timerPrefab);
    }

    public static void InitializeOnline(OnlineGameStateManager gameStateManagerPrefab, OnlineTurnStateManager turnStateManagerPrefab, OnlineTimer timerPrefab)
    {
        if(!NetworkManager.Singleton.IsServer)
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
    }
}
