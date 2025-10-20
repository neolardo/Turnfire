using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryToggleUI : MonoBehaviour
{
    [SerializeField] private Image _leftImage; 
    [SerializeField] private Image _rightImage;
    [SerializeField] private TextMeshProUGUI _leftText;
    [SerializeField] private TextMeshProUGUI _rightText;
    [SerializeField] private Vector2 _leftAnchorDelta;
    [SerializeField] private Vector2 _rightAnchorDelta;
    [SerializeField] private Color _toggledOnColor;
    [SerializeField] private Color _toggledOffColor;

    private Vector2 _initialAnchorLeft;
    private Vector2 _initialAnchorRight;
    public bool IsToggledLeft => _isToggledLeft;

    private bool _isToggledLeft;

    private void Start()
    {
        _initialAnchorLeft = _leftImage.rectTransform.anchorMax;
        _initialAnchorRight = _rightImage.rectTransform.anchorMax;
    }

    public void SetLeftToggleValue(bool isToggledLeft)
    {
        _isToggledLeft = isToggledLeft;
    }

    public void Toggle()
    {
        _isToggledLeft = !_isToggledLeft;

        UpdateLayout();
    }

    private void UpdateLayout()
    {
        _leftImage.enabled = _isToggledLeft;
        _rightImage.enabled = !_isToggledLeft;
        if (_isToggledLeft)
        {
            _leftImage.rectTransform.anchorMax = _initialAnchorLeft;
            _leftImage.rectTransform.anchorMin = _initialAnchorLeft;
            _rightImage.rectTransform.anchorMax = _initialAnchorRight + _rightAnchorDelta;
            _rightImage.rectTransform.anchorMin = _initialAnchorRight + _rightAnchorDelta;
            _leftText.color = _toggledOnColor;
            _rightText.color = _toggledOffColor;
        }
        else
        {
            _leftImage.rectTransform.anchorMax = _initialAnchorLeft + _leftAnchorDelta;
            _leftImage.rectTransform.anchorMin = _initialAnchorLeft  + _leftAnchorDelta;
            _rightImage.rectTransform.anchorMax = _initialAnchorRight;
            _rightImage.rectTransform.anchorMin = _initialAnchorRight;
            _leftText.color = _toggledOffColor;
            _rightText.color = _toggledOnColor;
        }
    }
}
