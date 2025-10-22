using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettingsDefinition", menuName = "Scriptable Objects/GameplaySettingsDefinition")]
public class GameplaySettingsDefinition : ScriptableObject
{
    [Header("Timers")]
    public int CountdownSecondsBeforeStart;
    public int DelaySecondsAfterCountdown;
    public int SecondsAvaiablePerPlayerTurn;
    public int GameplayTimerFirstThresholdSeconds;
    public int GameplayTimerSecondThresholdSeconds;

    [Header("Drops")]
    public int MinimumNumberOfDropsPerRound;
    public int MaximumNumberOfDropsPerRound;
    public CollectibleDefinition[] PossibleDrops;
}
