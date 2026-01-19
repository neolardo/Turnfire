using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class OfflineProjectile : SimplePoolable, IProjectile
{
    private ProjectileDefinition _definition;
    private ProjectilePhysics _physics;
    private ProjectileView _view;
    private IProjectileBehavior _behavior;
    public bool IsReady {get; private set;}

    public event Action<IProjectile> Exploded;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<CircleCollider2D>();
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var cameraController = FindFirstObjectByType<CameraController>();
        _view = new ProjectileView(transform, spriteRenderer, cameraController);
        _physics = new ProjectilePhysics(rb, col);
        IsReady = true;
    }

    public void Initialize(ProjectileDefinition definition, IProjectileBehavior behavior)
    {
        _definition = definition;
        _view.Initialize(_definition);
        _physics.Initialize(_definition);

        _behavior = behavior;
        _behavior.Exploded += OnExploded;
        _behavior.ContactedWithoutExplosion += OnContactedWithoutExplosion;
    }

    private void OnDisable()
    {
        if (_behavior != null)
        {
            _behavior.Exploded -= OnExploded;
            _behavior.ContactedWithoutExplosion -= OnContactedWithoutExplosion;
            _behavior = null;
        }
    }

    #region Contact

    private void OnTriggerEnter2D(Collider2D collider)
    {
        _physics.OnTriggerEnter2D(collider);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _physics.OnCollisionEnter2D(collision);
    }
    private void OnContactedWithoutExplosion(HitboxContactContext context)
    {
        if (_definition.ContactSFX != null)
        {
            AudioManager.Instance.PlaySFXAt(_definition.ContactSFX, context.ContactPoint);
        }
    }

    #endregion


    #region Launch

    public void Launch(ItemUsageContext itemContext, float fireStrength)
    {
        _view.Show();
        _behavior.Launch(new ProjectileLaunchContext(itemContext, fireStrength * _physics.RigidbodyMass, _physics));
    }

    #endregion


    #region Explode

    private void OnExploded(ExplosionInfo ei)
    {
        Exploded?.Invoke(this);
        _view.Hide();
    }

    public void ForceExplode()
    {
        _behavior.ForceExplode();
    }

    #endregion

    #region Poolable

    public override void OnReleasedBackToPool()
    {
        base.OnReleasedBackToPool();
        _physics.Stop();
    }

    #endregion


}
