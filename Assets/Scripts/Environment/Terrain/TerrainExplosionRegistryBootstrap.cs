using Unity.Netcode;
using UnityEngine;

public class TerrainExplosionRegistryBootstrap : MonoBehaviour
{
    [SerializeField] private OnlineTerrainExplosionRegistry _onlineRegistryPrefab;
    [SerializeField] private OfflineTerrainExplosionRegistry _offlineRegistryPrefab;
    private void Start()
    {
        var isOnlineGame = GameplaySceneSettingsStorage.Current.IsOnlineGame;
        if (isOnlineGame && NetworkManager.Singleton.IsServer)
        {
            var registry = Instantiate(_onlineRegistryPrefab);
            registry.GetComponent<NetworkObject>().Spawn();
        }
        else if (!isOnlineGame)
        {
            Instantiate(_offlineRegistryPrefab);
        }
    }
}
