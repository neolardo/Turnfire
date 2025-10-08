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
    [SerializeField] private List<InventoryItemSlotUI> _itemSlots;
    [SerializeField] private RectTransform _selectionFrame;
    [SerializeField] private RectTransform _previewFrame;

    private bool _isDestroyItemTypeToggleActive;
    private Character _currentCharacter;
    private InventoryItemSlotUI _previewedSlot;
    private InventoryItemSlotUI _selectedSlot;

    private void Awake()
    {
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ToggleInventoryCreateDestroyPerformed += ToggleItemType;
        inputManager.SelectInventorySlotPerformed += SelectPreviewedItem;
        _isDestroyItemTypeToggleActive = true;
        _createToggle.isOn = !_isDestroyItemTypeToggleActive;
        _destroyToggle.isOn = _isDestroyItemTypeToggleActive;

        foreach (var slot in _itemSlots)
        {
            slot.Hovered += PreviewSlot;
            slot.UnHovered += UnPreviewSlot;
        }
    }

    #region Select and Preview

    public void LoadItemInfo(ItemDefinition itemData)
    {
        _titleText.text = itemData.Name;
        _descriptionText.text = itemData.Description;

        //TODO
    }

    public void SelectPreviewedItem()
    {
        if (_previewedSlot != null && _previewedSlot.Item != null)
        {
            MoveSelectedFrame(_previewedSlot.RectTransform);
            _selectedSlot = _previewedSlot;
            _currentCharacter.SelectItem(_selectedSlot.Item);
        }
    }

    public void PreviewSlot(InventoryItemSlotUI slot)
    {
        if (_previewedSlot != null)
        {
            UnPreviewSlot(_previewedSlot);
        }

        _previewedSlot = slot;
        MovePreviewFrame(slot.RectTransform);
        if (slot.Item != null)
        {
            LoadItemInfo(slot.Item.Definition);
        }
    }

    public void UnPreviewSlot(InventoryItemSlotUI slot)
    {
        if (_previewedSlot == slot)
        {
            _previewFrame.gameObject.SetActive(false);
            _previewedSlot = null;
            if (_selectedSlot != null)
            {
                LoadItemInfo(_selectedSlot.Item.Definition);
            }
        }
    }

    private void MoveSelectedFrame(RectTransform slotTarget)
    {
        _selectionFrame.SetParent(slotTarget);
        _selectionFrame.anchorMin = Vector2.zero;
        _selectionFrame.anchorMax = Vector2.one;
        _selectionFrame.offsetMin = Vector2.zero;
        _selectionFrame.offsetMax = Vector2.zero;
        _selectionFrame.gameObject.SetActive(true);
    }

    private void MovePreviewFrame(RectTransform slotTarget)
    {
        _previewFrame.SetParent(slotTarget);
        _previewFrame.anchorMin = Vector2.zero;
        _previewFrame.anchorMax = Vector2.one;
        _previewFrame.offsetMin = Vector2.zero;
        _previewFrame.offsetMax = Vector2.zero;
        _previewFrame.gameObject.SetActive(true);
    }


    #endregion
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
        foreach (var itemSlot in _itemSlots)
        {
            itemSlot.UnloadItem();
        }
        var items = _currentCharacter.GetAllItems().ToList();
        var selectedItem = _currentCharacter.GetSelectedItem();
        for (int i = 0; i < items.Count; i++)
        {
            _itemSlots[i].LoadItem(items[i]); //TODO
            if(selectedItem != null && selectedItem == items[i])
            {
                _selectedSlot = _itemSlots[i];
                LoadItemInfo(selectedItem.Definition);
            }
        }
    }

    public void ToggleItemType()
    {
        _isDestroyItemTypeToggleActive = !_isDestroyItemTypeToggleActive;
        _createToggle.isOn = !_isDestroyItemTypeToggleActive;
        _destroyToggle.isOn = _isDestroyItemTypeToggleActive;
    }
}
