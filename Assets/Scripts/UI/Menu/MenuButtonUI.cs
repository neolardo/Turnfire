using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonUI : ScreenSizeDependantUI, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private UISoundsDefinition _uiSounds;
    [SerializeField] private Sprite _pressedSprite;
    [SerializeField] private Sprite _hoveredSprite;
    [SerializeField] private Sprite _disabledSprite;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color _disabledTextColor;
    [SerializeField] private bool _disabled;
    private Sprite _normalSprite;

    private Image _image;
    private bool _hovered;
    private InputManager _inputManager;

    public event Action ButtonPressed;

    private readonly Vector2 TextHoverOffsetPixels = new Vector2(0, 1);
    private Vector2 _originalTextPosition;
    private RectTransform _parentCanvasRect;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _inputManager = FindFirstObjectByType<InputManager>();
        _normalSprite = _image.sprite;
        var canvas = FindFirstObjectByType<Canvas>();
        _parentCanvasRect = canvas.GetComponent<RectTransform>();
        if(_disabled)
        {
            _image.sprite = _disabledSprite;
            _text.color = _disabledTextColor;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UnHoverButton();
        _inputManager.MenuConfirmPerformed += OnMenuButtonConfirmPerformed;
    }

    private void OnDisable()
    {
        _inputManager.MenuConfirmPerformed -= OnMenuButtonConfirmPerformed;
    }

    private void OnMenuButtonConfirmPerformed()
    {
        if (_hovered || EventSystem.current.currentSelectedGameObject == gameObject)
        {
            Press();
        }
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

    private void Press()
    {
        SetPressedVisuals();
        InvokePressed();
    }

    private void SetPressedVisuals()
    {
        if (_disabled)
        {
            return; 
        }

        AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
        _image.sprite = _pressedSprite;
        _text.rectTransform.anchoredPosition = _originalTextPosition;
    }

    private void InvokePressed()
    {
        if (_disabled)
        {
            return;
        }

        ButtonPressed?.Invoke();
    }

    private void HoverButton()
    {
        if (_disabled)
        {
            return;
        }

        AudioManager.Instance.PlayUISound(_uiSounds.Hover);
        _image.sprite = _hoveredSprite;
        float scale = _parentCanvasRect.sizeDelta.y / _uiDefinition.TargetScreenHeightInPixels;
        var offset = TextHoverOffsetPixels * scale;
        _text.rectTransform.anchoredPosition = _originalTextPosition + offset;
        _hovered = true;
    }

    private void UnHoverButton()
    {
        if (_disabled)
        {
            return;
        }

        _image.sprite = _normalSprite;
        _hovered = false;
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
        _text.rectTransform.anchoredPosition = _hovered? _originalTextPosition + offset : _originalTextPosition;
    }
}
