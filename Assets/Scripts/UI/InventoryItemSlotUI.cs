using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private RawImage _slotRawImage;
    [SerializeField] private RawImage _itemRawImage;
    private InputManager _inputManager;
    public Item Item => _item;
    private Item _item;
    public RectTransform RectTransform => _rectTransform;
    private RectTransform _rectTransform;

    public event Action<InventoryItemSlotUI> Hovered;
    public event Action<InventoryItemSlotUI> UnHovered;

    private void Awake()
    {
        _inputManager = FindFirstObjectByType<InputManager>();
        _rectTransform = GetComponent<RectTransform>();
    }

    #region Item

    public void LoadItem(Item item)
    {
        _item = item;
        _itemRawImage.gameObject.SetActive(true); //TODO: sprite
    }

    public void UnloadItem()
    {
        _item = null;
        _itemRawImage.gameObject.SetActive(false);
    } 

    #endregion


    #region Hovering

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_inputManager.CurrentInputDevice is Mouse || _inputManager.CurrentInputDevice is Keyboard)
        {
            Hover();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_inputManager.CurrentInputDevice is Mouse || _inputManager.CurrentInputDevice is Keyboard)
        {
            UnHover();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_inputManager.CurrentInputDevice is not Mouse && _inputManager.CurrentInputDevice is not Keyboard)
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
