using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rb;
    private IProjectileBehavior _behavior;
    private ProjectileDefinition _definition;

    public event Action<ExplosionInfo> Exploded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
            _behavior.OnContact(new ProjectileContactContext(this, _rb.position));
        }
    }

    public void Launch(ItemUsageContext itemContext, float fireStrength)
    {
        gameObject.SetActive(true);
        _behavior.Launch(new ProjectileLaunchContext(itemContext, fireStrength, _rb));
    }

    private void OnExploded(ExplosionInfo ei)
    {
        gameObject.SetActive(false);
        Exploded?.Invoke(ei);
    }


}
