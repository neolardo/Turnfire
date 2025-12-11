using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
[RequireComponent(typeof(RectTransform))]
public class InventoryItemSlotUI : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler, 
    IPointerDownHandler, 
    ISelectHandler,
    IDeselectHandler
{
    [SerializeField] private Image _slotImage;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Sprite _selectedSlotSprite;
    [SerializeField] private Sprite _deselectedSlotSprite;
    [SerializeField] private TextMeshProUGUI _quantityText;
    private LocalGameplayInput _inputManager;
    public Item Item => _item;
    private Item _item;

    public event Action<InventoryItemSlotUI> Selected;
    public event Action<InventoryItemSlotUI> Hovered;
    public event Action<InventoryItemSlotUI> UnHovered;

    private void Awake()
    {
        _inputManager = FindFirstObjectByType<LocalGameplayInput>();
        var selectable = GetComponent<Selectable>();
        selectable.transition = Selectable.Transition.None;
    }

    #region Item

    public void LoadItem(Item item)
    {
        _item = item;
        _itemImage.sprite = item.Definition.Sprite;
        _itemImage.gameObject.SetActive(true);
        _quantityText.text = (item.Definition.IsQuantityInfinite || item.Definition.MaximumQuantity == 1) ? string.Empty : item.Quantity.ToString();
    }

    public void UnloadItem()
    {
        _item = null;
        _itemImage.sprite = null;
        _itemImage.gameObject.SetActive(false);
        _quantityText.text = string.Empty;
    }

    public void OnSlotDeselected()
    {
        _slotImage.sprite = _deselectedSlotSprite;
    }

    public void OnSlotSelected()
    {
        _slotImage.sprite = _selectedSlotSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Selected?.Invoke(this);
    }

    #endregion

    #region Hovering

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UnHover();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(_inputManager.CurrentInputDevice is not Mouse && _inputManager.CurrentInputDevice is not Keyboard)
        {
            Hover();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_inputManager.CurrentInputDevice is not Mouse && _inputManager.CurrentInputDevice is not Keyboard)
        {
            UnHover();
        }
    }


    private void UnHover()
    {
        UnHovered?.Invoke(this);
    }

    private void Hover()
    {
        Hovered?.Invoke(this);
    }


    #endregion

}
