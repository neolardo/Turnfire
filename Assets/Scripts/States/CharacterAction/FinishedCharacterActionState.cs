using UnityEngine;

public class FinishedCharacterActionState : CharacterActionState
{
    public FinishedCharacterActionState(MonoBehaviour manager) : base(manager)
    {
    }

    public override CharacterActionStateType State => CharacterActionStateType.Finished;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        EndState();
    }
}
