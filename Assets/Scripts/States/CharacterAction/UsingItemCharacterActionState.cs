using System.Collections;
using UnityEngine;

public class UsingItemCharacterActionState : CharacterActionState
{
    public UsingItemCharacterActionState() : base(CoroutineRunner.Instance)
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
