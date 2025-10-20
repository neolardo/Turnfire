using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private IProjectileBehavior _behavior;
    private ProjectileDefinition _definition;
    private ExplosionPool _explosionManager; //TODO

    public event Action<ExplosionInfo> Exploded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _explosionManager = FindAnyObjectByType<ExplosionPool>();
    }

    public void Initialize(ProjectileDefinition definition, IProjectileBehavior behavior)
    {
        if(_behavior != null) 
        {
            _behavior.Exploded -= OnExploded;
        }
        _behavior = behavior;
        _behavior.Exploded += OnExploded;
        _definition = definition;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.GroundTag) || collision.CompareTag(Constants.CharacterTag) || collision.CompareTag(Constants.DeadZoneTag))
        {
            _behavior.OnContact(new ProjectileContactContext(this, _rb.position, _explosionManager));
        }
    }

    public void Launch(ItemUsageContext itemContext, float fireStrength)
    {
        gameObject.SetActive(true);
        _spriteRenderer.sprite = _definition.Sprite;
        _behavior.Launch(new ProjectileLaunchContext(itemContext, fireStrength, _rb));
    }

    private void OnExploded(ExplosionInfo ei)
    {
        Exploded?.Invoke(ei);
        gameObject.SetActive(false);
    }


}
