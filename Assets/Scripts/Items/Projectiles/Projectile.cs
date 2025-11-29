using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private CircleCollider2D _col;
    private SpriteRenderer _spriteRenderer;
    private IProjectileBehavior _behavior;
    private ProjectileDefinition _definition;
    private ExplosionPool _explosionPool;
    public ExplosionPool ExplosionPool => _explosionPool;
    public CircleCollider2D Collider => _col;
    public Rigidbody2D Rigidbody => _rb;

    public event Action<ExplosionInfo> Exploded;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _explosionPool = FindAnyObjectByType<ExplosionPool>();
    }

    public void Initialize(ProjectileDefinition definition, IProjectileBehavior behavior)
    {
        _col.radius = definition.ColliderRadius;
        _col.isTrigger = true;
        _col.sharedMaterial = null;
        _rb.gravityScale = 1;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0;
        _rb.Sleep();
        _rb.WakeUp();
        if (_behavior != null) 
        {
            _behavior.Exploded -= OnExploded;
        }
        _behavior = behavior;
        _behavior.Exploded += OnExploded;
        _behavior.SetProjectile(this);
        _definition = definition;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.GroundTag) || collider.CompareTag(Constants.CharacterTag) || collider.CompareTag(Constants.DeadZoneTag))
        {
            _behavior.OnContact(new ProjectileContactContext(_rb.position, collider.tag));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _behavior.OnContact(new ProjectileContactContext(_rb.position, collision.collider.tag));
    }

    public void Launch(ItemUsageContext itemContext, float fireStrength)
    {
        _spriteRenderer.sprite = _definition.Sprite;
        gameObject.SetActive(true);
        _behavior.Launch(new ProjectileLaunchContext(itemContext, fireStrength * _rb.mass));
    }

    private void OnExploded(ExplosionInfo ei)
    {
        Exploded?.Invoke(ei);
        gameObject.SetActive(false);
    }

    public void ForceExplode()
    {
        _behavior.ForceExplode();
    }


}
