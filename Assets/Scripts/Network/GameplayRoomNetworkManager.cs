using Unity.Netcode;
using UnityEngine;

public class GameplayRoomNetworkManager : MonoBehaviour
{
    private void Awake()
    {
        var isOnlineGame = GameplaySceneSettingsStorage.Current.IsOnlineGame;
        if(isOnlineGame && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        GameServices.SceneLoader.LoadMenuScene();
    }
}
