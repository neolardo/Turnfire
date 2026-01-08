using UnityEngine;

public class GameplayerServicesBootstrap : MonoBehaviour
{
    [Header("Online")]
    [SerializeField] OnlineGameStateManager _onlineGameStateManagerPrefab;
    [SerializeField] OnlineTurnStateManager _onlineTurnStateManagerPrefab;
    [SerializeField] OnlineTimer _onlineTimerPrefab;
    [SerializeField] OnlineSceneLoader _onlineSceneLoaderPrefab;
    [SerializeField] OnlineDropManager _onlineDropManagerPrefab;
    [Header("Offline")]
    [SerializeField] OfflineGameStateManager _offlineGameStateManagerPrefab;
    [SerializeField] OfflineTurnStateManager _offlineTurnStateManagerPrefab;
    [SerializeField] OfflineTimer _offlineTimerPrefab;
    [SerializeField] OfflineSceneLoader _offlineSceneLoaderPrefab;
    [SerializeField] OfflineDropManager _offlineDropManagerPrefab;

    private void Awake()
    {
        bool isOnlineGame = GameplaySceneSettingsStorage.Current.IsOnlineGame;
        if(isOnlineGame)
        {
            GameServices.InitializeOnline(_onlineGameStateManagerPrefab, _onlineTurnStateManagerPrefab, _onlineTimerPrefab, _onlineSceneLoaderPrefab, _onlineDropManagerPrefab);
        }
        else
        {
            GameServices.InitializeOffline(_offlineGameStateManagerPrefab, _offlineTurnStateManagerPrefab, _offlineTimerPrefab, _offlineSceneLoaderPrefab, _offlineDropManagerPrefab);
        }
    }
}
