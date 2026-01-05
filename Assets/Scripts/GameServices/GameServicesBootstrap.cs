using UnityEngine;

public class GameServicesBootstrap : MonoBehaviour
{
    private void Awake()
    {
        bool isOnline = true;
        GameServices.Initialize(isOnline);
    }
}
