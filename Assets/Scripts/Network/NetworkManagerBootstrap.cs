using Unity.Netcode;
using UnityEngine;

public class NetworkManagerBootstrap : MonoBehaviour
{
    private void Awake()
    {
        var nm = GetComponent<NetworkManager>();
        if (nm == null)
        {
            Debug.LogError("NetworkManagerGuard must be on the same GameObject as NetworkManager.");
            return;
        }

        if (NetworkManager.Singleton != null && NetworkManager.Singleton != nm)
        {
            Debug.LogWarning($"Duplicate NetworkManager detected. Destroying {gameObject.name}.");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}