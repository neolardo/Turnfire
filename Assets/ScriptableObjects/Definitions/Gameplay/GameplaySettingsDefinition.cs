using UnityEngine;

[CreateAssetMenu(fileName = "GameplaySettingsDefinition", menuName = "Scriptable Objects/GameplaySettingsDefinition")]
public class GameplaySettingsDefinition : ScriptableObject
{
    public int CountdownSecondsBeforeStart;
    public int DelaySecondsAfterCountdown;
    public int SecondsAvaiablePerPlayerTurn;
    public int GameplayTimerFirstThresholdSeconds;
    public int GameplayTimerSecondThresholdSeconds;
}
