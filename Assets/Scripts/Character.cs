using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    [HideInInspector] public bool IsAlive => _health > 0;
    [HideInInspector] public bool IsMoving => _rb.linearVelocity.magnitude > Mathf.Epsilon;
    [HideInInspector] public bool IsUsingSelectedItem => _selectedItem.Behavior.IsInUse;
    public Transform ItemTransform => _itemRenderer.transform;

    [SerializeField] private CharacterHealthbarRenderer _healthbarRenderer;
    [SerializeField] private CharacterItemRenderer _itemRenderer;
    [SerializeField] private CharacterAnimator _animator;

    public CharacterDefinition CharacterDefinition;

    private Rigidbody2D _rb;
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
                HealthChanged?.Invoke(NormalizedHealth);
            }
        }
    }

    public float NormalizedHealth => _health / CharacterDefinition.MaxHealth;

    private List<Item> _items;
    private Item _selectedItem;

    /// <summary>
    /// Fires an event on every health change containing the normalized health ratio of this character.
    /// </summary>
    public event Action<float> HealthChanged;
    public event Action Died;
    public event Action<Item> SelectedItemChanged;

    private void Awake()
    {
        Health = CharacterDefinition.MaxHealth;
        _items = new List<Item>();
        foreach(var itemDefinition in CharacterDefinition.InitialItems)
        {
            _items.Add(CollectibleFactory.CreateCollectible(itemDefinition));
        }
        if(_items.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no initial items.");
        }
        _selectedItem = _items.FirstOrDefault();
        SelectedItemChanged += _itemRenderer.ChangeItem;
        _rb = GetComponent<Rigidbody2D>();
        HealthChanged += _healthbarRenderer.SetCurrentHealth;
    }

    private void Start() // after child awake run
    {
        _itemRenderer.ChangeItem(_selectedItem);
        _healthbarRenderer.SetCurrentHealth(1);
    }

    #region Health

    public void Damage(int value)
    {
        _animator.PlayHurtAnimation();
        Health = Mathf.Max(0, Health - value);
        if (!IsAlive)
        {
            Die();
        }
    }

    public void Kill()
    {
        Damage(Health);
    }

    private void Die()
    {
        //TODO
        _animator.PlayDeathAnimation();
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.grey;
        Debug.Log(gameObject.name + " died.");
        Died?.Invoke();
    }


    #endregion

    #region Aim

    public void StartAiming(Vector2 aimVector)
    {
        _itemRenderer.ShowItem();
        _itemRenderer.ChangeAim(aimVector);
        _animator.ChangeAimFrame(aimVector);
    }

    public void ChangeAim(Vector2 aimVector)
    {
        _itemRenderer.ChangeAim(aimVector);
        _animator.ChangeAimFrame(aimVector);
    }

    public void CancelAiming()
    {
        _itemRenderer.HideItem();
        _animator.PlayIdleAnimation();
    }

    #endregion

    #region Movement

    public void Push(Vector2 impulse)
    {
        _rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void Jump(Vector2 aimDirection)
    {
        _rb.AddForce(aimDirection * CharacterDefinition.JumpStrength, ForceMode2D.Impulse);
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

    public void InitializeMovementPreview(TrajectoryRenderer trajectoryRenderer)
    {
        trajectoryRenderer.SetOrigin(transform);
        trajectoryRenderer.SetTrajectoryMultipler(CharacterDefinition.JumpStrength);
    }

    #endregion

    #region Items

    public void UseSelectedItem(ItemUsageContext context)
    {
        _itemRenderer.UseItemThenHide();
        _selectedItem.Behavior.Use(context);
        _animator.PlayUseItemAnimation();
    }

    public bool TryAddItem(Item item)
    {
        if (_items.Any(i => i.IsSameType(item)))
        {
            return false;
        }
        else
        {
            _items.Add(item);
            return true;
        }
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _items;
    }

    public void SelectItem(Item item)
    {
        if (_items.Contains(item) && item != _selectedItem)
        {
            _selectedItem = item;
            SelectedItemChanged?.Invoke(item);
        }
    }

    public Item GetSelectedItem()
    {
        return _selectedItem;
    }    

    #endregion

}
