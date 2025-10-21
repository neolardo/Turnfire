using UnityEngine;
using UnityEngine.UI;

public class PixelSliderUI : MonoBehaviour
{
    [SerializeField] private Image _sliderValueImage;

    private const float _initialScale = 1;

    public void SetSliderValue(float normalizedValue)
    {
        _sliderValueImage.transform.localScale = new Vector3(_initialScale * normalizedValue, _sliderValueImage.transform.localScale.y, _sliderValueImage.transform.localScale.z);
    }
}
