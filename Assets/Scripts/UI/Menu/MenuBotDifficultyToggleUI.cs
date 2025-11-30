using System;
using TMPro;
using UnityEngine;

public class MenuBotDifficultyToggleUI : MonoBehaviour
{
    [SerializeField] private MenuArrowButtonUI _leftButton;
    [SerializeField] private MenuArrowButtonUI _rightButton;
    [SerializeField] private TextMeshProUGUI _valueText;
    private LocalMenuInput _inputManager;
    private const int Min = (int)BotDifficulty.Easy;
    private const int Max = (int)BotDifficulty.Hard;
    private BotDifficulty _value;

    public event Action<BotDifficulty> ValueChanged;

    public BotDifficulty Value
    {
        get
        {
            return _value;
        }
        private set
        {
            if (_value != value)
            {
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }
    }

    private void Awake()
    {
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _leftButton.ArrowPressed += DecrementValue;
        _rightButton.ArrowPressed += IncrementValue;
    }

    private void OnEnable()
    {
        _inputManager.MenuNavigateUpPerformed += _leftButton.Press;
        _inputManager.MenuNavigateDownPerformed += _rightButton.Press;
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        _inputManager.MenuNavigateUpPerformed -= _leftButton.Press;
        _inputManager.MenuNavigateDownPerformed -= _rightButton.Press;
    }

    private void IncrementValue()
    {
        Value++;
        Refresh();
    }

    private void DecrementValue()
    {
        Value--;
        Refresh();
    }

    private void Refresh()
    {
        _valueText.text = Enum.GetName(typeof(BotDifficulty), Value).ToLower();
        _rightButton.SetIsActive((int)Value < Max);
        _leftButton.SetIsActive((int)Value > Min);
    }

}
