using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OfflinePackage : MonoBehaviour, IPackage
{
    [SerializeField] private SFXDefiniton spawnSFX;
    [SerializeField] private SFXDefiniton collectSFX;
    public bool IsMoving => _rb.linearVelocity.magnitude > 0;
    public bool IsActiveInHierarchy => gameObject.activeInHierarchy;
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

    private void OnEnable()
    {
        if (_destroyed)
        {
            return;
        }
        _cameraController.SetPackageTarget(this);
        AudioManager.Instance.PlaySFXAt(spawnSFX, transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.CharacterTag))
        {
            var character = collision.GetComponent<Character>();
            if (character.TryAddItem(_itemInstance))
            {
                AudioManager.Instance.PlaySFXAt(collectSFX, transform.position);
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

        _destroyed = true;
        Destroyed?.Invoke(this);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void SetItem(ItemInstance item)
    {
        _itemInstance = item;
    }
}
