using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkTransform), typeof(NetworkRigidbody2D))]
public class OnlinePackage : IsActiveSyncedNetworkBehavior, IPackage
{
    [SerializeField] private SFXDefiniton spawnSFX;
    [SerializeField] private SFXDefiniton collectSFX;
    public bool IsMoving => _destroyed? false : _rb.linearVelocity.magnitude > 0;
    public bool IsActiveInHierarchy => _destroyed ? false : gameObject.activeInHierarchy;
    public Transform Transform => transform;

    private Rigidbody2D _rb;
    private ItemInstance _itemInstance;
    private CameraController _cameraController;
    private bool _destroyed;

    public event Action<IPackage> Destroyed;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cameraController = FindFirstObjectByType<CameraController>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _rb.simulated = IsServer;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_destroyed)
        {
            return;
        }
        _cameraController.SetPackageTarget(transform);
        AudioManager.Instance.PlaySFXAt(spawnSFX, transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer)
        {
            return;
        }

        if (collision.CompareTag(Constants.CharacterTag))
        {
            var character = collision.GetComponent<Character>();
            if (character.TryAddItem(_itemInstance))
            {
                Destroy();
            }
        }
    }

    public void Destroy()
    {
        if (_destroyed)
        {
            return;
        }

        var netObj = GetComponent<NetworkObject>();
        netObj.Despawn();
    }

    public override void OnNetworkDespawn()
    {
        AudioManager.Instance.PlaySFXAt(collectSFX, transform.position);
        _destroyed = true;
        Destroyed?.Invoke(this);
        gameObject.SetActive(false);
        base.OnNetworkDespawn();
    }

    public void SetItem(ItemInstance item)
    {
        _itemInstance = item;
    }
}
