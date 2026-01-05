using UnityEngine;

public class GameplayTimerUI : TimerUI
{
    [SerializeField] private GameplaySettingsDefinition _timerSettings;
    [SerializeField] private Color _firstThresholdColor;
    [SerializeField] private Color _secondThresholdColor;
    [SerializeField] private UISoundsDefinition _uiSounds;
    private Color _normalColor;

    private void Start()
    {
        _normalColor = _timerText.color;
        var timer = GameServices.GameplayTimer;
        timer.Initialize(_timerSettings.SecondsAvaiablePerPlayerTurn);
        this.Initialize(timer);
    }

    protected override void OnTimerEnded()
    {
        base.OnTimerEnded();
        AudioManager.Instance.PlayUISound(_uiSounds.TimeIsUp);
    }

    protected override void UpdateDisplay()
    {
        base.UpdateDisplay();
        var currentTime = _timer.CurrentTime;
        if (currentTime <= _timerSettings.GameplayTimerSecondThresholdSeconds)
        {
            _timerText.color = _secondThresholdColor;
        }
        else if (currentTime <= _timerSettings.GameplayTimerFirstThresholdSeconds)
        {
            _timerText.color = _firstThresholdColor;
        }
        else
        {
            _timerText.color = _normalColor;
        }
    }
}
