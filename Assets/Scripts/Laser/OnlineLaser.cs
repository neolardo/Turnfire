using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class OnlineLaser : NetworkBehaviour, ILaser
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
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        IsReady = true;
    }

    public void Initialize(int maxBounceCount, float maxDistance)
    {
        if(!IsServer)
        {
            return;
        }     
        _physics.Initialize(maxBounceCount, maxDistance);
    }

    public void StartBeam(Vector2 aimOrigin, Vector2 aimVector, Character owner)
    {
        if(!IsServer)
        {
            return;
        }
        var points = _physics.CalculateLaserPath(aimOrigin, aimVector, owner);
        StartLaserClientRpc(points.ToArray());
    }

    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Server)]
    private void StartLaserClientRpc(Vector2[] points)
    {
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
