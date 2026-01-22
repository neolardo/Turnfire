using UnityEngine;

public class AimCircleUI : MonoBehaviour
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private RectTransform _innerCircle;
    [SerializeField] private RectTransform _outerCircle;
    [SerializeField] private int _outerCirclePixelSize;
    [SerializeField] private int _innerCirclePixelSize;

    public static readonly Vector2 DefaultOffsetPercent = new Vector2(.13f, .76f);
    public float MouseAimRadiusScreenHeightRatio { get; private set; }

    private Vector2 _circleCenter;
    private RectTransform _rootCanvasRect;
    private Camera _camera;
    private SettingsController _settingsController;

    private void Awake()
    {
        _rootCanvasRect = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();
        _camera = Camera.main;
        float outerRatio = _outerCirclePixelSize / (float)_uiDefinition.TargetScreenHeightInPixels;
        float innerRatio =  _innerCirclePixelSize / (float)_uiDefinition.TargetScreenHeightInPixels;
        MouseAimRadiusScreenHeightRatio = (outerRatio - innerRatio) / 2f;
        _settingsController = FindFirstObjectByType<SettingsController>(FindObjectsInactive.Include);
    }

    public void ShowCircles(Vector2 initialScreenPosition)
    {
        Vector2 canvasSize = _rootCanvasRect.sizeDelta;
        var outerWidth = canvasSize.y / _uiDefinition.TargetScreenHeightInPixels * _outerCirclePixelSize;
        var innerWidth = canvasSize.y / _uiDefinition.TargetScreenHeightInPixels * _innerCirclePixelSize;

        _outerCircle.sizeDelta = new Vector2(outerWidth,outerWidth);
        _innerCircle.sizeDelta = new Vector2(innerWidth, innerWidth);

        Vector2 localPoint;
        bool isValid = initialScreenPosition.x >= 0 && initialScreenPosition.y >= 0;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootCanvasRect, initialScreenPosition, _camera, out localPoint);
        if (!isValid)
        {
            _circleCenter = DefaultOffsetPercent * canvasSize - canvasSize/2f;
        }
        else
        {
            _circleCenter = localPoint;
        }

        _innerCircle.anchoredPosition = _circleCenter;
        _outerCircle.anchoredPosition = _circleCenter;

        _innerCircle.gameObject.SetActive(true);
        _outerCircle.gameObject.SetActive(true);
    }

    public void HideCircles()
    {
        _innerCircle.gameObject.SetActive(false);
        _outerCircle.gameObject.SetActive(false);
    }

    public void UpdateCircles(Vector2 aimVector)
    {
        MoveInnerCircle(aimVector);
    }

    private void MoveInnerCircle(Vector2 aimVector)
    {
        float maxRadius = (_outerCircle.sizeDelta.x - _innerCircle.sizeDelta.x) * 0.5f;
        Vector2 offset = aimVector * maxRadius;
        if(_settingsController.GetInvertedInput())
        {
            offset *= -1;
        }
        _innerCircle.anchoredPosition = _circleCenter + offset;
    }

}
