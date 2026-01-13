using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(NetworkTransform), typeof(NetworkRigidbody2D))]
public class OnlineProjectile : IsActiveSyncedNetworkBehavior, IProjectile
{
    private ProjectileDefinition _definition;
    private ProjectilePhysics _physics;
    private ProjectileView _view;
    private IProjectileBehavior _behavior;
    private Rigidbody2D _rb;

    public bool IsReady { get; private set; }

    public event Action<IProjectile> Exploded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<CircleCollider2D>();
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var cameraController = FindFirstObjectByType<CameraController>();
        _view = new ProjectileView(transform, spriteRenderer, cameraController);
        _physics = new ProjectilePhysics(_rb, col);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            var container = FindFirstObjectByType<ProjectileContainer>();
            transform.parent = container.transform;
        }
        IsReady = true;
    }

    public void Initialize(ProjectileDefinition definition, IProjectileBehavior behavior) 
    {
        if(!IsServer)
        { 
            return;
        }
        InitializeClientRpc(definition.Id);

        _behavior = behavior;
        _behavior.Exploded += OnExploded;
        _behavior.ContactedWithoutExplosion += OnContactedWithoutExplosion;
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        if (_behavior != null && IsServer)
        {
            _behavior.Exploded -= OnExploded;
            _behavior.ContactedWithoutExplosion -= OnContactedWithoutExplosion;
            _behavior = null;
        }
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InitializeClientRpc(int projectileDefinitionId)
    {
        _definition = GameServices.ProjectileDatabase.GetById(projectileDefinitionId);
        _view.Initialize(_definition);
        _physics.Initialize(_definition);
    }

    #region Contact

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsServer)
        {
            return;
        }
        _physics.OnTriggerEnter2D(collider);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
        {
            return;
        }
        _physics.OnCollisionEnter2D(collision);
    }

    private void OnContactedWithoutExplosion(HitboxContactContext context)
    {
        if (!IsServer)
        {
            return;
        }
        OnContactedWithoutExplosionClientRpc(context.ContactPoint);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void OnContactedWithoutExplosionClientRpc(Vector2 contactPoint)
    {
        if (_definition.ContactSFX != null)
        {
            AudioManager.Instance.PlaySFXAt(_definition.ContactSFX, contactPoint);
        }
    }

    #endregion

    #region Launch

    public void Launch(ItemUsageContext itemContext, float fireStrength)
    {
        if (!IsServer)
        {
            return;
        }
        LaunchClientRpc();
        _behavior.Launch(new ProjectileLaunchContext(itemContext, fireStrength * _physics.RigidbodyMass, _physics));
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void LaunchClientRpc()
    {
        _view.Show();
    }

    #endregion

    #region Explode

    public void ForceExplode()
    {
        _behavior.ForceExplode();
    }

    private void OnExploded(ExplosionInfo ei)
    {
        Exploded?.Invoke(this);
        OnExplodedClientRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void OnExplodedClientRpc()
    {
        _view.Hide();
    } 

    #endregion

}
