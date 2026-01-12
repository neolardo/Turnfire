using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour, IConditionalEnumerable
{
    [SerializeField] private CharacterHealthbarRenderer _healthbarRenderer;
    [SerializeField] private CharacterAnimator _animator;
    [SerializeField] private CharacterDefinition _definition;
    [SerializeField] private SectorHitbox _meleeHitbox;

    private ICharacterState _state;
    private ICharacterPhysics _physics;
    private CharacterView _view;
    private CharacterLogic _logic;
    public SectorHitbox MeleeHitbox => _meleeHitbox;
    public Collider2D Collider => _physics.Collider;
    public Transform ItemTransform => _view.ItemTransform;
    public Team Team => _state.Team;
    public int Health => _state.Health;
    public bool IsAlive => _state.IsAlive;
    public bool IsMoving => _physics.IsMoving;
    public bool IsUsingSelectedItem => _state.IsUsingSelectedItem;
    public float NormalizedHealth => _state.NormalizedHealth;
    public Vector2 FeetPosition => _physics.FeetPosition;
    public Vector2 FeetOffset => _physics.FeetOffset;
    public float JumpStrength => _state.JumpStrength;
    public bool EnumeratorCondition => IsAlive;
    public ItemInstance SelectedItem => _state.SelectedItem;

    public event Action<float, int> HealthChanged;
    public event Action Jumped;
    public event Action Died;
    public event Action<ItemInstance> SelectedItemChanged;
    public event Action SelectedItemUsed;
    public event Action<ArmorDefinition> BlockedWithArmor;

    public void Initialize(Team team, ICharacterState state, ICharacterPhysics physics)
    {
        _physics = physics;
        _state = state;
        _state.Initialize(this, _definition, team);
        _view = new CharacterView(_animator, _definition, _healthbarRenderer, team);
        _logic = new CharacterLogic(this, _state, _definition);
        SubscribeToStateChangedEvents();
        _logic.SelectInitialItem();
    }

    private void OnDestroy()
    {
        UnsubscribeFromStateChangedEvents();
    }

    private void SubscribeToStateChangedEvents()
    {
        _state.HealthChanged += InvokeHealthChanged;
        _state.HealthChanged += _view.OnHealthChanged;
        _state.Healed += _view.OnHealed;
        _state.Died += _view.OnDied;
        _state.Died += InvokeDied;
        _state.Hurt += _view.OnHurt;
        _state.Blocked += _view.OnBlocked;
        _state.Blocked += InvokeBlockedWithArmor;

        _state.Jumped += _physics.Jump;
        _state.Jumped += _view.OnJumpStarted;
        _state.Jumped += InvokeJumped;
        _state.Pushed += _physics.Push;

        _state.ItemUsed += _view.OnItemUsed;
        _state.ItemUsed += InvokeSelectedItemUsed;
        _state.ItemSelected += InvokeSelectedItemChanged;

        _state.ArmorEquipped += _view.OnArmorEquipped;
        _state.ArmorUnequipped += _view.OnArmorUnequipped;
    }   

    private void UnsubscribeFromStateChangedEvents()
    {
        if (_state == null)
        {
            return;
        }
        _state.HealthChanged -= InvokeHealthChanged;
        _state.HealthChanged -= _view.OnHealthChanged;
        _state.Healed -= _view.OnHealed;
        _state.Died -= _view.OnDied;
        _state.Died -= InvokeDied;
        _state.Hurt -= _view.OnHurt;
        _state.Blocked -= _view.OnBlocked;
        _state.Blocked -= InvokeBlockedWithArmor;

        _state.Jumped -= _physics.Jump;
        _state.Jumped -= _view.OnJumpStarted;
        _state.Jumped -= InvokeJumped;
        _state.Pushed -= _physics.Push;

        _state.ItemUsed -= _view.OnItemUsed;
        _state.ItemUsed -= InvokeSelectedItemUsed;
        _state.ItemSelected -= InvokeSelectedItemChanged;

        _state.ArmorEquipped -= _view.OnArmorEquipped;
        _state.ArmorUnequipped -= _view.OnArmorUnequipped;
    }

    #region Health

    public void TakeDamage(IDamageSourceDefinition damageSource, int damageValue)
    {
        _state.RequestTakeDamage(damageSource, damageValue);
    }

    public void Heal(int value)
    {
        _state.RequestHeal(value);
    }

    public void Kill()
    {
        _state.RequestKill();
    }

    #endregion

    #region Aim

    public void StartAiming()
    {
        _view.StartAiming(SelectedItem);
    }

    public void ChangeAim(Vector2 aimVector)
    {
        _view.ChangeAim(aimVector);
    }

    public void CancelAiming()
    {
        _view.CancelAiming();
    }

    #endregion

    #region Armor

    public bool TryEquipArmor(ArmorDefinition definition, ArmorBehavior behavior)
    {
        return _state.TryEquipArmor(definition, behavior);
    }

    public bool CanEquipArmor(ArmorDefinition definition)
    {
        return _state.CanEquipArmor(definition);
    }

    #endregion

    #region Movement

    public void Push(Vector2 impulse)
    {
        _logic.Push(impulse);  
    }

    public void Jump(Vector2 aimDirection)
    {
        _logic.Jump(aimDirection);
    }

    public void ApplyJumpBoost(float jumpBoost)
    {
        _state.RequestApplyJumpBoost(jumpBoost);
    }

    public void RemoveJumpBoost()
    {
        _state.RequestRemoveJumpBoost();
    }

    public void PrepareToJump()
    {
        _view.PrepareToJump();
    }

    public void ChangeJumpAim(Vector2 aimDirection)
    {
        _view.ChangeJumpAim(aimDirection);
    }

    public void CancelJump()
    {
        _view.CancelJump();
    }


    #endregion

    #region Items

    public bool TryAddItem(ItemInstance item)
    {
        return _logic.TryAddItem(item);
    }

    public IEnumerable<ItemInstance> GetAllItems()
    {
        return _state.GetAllItems();
    }

    #region Selected Item

    public void UseSelectedItem(ItemUsageContext context)
    {
        _logic.UseSelectedItem(context);
    }

    public bool TrySelectItem(ItemInstance item)
    {
        return _logic.TrySelectItem(item);
    }

    #endregion

    #endregion

    #region Invoke Events

    private void InvokeJumped(Vector2 jumpVector)
    {
        Jumped?.Invoke();
    }
    private void InvokeDied()
    {
        Died?.Invoke();
    }
    private void InvokeHealthChanged(float normalizedValue, int value)
    {
        HealthChanged?.Invoke(normalizedValue, value);
    }
    private void InvokeSelectedItemChanged(ItemInstance newItem)
    {
        SelectedItemChanged?.Invoke(newItem);
    }
    private void InvokeSelectedItemUsed(ItemInstance item, ItemUsageContext context)
    {
        SelectedItemUsed?.Invoke();
    }
    private void InvokeBlockedWithArmor(ArmorDefinition armor)
    {
        BlockedWithArmor?.Invoke(armor);
    }

    #endregion

    #region Simulation Helpers

    public bool OverlapPoint(Vector2 point)
    {
        return _physics.OverlapPoint(point);
    }

    public Vector2 NormalAtPoint(Vector2 point)
    {
        return _physics.NormalAtPoint(point);
    }

    #endregion

}
