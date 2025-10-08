using System;
using UnityEngine;

public interface IProjectileBehavior
{
    public event Action<ExplosionInfo> Exploded;
    public void Launch(ProjectileLaunchContext context);
    public void OnContact(ProjectileContactContext context);

}
