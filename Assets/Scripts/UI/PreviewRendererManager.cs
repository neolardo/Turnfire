using System.Collections.Generic;
using UnityEngine;

public class PreviewRendererManager : MonoBehaviour
{
    [SerializeField] private PixelTrajectoryRenderer _trajectoryRenderer;
    public PixelTrajectoryRenderer TrajectoryRenderer => _trajectoryRenderer;

    private Dictionary<PreviewRendererType, IPreviewRenderer> _renderersDict;
    private PreviewRendererType _currentRendererType;
    private IPreviewRenderer CurrentRenderer => _currentRendererType == PreviewRendererType.None ? null : _renderersDict[_currentRendererType];
    private LocalInputHandler _inputHandler;

    private void Awake()
    {
        _renderersDict = new Dictionary<PreviewRendererType, IPreviewRenderer>();
        _renderersDict[PreviewRendererType.Trajectory] = _trajectoryRenderer;

        _inputHandler = FindFirstObjectByType<LocalInputHandler>();
        _inputHandler.AimStarted += OnAimStarted;
        _inputHandler.AimChanged += OnAimChanged;
        _inputHandler.AimCancelled += OnAimCancelled;
        _inputHandler.ImpulseReleased += OnImpulseReleased;
    }

    private void OnDestroy()
    {
        _inputHandler.AimStarted -= OnAimStarted;
        _inputHandler.AimChanged -= OnAimChanged;
        _inputHandler.AimCancelled -= OnAimCancelled;
        _inputHandler.ImpulseReleased -= OnImpulseReleased;
    }

    public void SelectRenderer(PreviewRendererType renderer)
    {
        if (CurrentRenderer != null)
        {
            (CurrentRenderer as MonoBehaviour).gameObject.SetActive(false);
        }
        _currentRendererType = renderer;
        if (CurrentRenderer != null)
        {
            (CurrentRenderer as MonoBehaviour).gameObject.SetActive(true);
        }
    }

    private void OnAimStarted(Vector2 aimStartPosition)
    {
        if (CurrentRenderer == null)
        {
            return;
        }
        CurrentRenderer.OnAimStarted(aimStartPosition);
    }

    private void OnAimChanged(Vector2 aimVector)
    {
        if (CurrentRenderer == null)
        {
            return;
        }
        CurrentRenderer.OnAimChanged(aimVector);
    }
    private void OnImpulseReleased(Vector2 impulse)
    {
        if (CurrentRenderer == null)
        {
            return;
        }
        CurrentRenderer.OnImpulseReleased(impulse);
    }

    private void OnAimCancelled()
    {
        if(CurrentRenderer == null)
        {
            return;
        }
        CurrentRenderer.OnAimCancelled();
    }


}
