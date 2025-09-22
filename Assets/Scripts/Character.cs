using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    [HideInInspector] public bool IsAlive => _health > 0;
    [HideInInspector] public bool IsMoving => _rb.linearVelocity.magnitude > Mathf.Epsilon;
    [HideInInspector] public bool IsFiring => _weapon.IsFiring;
    [HideInInspector] public float FireStrength => _weapon.Projectile.ProjectileData.FireStrength; //TODO
    [SerializeField] private Weapon _weapon;
    [SerializeField] private Healthbar _healthbar; //TODO
    public CharacterData CharacterData;
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

    public event Action<int> HealthChanged;

    private void Awake()
    {
        Health = CharacterData.MaxHealth;
        _rb = GetComponent<Rigidbody2D>();
        _healthbar.Follow(transform);
        _healthbar.SetMaxHealth(Health);
        HealthChanged += _healthbar.SetCurrentHeath; //TODO
    }

    public void Damage(int value)
    {
        Health = Mathf.Max(0, Health - value);
        if(!IsAlive)
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
        Debug.Log(gameObject.name + " died." );
    }

    public void Push(Vector2 impulse)
    {
        _rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void Jump(Vector2 aimDirection)
    {
        _rb.AddForce(aimDirection * CharacterData.JumpStrength, ForceMode2D.Impulse);
    }

    public void Fire(Vector2 aimDirection)
    {
        _weapon.Fire(aimDirection);
    }
}
