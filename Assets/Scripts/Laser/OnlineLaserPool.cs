using System.Collections.Generic;

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

    ILaser IPool<ILaser>.Get()
    {
        return Get();
    }

    IEnumerable<ILaser> IPool<ILaser>.GetMultiple(int count)
    {
        return GetMultiple(count);
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
