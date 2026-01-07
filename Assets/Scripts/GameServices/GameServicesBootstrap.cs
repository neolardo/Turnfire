using UnityEngine;

public class GameplayerServicesBootstrap : MonoBehaviour
{
    [Header("Online")]
    [SerializeField] OnlineGameStateManager _onlineGameStateManagerPrefab;
    [SerializeField] OnlineTurnStateManager _onlineTurnStateManagerPrefab;
    [SerializeField] OnlineTimer _onlineTimerPrefab;
    [SerializeField] OnlineSceneLoader _onlineSceneLoaderPrefab;
    [Header("Offline")]
    [SerializeField] OfflineGameStateManager _offlineGameStateManagerPrefab;
    [SerializeField] OfflineTurnStateManager _offlineTurnStateManagerPrefab;
    [SerializeField] OfflineTimer _offlineTimerPrefab;
    [SerializeField] OfflineSceneLoader _offlineSceneLoaderPrefab;

    private void Awake()
    {
        bool isOnlineGame = GameplaySceneSettingsStorage.Current.IsOnlineGame;
        if(isOnlineGame)
        {
            GameServices.InitializeOnline(_onlineGameStateManagerPrefab, _onlineTurnStateManagerPrefab, _onlineTimerPrefab, _onlineSceneLoaderPrefab);
        }
        else
        {
            GameServices.InitializeOffline(_offlineGameStateManagerPrefab, _offlineTurnStateManagerPrefab, _offlineTimerPrefab, _offlineSceneLoaderPrefab);
        }
    }
}
