using UnityEngine;

public class ReadyToMoveTurnState : TurnState
{
    public override TurnStateType State => TurnStateType.ReadyToMove;
    private TrajectoryRenderer _trajectoryRenderer;
    private InputManager _inputManager;

    public ReadyToMoveTurnState(TrajectoryRenderer trajectoryRenderer, InputManager inputManager, MonoBehaviour manager) : base(manager)
    {
        _trajectoryRenderer = trajectoryRenderer;
        _inputManager = inputManager;
    }

    protected override void SubscribeToEvents()
    {
        _inputManager.ImpulseReleased += OnImpulseReleased;
    }
    protected override void UnsubscribeFromEvents()
    {
        _inputManager.ImpulseReleased -= OnImpulseReleased;
    }

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        _inputManager.IsAimingEnabled = true;
        _trajectoryRenderer.SetStartTransform(currentCharacter.transform);
        _trajectoryRenderer.SetTrajectoryMultipler(currentCharacter.CharacterData.JumpStrength);
    }

    private void OnImpulseReleased(Vector2 aimDirection)
    {
        if (IsActive)
        {
            _currentCharacter.Jump(aimDirection);
            EndState();
        }
    }

    protected override void EndState()
    {
        base.EndState();
        _inputManager.IsAimingEnabled = false;
    }
}
