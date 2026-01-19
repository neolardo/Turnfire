using UnityEngine;

public class ReadyToMoveCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToMove;
    private PreviewRendererManager _previewRenderer;
    private ITeamInputSource _inputSource;
    public ReadyToMoveCharacterActionState(PreviewRendererManager previewRenderer) : base(CoroutineRunner.Instance)
    {
        _previewRenderer = previewRenderer;
    }

    protected override void SubscribeToEvents()
    {
        _inputSource.ImpulseReleased += OnImpulseReleased;
        _inputSource.AimStarted += OnAimStarted;
        _inputSource.AimChanged += OnAimChanged;
        _inputSource.AimCancelled += OnAimCancelled;
        _inputSource.ActionSkipped += _currentCharacter.SkipAction;
        _currentCharacter.ActionSkipped += EndState;
        _currentCharacter.Jumped += EndState;
        _currentCharacter.Died += EndState;
    }
    protected override void UnsubscribeFromEvents()
    {
        _inputSource.ImpulseReleased -= OnImpulseReleased;
        _inputSource.AimStarted -= OnAimStarted;
        _inputSource.AimChanged -= OnAimChanged;
        _inputSource.AimCancelled -= OnAimCancelled;
        _inputSource.ActionSkipped -= _currentCharacter.SkipAction;
        _currentCharacter.ActionSkipped -= EndState;
        _currentCharacter.Jumped -= EndState;
        _currentCharacter.Died -= EndState;
    }

    public override void StartState(Character currentCharacter)
    {
        _inputSource = currentCharacter.Team.InputSource;
        base.StartState(currentCharacter);
        GameServices.GameplayTimer.Resume();
        _inputSource.IsAimingEnabled = true;
        _inputSource.IsActionSkippingEnabled = true;
         currentCharacter.InitializeMovementPreview(_previewRenderer);
        _inputSource.RequestAction(State);
    }

    private void OnAimStarted(Vector2 initialPosition)
    {
        _currentCharacter.PrepareToJump();
    }

    private void OnAimChanged(Vector2 aimVector)
    {
        _currentCharacter.ChangeJumpAim(aimVector);
    }

    private void OnAimCancelled()
    {
        _currentCharacter.CancelJump();
    }

    private void OnImpulseReleased(Vector2 aimDirection)
    {
        _currentCharacter.Jump(aimDirection);
    }

    protected override void EndState()
    {
        _inputSource.ForceCancelAiming();
        _inputSource.IsAimingEnabled = false;
        _inputSource.IsActionSkippingEnabled = false;
        GameServices.GameplayTimer.Pause();
        base.EndState();
    }
}
