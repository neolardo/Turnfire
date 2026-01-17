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
        Debug.Log($"Using item state started with a value of : {_currentCharacter.IsUsingSelectedItem}");
        while (_currentCharacter.IsUsingSelectedItem && _currentCharacter.IsAlive && IsActive)
        {
            yield return null;
        }
        UnityEngine.Debug.Log("Using item state finished");
        EndState();
    }
}
