using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineLaserPool : OnlinePool<OnlineLaser>, IPool<ILaser>
{
    private void Start()
    {
        GameServices.Register(this);
    }
    protected override OnlineLaser CreateInstance()
    {
        var laser = base.CreateInstance();
        laser.BeamEnded += OnLaserBeamEnded;
        return laser;   
    }

    protected override void CreateInitialItems()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        var container = FindFirstObjectByType<LaserContainer>();
        _container = container.transform;
        base.CreateInitialItems();
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
        base.Release(laser as OnlineLaser);
    }

}
