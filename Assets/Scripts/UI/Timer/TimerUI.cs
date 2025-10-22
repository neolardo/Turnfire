using System;
using TMPro;
using UnityEngine;

public abstract class TimerUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _timerText;
    [SerializeField] private TimerFormat _format;
    protected float _currentTime;
    protected float _initialTime;
    protected bool _isRunning;
    private string _endText;

    public event Action TimerEnded;

    protected virtual void Awake()
    {
        _isRunning = false;
        UpdateDisplay();
    }

    protected void Initialize(float initialTime, string endText = null)
    {
        _initialTime = initialTime;
        _endText = endText;
        UpdateDisplay();
    }


    void Update()
    {
        if (!_isRunning)
        {
            return;
        }

        DecrementTime();
        UpdateDisplay();
    }

    protected virtual void DecrementTime()
    {
        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0)
        {
            OnTimerEnded();
        }
    }

    protected virtual void OnTimerEnded()
    {
        _currentTime = 0;
        _isRunning = false;
        TimerEnded?.Invoke();
    }

    private void UpdateDisplay()
    {
        if (!_isRunning && !string.IsNullOrEmpty(_endText))
        {
            _timerText.text = _endText;
            return;
        }

        switch (_format)
        {
            case TimerFormat.MinutesAndSeconds:
                int minutes = Mathf.FloorToInt(_currentTime / 60f);
                int seconds = Mathf.CeilToInt(_currentTime % 60f);
                _timerText.text = $"{minutes}:{seconds:00}";
                break;
            case TimerFormat.OnlySeconds:
                _timerText.text = $"{Mathf.CeilToInt(_currentTime):0}";
                break;
            default:
                Debug.LogWarning($"Invalid timer format: {_format}");
                break;
        }

    }


    public virtual void StartTimer()
    {
        _currentTime = _initialTime;
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }

    public void ResumeTimer()
    {
        _isRunning = true;
    }
}
