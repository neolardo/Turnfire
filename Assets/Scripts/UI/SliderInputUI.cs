using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SliderInputUI : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    [SerializeField] private UISoundsDefinition _uiSounds;
    [Header("Inner Value")]
    [SerializeField] private Image _sliderValueImage;
    [SerializeField] private PixelUIScaler _sliderValuePixelUIScaler;
    [Header("Sprites")]
    [SerializeField] private Sprite _emptyHoveredSprite;
    [SerializeField] private Sprite _fullSprite;
    [SerializeField] private Sprite _fullHoveredSprite;
    private Sprite _emptySprite;
    private Image _image; 
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private bool _isHovered;
    private bool _isDragged;
    private bool IsHoveredOrDragged => _isHovered || _isDragged;
    public float Value { get; private set; }

    private readonly Vector2Int _originalInnerPixelOffset = new Vector2Int(1, 1);
    private readonly Vector2Int _hoveredInnerPixelOffset = new Vector2Int(1, 2);
    private const float _initialScale = 1;
    private const float _maxValue = 1;
    private const float _valueIncrement = .8f;

    public event Action<float> ValueChanged;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
        _canvas = GetComponentInParent<Canvas>();
        _image = GetComponent<Image>();
        _emptySprite = _image.sprite;
        if (_canvas == null)
        {
            Debug.LogError("RawImageSliderInput must be placed under a Canvas.");
        }
    }

    private void OnEnable()
    { 
        RefreshVisuals();
    }

    public void IncrementSliderValue()
    {
        if(_isHovered || EventSystem.current.currentSelectedGameObject == this)
        {
            SetValue(Mathf.Clamp01(Value + _valueIncrement));
        }
    }

    public void DecrementSliderValue()
    {
        SetValue(Mathf.Clamp01(Value - _valueIncrement));
    }

    public void SetInitialValue(float initialValue)
    {
        Value = initialValue;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateValueFromPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateValueFromPointer(eventData);
    }

    private void UpdateValueFromPointer(PointerEventData eventData)
    {
        if (_canvas == null)
            return;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, _canvas.worldCamera, out localPoint))
        {
            return;
        }

        float width = _rectTransform.rect.width;
        float x = localPoint.x + width * 0.5f;
        float normalized = Mathf.Clamp01(x / width);

        SetValue(normalized);
    }

    private void SetValue(float value)
    {
        if (Mathf.Approximately(Value, value))
            return;

        Value = value;
        ValueChanged?.Invoke(Value);
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        _sliderValueImage.transform.localScale = new Vector3(_initialScale * Value, _sliderValueImage.transform.localScale.y, _sliderValueImage.transform.localScale.z);
        _sliderValuePixelUIScaler.SetPixelOffset(IsHoveredOrDragged ? _hoveredInnerPixelOffset : _originalInnerPixelOffset);
        if(Value < _maxValue)
        {
            _image.sprite = IsHoveredOrDragged ? _emptyHoveredSprite : _emptySprite;
        }
        else
        {
            _image.sprite = IsHoveredOrDragged ? _fullHoveredSprite : _fullSprite;
        }
    }

    public void HoverSlider()
    {
        if(!IsHoveredOrDragged)
        {
            AudioManager.Instance.PlayUISound(_uiSounds.Hover);
        }
        _isHovered = true;
        RefreshVisuals();
    }


    public void UnhoverSlider()
    {
        _isHovered = false;
        RefreshVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverSlider();
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnhoverSlider();
    }

    public void OnSelect(BaseEventData eventData)
    {
        HoverSlider();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UnhoverSlider();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragged = false;
    }
}
