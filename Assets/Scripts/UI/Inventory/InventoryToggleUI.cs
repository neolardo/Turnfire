using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryToggleUI : MonoBehaviour
{
    [SerializeField] private Image _leftImage; 
    [SerializeField] private Image _rightImage;
    [SerializeField] private TextMeshProUGUI _leftText;
    [SerializeField] private TextMeshProUGUI _rightText;
    [SerializeField] private PixelUIScaler _leftPixelUI;
    [SerializeField] private PixelUIScaler _rightPixelUI;
    [SerializeField] private Vector2 _leftUntoggledPixelOffset;
    [SerializeField] private Vector2 _rightUntoggledPixelOffset;
    [SerializeField] private Vector2 _leftToggledPixelOffset;
    [SerializeField] private Vector2 _rightToggledPixelOffset;
    [SerializeField] private Color _toggledOnColor;
    [SerializeField] private Color _toggledOffColor;

    public bool IsToggledLeft => _isToggledLeft;

    private bool _isToggledLeft;


    private void Start()
    {
        UpdateLayout();
    }

    public void InitializeToggledLeftValue(bool isToggledLeft)
    {
        _isToggledLeft = isToggledLeft;
        UpdateLayout();
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
            _leftPixelUI.SetPixelOffset(_leftToggledPixelOffset);
            _rightPixelUI.SetPixelOffset(_rightUntoggledPixelOffset);
            _leftText.color = _toggledOnColor;
            _rightText.color = _toggledOffColor;
        }
        else
        {
            _leftPixelUI.SetPixelOffset(_leftUntoggledPixelOffset);
            _rightPixelUI.SetPixelOffset(_rightToggledPixelOffset);
            _leftText.color = _toggledOffColor;
            _rightText.color = _toggledOnColor;
        }
    }
}
