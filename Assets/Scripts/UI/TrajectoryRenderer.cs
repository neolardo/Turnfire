using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryRenderer : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private RectTransform _innerCircle;
    [SerializeField] private RectTransform _outerCircle;
    [SerializeField] private int _resolution = 30;
    [SerializeField] private float _timeStep = 0.1f;
    private Transform _startTransform;
    private Vector2 _circleCenter;
    private float _trajectoryMultiplier, _gravity;
    private RectTransform _rootCanvasRect;


    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _rootCanvasRect = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();
        _gravity = Physics2D.gravity.y;
    }

    public void SetTrajectoryMultipler(float multiplier)
    {
        _trajectoryMultiplier = multiplier;
    }

    public void SetStartTransform(Transform startTransform)
    {
        _startTransform = startTransform;
    }

    public void ShowTrajectory(Vector2 initialPosition)
    {
        Vector2 canvasSize = _rootCanvasRect.rect.size;
        _outerCircle.sizeDelta = new Vector2(canvasSize.x * Constants.AimCircleOuterRadiusPercent*2, canvasSize.x * Constants.AimCircleOuterRadiusPercent*2);
        _innerCircle.sizeDelta = new Vector2(canvasSize.x * Constants.AimCircleInnerRadiusPercent*2, canvasSize.x * Constants.AimCircleInnerRadiusPercent*2);

        if (initialPosition.x < 0 && initialPosition.y < 0)
        {
            _circleCenter = new Vector2(Constants.AimCircleOffsetPercentX * canvasSize.x, Constants.AimCircleOffsetPercentY * canvasSize.x);
        }
        else
        {
            _circleCenter = initialPosition;
        }
        _innerCircle.position = _circleCenter;
        _outerCircle.position = _circleCenter;
        _innerCircle.gameObject.SetActive(true);
        _outerCircle.gameObject.SetActive(true);
    }

    public void HideTrajectory()
    {
        _innerCircle.gameObject.SetActive(false);
        _outerCircle.gameObject.SetActive(false);
        ClearTrajectory();
    }

    public void DrawTrajectory(Vector2 aimVector)
    {
        _lineRenderer.positionCount = _resolution;
        _innerCircle.position = _circleCenter - aimVector * (_outerCircle.sizeDelta.x/2 - _innerCircle.sizeDelta.x/2);
        aimVector *= _trajectoryMultiplier; 
        for (int i = 0; i < _resolution; i++)
        {
            float t = i * _timeStep;
            float x = _startTransform.position.x + aimVector.x * t;
            float y = _startTransform.position.y + aimVector.y * t + 0.5f * _gravity * t * t;

            _lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    private void ClearTrajectory()
    {
        _lineRenderer.positionCount = 0;
    }
}
