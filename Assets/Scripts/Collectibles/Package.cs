using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Package : MonoBehaviour
{
    [SerializeField] private SFXDefiniton spawnSFX;
    [SerializeField] private SFXDefiniton collectSFX;
    public bool IsMoving => _rb.linearVelocity.magnitude > 0;
    private Rigidbody2D _rb;
    private ICollectible _collectible;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        AudioManager.Instance.PlaySFXAt(spawnSFX, transform);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.DeadZoneTag))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (collider.CompareTag(Constants.CharacterTag))
        {
            if(_collectible.TryCollect(collider.GetComponent<Character>()))
            {
                AudioManager.Instance.PlaySFXAt(collectSFX, transform.position);
                Destroy(gameObject);
            }
        }
    }

    public void SetCollectible(ICollectible collectible)
    {
        _collectible = collectible;
    }
}
