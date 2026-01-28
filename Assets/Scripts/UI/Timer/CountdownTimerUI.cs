using System.Collections;
using UnityEngine;

public class CountdownTimerUI : TimerUI
{
    [SerializeField] private GameplaySettingsDefinition _timerSettings;

    private void Awake()
    {
        if(GameServices.IsInitialized)
        {
            OnGameServicesInitialized();
        }
        else
        {
            GameServices.Initialized += OnGameServicesInitialized;
        }
    }
    private void OnDestroy()
    {
        GameServices.Initialized -= OnGameServicesInitialized;
    }

    private void OnGameServicesInitialized()
    {
        var timer = GameServices.CountdownTimer;
        timer.Initialize(_timerSettings.CountdownSecondsBeforeStart);
        Initialize(timer);
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
