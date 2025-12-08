using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{

    [SerializeField] private CharacterHealthbarRenderer _healthbarRenderer;
    [SerializeField] private CharacterAnimator _animator;
    [SerializeField] private CharacterDefinition _characterDefinition;
    [SerializeField] private SectorHitbox _meleeHitbox;
    private List<Item> _items;
    private Item _selectedItem;
    private Rigidbody2D _rb;
    private float _jumpBoost;
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
        private set
        {
            if (_health != value)
            {
                _health = value;
                HealthChanged?.Invoke(NormalizedHealth, Health);
            }
        }
    }

    public CharacterDefinition CharacterDefinition => _characterDefinition;
    public CharacterArmorManager ArmorManager { get; private set; }
    public SectorHitbox MeleeHitbox => _meleeHitbox;
    public Team Team { get; private set; }
    public Collider2D Collider { get; private set; }
    public Transform ItemTransform => _animator.ItemTransform;
    public bool IsAlive => _health > 0;
    public bool IsMoving => _rb.linearVelocity.magnitude > Mathf.Epsilon;
    public bool IsUsingSelectedItem => _selectedItem == null ? false : _selectedItem.Behavior.IsInUse || _animator.IsPlayingNonIdleAnimation;
    public float NormalizedHealth => _health / (float)CharacterDefinition.MaxHealth;
    public Vector2 FeetPosition => (Vector2)transform.position + Vector2.down * Collider.bounds.extents.y;
    public Vector2 FeetOffset => Vector2.down * Collider.bounds.extents.y;

    public float JumpStrength => _jumpBoost + CharacterDefinition.JumpStrength;

    public event Action<float, int> HealthChanged;
    public event Action Jumped;
    public event Action Died;
    public event Action<Item> SelectedItemChanged;
    public event Action SelectedItemUsed;

    #region Initialization

    private void Awake()
    {
        Health = CharacterDefinition.MaxHealth;
        ArmorManager = new CharacterArmorManager();
        ArmorManager.ArmorEquipped += _animator.PlayEquipArmorAnimation;
        ArmorManager.ArmorUnequipped += _animator.PlayUnequipArmorAnimation;
        _items = new List<Item>();
        foreach (var itemDefinition in CharacterDefinition.InitialItems)
        {
            TryAddItem(new Item(itemDefinition, false));
        }
        _rb = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        _selectedItem = _items.FirstOrDefault();
        HealthChanged += _healthbarRenderer.SetCurrentHealth;
        _healthbarRenderer.Initilaize(_health);
    }

    public void SetTeam(Team team)
    {
        Team = team;
        _animator.SetTeamColor(Team.TeamColor);
    }

    #endregion

    #region Health

    public void Damage(int value)
    {
        if (ArmorManager.IsProtected)
        {
            var armor = ArmorManager.BlockAttack();
            _animator.PlayGuardAnimation(armor);
        }
        else
        {
            _animator.PlayHurtAnimation();
            Health = Mathf.Max(0, Health - value);
            if (!IsAlive)
            {
                Die();
            }
        }
    }

    public void Heal(int value)
    {
        Health = Mathf.Min(Health + value, CharacterDefinition.MaxHealth);
        _animator.PlayHealAnimation();
    }

    public void Kill()
    {
        Damage(Health);
    }

    private void Die()
    {
        _animator.PlayDeathAnimation();
        gameObject.layer = Constants.DeadCharacterLayer;
        Debug.Log(gameObject.name + " died.");
        Died?.Invoke();
    }


    #endregion

    #region Aim

    public void StartAiming()
    {
        _animator.StartAiming(_selectedItem);
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

    public void Push(Vector2 impulse)
    {
        _rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void AddJumpBoost(float jumpBoost)
    {
        _jumpBoost = jumpBoost;
        //TODO: recalculate jump graph
    }

    public void RemoveJumpBoost()
    {
        _jumpBoost = 0;
    }

    public void Jump(Vector2 aimDirection)
    { 
        var jumpForce = aimDirection * (CharacterDefinition.JumpStrength + _jumpBoost);
        _rb.AddForce(jumpForce, ForceMode2D.Impulse);
        _animator.OnJumpStarted(jumpForce);
        Jumped?.Invoke();
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

    #region Items

    public bool TryAddItem(Item item)
    {
        var existingItem = _items.FirstOrDefault(i => i.IsSameType(item));
        if (existingItem == null)
        {
            _items.Add(item);
            item.CollectibleDestroyed += OnItemDestroyed;
            if (_selectedItem == null && item.Definition.ItemType == ItemType.Weapon)
            {
                TrySelectItem(item);
            }
            return true;
        }
        else
        {
            return existingItem.TryMerge(item); 
        }
    }

    private void OnItemDestroyed(ICollectible collectible)
    {
        var item = _items.FirstOrDefault(i => i == collectible);
        if(item != null)
        {
            RemoveItem(item);   
        }
    }

    private void RemoveItem(Item item)
    {
        _items.Remove(item);
        if(_selectedItem == item)
        {
            TrySelectItem(_items.FirstOrDefault(i => i.Definition.ItemType == ItemType.Weapon));
        }
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _items;
    }

    #region Selected Item

    public void UseSelectedItem(ItemUsageContext context)
    {
        _animator.PlayItemActionAnimation(context.AimVector, _selectedItem);
        _selectedItem.Behavior.Use(context);
        SelectedItemUsed?.Invoke();
    }

    public void TrySelectItem(Item item)
    {
        if ((item == null) || (_items.Contains(item) && item != _selectedItem))
        {
            if (item != null && item.Definition.UseInstantlyWhenSelected)
            {
                var context = new ItemUsageContext(this);
                if (item.Behavior.CanUseItem(context))
                {
                    _selectedItem = item;
                    SelectedItemChanged?.Invoke(item);
                    UseSelectedItem(context);
                }
            }
            else
            {
                _selectedItem = item;
                SelectedItemChanged?.Invoke(item);
            }
        }
    }

    public Item GetSelectedItem()
    {
        return _selectedItem;
    } 

    #endregion

    #endregion

    #region Simulation Helpers

    public bool OverlapPoint(Vector2 point)
    {
        return Collider.bounds.Contains(point);
    }

    public Vector2 NormalAtPoint(Vector2 point)
    {
        float linearHalfLength = Collider.bounds.extents.y - Collider.bounds.extents.x;
        if (Mathf.Abs(point.y - transform.position.y) < linearHalfLength)
        {
            return new Vector2(point.x - transform.position.x, 0).normalized;
        }
        else
        {
            return (point - (Vector2)transform.position).normalized;
        }
    }

    #endregion

}
