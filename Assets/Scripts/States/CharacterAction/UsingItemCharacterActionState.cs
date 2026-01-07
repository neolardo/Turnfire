using System.Collections;

public class UsingItemCharacterActionState : CharacterActionState
{
    public UsingItemCharacterActionState(UISoundsDefinition uiSounds) : base(CoroutineRunner.Instance, uiSounds)
    {
    }
    public override CharacterActionStateType State => CharacterActionStateType.UsingItem;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        StartCoroutine(WaitForItemUsageToFinishThenFinishState());
    }

    private IEnumerator WaitForItemUsageToFinishThenFinishState()
    {
        while (_currentCharacter.IsUsingSelectedItem && _currentCharacter.IsAlive && IsActive)
        {
            yield return null;
        }
        EndState();
    }
}
