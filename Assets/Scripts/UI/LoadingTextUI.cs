using TMPro;
using UnityEngine;

public class LoadingTextUI : MonoBehaviour
{
    [SerializeField] private float _stepDurationSeconds;
    [SerializeField] private TextMeshProUGUI _text;
    private float _elapsedSeconds;
    private int _dotCount = InitialDotCount;
    private string _originalText;

    private const int MaxDotCount = 3;
    private const int InitialDotCount = 1;
    private const char DotChar = '.';

    private void Awake()
    {
        _originalText = _text.text;
        RefreshText();
    }

    private void OnEnable()
    {
        _elapsedSeconds = 0;
        _dotCount = InitialDotCount;
        RefreshText();
    }

    private void Update()
    {
        _elapsedSeconds += Time.deltaTime;
        if (_elapsedSeconds >= _stepDurationSeconds)
        {
            UpdateDotCount();
            RefreshText();
            _elapsedSeconds = 0;
        }
    }

    private void UpdateDotCount()
    {
        _dotCount++;
        if (_dotCount > MaxDotCount)
        {
            _dotCount = InitialDotCount;
        }
    }

    private void RefreshText()
    {
        _text.text = _originalText;
        for (int i = 0; i < _dotCount; i++)
        {
            _text.text += DotChar;
        }
    }


}
