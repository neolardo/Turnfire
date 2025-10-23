using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private InventoryToggleUI _weaponConsumableToggle;

    [Header("Item Info")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private List<SliderWithTextUI> _sliders;

    [Header("Item Slots")]
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private RectTransform _previewFrame;
    private List<InventoryItemSlotUI> _itemSlots;

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
        _weaponConsumableToggle.InitializeToggledLeftValue(true);

        _itemSlots = new List<InventoryItemSlotUI>();
        for (int i = 0; i < _slotContainer.childCount; i++)
        {
            var slot = _slotContainer.GetChild(i).GetComponent<InventoryItemSlotUI>();
            _itemSlots.Add(slot);   
        }

        foreach (var slot in _itemSlots)
        {
            slot.Hovered += PreviewSlot;
            slot.UnHovered += UnPreviewSlot;
        }
        _previewFrame.gameObject.SetActive(false);
        LoadItemInfo(null);
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
        DeselectSlot();
        foreach (var itemSlot in _itemSlots)
        {
            itemSlot.UnloadItem();
        }
        var items = _currentCharacter.GetAllItems().ToList();
        var selectedItem = _currentCharacter.GetSelectedItem();

        bool isToggledToWeapon = _weaponConsumableToggle.IsToggledLeft;
        var visibleItemType = isToggledToWeapon ? ItemType.Weapon : ItemType.Consumable;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Definition.ItemType == visibleItemType)
            {
                _itemSlots[i].LoadItem(items[i]);
                if (selectedItem != null && selectedItem == items[i])
                {
                    SelectSlot(_itemSlots[i], false);
                }
            }
        }
    }

    public void ToggleItemType()
    {
        _weaponConsumableToggle.Toggle();
        AudioManager.Instance.PlayUISound(_uiSounds.Toggle);
        RefreshInventory();
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
                _sliders[i].SetText(rangedStats[i].Group.DisplayName);
                _sliders[i].SetSliderValue(rangedStats[i].NormalizedValue);
                _sliders[i].gameObject.SetActive(true);
            }
        }
    }

    private void SelectPreviewedSlot()
    {
        SelectSlot(_previewedSlot);
    }

    private void SelectSlot(InventoryItemSlotUI slot, bool playUISound = true)
    {
        if (slot != null && slot.Item != null)
        {
            if (playUISound)
            {
                AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
            }
            _selectedSlot?.DeselectSlot();
            _selectedSlot = slot;
            _selectedSlot.SelectSlot();
            _currentCharacter.SelectItem(slot.Item);
            LoadItemInfo(slot.Item.Definition);
        }
    }

    private void DeselectSlot()
    {
        if (_selectedSlot != null)
        {
            _selectedSlot?.DeselectSlot();
            _selectedSlot = null;
            LoadItemInfo(null);
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
