using UnityEngine;

public class ReadyToMoveCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToMove;
    private TrajectoryRenderer _trajectoryRenderer;
    private GameplayUIManager _uiManager;
    private IGameplayInputSource _inputSource;

    public ReadyToMoveCharacterActionState(TrajectoryRenderer trajectoryRenderer, GameplayUIManager uiManager, MonoBehaviour manager, UISoundsDefinition uiSounds) : base(manager, uiSounds)
    {
        _trajectoryRenderer = trajectoryRenderer;
        _uiManager = uiManager;
    }

    protected override void SubscribeToEvents()
    {
        _inputSource.ImpulseReleased += OnImpulseReleased;
        _inputSource.AimStarted += OnAimStarted;
        _inputSource.AimChanged += OnAimChanged;
        _inputSource.AimCancelled += OnAimCancelled;
        _inputSource.ActionSkipped += OnActionSkipped;
    }
    protected override void UnsubscribeFromEvents()
    {
        _inputSource.ImpulseReleased -= OnImpulseReleased;
        _inputSource.AimStarted -= OnAimStarted;
        _inputSource.AimChanged -= OnAimChanged;
        _inputSource.AimCancelled -= OnAimCancelled;
        _inputSource.ActionSkipped -= OnActionSkipped;
    }

    public override void StartState(Character currentCharacter)
    {
        _inputSource = currentCharacter.Team.InputSource;
        base.StartState(currentCharacter);
        _uiManager.ResumeGameplayTimer();
        _inputSource.IsAimingEnabled = true;
        _inputSource.IsOpeningInventoryEnabled = true;
        currentCharacter.InitializeMovementPreview(_trajectoryRenderer);
        _inputSource.StartProvidingInputForAction(State);
    }

    private void OnAimStarted(Vector2 initialPosition)
    {
        _trajectoryRenderer.ShowTrajectory(initialPosition);
        _currentCharacter.PrepareToJump();
    }

    private void OnAimChanged(Vector2 aimVector)
    {
        _trajectoryRenderer.DrawTrajectory(aimVector);
        _currentCharacter.ChangeJumpAim(aimVector);
    }

    private void OnAimCancelled()
    {
        _trajectoryRenderer.HideTrajectory();
        _currentCharacter.CancelJump();
    }

    private void OnImpulseReleased(Vector2 aimDirection)
    {
        _trajectoryRenderer.HideTrajectory();
        _currentCharacter.Jump(aimDirection);
        EndState();
    }

    protected override void EndState()
    {
        _inputSource.ForceCancelAiming();
        _inputSource.IsAimingEnabled = false;
        _inputSource.IsOpeningInventoryEnabled = false;
        _uiManager.PauseGameplayTimer();
        base.EndState();
    }
}
