using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(OneShotAnimator))]
[RequireComponent(typeof(NetworkTransform))]
public class OnlineExplosion : NetworkBehaviour, IExplosion
{
    [SerializeField] private ExplosionAnimatorDefinition _animatorDefinition;
    private ExplosionView _view;
    private ExplosionBehavior _behavior;

    public event Action<IExplosion> Exploded;

    public bool IsExploding => _behavior == null ? false : _behavior.IsExploding;

    private bool _awakeCalled;
    public bool IsReady => _awakeCalled && IsSpawned;

    private void Awake()
    {
        var animator = GetComponent<OneShotAnimator>();
        _view = new ExplosionView(animator, _animatorDefinition);
        _awakeCalled = true;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("explosion spawned");
    }

    public void Initialize(ExplosionDefinition explosionDefinition)
    {
        if(!IsServer)
        {
            return;
        }
        InitializeClientRpc(explosionDefinition.Id);
        float frameDuration = _animatorDefinition.ExplosionAnimationDurationPerFrame;
        float explosionDuration = explosionDefinition.Animation.GetTotalDuration(frameDuration);
        _behavior = new ExplosionBehavior(explosionDefinition, explosionDuration, transform);
        _behavior.Exploded += OnExplosionFinished;
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (_behavior != null && IsServer)
        {
            _behavior.Exploded -= OnExplosionFinished;
            _behavior = null;
        }
    } 

    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Server)]
    private void InitializeClientRpc(int explosionDefinitionId)
    {
        var def = GameServices.ExplosionDatabase.GetById(explosionDefinitionId);
        _view.Initialize(def);
    }

    public IEnumerable<Character> Explode(Vector2 contactPoint, int damage, IDamageSourceDefinition damageSource)
    {
        if(!IsServer)
        {
            return null;
        }
        ExplosionStartedClientRpc();
        return _behavior.Explode(contactPoint, damage, damageSource);
    }

    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Server)]
    private void ExplosionStartedClientRpc()
    {
        _view.PlayExplosionAnimation();
    }

    private void OnExplosionFinished()
    {
        ExplosionFinishedClientRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void ExplosionFinishedClientRpc()
    {
        Exploded?.Invoke(this);
    }
}
