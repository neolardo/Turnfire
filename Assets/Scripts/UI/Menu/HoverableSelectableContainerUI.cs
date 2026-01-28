using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverableSelectableContainerUI : Selectable, ISubmitHandler
{
    [Header("Pixel UI")]
    [SerializeField] protected PixelUIDefinition _uiDefinition;
    [SerializeField] protected UISoundsDefinition _uiSounds;
    [SerializeField] protected bool _animateOnSelect = true;
    protected RectTransform _rectTransform;
    protected RectTransform _parentCanvasRect;
    protected Vector2 _originalAnchoredPosition;
    protected bool _isHovered; 
    protected bool _isAnchoredPositionInitialized;
    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            var oldVal = _isSelected;
            _isSelected = value;
            if (value != oldVal)
            {
                SelectionChanged?.Invoke(value);
            }
        }
    }

    public event Action<bool> SelectionChanged;
    public event Action SubmitPerformed;

    protected override void Awake()
    {
        base.Awake();
        transition = Transition.None;
        var canvas = FindFirstObjectByType<Canvas>();
        _parentCanvasRect = canvas.GetComponent<RectTransform>();
        _rectTransform = GetComponent<RectTransform>();
    }
    protected override void Start()
    {
        if (!_isAnchoredPositionInitialized)
        { 
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
            _isAnchoredPositionInitialized = true;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        IsSelected = true;
        if (_animateOnSelect)
        {
            Hover();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        IsSelected = false;
        if (_animateOnSelect)
        {
            UnHover();
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        SubmitPerformed?.Invoke();
    }

    protected virtual void Hover()
    {
        if (!interactable || _isHovered)
        {
            return;
        }
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUISound(_uiSounds.Hover);
        }
        _rectTransform.anchoredPosition += _uiDefinition.CalculateHoverOffset(_parentCanvasRect.sizeDelta.y);
        _isHovered = true;
    }


    protected virtual void UnHover()
    {
        if (!interactable || !_isHovered)
        {
            return;
        }
        _rectTransform.anchoredPosition = _originalAnchoredPosition;
        _isHovered = false;
    }

}
