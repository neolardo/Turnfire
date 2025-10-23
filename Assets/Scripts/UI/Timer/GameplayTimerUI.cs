using UnityEngine;

public class GameplayTimerUI : TimerUI
{
    [SerializeField] private GameplaySettingsDefinition _timerSettings;
    [SerializeField] private Color _firstThresholdColor;
    [SerializeField] private Color _secondThresholdColor;
    [SerializeField] private UISoundsDefinition _uiSounds;
    private Color _normalColor;

    protected override void Awake()
    {
        base.Awake();
        _normalColor = _timerText.color;
        Initialize(_timerSettings.SecondsAvaiablePerPlayerTurn);
    }

    protected override void OnTimerEnded()
    {
        base.OnTimerEnded();
        AudioManager.Instance.PlayUISound(_uiSounds.TimeIsUp);
    }

    protected override void DecrementTime()
    {
        base.DecrementTime();
        if (_currentTime <= _timerSettings.GameplayTimerSecondThresholdSeconds)
        {
            _timerText.color = _secondThresholdColor;
        }
        else if (_currentTime <= _timerSettings.GameplayTimerFirstThresholdSeconds)
        {
            _timerText.color = _firstThresholdColor;
        }
    }
  
    public override void StartTimer()
    {
        base.StartTimer();
        _timerText.color = _normalColor;
    }

}
