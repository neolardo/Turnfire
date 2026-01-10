using System;
using System.Collections.Generic;
using UnityEngine;

public interface IExplosion
{
    bool IsExploding { get; }
    bool IsReady { get; }
    
    event Action<IExplosion> Exploded;
    void Initialize(ExplosionDefinition explosionDefinition);
    IEnumerable<Character> Explode(Vector2 contactPoint, int damage);
}
