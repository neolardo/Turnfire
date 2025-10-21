using TMPro;
using UnityEngine;

public class MenuNumericDisplayUI : MonoBehaviour
{
    [SerializeField] private MenuArrowButtonUI _upButton;
    [SerializeField] private MenuArrowButtonUI _downButton;
    [SerializeField] private TextMeshProUGUI _valueText;
    private int _min;
    private int _max;
    private int _value;

    public int Value => _value;

    public void Initialize(int min, int max, int initialValue)
    {
        _min = min;
        _max = max;
        _value = initialValue;
        Refresh();
    }


    private void Awake()
    {
        _upButton.ArrowPressed += IncrementValue; 
        _downButton.ArrowPressed += DecrementValue;
    }

    private void IncrementValue()
    {
        _value++;
        Refresh();
    }

    private void DecrementValue()
    {
        _value--;
        Refresh();
    }

    private void Refresh()
    {
        _valueText.text = _value.ToString();
        _upButton.SetIsActive(_value < _max);
        _downButton.SetIsActive(_value > _min);
    }

}
