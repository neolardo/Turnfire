using TMPro;
using UnityEngine;

public class SliderWithTextUI : MonoBehaviour
{
    [SerializeField] private InventorySliderUI _slider;
    [SerializeField] private TextMeshProUGUI _text;

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetSliderValue(float normalizedValue)
    {
        _slider.SetSliderValue( normalizedValue);
    }

}
