using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OneShotAnimator))]
public class OfflineExplosion : MonoBehaviour, IExplosion
{
    [SerializeField] private ExplosionAnimatorDefinition _animatorDefinition;
    private ExplosionView _view;
    private ExplosionBehavior _behavior;

    public event Action<IExplosion> Exploded;

    public bool IsExploding => _behavior.IsExploding;

    public bool IsReady {get; private set;}

    private void Awake()
    {
        var animator = GetComponent<OneShotAnimator>();
        _view = new ExplosionView(animator, _animatorDefinition);
        IsReady = true;
    }

    public void Initialize(ExplosionDefinition explosionDefinition)
    {
        _view.Initialize(explosionDefinition);
        float frameDuration = _animatorDefinition.ExplosionAnimationDurationPerFrame;
        float explosionDuration = explosionDefinition.Animation.GetTotalDuration(frameDuration);
        _behavior = new ExplosionBehavior(explosionDefinition, explosionDuration, transform);
        _behavior.Exploded += OnExplosionFinished;
    }

    public IEnumerable<Character> Explode(Vector2 contactPoint, int damage, IDamageSourceDefinition damageSource)
    {
        _view.PlayExplosionAnimation();
        return _behavior.Explode(contactPoint, damage, damageSource);
    }

    private void OnExplosionFinished()
    {
        Exploded?.Invoke(this);
    }

}
