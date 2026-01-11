using Unity.VisualScripting;
using UnityEngine;

public static class TeamInputSourceFactory
{
    private static OfflineHumanTeamInputSource _offlineHumanInput;

    public static ITeamInputSource Create(InputSourceType inputType, Transform parent)
    {
        switch (inputType)
        {
            case InputSourceType.OfflineHuman:
                if (_offlineHumanInput == null)
                {
                    _offlineHumanInput = parent.AddComponent<OfflineHumanTeamInputSource>();
                }
                return _offlineHumanInput;
            case InputSourceType.OnlineHuman:
                return parent.AddComponent<OnlineHumanTeamInputSource>();
            case InputSourceType.OfflineBot:
                return parent.AddComponent<OfflineBotTeamInputSource>();
            case InputSourceType.OnlineBot:
                return parent.AddComponent<OnlineBotTeamInputSource>();
            default:
                throw new System.Exception($"Invalid {nameof(ITeamInputSource)} when creating input sources.");
        }
    }
}
