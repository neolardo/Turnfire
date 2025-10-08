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

    [SerializeField] private HealthbarUI _healthbar; //TODO

    [SerializeField] CharacterItemRenderer _itemRenderer;

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
                HealthChanged?.Invoke(_health);
            }
        }
    }

    private List<Item> _items;
    private Item _selectedItem;

    public event Action<int> HealthChanged;
    public event Action Died;

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
        _rb = GetComponent<Rigidbody2D>();
        _healthbar.Follow(transform);
        _healthbar.SetMaxHealth(Health);
        HealthChanged += _healthbar.SetCurrentHeath; //TODO
    }

    #region Health

    public void Damage(int value)
    {
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
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.grey;
        Debug.Log(gameObject.name + " died.");
        Died?.Invoke();
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

    public void InitializeMovementPreview(TrajectoryRenderer trajectoryRenderer)
    {
        trajectoryRenderer.SetStartTransform(transform);
        trajectoryRenderer.SetTrajectoryMultipler(CharacterDefinition.JumpStrength);
    }

    #endregion

    #region Items

    public void UseSelectedItem(ItemUsageContext context)
    {
        _selectedItem.Behavior.Use(context);
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
        if (_items.Contains(item))
        {
            _selectedItem = item;
        }
    }

    public Item GetSelectedItem()
    {
        return _selectedItem;
    }    

    #endregion

}
