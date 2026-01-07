using Unity.Netcode;
using UnityEngine;

public class GameplayInputSourceFactory : MonoBehaviour
{
    [SerializeField] private OfflineHumanTeamInputSource _offlineHumanInputPrefab;
    [SerializeField] private OnlineHumanTeamInputSource _onlineHumanInputPrefab;
    [SerializeField] private OfflineBotTeamInputSource _offlineBotInputPrefab;
    [SerializeField] private OnlineBotTeamInputSource _onlineBotInputPrefab;

    private OfflineHumanTeamInputSource _offlineHumanInput;

    public ITeamInputSource Create(InputSourceType inputType, Transform parent)
    {
        switch (inputType)
        {
            case InputSourceType.OfflineHuman:
                if (_offlineHumanInput == null)
                {
                    _offlineHumanInput = Instantiate(_offlineHumanInputPrefab, parent);
                }
                return _offlineHumanInput;
            case InputSourceType.OnlineHuman:
                return Instantiate(_onlineHumanInputPrefab, parent);
            case InputSourceType.OfflineBot:
                return Instantiate(_offlineBotInputPrefab, parent);
            case InputSourceType.OnlineBot:
                return Instantiate(_onlineBotInputPrefab, parent);
            default:
                throw new System.Exception($"Invalid {nameof(ITeamInputSource)} when creating input sources.");
        }
    }
}
