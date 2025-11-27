using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(OneShotAnimator))]
public class Explosion : MonoBehaviour
{
    [SerializeField] private ExplosionAnimatorDefinition _animatorDefinition;
    private ExplosionDefinition _explosionDefinition;
    private OneShotAnimator _animator;
    private List<Character> _explodedCharacters;

    public event Action<Explosion> ExplosionFinished;

    private const float DelayAfterExplosion = .3f;

    private readonly Collider2D[] _overlapCheckColliders = new Collider2D[Constants.OverlapHitColliderNumMax];

    public bool IsExploding { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<OneShotAnimator>();
        _explodedCharacters = new List<Character>();
    }

    public void Initialize(ExplosionDefinition explosionDefinition)
    {
        _animator.SetAnimation(explosionDefinition.Animation);
        _animator.SetSFX(explosionDefinition.SFX);
        _explosionDefinition = explosionDefinition;
    }

    public IEnumerable<Character> Explode(Vector2 contactPoint, int damage)
    {
        _explodedCharacters.Clear();
        transform.position = contactPoint;
        IsExploding = true;
        _animator.PlayAnimation(_animatorDefinition.ExplosionAnimationDurationPerFrame);

        var mask = LayerMaskHelper.GetCombinedLayerMask(Constants.GroundLayer, Constants.CharacterLayer);
        var explosionRadius = _explosionDefinition.Radius.CalculateValue();
        var explosionStrength = _explosionDefinition.Force.CalculateValue();
        var filter = new ContactFilter2D();
        filter.SetLayerMask(mask);
        int numHits = Physics2D.OverlapCircle(contactPoint, explosionRadius, filter, _overlapCheckColliders);
        for(int i = 0; i < numHits; i++)
        {
            var hit = _overlapCheckColliders[i];
            if (hit.TryGetComponent(out Character character))
            {
                var pushVector = ((Vector2)character.transform.position - contactPoint) / explosionRadius;
                character.Damage(damage);
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
        yield return new WaitUntil(() => !_animator.IsPlaying);
        yield return new WaitForSeconds(DelayAfterExplosion);
        IsExploding = false;
        ExplosionFinished?.Invoke(this);
    }


}
