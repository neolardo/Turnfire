using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileDefinition : ScriptableObject
{
    public Sprite Sprite;
    public RangedStatInt Damage;
    public ExplosionDefinition ExplosionDefinition;
    public abstract IProjectileBehavior CreateProjectileBehavior();
    public virtual IEnumerable<RangedStat> GetRangedStats()
    {
        return new[] { Damage };
    }
}
