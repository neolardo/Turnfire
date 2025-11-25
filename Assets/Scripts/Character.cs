using System;
using System.Collections.Generic;
using System.Linq;
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

    public Vector2 FeetPosition => (Vector2)transform.position + Vector2.down *_col.bounds.extents.y;

    public Vector2 FeetOffset => Vector2.down * _col.bounds.extents.y;

    public event Action<float, int> HealthChanged;
    public event Action Died;
    public event Action<Item> SelectedItemChanged;

    private void Awake()
    {
        Health = CharacterDefinition.MaxHealth;
        _items = new List<Item>();
        foreach(var itemDefinition in CharacterDefinition.InitialItems)
        {
            TryAddItem(CollectibleFactory.CreateCollectible(itemDefinition, false));
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
        gameObject.layer = Constants.DeadCharacterLayer;
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

    public void Jump(Vector2 aimDirection)
    { 
        var jumpForce = aimDirection * CharacterDefinition.JumpStrength;
        _rb.AddForce(jumpForce, ForceMode2D.Impulse);
        _animator.OnJumpStarted(jumpForce);
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

    public Vector2 SimulateJumpAndCalculateDestination(Vector2 start, Vector2 jumpVector, DestructibleTerrainManager terrain)
    {
        Vector2 pos = start;
        var velocity = jumpVector * CharacterDefinition.JumpStrength / _rb.mass;
        const float dt = Constants.ParabolicPathSimulationDeltaForMovement;
        for (float t = 0; t < Constants.MaxParabolicPathSimulationTime; t += Constants.ParabolicPathSimulationDeltaForMovement)
        {
            pos += velocity * dt;
            velocity += Physics2D.gravity * dt;

            if (!terrain.IsPointInsideBounds(pos) || terrain.OverlapPoint(pos))
                return pos;
        }

        return default;
    }

    #endregion

    #region Items

    public void UseSelectedItem(ItemUsageContext context)
    {
        _animator.PlayItemActionAnimation(_selectedItem);
        _selectedItem.Behavior.Use(context);
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
            SelectItem(_items.FirstOrDefault());
        }
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _items;
    }

    public void SelectItem(Item item)
    {
        if ((item == null) || (_items.Contains(item) && item != _selectedItem))
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

    #region Simulation Helpers

    public bool OverlapPoint(Vector2 point)
    {
        return _col.bounds.Contains(point);
    }

    public Vector2 NormalAtPoint(Vector2 point)
    {
        float linearHalfLength = _col.bounds.extents.y - _col.bounds.extents.x;
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
