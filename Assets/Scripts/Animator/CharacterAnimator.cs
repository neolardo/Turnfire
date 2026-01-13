using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private CharacterAnimatorDefinition _animatorDefinition;
    [SerializeField] private CharacterBodyAnimator _bodyAnimator;
    [SerializeField] private CharacterItemActionAnimator _itemActionAnimator;
    [SerializeField] private CharacterItemRenderer _itemRenderer;

    public Transform ItemTransform => _itemRenderer.transform;

    private bool _isCurrentWeaponRanged;
    public bool IsPlayingNonIdleAnimation => _bodyAnimator.IsPlayingNonIdleAnimation;

    private const float JumpCancelThreshold = 1.0f;

    public void Initialize(CharacterDefinition definition, Color teamColor)
    {
        _bodyAnimator.Initialize(definition, teamColor);
    }

    public void StartAiming(ItemInstance weapon)
    {
        _isCurrentWeaponRanged = (weapon.Definition as WeaponDefinition).IsRanged;
        _itemRenderer.ChangeItem(weapon);
        _itemRenderer.ShowItem();
        var dir = _bodyAnimator.IsFacingLeft ? Vector2.left : Vector2.right;
        _itemRenderer.ChangeAim(dir);
        _bodyAnimator.PlayAimAnimation(dir, _isCurrentWeaponRanged);
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
        _bodyAnimator.PlayAimAnimation(aimVector, _isCurrentWeaponRanged);
    }

    public void PlayIdleAnimation()
    {
        _bodyAnimator.PlayIdleAnimation();
    }

    public void PlayItemActionAnimation(Vector2 aimVector, ItemInstance item)
    {
        if(item.Definition.HideItemDuringUsage)
        {
            _itemRenderer.HideItem();
        }
        else
        {
            if(item.Definition.ItemType == ItemType.Weapon && !(item.Definition as WeaponDefinition).IsRanged)
            {
                _itemRenderer.StartMoveAlongAnimationThenHide(aimVector);
            }
            else
            {
                _itemRenderer.HideItemAfterDelay(item.Definition.ItemUsagePostDelay);
            }
        }
        if (item.Definition.ItemActionAnimation != null)
        {
            _itemActionAnimator.SetAnimation(item.Definition.ItemActionAnimation);
            _itemActionAnimator.PlayItemActionAnimation(_animatorDefinition.ItemActionAnimationFrameDuration);
        }
        if (item.Definition.ItemActionSFX != null)
        {
            AudioManager.Instance.PlaySFXAt(item.Definition.ItemActionSFX, transform.position);
        }
        _bodyAnimator.PlayItemActionAnimation(aimVector, item);
    }

    public void PlayDeathAnimation()
    {
        _bodyAnimator.PlayDeathAnimation();
    }

    public void PlayEquipArmorAnimation(ArmorDefinition armor)
    {
        _bodyAnimator.PlayEquipArmorAnimation(armor);
        AudioManager.Instance.PlaySFXAt(armor.EquipSFX, transform);
    }

    public void PlayUnequipArmorAnimation(ArmorDefinition armor)
    {
        _bodyAnimator.PlayUnequipArmorAnimation(armor);
        AudioManager.Instance.PlaySFXAt(armor.UnequipSFX, transform);
    }

    public void PlayGuardAnimation(ArmorDefinition armor)
    {
        _itemRenderer.HideItem();
        _bodyAnimator.PlayGuardAnimation(armor);
    }

    public void PlayHurtAnimation(IDamageSourceDefinition damageSource)
    {
        _itemRenderer.HideItem();
        _bodyAnimator.PlayHurtAnimation();
        if (damageSource.HitSFX != null)
        {
            AudioManager.Instance.PlaySFXAt(damageSource.HitSFX, transform);
        }
    }
    public void PlayHealAnimation()
    {
        _bodyAnimator.PlayHealAnimation();
    }

    public void OnJumpStarted(Vector2 jumpVector)
    {
        if (jumpVector.y <= JumpCancelThreshold)
        {
            _bodyAnimator.PlayCancelJumpAnimation();
        }
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

    public void OnIsGroundedChanged(bool isGrounded)
    {
        _bodyAnimator.OnIsGroundedChanged(isGrounded);
    }

}
