using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MenuArrowButtonUI : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private UISoundsDefinition _uiSounds;
    [SerializeField] private Sprite _inactiveSprite;
    [SerializeField] private Sprite _hoveredSprite;
    private Sprite _activeSprite;
    private LocalMenuInput _inputManager;

    private Image _image;
    private bool _hovered;

    public event Action ArrowPressed;

    private bool _isActive = true;

    public void SetIsActive( bool isActive)
    { 
        _isActive = isActive;
        UnHoverButton();
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _activeSprite = _image.sprite;
        _image.sprite = _isActive ? _activeSprite : _inactiveSprite;
        _inputManager = FindFirstObjectByType<LocalMenuInput>();
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
        Press();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        HoverButton(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        HoverButton();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UnHoverButton();
    }

    public void Press()
    {
        if(!_isActive)
        {
            return;
        }

        AudioManager.Instance.PlayUISound(_uiSounds.Confirm);
        _image.sprite = _activeSprite;
        ArrowPressed?.Invoke();
    }

    private void HoverButton(bool playSound = true)
    {
        if (!_isActive)
        {
            return;
        }
        if (playSound)
        {
            AudioManager.Instance.PlayUISound(_uiSounds.Hover);
        }
        _image.sprite = _hoveredSprite;
        _hovered = true;
    }

    private void UnHoverButton()
    {
        _image.sprite = _isActive? _activeSprite : _inactiveSprite;
        _hovered = false;
    }

}
