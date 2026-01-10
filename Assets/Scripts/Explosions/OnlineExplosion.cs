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

    public bool IsExploding => _behavior.IsExploding;

    public bool IsReady { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        var animator = GetComponent<OneShotAnimator>();
        _view = new ExplosionView(animator, _animatorDefinition);
        IsReady = true;
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

    [Rpc(SendTo.Everyone, InvokePermission =RpcInvokePermission.Server)]
    private void InitializeClientRpc(int explosionDefinitionId)
    {
        var def = GameServices.ExplosionDatabase.GetById(explosionDefinitionId);
        _view.Initialize(def);
    }

    public IEnumerable<Character> Explode(Vector2 contactPoint, int damage)
    {
        if(!IsServer)
        {
            return null;
        }
        ExplosionStartedClientRpc();
        return _behavior.Explode(contactPoint, damage);
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
