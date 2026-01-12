using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameplayerServicesBootstrap : MonoBehaviour
{
    [SerializeField] private OnlineGameplayServiceFactory _onlineServiceFactory;
    [SerializeField] private OfflineGameplayServiceFactory _offlineServiceFactory;

    private void Awake()
    {
        bool isOnlineGame = GameplaySceneSettingsStorage.Current.IsOnlineGame;
        var container = GetComponent<GameServicesContainer>().transform;

        GameServices.ClearServices();
        if (isOnlineGame)
        {
            StartCoroutine(CreateOnlineServices(container));
        }
        else
        {
            StartCoroutine(CreateOfflineServices(container));
        }
    }

    private IEnumerator CreateOfflineServices(Transform container)
    {
        var factory = Instantiate(_offlineServiceFactory, container);
        factory.CreateServices(container);
        yield break;
    }
    private IEnumerator CreateOnlineServices(Transform container)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            yield break;
        }
        var factory = Instantiate(_onlineServiceFactory, container);
        var netObj = factory.GetComponent<NetworkObject>();
        netObj.Spawn();
        yield return new WaitUntil(() => netObj.IsSpawned);
        factory.CreateServices(container);
    }
}
