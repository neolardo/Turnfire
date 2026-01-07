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

    private CharacterModel _model;
    private CharacterView _view;
    private ICharacterMovementController _movementController;
    private ICharacterItemsController _itemsController;
    public CharacterArmorManager ArmorManager { get; private set; }
    public SectorHitbox MeleeHitbox => _meleeHitbox;
    public Collider2D Collider => _movementController.Collider;
    public Transform ItemTransform => _view.ItemTransform;
    public Team Team => _model.Team;
    public bool IsAlive => _model.IsAlive;
    public bool IsMoving => _movementController.IsMoving;
    public bool IsUsingSelectedItem => _itemsController.IsUsingSelectedItem || _view.IsPlayingNonIdleAnimation;
    public float NormalizedHealth => _model.NormalizedHealth;
    public Vector2 FeetPosition => _movementController.FeetPosition;
    public Vector2 FeetOffset => _movementController.FeetOffset;
    public float JumpStrength => _itemsController.JumpBoost + CharacterDefinition.JumpStrength;
    public bool EnumeratorCondition => IsAlive;
    public Item SelectedItem => _itemsController.SelectedItem;

    public event Action<float, int> HealthChanged;
    public event Action Jumped;
    public event Action Died;
    public event Action<Item> SelectedItemChanged;
    public event Action SelectedItemUsed;

    public void Initialize(Team team)
    {
        ArmorManager = new CharacterArmorManager();
        _animator.Initialize(_definition, team.TeamColor);
        _model = new CharacterModel( _definition, team, ArmorManager);
        _view = new CharacterView(_animator, _definition, _healthbarRenderer, ArmorManager);
        //TODO: create and initialize controllers
        _model.Healed += _view.OnHealed;
        _model.Hurt += _view.OnHealed;
        _model.Died += _view.OnDied;
        _model.Died += InvokeDied;
        _model.Blocked += _view.OnBlocked;
        _model.HealthChanged += InvokeHealthChanged;
        _movementController.Jumped += _view.OnJumpStarted;
        _movementController.Jumped += InvokeJumped;
        _itemsController.SelectedItemUsed += _view.OnItemUsed;
        _itemsController.SelectedItemUsed += InvokeSelectedItemUsed;
        _itemsController.SelectedItemChanged += InvokeSelectedItemChanged;
    }

    private void OnDestroy()
    {
        _model.Healed -= _view.OnHealed;
        _model.Hurt -= _view.OnHealed;
        _model.Died -= _view.OnDied;
        _model.Died -= InvokeDied;
        _model.Blocked -= _view.OnBlocked;
        _model.HealthChanged -= InvokeHealthChanged;
        _movementController.Jumped -= _view.OnJumpStarted;
        _movementController.Jumped -= InvokeJumped;
        _itemsController.SelectedItemUsed -= _view.OnItemUsed;
        _itemsController.SelectedItemUsed -= InvokeSelectedItemUsed;
        _itemsController.SelectedItemChanged -= InvokeSelectedItemChanged;
    }

    #region Health

    public void Damage(int value)
    {
        _model.Damage(value);
    }

    public void Heal(int value)
    {
        _model.Heal(value);
    }

    public void Kill()
    {
        _model.Kill();
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

    #region Movement

    public void Push(Vector2 impulse)
    {
        _movementController.Push(impulse);  
    }

    public void AddJumpBoost(float jumpBoost)
    {
        _itemsController.ApplyJumpBoost(jumpBoost);
    }

    public void RemoveJumpBoost()
    {
        _itemsController.RemoveJumpBoost();
    }

    public void Jump(Vector2 aimDirection)
    {
        var jumpForce = aimDirection * JumpStrength;
        _movementController.StartJump(jumpForce);
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

    public bool TryAddItem(Item item)
    {
        return _itemsController.TryAddItem(item);
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _itemsController.GetAllItems();
    }

    #region Selected Item

    public void UseSelectedItem(ItemUsageContext context)
    {
        _itemsController.UseSelectedItem(context);
    }

    public bool TrySelectItem(Item item)
    {
        return _itemsController.TrySelectItem(item);
        //TODO: use here?
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
    private void InvokeSelectedItemChanged(Item newItem)
    {
        SelectedItemChanged?.Invoke(newItem);
    }
    private void InvokeSelectedItemUsed(Item item, ItemUsageContext context)
    {
        SelectedItemUsed?.Invoke();
    }

    #endregion

    #region Simulation Helpers

    public bool OverlapPoint(Vector2 point)
    {
        return _movementController.OverlapPoint(point);
    }

    public Vector2 NormalAtPoint(Vector2 point)
    {
        return _movementController.NormalAtPoint(point);
    }

    #endregion

}
