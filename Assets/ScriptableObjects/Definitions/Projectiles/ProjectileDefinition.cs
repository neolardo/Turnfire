using UnityEngine;

public abstract class ProjectileDefinition : ScriptableObject
{
    public Sprite Sprite;
    public ExplosionDefinition ExplosionDefinition;
    public abstract IProjectileBehavior CreateProjectileBehavior();
}
