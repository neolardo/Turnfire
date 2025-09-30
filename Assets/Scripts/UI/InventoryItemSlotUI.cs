using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlotUI : MonoBehaviour
{
    [SerializeField] private RawImage _slotRawImage;
    [SerializeField] private RawImage _itemRawImage;

    private ItemData _itemData;
    private Color _originalColor;

    public event Action<ItemData> Selected;

    private void Awake()
    {
        _originalColor = _slotRawImage.color;   
    }

    public void LoadItemData(ItemData itemData)
    {
        _itemData = itemData;
        _itemRawImage.gameObject.SetActive(true); //TODO: sprite
    }

    public void UnloadItemData()
    {
        _itemData = null;
        _itemRawImage.gameObject.SetActive(false);
    }

    public void SelectSlot()
    {
        _slotRawImage.color = Color.green; //TODO
        Selected?.Invoke(_itemData);
    }

    public void DeselectSlot()
    {
        _slotRawImage.color = _originalColor; //TODO
    }

}
