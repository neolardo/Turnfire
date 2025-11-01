using UnityEngine;

public class ItemPreviewRendererManager : MonoBehaviour
{
    [SerializeField] private ItemPreviewRendererSettingsDefinition _previewSettings;
    [SerializeField] private TrajectoryRenderer _trajectoryRenderer;
    public TrajectoryRenderer TrajectoryRenderer => _trajectoryRenderer;

    private ItemPreviewRendererType _currentRenderer;

    private void Awake()
    {
        _trajectoryRenderer.SetPreviewSettings(_previewSettings);
    }

    public void SelectRenderer(ItemPreviewRendererType renderer)
    {
        _currentRenderer = renderer;
        RefreshCurrentRenderer();
    }

    private void RefreshCurrentRenderer()
    {
        // deactivate all
        TrajectoryRenderer.gameObject.SetActive(false);

        // activate current
        switch (_currentRenderer)
        {
            case ItemPreviewRendererType.Trajectory:
                TrajectoryRenderer.gameObject.SetActive(true);
                break;
        }
    }


}
