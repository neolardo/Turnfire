using System.Collections;
using UnityEngine;

public class FiringCharacterActionState : CharacterActionState
{
    public FiringCharacterActionState(MonoBehaviour manager) : base(manager)
    {
    }

    public override CharacterActionStateType State => CharacterActionStateType.Firing;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        StartCoroutine(WaitForWeaponToFinishFiringThenFinishState());
    }

    private IEnumerator WaitForWeaponToFinishFiringThenFinishState()
    {
        while (_currentCharacter.IsFiring && _currentCharacter.IsAlive && IsActive)
        {
            yield return null;
        }
        EndState();
    }
}
