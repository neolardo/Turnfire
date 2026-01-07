using System.Collections;

public class MovingCharacterActionState : CharacterActionState
{
    public MovingCharacterActionState(UISoundsDefinition uiSounds) : base(CoroutineRunner.Instance, uiSounds)
    {
    }

    public override CharacterActionStateType State => CharacterActionStateType.Moving;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        StartCoroutine(WaitForCharacterToStopThenEndState());
    }

    private IEnumerator WaitForCharacterToStopThenEndState()
    {
        while (_currentCharacter.IsMoving && _currentCharacter.IsAlive && IsActive)
        {
            yield return null;
        }
        EndState();
    }
}
