using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderWithTextUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _text;

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SetSliderValue(float value)
    {
        _slider.value = value;
    }

}
