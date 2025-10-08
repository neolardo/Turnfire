using UnityEngine;

public class ItemPreviewRendererManager : MonoBehaviour
{
    [SerializeField] private TrajectoryRenderer _trajectoryRenderer;
    public TrajectoryRenderer TrajectoryRenderer => _trajectoryRenderer;

    private ItemPreviewRendererType _currentRenderer;

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
