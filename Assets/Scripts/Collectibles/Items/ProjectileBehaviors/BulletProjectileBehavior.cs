using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectileBehavior : UnityDriven, IProjectileBehavior
{
    private List<Character> _explodedCharacters;

    public event Action<ExplosionInfo> Exploded;

    BulletProjectileDefinition _definition;
    public BulletProjectileBehavior(BulletProjectileDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        _explodedCharacters = new List<Character>();
    }

    public void Launch(ProjectileLaunchContext context) 
    {
        context.ProjectileRigidbody.linearVelocity = Vector2.zero;
        context.ProjectileRigidbody.transform.position = context.AimOrigin + context.AimVector.normalized * Constants.ProjectileOffset;
        context.ProjectileRigidbody.AddForce(context.AimVector, ForceMode2D.Impulse);
    }

    public void OnContact(ProjectileContactContext context)
    {
        _explodedCharacters.Clear();
        var mask = LayerMaskHelper.GetCombinedLayerMask(Constants.GroundLayer, Constants.CharacterLayer);
        var explosionRadius = _definition.ExplosionDefinition.ExplosionRadius.CalculateValue();
        var explosionStrength = _definition.ExplosionDefinition.ExplosionForce.CalculateValue();
        var explosionDamage = _definition.ExplosionDefinition.Damage.CalculateValue();
        Collider2D[] hits = Physics2D.OverlapCircleAll(context.ContactPoint, explosionRadius, mask);
        DrawDebugCircle(context.ContactPoint, explosionRadius, 12, Color.green);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Character character))
            {
                var pushVector = ((Vector2)character.transform.position - context.ContactPoint) / explosionRadius;
                character.Damage(explosionDamage);
                character.Push(pushVector * explosionStrength);
                _explodedCharacters.Add(character);
            }
            else if (hit.TryGetComponent(out DestructibleTerrain destTerrain))
            {
                destTerrain.ApplyExplosion(context.ContactPoint, explosionRadius);
            }
        }
        var exp = context.ExplosionManager.GetExplosion();
        exp.Explode(context.ContactPoint);
        Exploded?.Invoke(new ExplosionInfo(_explodedCharacters, context.Projectile));
    }

    //TODO: delete
    private void DrawDebugCircle(Vector2 center, float radius, int segments, Color color)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angleStep);
            Vector3 newPoint = center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

            Debug.DrawLine(prevPoint, newPoint, color);
            prevPoint = newPoint;
        }
    }

}
