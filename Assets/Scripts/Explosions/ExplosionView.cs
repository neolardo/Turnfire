public class ExplosionView 
{
    private ExplosionAnimatorDefinition _animatorDefinition;
    private OneShotAnimator _animator;

    public ExplosionView( OneShotAnimator animator, ExplosionAnimatorDefinition animatorDefinition)
    {
        _animatorDefinition = animatorDefinition;
        _animator = animator;
    }

    public void Initialize(ExplosionDefinition explosionDefinition)
    {
        _animator.SetAnimation(explosionDefinition.Animation);
        _animator.SetSFX(explosionDefinition.SFX);
    }
    public void PlayExplosionAnimation()
    {
        _animator.PlayAnimation(_animatorDefinition.ExplosionAnimationDurationPerFrame);
    }

    public void Hide()
    {
        _animator.Hide();
    }

}