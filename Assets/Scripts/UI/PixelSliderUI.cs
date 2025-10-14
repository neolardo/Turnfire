using UnityEngine;
using UnityEngine.UI;

public class PixelSliderUI : MonoBehaviour
{
    [SerializeField] private Image _sliderValueImage;

    private float _initialScale;

    private void Awake()
    {
        _initialScale = _sliderValueImage.transform.localScale.x;
    }

    public void SetSliderValue(float normalizedValue)
    {
        _sliderValueImage.transform.localScale = new Vector3(_initialScale * normalizedValue, _sliderValueImage.transform.localScale.y, _sliderValueImage.transform.localScale.z);
    }
}
