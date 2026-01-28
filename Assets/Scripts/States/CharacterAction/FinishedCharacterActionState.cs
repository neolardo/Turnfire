public class FinishedCharacterActionState : CharacterActionState
{
    public FinishedCharacterActionState() : base(CoroutineRunner.Instance)
    {
    }

    public override CharacterActionStateType State => CharacterActionStateType.Finished;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        EndState();
    }
}
