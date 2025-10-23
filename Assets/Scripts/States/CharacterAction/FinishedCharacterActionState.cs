using UnityEngine;

public class FinishedCharacterActionState : CharacterActionState
{
    public FinishedCharacterActionState(MonoBehaviour manager, UISoundsDefinition uiSounds) : base(manager, uiSounds)
    {
    }

    public override CharacterActionStateType State => CharacterActionStateType.Finished;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        EndState();
    }
}
