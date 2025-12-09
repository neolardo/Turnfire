using UnityEngine;

public readonly struct JumpLink 
{
    public readonly int FromId;
    public readonly int ToId;
    public readonly Vector2 JumpVector;

    public bool IsPossible(float jumpStrength)
    {
        return JumpVector.magnitude <= jumpStrength + Mathf.Epsilon;
    }

    public JumpLink(int fromId, int toId, Vector2 jumpVector)
    {
        FromId = fromId;
        ToId = toId;
        JumpVector = jumpVector;
    }
}
