using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryToggleUI : MonoBehaviour
{
    [SerializeField] private InventoryToggleButtonUI _leftButton; 
    [SerializeField] private InventoryToggleButtonUI _rightButton; 
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

    public event Action Toggled;

    private void Awake()
    {
        _leftButton.ButtonPressed += ToggleToLeft;
        _rightButton.ButtonPressed += ToggleToRight;
    }

    private void Start()
    {
        UpdateLayout();
    }

    public void InitializeToggledLeftValue(bool isToggledLeft)
    {
        _isToggledLeft = isToggledLeft;
        Toggled?.Invoke();
        UpdateLayout();
    }

    public void Toggle()
    {
        _isToggledLeft = !_isToggledLeft;
        Toggled?.Invoke();
        UpdateLayout();
    }

    private void ToggleToLeft()
    {
        if(!_isToggledLeft)
        {
            Toggle();
        }
    }

    private void ToggleToRight()
    {
        if (_isToggledLeft)
        {
            Toggle();
        }
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
