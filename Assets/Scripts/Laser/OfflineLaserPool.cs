using System.Collections.Generic;
using UnityEngine;

public class OfflineLaserPool : OfflinePool<OfflineLaser>, IPool<ILaser>
{
    private void Start()
    {
        GameServices.Register(this);
    }
    protected override OfflineLaser CreateInstance()
    {
        var laser = base.CreateInstance();
        laser.BeamEnded += OnLaserBeamEnded;
        return laser;
    }

    ILaser IPool<ILaser>.Get()
    {
        return Get();
    }

    IEnumerable<ILaser> IPool<ILaser>.GetMultiple(int count)
    {
        return GetMultiple(count);
    }

    ILaser IPool<ILaser>.GetAndPlace(Vector2 position)
    {
        return GetAndPlace(position);
    }

    private void OnLaserBeamEnded(ILaser laser)
    {
        Release(laser);
    }

    public void Release(ILaser laser)
    {
        base.Release(laser as OfflineLaser);
    }
}
