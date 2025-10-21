using UnityEngine;

[RequireComponent (typeof(OneShotAnimator))]
public class CharacterItemActionAnimator : MonoBehaviour
{
    private OneShotAnimator _animator;
    private void Awake()
    {
        _animator = GetComponent<OneShotAnimator>();
    }
    public void SetAnimation(AnimationDefinition animation)
    {
        _animator.SetAnimation(animation);
    }

    public void PlayItemActionAnimation(float frameDuration)
    {
        _animator.PlayAnimation(frameDuration);
    }

}
