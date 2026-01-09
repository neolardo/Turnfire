using UnityEngine;

public class CharacterView : MonoBehaviour
{
    private CharacterHealthbarRenderer _healthbarRenderer;
    private CharacterAnimator _animator;
    private CharacterDefinition _definition;
    private CharacterArmorManager _armorManager;
    public Transform ItemTransform => _animator.ItemTransform;
    public bool IsPlayingNonIdleAnimation => _animator.IsPlayingNonIdleAnimation;
    public CharacterView(CharacterAnimator animator, CharacterDefinition definition, CharacterHealthbarRenderer healthbarRenderer, CharacterArmorManager armorManager, Team team)
    {
        _definition = definition;
        _armorManager = armorManager;
        _armorManager.ArmorEquipped += _animator.PlayEquipArmorAnimation;
        _armorManager.ArmorUnequipped += _animator.PlayUnequipArmorAnimation;
        _animator = animator;
        _animator.Initialize(_definition, team.TeamColor);
        _healthbarRenderer = healthbarRenderer;
        _healthbarRenderer.Initilaize(_definition.MaxHealth);
    }

    private void OnDestroy()
    {
        _armorManager.ArmorEquipped -= _animator.PlayEquipArmorAnimation;
        _armorManager.ArmorUnequipped -= _animator.PlayUnequipArmorAnimation;
    }

    #region Health

    public void OnHurt()
    {
        _animator.PlayHurtAnimation();
    }

    public void OnHealed()
    {
        _animator.PlayHealAnimation();
    }

    public void OnDied()
    {
        _animator.PlayDeathAnimation();
    }

    public void OnBlocked(ArmorDefinition armor)
    {
        _animator.PlayGuardAnimation(armor);
    }
   
    #endregion

    #region Aim

    public void StartAiming(Item item)
    {
        _animator.StartAiming(item);
    }

    public void ChangeAim(Vector2 aimVector)
    {
        _animator.ChangeAim(aimVector);
    }

    public void CancelAiming()
    {
        _animator.CancelAiming();
    }

    #endregion

    #region Movement

    public void OnJumpStarted(Vector2 jumpVector)
    {
        _animator.OnJumpStarted(jumpVector);
    }

    public void PrepareToJump()
    {
        _animator.PlayPrepareToJumpAnimation();
    }

    public void ChangeJumpAim(Vector2 aimDirection)
    {
        _animator.ChangeJumpAim(aimDirection);
    }

    public void CancelJump()
    {
        _animator.PlayCancelJumpAnimation();
    }


    #endregion

    #region Item

    public void OnItemUsed(Item item, ItemUsageContext context)
    {
        _animator.PlayItemActionAnimation(context.AimVector, item);
    }

    #endregion

}
