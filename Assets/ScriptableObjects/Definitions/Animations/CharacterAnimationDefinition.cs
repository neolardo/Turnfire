using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimationDefinition", menuName = "Scriptable Objects/Animations/CharacterAnimationDefinition")]
public class CharacterAnimationDefinition : ScriptableObject
{
    public CharacterFrame[] Frames;
}
