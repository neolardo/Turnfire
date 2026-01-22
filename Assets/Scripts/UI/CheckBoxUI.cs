using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CheckBoxUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    ISelectHandler,
    IDeselectHandler,
    ISubmitHandler
    {
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private UISoundsDefinition _uiSounds;
    [SerializeField] private Sprite _checkedSprite;
    [SerializeField] private Sprite _hoveredSprite;
    [SerializeField] private bool _initialValue;
    [SerializeField] private HoverableSelectableContainerUI _containerUI;
    private Sprite _normalSprite;
    private Image _image;
    private bool _hovered;
    private bool _value;
    public bool Value => _value;

    public event Action<bool> ValueChanged;
    private void Awake()
    {
        _image = GetComponent<Image>();
        _normalSprite = _image.sprite;
        var canvas = FindFirstObjectByType<Canvas>();
        _containerUI.SelectionChanged += OnContainerSelectionChanged;
        _containerUI.SubmitPerformed += OnSubmitPerformed;
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
    }

    public void OnDecrementOrIncrementValuePerformed()
    {
        if (_containerUI.IsSelected || _hovered)
        {
            ToggleValue();
        }
    }

    private void OnContainerSelectionChanged(bool isContainerHovered)
    {
        if (isContainerHovered)
        {
            HoverButton();
        }
        else
        {
            UnHoverButton();
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

    public void OnSubmit(BaseEventData eventData)
    {
        OnSubmitPerformed();
    }

    private void OnSubmitPerformed()
    {
        ToggleValue();
    }

    public void SetInitialValue(bool initialValue)
    {
        _initialValue = initialValue;
        _value = initialValue;
        RefreshVisuals();
    }

    public void ToggleValue(bool playSound = true)
    {
        if(playSound)
        {
            AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
        }
        _value = !_value;
        ValueChanged?.Invoke(Value);
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        _image.sprite = _value ? _checkedSprite : _normalSprite;
    }

    private void HoverButton()
    {
        if(_hovered)
        {
            return;
        }
        AudioManager.Instance.PlayUISound(_uiSounds.Hover);
        _image.sprite = _hoveredSprite;
        _hovered = true;
    }

    private void UnHoverButton()
    {
        if (!_hovered)
        {
            return;
        }
        _image.sprite = _value ? _checkedSprite : _normalSprite;
        _hovered = false;
    }
}
