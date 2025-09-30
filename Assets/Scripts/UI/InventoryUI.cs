using System.Collections.Generic;
using System.Linq;
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

    [Header("Item Slots")]
    [SerializeField] private List<InventoryItemSlotUI> itemSlots;

    private bool _isDestroyItemTypeToggleActive;
    private Character _currentCharacter;

    private void Awake()
    {
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ToggleInventoryCreateDestroyPerformed += ToggleItemType;
        _isDestroyItemTypeToggleActive = true;
        _createToggle.isOn = !_isDestroyItemTypeToggleActive;
        _destroyToggle.isOn = _isDestroyItemTypeToggleActive;

        foreach (var slot in itemSlots)
        {
            slot.Selected += OnItemSelected;
        }
    }

    public void LoadItemInfo(ItemData itemData)
    {
        _titleText.text = itemData.Name;
        _descriptionText.text = itemData.Description;

        if (itemData is WeaponData weaponData)
        {
            _slider1.SetSliderValue(weaponData.FireStrength.Avarage); //TODO
            _slider1.SetText("Fire Strength");
            //TODO
        }
        else if (itemData is ModifierData modifierData)
        {
            //TODO
        }
    }

    public void OnItemSelected(ItemData itemData)
    {
        LoadItemInfo(itemData);
    }

    public void LoadCharacterData(Character c)
    {
        _currentCharacter = c;
    }

    private void OnEnable()
    {
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        //TODO
        foreach (var itemSlot in itemSlots)
        {
            itemSlot.UnloadItemData();
        }
        var items = _currentCharacter.GetAllItems().ToList();
        for (int i = 0; i < items.Count; i++)
        {
            itemSlots[i].LoadItemData(items[i].ItemData); //TODO
        }
    }


    public void ToggleItemType()
    {
        _isDestroyItemTypeToggleActive = !_isDestroyItemTypeToggleActive;
        _createToggle.isOn = !_isDestroyItemTypeToggleActive;
        _destroyToggle.isOn = _isDestroyItemTypeToggleActive;
    }
}
