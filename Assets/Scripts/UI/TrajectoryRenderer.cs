using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryRenderer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private RectTransform _innerCircle;
    [SerializeField] private RectTransform _outerCircle;
    [SerializeField] private int _maxSegments = 50;
    [SerializeField] private float _maxLength = 2.5f;
    [SerializeField] private float _stepTime = 0.05f;
    [SerializeField] private float _lineThickness = 2f;

    private ItemPreviewRendererSettingsDefinition _previewSettings;
    private LineRenderer _line;
    private Vector2 _circleCenter;
    private RectTransform _rootCanvasRect;
    private float _trajectoryMultiplier;
    private Transform _origin;
    private bool _useGravity = true;
    private Camera _camera;


    private void Awake()
    {
        _rootCanvasRect = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();
        _line = GetComponent<LineRenderer>();
        _line.widthMultiplier =  _lineThickness;
        _line.textureMode = LineTextureMode.Stretch;
        _line.useWorldSpace = true;
        _line.alignment = LineAlignment.TransformZ;
        _camera = Camera.main;
    }

    public void SetPreviewSettings(ItemPreviewRendererSettingsDefinition previewSettings)
    {
        _previewSettings = previewSettings;
    }

    private void DrawStraightTrajectory(Vector2 aimVector)
    {
        Vector2 start = _origin.position;
        Vector2 end = start + aimVector.normalized * _maxLength;

        _line.positionCount = 2;
        _line.SetPosition(0, SnapToGrid(start));
        _line.SetPosition(1, SnapToGrid(end));
    }

    private void DrawCurvedTrajectory(Vector2 aimVector)
    {
        Vector2 gravity = _useGravity ? Physics2D.gravity : Vector2.zero;

        Vector3[] points = new Vector3[_maxSegments];
        Vector2 startPos = _origin.position;
        Vector2 lastPos = startPos;
        points[0] = startPos;

        float elapsed = 0f;
        float totalDistance = 0f;
        int count = 1;

        while (count < _maxSegments && totalDistance < _maxLength)
        {
            elapsed += _stepTime;
            Vector2 newPos = startPos + aimVector * elapsed + 0.5f * gravity * (elapsed * elapsed);
            totalDistance += Vector2.Distance(lastPos, newPos);

            if (totalDistance > _maxLength)
            {
                break;
            }

            points[count] = SnapToGrid(newPos);
            lastPos = newPos;
            count++;
        }

        _line.positionCount = count;
        _line.SetPositions(points);
    }

    private Vector3 SnapToGrid(Vector2 pos)
    {
        float pixelSize = 1f / _uiDefinition.PixelsPerUnit;
        return new Vector3(
            Mathf.Round(pos.x / pixelSize) * pixelSize,
            Mathf.Round(pos.y / pixelSize) * pixelSize,
            0f
        );
    }

    public void ToggleGravity(bool state) => _useGravity = state;


    public void ShowTrajectory(Vector2 initialPosition)
    {
        ShowCircles(initialPosition);
        DrawTrajectory(Vector2.zero);
        _line.enabled = true;
    }

    private void ShowCircles(Vector2 initialScreenPosition)
    {
        Vector2 canvasSize = _rootCanvasRect.rect.size;
        _outerCircle.sizeDelta = new Vector2(canvasSize.x * _previewSettings.AimCircleOuterRadiusPercent * 2f,
                                             canvasSize.x * _previewSettings.AimCircleOuterRadiusPercent * 2f);
        _innerCircle.sizeDelta = new Vector2(canvasSize.x * _previewSettings.AimCircleInnerRadiusPercent * 2f,
                                             canvasSize.x * _previewSettings.AimCircleInnerRadiusPercent * 2f);

        Vector2 localPoint;
        bool isValid = !initialScreenPosition.Approximately(LocalGameplayInput.DefaultAimStartPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootCanvasRect, initialScreenPosition, _camera, out localPoint);
        if (!isValid)
        {
            _circleCenter = new Vector2(_previewSettings.DefaultAimCircleOffsetPercentX * canvasSize.x, _previewSettings.DefaultAimCircleOffsetPercentY * canvasSize.x);
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


    public void DrawTrajectory(Vector2 aimVector)
    {
        MoveInnerCircle(aimVector);
        aimVector *= _trajectoryMultiplier;

        if (_useGravity)
        {
            DrawCurvedTrajectory(aimVector);
        }
        else
        {
            DrawStraightTrajectory(aimVector);
        }
    }

    private void MoveInnerCircle(Vector2 aimVector)
    {
        float maxRadius = (_outerCircle.sizeDelta.x - _innerCircle.sizeDelta.x)* 0.5f;
        Vector2 offset = -aimVector * maxRadius;
        _innerCircle.anchoredPosition = _circleCenter + offset;
    }

    public void HideTrajectory()
    {
        _innerCircle.gameObject.SetActive(false);
        _outerCircle.gameObject.SetActive(false);
        _line.enabled = false;
    }

    public void SetOrigin(Transform origin)
    {
        _origin = origin;
    }

    public void SetTrajectoryMultipler(float multiplier)
    {
        _trajectoryMultiplier = multiplier;
    }
}
