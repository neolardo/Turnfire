using System;
using TMPro;
using UnityEngine;

public class MenuNumericDisplayUI : MonoBehaviour
{
    [SerializeField] private MenuArrowButtonUI _upButton;
    [SerializeField] private MenuArrowButtonUI _downButton;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private HoverableSelectableContainerUI _containerUI;
    private LocalMenuUIInputSource _inputManager;
    private int _min;
    private int _max;
    private int _value;

    public event Action<int> ValueChanged;

    public int Value
    {
        get
        {
            return _value;
        }
        private set
        {
            if(_value != value) 
            {
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }
    }

    public void Initialize(int min, int max, int initialValue)
    {
        _min = min;
        _max = max;
        Value = initialValue;
        Refresh();
    }


    private void Awake()
    {
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
        _upButton.ArrowPressed += IncrementValue; 
        _downButton.ArrowPressed += DecrementValue;
    }

    private void OnEnable()
    {
        _inputManager.MenuDecrementValuePerformed += OnDecrementValuePerformed;
        _inputManager.MenuIncrementValuePerformed += OnIncrementValuePerformed;
    }

    private void OnDisable()
    {
        _inputManager.MenuDecrementValuePerformed -= OnDecrementValuePerformed;
        _inputManager.MenuIncrementValuePerformed -= OnIncrementValuePerformed;
    }

    private void OnDecrementValuePerformed()
    {
        if(_containerUI.IsSelected)
        {
            _downButton.Press();
        }
    }
    private void OnIncrementValuePerformed()
    {
        if (_containerUI.IsSelected)
        {
            _upButton.Press();
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
        _valueText.text = Value.ToString();
        _upButton.SetIsActive(Value < _max);
        _downButton.SetIsActive(Value > _min);
    }

}
