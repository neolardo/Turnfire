using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Package : MonoBehaviour
{
    [SerializeField] private SFXDefiniton spawnSFX;
    [SerializeField] private SFXDefiniton collectSFX;
    public bool IsMoving => _rb.linearVelocity.magnitude > 0;
    private Rigidbody2D _rb;
    private ICollectible _collectible;

    private bool _destroyed;

    public event Action<Package> Destroyed;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        AudioManager.Instance.PlaySFXAt(spawnSFX, transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.CharacterTag))
        {
            var character = collision.GetComponent<Character>();
            if (_collectible.TryCollect(character))
            {
                BotEvaluationStatistics.GetData(character.Team).OpenedPackageCount++;
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

    public void SetCollectible(ICollectible collectible)
    {
        _collectible = collectible;
    }
}
