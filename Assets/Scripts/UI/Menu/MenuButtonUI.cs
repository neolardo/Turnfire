using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MenuButtonUI : ScreenSizeDependantHoverableSelectableContainerUI
{
    [SerializeField] private Sprite _pressedSprite;
    [SerializeField] private Sprite _hoveredSprite;
    [SerializeField] private Sprite _disabledSprite;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color _disabledTextColor;
    private LocalMenuUIInputSource _inputManager;
    private Sprite _normalSprite;
    private Color _normalTextColor;

    private Image _image;

    public event Action ButtonPressed;

    protected override void Awake()
    {
        base.Awake();
        _rectTransform = _text.rectTransform;
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
        _image = GetComponent<Image>();
        _normalSprite = _image.sprite;
        _normalTextColor = _text.color;

        if (!interactable)
        {
            _image.sprite = _disabledSprite;
            _text.color = _disabledTextColor;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UnHover();
        _inputManager.MenuConfirmPerformed += OnMenuButtonConfirmPerformed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _inputManager.MenuConfirmPerformed -= OnMenuButtonConfirmPerformed;
    }

    private void OnMenuButtonConfirmPerformed()
    {
        if (_isHovered || EventSystem.current.currentSelectedGameObject == gameObject)
        {
            Press();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        UnHover();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        SetPressedVisuals();
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        InvokePressed();
    }

    public void Press()
    {
        SetPressedVisuals();
        InvokePressed();
    }

    private void SetPressedVisuals()
    {
        if (!interactable)
        {
            return; 
        }

        AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
        _image.sprite = _pressedSprite;
        _rectTransform.anchoredPosition = _originalAnchoredPosition;
    }

    private void InvokePressed()
    {
        if (!interactable)
        {
            return;
        }

        ButtonPressed?.Invoke();
    }

    protected override void Hover()
    {
        if (!interactable)
        {
            return;
        }
        base.Hover();

        _image.sprite = _hoveredSprite;
    }

    protected override void UnHover()
    {
        if (!interactable)
        {
            return;
        }
        base.UnHover();

        _image.sprite = _normalSprite;
    }

    public void SetIsInteractable(bool interactable)
    {
        this.interactable = interactable;
        if (!interactable)
        {
            _image.sprite = _disabledSprite;
            _text.color = _disabledTextColor;
        }
        else
        {
            _image.sprite = _normalSprite;
            _text.color = _normalTextColor;
        }    
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

}
