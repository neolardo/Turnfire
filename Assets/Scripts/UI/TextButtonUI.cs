using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextButtonUI : ScreenSizeDependantUI,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private UISoundsDefinition _uiSounds;
    [SerializeField] private Color _pressedColor;
    [SerializeField] private TextMeshProUGUI _text;
    private Color _normalColor;

    private bool _hovered;

    public event Action ButtonPressed;

    private readonly Vector2 TextHoverOffsetPixels = new Vector2(0, 1);
    private Vector2 _originalTextPosition;
    private RectTransform _parentCanvasRect;

    private void Awake()
    {
        _normalColor = _text.color;
        var canvas = FindFirstObjectByType<Canvas>();
        _parentCanvasRect = canvas.GetComponent<RectTransform>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UnHoverButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverButton();
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnHoverButton();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPressedVisuals();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        InvokePressed();
    }

    public void OnSelect(BaseEventData eventData)
    {
        HoverButton();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UnHoverButton();
    }

    public void PressIfHoveredOrSelected()
    {
        if(_hovered || EventSystem.current.currentSelectedGameObject == gameObject)
        {
            Press();
        }
    }

    private void Press()
    {
        SetPressedVisuals();
        InvokePressed();
    }

    private void SetPressedVisuals()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
        _text.color = _pressedColor;
        _text.rectTransform.anchoredPosition = _originalTextPosition;
    }

    private void InvokePressed()
    {
        ButtonPressed?.Invoke();
    }

    private void HoverButton()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.Hover);
        float scale = _parentCanvasRect.sizeDelta.y / _uiDefinition.TargetScreenHeightInPixels;
        var offset = TextHoverOffsetPixels * scale;
        _text.rectTransform.anchoredPosition = _originalTextPosition + offset;
        _hovered = true;
    }

    private void UnHoverButton()
    {
        _hovered = false;
        _text.color = _normalColor;
        _text.rectTransform.anchoredPosition = _originalTextPosition;
    }

    protected override void OneFrameAfterOnEnable()
    {
        _originalTextPosition = _text.rectTransform.anchoredPosition;
    }

    protected override void OneFrameAfterSizeChanged()
    {
        float scale = _parentCanvasRect.sizeDelta.y / _uiDefinition.TargetScreenHeightInPixels;
        var offset = TextHoverOffsetPixels * scale;
        _text.rectTransform.anchoredPosition = _hovered ? _originalTextPosition + offset : _originalTextPosition;
    }
}
