using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private InventoryToggleUI _weaponModifierToggle;

    [Header("Item Info")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private List<SliderWithTextUI> _sliders;

    [Header("Item Slots")]
    [SerializeField] private List<InventoryItemSlotUI> _itemSlots;
    [SerializeField] private RectTransform _previewFrame;

    [Header("Sounds")]
    [SerializeField] private UISoundsDefinition _uiSounds;

    private Character _currentCharacter;
    private InventoryItemSlotUI _previewedSlot;
    private InventoryItemSlotUI _selectedSlot;


    private void Awake()
    {
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ToggleInventoryCreateDestroyPerformed += ToggleItemType;
        inputManager.SelectInventorySlotPerformed += SelectPreviewedSlot;
        _weaponModifierToggle.SetLeftToggleValue(true);

        foreach (var slot in _itemSlots)
        {
            slot.Hovered += PreviewSlot;
            slot.UnHovered += UnPreviewSlot;
        }
    }

    public void LoadCharacterData(Character c)
    {
        _currentCharacter = c;
    }

    private void OnEnable()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.InventoryOn);
        RefreshInventory();
    }

    private void OnDisable()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.InventoryOff);
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
            _itemSlots[i].LoadItem(items[i]);
            if (selectedItem != null && selectedItem == items[i])
            {
                SelectSlot(_itemSlots[i]);
            }
        }
    }

    public void ToggleItemType()
    {
        _weaponModifierToggle.Toggle();
        AudioManager.Instance.PlayUISound(_uiSounds.Toggle);
    }

    #region Select and Preview

    public void LoadItemInfo(ItemDefinition itemDefinition)
    {
        if(itemDefinition == null)
        {
            _titleText.text = "Empty";
            _descriptionText.text = "This slot is empty.";

            foreach (var s in _sliders)
            {
                s.gameObject.SetActive(false);
            }
        }
        else
        {
            _titleText.text = itemDefinition.Name;
            _descriptionText.text = itemDefinition.Description;

            foreach (var s in _sliders)
            {
                s.gameObject.SetActive(false);
            }

            var rangedStats = itemDefinition.GetRangedStats().ToList();
            for (int i = 0; i < _sliders.Count && i < rangedStats.Count; i++)
            {
                _sliders[i].SetText(rangedStats[i].Group.Name);
                _sliders[i].SetSliderValue(rangedStats[i].NormalizedValue);
                _sliders[i].gameObject.SetActive(true);
            }
        }
    }

    private void SelectPreviewedSlot()
    {
        SelectSlot(_previewedSlot);
    }

    private void SelectSlot(InventoryItemSlotUI slot)
    {
        if (slot != null && slot.Item != null)
        {
            AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
            _selectedSlot?.DeselectSlot();
            _selectedSlot = slot;
            _selectedSlot.SelectSlot();
            _currentCharacter.SelectItem(slot.Item);
            LoadItemInfo(slot.Item.Definition);
        }
    }

    private void PreviewSlot(InventoryItemSlotUI slot)
    {
        if (_previewedSlot != null)
        {
            UnPreviewSlot(_previewedSlot);
        }

        AudioManager.Instance.PlayUISound(_uiSounds.Hover);
        _previewedSlot = slot;
        MovePreviewFrame(slot.transform);
        LoadItemInfo(slot.Item == null ? null : slot.Item.Definition);
    }

    private void UnPreviewSlot(InventoryItemSlotUI slot)
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

    private void MovePreviewFrame(Transform slotTarget)
    {
        _previewFrame.SetParent(slotTarget);
        _previewFrame.anchorMin = Vector2.zero;
        _previewFrame.anchorMax = Vector2.one;
        _previewFrame.offsetMin = Vector2.zero;
        _previewFrame.offsetMax = Vector2.zero;
        _previewFrame.gameObject.SetActive(true);
    }


    #endregion

}
