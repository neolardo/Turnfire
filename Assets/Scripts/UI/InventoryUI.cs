using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private Toggle _createToggle;
    [SerializeField] private Toggle _destroyToggle;
    [SerializeField] private GridLayoutGroup _itemGrid;

    [Header("Item Info")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private SliderWithTextUI _slider1; //TODO
    [SerializeField] private SliderWithTextUI _slider2;
    [SerializeField] private SliderWithTextUI _slider3;

    private bool _isDestroyItemTypeToggleActive;

    private void Awake()
    {
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ToggleInventoryCreateDestroyPerformed += ToggleItemType;
        _isDestroyItemTypeToggleActive = true;
        _createToggle.isOn = !_isDestroyItemTypeToggleActive;
        _destroyToggle.isOn = _isDestroyItemTypeToggleActive;
    }

    public void ShowItemInfo(string title, string description, float damage, float distance, float explosion)
    {
        _titleText.text = title;
        _descriptionText.text = description;

        _slider1.SetSliderValue(damage); //TODO
        _slider1.SetText("Damage");
        _slider1.SetSliderValue(distance);
        _slider1.SetText("Distance");
        _slider1.SetSliderValue(explosion);
        _slider1.SetText("Explosion");
    }

    public void ToggleItemType()
    {
        _isDestroyItemTypeToggleActive = !_isDestroyItemTypeToggleActive;
        _createToggle.isOn = !_isDestroyItemTypeToggleActive;
        _destroyToggle.isOn = _isDestroyItemTypeToggleActive;
    }
}
