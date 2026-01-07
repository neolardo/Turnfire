using TMPro;
using UnityEngine;

public abstract class TimerUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _timerText;
    [SerializeField] private TimerFormat _format;
    private string _endText;
    protected ITimer _timer;

    protected void Initialize(ITimer timer, string endText = null)
    {
        _timer = timer;
        _timer.TimerEnded += OnTimerEnded;
        _endText = endText;
        UpdateDisplay();
    }

    protected virtual void OnTimerEnded()
    {
        if (!string.IsNullOrEmpty(_endText))
        {
            _timerText.text = _endText;
        }
    }

    void Update()
    {
        if (!_timer.IsRunning)
        {
            return;
        }

        UpdateDisplay();
    }

    protected virtual void UpdateDisplay()
    {
        var currentTime = _timer.CurrentTime;

        switch (_format)
        {
            case TimerFormat.MinutesAndSeconds:
                int minutes = Mathf.FloorToInt(currentTime / 60f);
                int seconds = Mathf.CeilToInt(currentTime % 60f);
                _timerText.text = $"{minutes}:{seconds:00}";
                break;
            case TimerFormat.OnlySeconds:
                _timerText.text = $"{Mathf.CeilToInt(currentTime):0}";
                break;
            default:
                Debug.LogWarning($"Invalid timer format: {_format}");
                break;
        }

    }
}
