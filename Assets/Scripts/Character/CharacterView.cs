using UnityEngine;

public class CharacterView
{
    private CharacterHealthbarRenderer _healthbarRenderer;
    private CharacterAnimator _animator;
    private CharacterDefinition _definition;
    public Transform ItemTransform => _animator.ItemTransform;
    public bool IsPlayingNonIdleAnimation => _animator.IsPlayingNonIdleAnimation;
    public CharacterView(CharacterAnimator animator, CharacterDefinition definition, CharacterHealthbarRenderer healthbarRenderer, Team team)
    {
        _definition = definition;
        _animator = animator;
        _animator.Initialize(_definition, team.TeamColor);
        _healthbarRenderer = healthbarRenderer;
        _healthbarRenderer.Initilaize(_definition.MaxHealth);
    }

    #region Health

    public void OnHealthChanged(float normalizedHealth, int health)
    {
        _healthbarRenderer.SetCurrentHealth(normalizedHealth, health);
    }

    public void OnHurt(IDamageSourceDefinition damageSource)
    {
        _animator.PlayHurtAnimation(damageSource);
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

    #region Armor

    public void OnArmorEquipped(ArmorDefinition armor)
    {
        _animator.PlayEquipArmorAnimation(armor);
    }

    public void OnArmorUnequipped(ArmorDefinition armor)
    {
        _animator.PlayUnequipArmorAnimation(armor);
    }

    #endregion

    #region Aim

    public void OnAimStarted(ItemInstance item)
    {
        _animator.StartAiming(item);
    }

    public void OnAimChanged(Vector2 aimVector)
    {
        _animator.ChangeAim(aimVector);
    }

    public void OnAimCancelled()
    {
        _animator.CancelAiming();
    }

    #endregion

    #region Movement

    public void OnJumpStarted(Vector2 jumpVector)
    {
        _animator.OnJumpStarted(jumpVector);
    }

    public void OnPreparedToJump()
    {
        _animator.PlayPrepareToJumpAnimation();
    }

    public void OnJumpAimChanged(Vector2 aimDirection)
    {
        _animator.ChangeJumpAim(aimDirection);
    }

    public void OnJumpCancelled()
    {
        _animator.PlayCancelJumpAnimation();
    }

    public void OnIsGroundedChanged(bool isGrounded)
    {
        _animator.OnIsGroundedChanged(isGrounded);
    }


    #endregion

    #region Item

    public void OnItemUsed(ItemInstance item, ItemUsageContext context)
    {
        _animator.PlayItemActionAnimation(context.AimVector, item);
    }

    #endregion

}
