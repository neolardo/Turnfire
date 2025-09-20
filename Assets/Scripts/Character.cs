using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    [HideInInspector] public bool IsAlive => _health > 0;
    [SerializeField] private Weapon _weapon;
    public CharacterData CharacterData;
    private Rigidbody2D _rb;
    private TrajectoryRenderer _trajectoryRenderer;
    private int _health;
    private TurnState _turnState;

    public event Action TurnFinished;

    private void Awake()
    {
        _health = CharacterData.MaxHealth;
        _rb = GetComponent<Rigidbody2D>();
        _trajectoryRenderer = FindFirstObjectByType<TrajectoryRenderer>();
    }

    public void OnImpulseReleased(Vector2 aimDirection)
    {
        if(_turnState == TurnState.Move)
        {
            Jump(aimDirection);
            _turnState = TurnState.Fire;
            _trajectoryRenderer.SetTrajectoryMultipler(_weapon.Projectile.ProjectileData.ImpulseStrength);
        }
        else if (_turnState == TurnState.Fire)
        {
            Fire(aimDirection);
            _turnState = TurnState.Finished;
            TurnFinished?.Invoke();
        }
    }

    public void Select()
    {
        _turnState = TurnState.Move;
        _trajectoryRenderer.SetStartTransform(transform); //TODO: decouple
        _trajectoryRenderer.SetTrajectoryMultipler(CharacterData.JumpStrength);
    }


    private void Jump(Vector2 aimDirection)
    {
        _rb.AddForce(aimDirection * CharacterData.JumpStrength, ForceMode2D.Impulse);
    }

    private void Fire(Vector2 aimDirection)
    {
        _weapon.Fire(aimDirection);
    }
}
