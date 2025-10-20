using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private CharacterAnimatorDefinition _animatorDefinition;
    [SerializeField] private CharacterBodyAnimator _bodyAnimator;
    [SerializeField] private CharacterItemActionAnimator _itemActionAnimator;
    [SerializeField] private CharacterItemRenderer _itemRenderer;

    public Transform ItemTransform => _itemRenderer.transform;

    public void SetTeamColor(Color color)
    {
        _bodyAnimator.SetTeamColor(color);
    }

    public void StartAiming(Item selectedItem, Vector2 aimDirection)
    {
        _itemRenderer.ChangeItem(selectedItem);
        _itemRenderer.ShowItem();
        _bodyAnimator.ChangeAimFrame(aimDirection);
        _bodyAnimator.PlayItemAimStartSFX();
    }

    public void CancelAiming()
    {
        _itemRenderer.HideItem();
        _bodyAnimator.PlayItemAimCancelSFX();
        PlayIdleAnimation();
    }

    public void ChangeAim(Vector2 aimVector)
    {
        _itemRenderer.ChangeAim(aimVector);
        _bodyAnimator.ChangeAimFrame(aimVector);
    }

    public void PlayIdleAnimation()
    {
        _bodyAnimator.PlayIdleAnimation();
    }

    public void PlayItemActionAnimation(Item item)
    {
        _itemRenderer.UseItemThenHide();
        if(item.Definition.ItemActionAnimationDefinition != null)
        {
            _itemActionAnimator.SetAnimation(item.Definition.ItemActionAnimationDefinition);
            _itemActionAnimator.PlayItemActionAnimation(_animatorDefinition.ItemActionAnimationFrameDuration);
        }
        _bodyAnimator.PlayItemActionAnimation();
    }

    public void PlayDeathAnimation()
    {
        _bodyAnimator.PlayDeathAnimation();
    }

    public void PlayHurtAnimation()
    {
        _bodyAnimator.PlayHurtAnimation();
    }

    public void PlayPrepareToJumpAnimation()
    {
        _bodyAnimator.PlayPrepareToJumpAnimation();
    }

    public void PlayCancelJumpAnimation()
    {
        _bodyAnimator.PlayCancelJumpAnimation();
    }

    public void ChangeJumpAim(Vector2 aimDirection)
    {
        _bodyAnimator.ChangeJumpAim(aimDirection);
    }

}
