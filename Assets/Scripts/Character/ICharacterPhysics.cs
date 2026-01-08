using UnityEngine;

public interface ICharacterPhysics 
{
    public Collider2D Collider { get;}
    public bool IsMoving { get; }
    public Vector2 FeetPosition { get; }
    public Vector2 FeetOffset { get; }

    public void Push(Vector2 impulse);
    public void Jump(Vector2 jumpVector);
    public bool OverlapPoint(Vector2 point);
    public Vector2 NormalAtPoint(Vector2 point);
}
