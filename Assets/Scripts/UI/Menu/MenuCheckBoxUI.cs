using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MenuCheckBoxUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    ISelectHandler,
    IDeselectHandler
    {
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private UISoundsDefinition _uiSounds;
    [SerializeField] private Sprite _checkedSprite;
    [SerializeField] private Sprite _hoveredSprite;
    [SerializeField] private bool _initialValue;
    private Sprite _normalSprite;
    private Image _image;
    private InputManager _inputManager;
    private bool _hovered;
    private bool _value;
    public bool Value => _value;

    public event Action<bool> ValueChanged;
    private void Awake()
    {
        _image = GetComponent<Image>();
        _inputManager = FindFirstObjectByType<InputManager>();
        _normalSprite = _image.sprite;
        var canvas = FindFirstObjectByType<Canvas>();
    }
    private void Start()
    {
        if (_value != _initialValue)
        {
            ToggleValue(false);
        }
    }

    private void OnEnable()
    {
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
            ToggleValue();
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
        ToggleValue();
    }

    public void OnSelect(BaseEventData eventData)
    {
        HoverButton();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UnHoverButton();
    }

    private void ToggleValue(bool playSound = true)
    {
        if(playSound)
        {
            AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
        }
        _value = !_value;
        ValueChanged?.Invoke(Value);
        _image.sprite = _value ? _checkedSprite : _normalSprite;
    }

    private void HoverButton()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.Hover);
        _image.sprite = _hoveredSprite;
        _hovered = true;
    }

    private void UnHoverButton()
    {
        _image.sprite = _value ? _checkedSprite : _normalSprite;
        _hovered = false;
    }
}
