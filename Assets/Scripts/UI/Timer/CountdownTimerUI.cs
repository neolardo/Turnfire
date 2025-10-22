using UnityEngine;

public class CountdownTimerUI : TimerUI
{
    [SerializeField] private GameplaySettingsDefinition _timerSettings;

    protected override void Awake()
    {
        var endText = _timerText.text;
        base.Awake();
        Initialize(_timerSettings.CountdownSecondsBeforeStart, endText);
    }
}
