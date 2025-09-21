using System.Collections;
using UnityEngine;

public class FiringTurnState : TurnState
{
    public FiringTurnState(MonoBehaviour manager) : base(manager)
    {
    }

    public override TurnStateType State => TurnStateType.Firing;

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
