using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private InventoryToggleUI _itemTypeToggle;

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
        var inputHandler = FindFirstObjectByType<LocalInputHandler>();
        inputHandler.ToggleInventoryCreateDestroyPerformed += _itemTypeToggle.Toggle;
        _itemTypeToggle.InitializeToggledLeftValue(true);
        _itemTypeToggle.Toggled += OnItemTypeToggled;

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
            slot.Selected += (s) => SelectSlot(s);
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
        EventSystem.current.SetSelectedGameObject(_itemSlots.First().gameObject);
    }

    private void OnDisable()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.InventoryOff);
    }

    private void RefreshInventory()
    {
        DeselectSlot();
        _previewFrame.gameObject.SetActive(false);
        foreach (var itemSlot in _itemSlots)
        {
            itemSlot.UnloadItem();
        }
        var items = _currentCharacter.GetAllItems().ToList();
        var selectedItem = _currentCharacter.SelectedItem;

        int slotIndex = 0;
        foreach(var item in items)
        {
            if (IsItemTypeTheToggledType(item))
            {
                var currentSlot = _itemSlots[slotIndex];
                currentSlot.LoadItem(item);
                if (selectedItem != null && selectedItem == item)
                {
                    MarkSlotAsSelected(currentSlot);
                }
                slotIndex++;
            }
        }
    }

    private bool IsItemTypeTheToggledType(ItemInstance item)
    {
        bool isToggledToWeapon = _itemTypeToggle.IsToggledLeft;
        return (isToggledToWeapon && item.Definition.ItemType == ItemType.Weapon) ||
                (!isToggledToWeapon && item.Definition.ItemType != ItemType.Weapon);
    }

    private void OnItemTypeToggled()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.Toggle);
        RefreshInventory();
    }

    private void ForceClose()
    {
        _currentCharacter.Team.InputSource.ForceCloseInventory();
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
                _sliders[i].SetSliderValue(rangedStats[i].NormalizedDislayValue);
                _sliders[i].gameObject.SetActive(true);
            }
        }
    }

    private void SelectSlot(InventoryItemSlotUI slot)
    {
        if (slot != null && slot.Item != null)
        {
            var selectionSucceeded = _currentCharacter.TrySelectItem(slot.Item); //TODO: use input source instead
            if (selectionSucceeded)
            {
                AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
                MarkSlotAsSelected(slot);
                LoadItemInfo(slot.Item.Definition);
                if (slot.Item.Definition.UseInstantlyWhenSelected)
                {
                    ForceClose();
                }
            }
            else
            {
                AudioManager.Instance.PlayUISound(_uiSounds.CannotUseItem);
            }
        }
    }

    private void MarkSlotAsSelected(InventoryItemSlotUI slot)
    {
        _selectedSlot?.OnSlotDeselected();
        _selectedSlot = slot;
        _selectedSlot.OnSlotSelected();
    }
    private void DeselectSlot()
    {
        if (_selectedSlot != null)
        {
            _selectedSlot?.OnSlotDeselected();
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
