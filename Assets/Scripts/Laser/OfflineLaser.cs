using System;
using System.Collections;
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
    public bool IsBeamAnimationInProgress => _view.IsAnimationInProgress;

    public event Action<ILaser> BeamEnded;

    private void Awake()
    {
        var renderer = FindFirstObjectByType<PixelLaserRenderer>();
        _view = new LaserView(renderer);
        _physics = new LaserPhysics();
        IsReady = true;
    }

    public void Initialize(int maxBounceCount, float maxDistance)
    {
        _physics.Initialize(maxBounceCount, maxDistance);
    }
    public void StartBeam(Vector2 aimOrigin, Vector2 aimVector, Character owner)
    {
        var points = _physics.CalculateLaserPath(aimOrigin, aimVector, owner);
        _view.StartLaser(points.ToArray());
        StartCoroutine(FireBeamFinishedWhenLaserAnimationEnded());
    }

    private IEnumerator FireBeamFinishedWhenLaserAnimationEnded()
    {
        yield return new WaitUntil(() => !_view.IsAnimationInProgress);
        BeamEnded?.Invoke(this);
    }
    public IEnumerable<Character> GetHitCharacters()
    {
        return _physics.GetHitCharacters();
    }
}
