using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    [HideInInspector] public bool IsAlive => _health > 0;
    [HideInInspector] public Transform Transform;
    public CharacterData CharacterData;
    private Rigidbody2D _rb;
    private int _health;

    private void Awake()
    {
        _health = CharacterData.MaxHealth;
        _rb = GetComponent<Rigidbody2D>();
        Transform = GetComponent<Transform>();
    }

    public void Jump(Vector2 normalizedVelocity)
    {
        _rb.AddForce(normalizedVelocity * CharacterData.JumpStrength, ForceMode2D.Impulse);
    }
}
