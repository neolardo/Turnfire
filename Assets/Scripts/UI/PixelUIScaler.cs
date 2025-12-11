using UnityEngine;

public class PixelUIScaler : ScreenSizeDependantUI
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private int _widthInPixels = 64;
    [SerializeField] private int _heightInPixels = 32;
    [SerializeField] private Vector2 _anchor = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 _offsetPixels = Vector2.zero;

    private Vector2 _pivot = new Vector2(0.5f, 0.5f);
    private RectTransform _rectTransform;
    private RectTransform _parentCanvasRect;

    public Vector2Int SizeInPixels => new Vector2Int(_widthInPixels, _heightInPixels);

    private bool _pivotSet;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (!_pivotSet)
        {
            _pivot = _rectTransform.pivot;
        }
        var canvas = GetComponentInParent<Canvas>();
        _parentCanvasRect = canvas.GetComponent<RectTransform>();
    }

    protected override void OneFrameAfterOnEnable()
    {
        ApplyScaling();
    }

    public void SetUIDefinition(PixelUIDefinition uiDefinition)
    {
        _uiDefinition = uiDefinition;
    }

    public void SetPixelOffset(Vector2 offset)
    {
        _offsetPixels = offset;
        ApplyScaling();
    }

    public void SetPosition(Vector2 anchor, Vector2 pivot, Vector2 offset)
    {
        _anchor = anchor;
        _pivot = pivot;
        _pivotSet = true;
        _offsetPixels = offset;
        ApplyScaling();
    }

    public void SetPositionAndSize(Vector2 anchor, Vector2 pivot, Vector2 position, Vector2Int size)
    {
        _anchor = anchor;
        _pivot = pivot;
        _pivotSet = true;
        _offsetPixels = position;
        _widthInPixels = size.x;
        _heightInPixels = size.y;
        ApplyScaling();
    }

    private void ApplyScaling()
    {
        if (_rectTransform == null || _uiDefinition == null)
            return;

        _rectTransform.pivot = _pivot;

        float scale = _parentCanvasRect.sizeDelta.y / _uiDefinition.TargetScreenHeightInPixels;

        // Convert pixel dimensions to scaled canvas size
        float width = _widthInPixels * scale;
        float height = _heightInPixels * scale;
        Vector2 offset = _offsetPixels * scale;

        _rectTransform.anchorMin = _anchor;
        _rectTransform.anchorMax = _anchor;
        _rectTransform.sizeDelta = new Vector2(width, height);
        _rectTransform.anchoredPosition = offset;
    }

    protected override void OneFrameAfterSizeChanged()
    {
        ApplyScaling();
    }
}
