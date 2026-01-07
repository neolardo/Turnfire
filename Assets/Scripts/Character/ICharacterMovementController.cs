using System;
using UnityEngine;

public interface ICharacterMovementController 
{
    public Collider2D Collider { get;}
    public bool IsMoving { get; }
    public Vector2 FeetPosition { get; }
    public Vector2 FeetOffset { get; }

    event Action<Vector2> Jumped;

    public void Push(Vector2 impulse);
    public void StartJump(Vector2 jumpVector);
    public bool OverlapPoint(Vector2 point);
    public Vector2 NormalAtPoint(Vector2 point);
}
