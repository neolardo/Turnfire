using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OfflineLaser : MonoBehaviour, ILaser
{
    private LaserPhysics _physics;
    private LaserView _view;

    public bool IsReady { get; set; }
    public Transform LaserHead => _view.LaserHead;
    public bool IsFirstRayRendered => _view.IsFirstRayRendered;
    public bool IsAnimationInProgress => _view.IsAnimationInProgress;

    private void Awake()
    {
        var renderer = FindFirstObjectByType<PixelLaserRenderer>();
        _view = new LaserView(renderer);
        _physics = new LaserPhysics();
        IsReady = true;
    }

    public void Initialize(int maxBounceCount, float maxDistance)
    {
        _physics.Inirialize(maxBounceCount, maxDistance);
    }
    public void StartLaser(Character owner, Vector2 aimOrigin, Vector2 aimVector)
    {
        var points = _physics.CalculateLaserPath(owner, aimOrigin, aimVector);
        _view.StartLaser(points.ToArray());
    }
    public IEnumerable<Character> GetHitCharacters()
    {
        return _physics.GetHitCharacters();
    }
}
