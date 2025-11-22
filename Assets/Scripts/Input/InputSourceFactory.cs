using UnityEngine;

public static class GameplayInputSourceFactory
{
    public static IGameplayInputSource Create(InputSourceType inputType, GameObject parent)
    {
        switch (inputType)
        {
            case InputSourceType.Local:
                return Object.FindFirstObjectByType<LocalGameplayInput>();
            case InputSourceType.Bot:
                return parent.AddComponent(typeof(BotGameplayInput)) as BotGameplayInput;
            case InputSourceType.Remote:
                return parent.AddComponent(typeof(RemoteGameplayInput)) as RemoteGameplayInput;
            default:
                throw new System.Exception($"Invalid {nameof(IGameplayInputSource)} when creating input sources.");
        }
    }
}
