using UnityEngine;

[CreateAssetMenu(fileName = "CloudAnimatorDefinition", menuName = "Scriptable Objects/Animators/CloudAnimatorDefinition")]
public class CloudAnimatorDefinition : ScriptableObject
{
    public float MinMoveUnitPerMinute;
    public float MaxMoveUnitPerMinute;
    public float MinMinutesToTurn;
    public float MaxMinutesToTurn;
}
