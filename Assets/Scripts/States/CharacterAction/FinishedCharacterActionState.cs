public class FinishedCharacterActionState : CharacterActionState
{
    public FinishedCharacterActionState(UISoundsDefinition uiSounds) : base(CoroutineRunner.Instance, uiSounds)
    {
    }

    public override CharacterActionStateType State => CharacterActionStateType.Finished;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        EndState();
    }
}
