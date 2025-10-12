using UnityEngine;

public class ReadyToMoveCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToMove;
    private TrajectoryRenderer _trajectoryRenderer;
    private InputManager _inputManager;

    public ReadyToMoveCharacterActionState(TrajectoryRenderer trajectoryRenderer, InputManager inputManager, MonoBehaviour manager) : base(manager)
    {
        _trajectoryRenderer = trajectoryRenderer;
        _inputManager = inputManager;
    }

    protected override void SubscribeToEvents()
    {
        _inputManager.ImpulseReleased += OnImpulseReleased;
        _inputManager.AimStarted += OnAimStarted;
        _inputManager.AimChanged += OnAimChanged;
        _inputManager.AimCancelled += OnAimCancelled;
    }
    protected override void UnsubscribeFromEvents()
    {
        _inputManager.ImpulseReleased -= OnImpulseReleased;
        _inputManager.AimStarted -= OnAimStarted;
        _inputManager.AimChanged -= OnAimChanged;
        _inputManager.AimCancelled -= OnAimCancelled;
    }

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        _inputManager.IsAimingEnabled = true;
        _inputManager.IsOpeningInventoryEnabled = true;
        currentCharacter.InitializeMovementPreview(_trajectoryRenderer);
    }

    private void OnAimStarted(Vector2 aimVector)
    {
        _trajectoryRenderer.ShowTrajectory(aimVector);
    }

    private void OnAimChanged(Vector2 aimVector)
    {
        _trajectoryRenderer.DrawTrajectory(aimVector);
    }

    private void OnAimCancelled()
    {
        _trajectoryRenderer.HideTrajectory();
    }

    private void OnImpulseReleased(Vector2 aimDirection)
    {
        _trajectoryRenderer.HideTrajectory();
        _currentCharacter.Jump(aimDirection);
        EndState();
    }

    protected override void EndState()
    {
        base.EndState();
        _inputManager.IsAimingEnabled = false;
        _inputManager.IsOpeningInventoryEnabled = false;
    }
}
