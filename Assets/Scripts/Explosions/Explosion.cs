using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OneShotAnimator))]
public class Explosion : MonoBehaviour
{
    [SerializeField] private ExplosionAnimatorDefinition _animatorDefinition;
    private ExplosionDefinition _explosionDefinition;
    private OneShotAnimator _animator;
    private List<Character> _explodedCharacters;

    public event Action<Explosion> ExplosionFinished;

    public bool IsAnimationPlaying => _animator.IsPlaying;

    private void Awake()
    {
        _animator = GetComponent<OneShotAnimator>();
        _explodedCharacters = new List<Character>();
    }

    public void Initialize(ExplosionDefinition explosionDefinition)
    {
        _animator.SetAnimation(explosionDefinition.AnimationDefinition);
        _explosionDefinition = explosionDefinition;
    }

    public IEnumerable<Character> Explode(Vector2 contactPoint, int damage)
    {
        _explodedCharacters.Clear();
        transform.position = contactPoint;
        _animator.PlayAnimation(_animatorDefinition.ExplosionAnimationDurationPerFrame);

        var mask = LayerMaskHelper.GetCombinedLayerMask(Constants.GroundLayer, Constants.CharacterLayer);
        var explosionRadius = _explosionDefinition.Radius.CalculateValue();
        var explosionStrength = _explosionDefinition.Force.CalculateValue();
        Collider2D[] hits = Physics2D.OverlapCircleAll(contactPoint, explosionRadius, mask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Character character))
            {
                var pushVector = ((Vector2)character.transform.position - contactPoint) / explosionRadius;
                character.Damage(damage);
                character.Push(pushVector * explosionStrength);
                _explodedCharacters.Add(character);
            }
            else if (hit.TryGetComponent(out SimpleDestructibleTerrain simpleDest))
            {
                simpleDest.ApplyExplosion(contactPoint, explosionRadius);
            }
            else if (hit.TryGetComponent(out DestructibleTerrainRenderer terrain))
            {
                terrain.ApplyExplosion(contactPoint, explosionRadius);
            }
        }

        StartCoroutine(WaitForExplosionToFinishThenFireEventCoroutine());
        return _explodedCharacters;
    }

    private IEnumerator WaitForExplosionToFinishThenFireEventCoroutine()
    {
        yield return new WaitUntil(() => !_animator.IsPlaying);
        ExplosionFinished?.Invoke(this);
    }


}
