using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileDefinition : ScriptableObject
{
    public Sprite Sprite;
    public ExplosionDefinition ExplosionDefinition;
    public abstract IProjectileBehavior CreateProjectileBehavior();
    public abstract IEnumerable<RangedStat> GetRangedStats();
}
