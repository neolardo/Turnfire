using UnityEngine;

public readonly struct JumpLink 
{
    public readonly int FromId;
    public readonly int ToId;
    public readonly Vector2 JumpVector;

    public JumpLink(int fromId, int toId, Vector2 jumpVector)
    {
        FromId = fromId;
        ToId = toId;
        JumpVector = jumpVector;
    }
}
