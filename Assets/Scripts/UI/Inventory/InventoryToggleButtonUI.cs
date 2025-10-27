using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(TransparentRaycastImage))]
public class InventoryToggleButtonUI : MonoBehaviour, IPointerDownHandler
{
    public event Action ButtonPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonPressed?.Invoke();
    }
}
