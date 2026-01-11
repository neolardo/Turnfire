
using UnityEngine;

public class LaserView
{
    private PixelLaserRenderer _renderer;
    public Transform LaserHead => _renderer.LaserHead;
    public bool IsFirstRayRendered => _renderer.IsFirstRayRendered;
    public bool IsAnimationInProgress => _renderer.IsAnimationInProgress;
    public LaserView(PixelLaserRenderer renderer)
    {
        _renderer = renderer;
    }

    public void StartLaser(Vector2[] laserPoints)
    {
        _renderer.StartLaser(laserPoints);
    }
}
