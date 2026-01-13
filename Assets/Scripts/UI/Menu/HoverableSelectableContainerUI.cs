using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverableSelectableContainerUI : Selectable
{
    [Header("Pixel UI")]
    [SerializeField] protected PixelUIDefinition _uiDefinition;
    [SerializeField] protected UISoundsDefinition _uiSounds;
    protected RectTransform _rectTransform;
    protected RectTransform _parentCanvasRect;
    protected Vector2 _originalAnchoredPosition;
    protected bool _isHovered;
    public bool IsSelected { get; private set; }
    protected bool _isAnchoredPositionInitialized;

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
        Hover();
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
        UnHover();
    }

    protected virtual void Hover()
    {
        if (!interactable || _isHovered)
        {
            return;
        }
        AudioManager.Instance.PlayUISound(_uiSounds.Hover);
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
