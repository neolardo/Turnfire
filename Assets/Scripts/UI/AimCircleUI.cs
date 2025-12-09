using UnityEngine;

public class AimCircleUI : MonoBehaviour
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private RectTransform _innerCircle;
    [SerializeField] private RectTransform _outerCircle;

    public const float OuterRadiusPercent = 0.1f;
    public const float InnerRadiusPercent = 0.04f;
    public static readonly Vector2 DefaultOffsetPercent = new Vector2(.15f, .8f);

    private Vector2 _circleCenter;
    private RectTransform _rootCanvasRect;
    private Camera _camera;

    private void Awake()
    {
        _rootCanvasRect = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();
        _camera = Camera.main;
    }

    public void ShowCircles(Vector2 initialScreenPosition)
    {
        Vector2 canvasSize = _rootCanvasRect.rect.size;
        _outerCircle.sizeDelta = new Vector2(canvasSize.x * OuterRadiusPercent * 2f,
                                             canvasSize.x * OuterRadiusPercent * 2f);
        _innerCircle.sizeDelta = new Vector2(canvasSize.x * InnerRadiusPercent * 2f,
                                             canvasSize.x * InnerRadiusPercent * 2f);

        Vector2 localPoint;
        bool isValid = !initialScreenPosition.Approximately(LocalGameplayInput.DefaultAimStartPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootCanvasRect, initialScreenPosition, _camera, out localPoint);
        if (!isValid)
        {
            _circleCenter = DefaultOffsetPercent * canvasSize.x;
        }
        else
        {
            _circleCenter = localPoint;//TODO: fix
        }
        Debug.Log(_circleCenter);

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
        Vector2 offset = -aimVector * maxRadius;
        _innerCircle.anchoredPosition = _circleCenter + offset;
    }

}
