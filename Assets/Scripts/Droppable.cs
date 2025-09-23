using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Droppable : MonoBehaviour
{
    public bool IsMoving => _rb.linearVelocity.magnitude > 0;
    private Rigidbody2D _rb;

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
    }

}
