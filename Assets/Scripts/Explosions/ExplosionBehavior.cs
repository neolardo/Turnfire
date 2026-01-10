using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : UnityDriven
{
    private ExplosionDefinition _explosionDefinition;
    private List<Character> _explodedCharacters;
    private float _explosionDuration;
    private readonly Collider2D[] _overlapCheckColliders = new Collider2D[Constants.OverlapHitColliderNumMax];
    private Transform _explosionTransform;
    private const float DelayAfterExplosion = .3f;
    public bool IsExploding { get; private set; }

    public event Action Exploded;


    public ExplosionBehavior(ExplosionDefinition explosionDefinition, float explosionDuration, Transform explosionTransform) :base(CoroutineRunner.Instance) 
    {
        _explodedCharacters = new List<Character>();
        _explosionDefinition = explosionDefinition;
        _explosionTransform = explosionTransform;
    }

    public IEnumerable<Character> Explode(Vector2 contactPoint, int damage,IDamageSourceDefinition damageSource)
    {
        IsExploding = true;
        _explosionTransform.position = contactPoint;
        _explodedCharacters.Clear();
        var mask = LayerMaskHelper.GetCombinedLayerMask(Constants.GroundLayer, Constants.CharacterLayer);
        var explosionRadius = _explosionDefinition.Radius.CalculateValue();
        var explosionStrength = _explosionDefinition.Force.CalculateValue();
        var filter = new ContactFilter2D();
        filter.SetLayerMask(mask);
        int numHits = Physics2D.OverlapCircle(contactPoint, explosionRadius, filter, _overlapCheckColliders);
        for (int i = 0; i < numHits; i++)
        {
            var hit = _overlapCheckColliders[i];
            if (hit.TryGetComponent(out Character character))
            {
                var pushVector = ((Vector2)character.transform.position - contactPoint) / explosionRadius;
                character.TakeDamage(damageSource, damage);
                character.Push(pushVector * explosionStrength);
                _explodedCharacters.Add(character);
            }
            else if (hit.TryGetComponent(out DestructibleTerrainReference terrainReference))
            {
                terrainReference.ApplyExplosion(contactPoint, explosionRadius);
            }
        }

        StartCoroutine(WaitForExplosionToFinishThenFireEventCoroutine());
        return _explodedCharacters;
    }

    private IEnumerator WaitForExplosionToFinishThenFireEventCoroutine()
    {
        yield return new WaitForSeconds(_explosionDuration + DelayAfterExplosion);
        IsExploding = false;
        Exploded?.Invoke();
    }
}
