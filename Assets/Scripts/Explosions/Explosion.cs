using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SimpleAnimator))]
public class Explosion : MonoBehaviour
{
    [SerializeField] private ExplosionAnimatorDefinition _animatorDefinition;
    private SimpleAnimator _animator;

    public event Action<Explosion> ExplosionFinished;

    private void Awake()
    {
        _animator = GetComponent<SimpleAnimator>();
    }

    public void Explode(Vector2 position)
    {
        transform.position = position;
        _animator.PlayAnimation(_animatorDefinition.ExplosionAnimationDurationPerFrame);
        StartCoroutine(WaitForExplosionToFinishThenFireEventCoroutine());
    }

    private IEnumerator WaitForExplosionToFinishThenFireEventCoroutine()
    {
        yield return new WaitUntil(() => !_animator.IsPlaying);
        ExplosionFinished?.Invoke(this);
    }


}
