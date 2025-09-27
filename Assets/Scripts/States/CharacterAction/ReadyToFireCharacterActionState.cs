using UnityEngine;

public class ReadyToFireCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToFire;
    private TrajectoryRenderer _trajectoryRenderer;
    private InputManager _inputManager;

    public ReadyToFireCharacterActionState(TrajectoryRenderer trajectoryRenderer, InputManager inputManager, MonoBehaviour manager) : base(manager)
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
        _trajectoryRenderer.SetTrajectoryMultipler(currentCharacter.FireStrength);
    }

    protected override void EndState()
    {
        base.EndState();
        _inputManager.IsAimingEnabled = false;
    }

    private void OnImpulseReleased(Vector2 aimDirection)
    {
        _currentCharacter.Fire(aimDirection);
        EndState();
    }
}
