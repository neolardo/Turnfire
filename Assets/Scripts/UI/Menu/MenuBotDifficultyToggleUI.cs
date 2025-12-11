using System;
using TMPro;
using UnityEngine;

public class MenuBotDifficultyToggleUI : HoverableSelectableContainerUI
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

    protected override void Awake()
    {
        base.Awake();
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
        _leftButton.ArrowPressed += DecrementValue;
        _rightButton.ArrowPressed += IncrementValue;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _inputManager.MenuDecrementValuePerformed += OnDecrementValuePerformed;
        _inputManager.MenuIncrementValuePerformed += OnIncrementValuePerformed;
    }

    protected override void Start()
    {
        base.Start();
        Refresh();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _inputManager.MenuDecrementValuePerformed -= OnDecrementValuePerformed;
        _inputManager.MenuIncrementValuePerformed -= OnIncrementValuePerformed;
    }

    private void OnDecrementValuePerformed()
    {
        if (IsSelected)
        {
            _leftButton.Press();
        }
    }
    private void OnIncrementValuePerformed()
    {
        if (IsSelected)
        {
            _rightButton.Press();
        }
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
