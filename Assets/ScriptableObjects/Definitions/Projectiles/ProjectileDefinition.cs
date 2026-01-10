using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileDefinition : DatabaseItemScriptableObject
{
    public Sprite Sprite;
    public RangedStatInt Damage;
    public ExplosionDefinition ExplosionDefinition;
    public SFXDefiniton ContactSFX;
    public float ColliderRadius = .1f;
    public abstract IProjectileBehavior CreateProjectileBehavior();
    public virtual IEnumerable<RangedStat> GetRangedStats()
    {
        return new[] { Damage };
    }
}
