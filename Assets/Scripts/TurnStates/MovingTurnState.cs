using System.Collections;
using UnityEngine;

public class MovingTurnState : TurnState
{
    public MovingTurnState(MonoBehaviour manager) : base(manager)
    {
    }

    public override TurnStateType State => TurnStateType.Moving;

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
