using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Package : MonoBehaviour
{
    public bool IsMoving => _rb.linearVelocity.magnitude > 0;
    private Rigidbody2D _rb;
    private Collectible _collectible;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.DeadZoneTag))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else if (collision.CompareTag(Constants.CharacterTag))
        {
            if(_collectible.TryCollect(collision.GetComponent<Character>()))
            {
                Debug.Log($"Package '{gameObject.name}' picked up!");
                Destroy(gameObject);
            }
        }
    }

    public void SetCollectible(Collectible collectible)
    {
        _collectible = collectible;
    }
}
