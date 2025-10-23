using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{

    [SerializeField] private CharacterHealthbarRenderer _healthbarRenderer;
    [SerializeField] private CharacterAnimator _animator;

    public CharacterDefinition CharacterDefinition;

    private List<Item> _items;
    private Item _selectedItem;
    private Rigidbody2D _rb;
    private Collider2D _col;
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

    public Team Team { get; private set; }

    public Transform ItemTransform => _animator.ItemTransform;
    public Collider2D Collider => _col;
    public bool IsAlive => _health > 0;
    public bool IsMoving => _rb.linearVelocity.magnitude > Mathf.Epsilon;
    public bool IsUsingSelectedItem => _selectedItem == null ? false : _selectedItem.Behavior.IsInUse;
    public float NormalizedHealth => _health / (float)CharacterDefinition.MaxHealth;

    public event Action<float, int> HealthChanged;
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
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _selectedItem = _items.FirstOrDefault();
        HealthChanged += _healthbarRenderer.SetCurrentHealth;
    }

    private void Start() // after child awake run
    {
        _healthbarRenderer.SetCurrentHealth(NormalizedHealth, Health);
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
        _animator.PlayDeathAnimation();
        Debug.Log(gameObject.name + " died.");
        Died?.Invoke();
    }

    public void SetTeam(Team team)
    {
        Team = team;
        _animator.SetTeamColor(Team.TeamColor);
    }


    #endregion

    #region Aim

    public void StartAiming(Vector2 aimVector)
    {
        _animator.StartAiming(_selectedItem, aimVector);
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
        trajectoryRenderer.ToggleGravity(true);
        trajectoryRenderer.SetTrajectoryMultipler(CharacterDefinition.JumpStrength);
    }

    #endregion

    #region Items

    public void UseSelectedItem(ItemUsageContext context)
    { 
        _selectedItem.Behavior.Use(context);
        _animator.PlayItemActionAnimation(_selectedItem);
    }

    public bool TryAddItem(Item item)
    {
        var existingItem = _items.FirstOrDefault(i => i.IsSameType(item));
        if (existingItem == null)
        {
            _items.Add(item);
            item.CollectibleDestroyed += OnCollectibleDestroyed;
            if (_items.Count == 1)
            {
                SelectItem(item);
            }
            return true;
        }
        else
        {
            return existingItem.TryMerge(item); 
        }
    }

    private void OnCollectibleDestroyed(ICollectible collectible)
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
            _selectedItem = _items.FirstOrDefault();
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
