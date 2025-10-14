using UnityEngine;

public class PixelUIScaler : MonoBehaviour
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private int _widthInPixels = 64;
    [SerializeField] private int _heightInPixels = 32;
    [SerializeField] private Vector2 _anchor = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 _offsetPixels = Vector2.zero;

    private RectTransform _rectTransform;
    private Vector2Int _lastScreenSize;
    private RectTransform _parentCanvasRect;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();
        _parentCanvasRect = canvas.GetComponent<RectTransform>();
        ApplyScaling();
        CacheScreenSize();
    }

    private void OnEnable()
    {
        ApplyScaling();
        CacheScreenSize();
    }

    private void Update()
    {
       if (ScreenSizeChanged())
       {
            ApplyScaling();
            CacheScreenSize();
        }
    }

    public void SetUIDefinition(PixelUIDefinition uiDefinition)
    {
        _uiDefinition = uiDefinition;
    }

    public void SetPosition(Vector2 anchor, Vector2 pivot, Vector2 offset)
    {
        _anchor = anchor;
        _rectTransform.pivot = pivot;
        _offsetPixels = offset;
        ApplyScaling();
    }

    public void SetPositionAndSize(Vector2 anchor, Vector2 pivot, Vector2 position, Vector2Int size)
    {
        _anchor = anchor;
        _rectTransform.pivot = pivot;
        _offsetPixels = position;
        _widthInPixels = size.x;
        _heightInPixels = size.y;
        ApplyScaling();
    }

    private bool ScreenSizeChanged()
    {
        return Screen.width != _lastScreenSize.x || Screen.height != _lastScreenSize.y;
    }

    private void CacheScreenSize()
    {
        _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
    }

    private void ApplyScaling()
    {
        if (_rectTransform == null || _uiDefinition == null)
            return;

        float scale = _parentCanvasRect.sizeDelta.y / _uiDefinition.TargetScreenHeightInPixels;


        // Convert pixel dimensions to scaled canvas size
        float width = _widthInPixels * scale;
        float height = _heightInPixels* scale;
        Vector2 offset = _offsetPixels * scale;

        _rectTransform.anchorMin = _anchor;
        _rectTransform.anchorMax = _anchor;
        _rectTransform.sizeDelta = new Vector2(width, height);
        _rectTransform.anchoredPosition = offset;
    }
}
