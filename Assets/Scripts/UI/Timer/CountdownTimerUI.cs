using System.Collections;
using UnityEngine;

public class CountdownTimerUI : TimerUI
{
    [SerializeField] private GameplaySettingsDefinition _timerSettings;

    private void Start()
    {
        var endText = _timerText.text;
        var timer = GameServices.CountdownTimer;
        timer.Initialize(_timerSettings.CountdownSecondsBeforeStart);
        Initialize(timer, endText);
    }

    protected override void OnTimerEnded()
    {
        base.OnTimerEnded();
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_timerSettings.DelaySecondsAfterCountdown);
        Hide();
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
