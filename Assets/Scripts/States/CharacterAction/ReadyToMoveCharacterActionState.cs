using System;
using UnityEngine;

public class ReadyToMoveCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToMove;
    private PixelTrajectoryRenderer _trajectoryRenderer;
    private GameplayUIManager _uiManager;
    private ITeamInputSource _inputSource;

    public ReadyToMoveCharacterActionState(PixelTrajectoryRenderer trajectoryRenderer, GameplayUIManager uiManager, UISoundsDefinition uiSounds) : base(CoroutineRunner.Instance, uiSounds)
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
        _currentCharacter.Jumped += EndState; //TODO: update every state to be event based
    }
    protected override void UnsubscribeFromEvents()
    {
        _inputSource.ImpulseReleased -= OnImpulseReleased;
        _inputSource.AimStarted -= OnAimStarted;
        _inputSource.AimChanged -= OnAimChanged;
        _inputSource.AimCancelled -= OnAimCancelled;
        _inputSource.ActionSkipped -= OnActionSkipped;
        _currentCharacter.Jumped -= EndState;
    }
    
    public override void StartState(Character currentCharacter)
    {
        _inputSource = currentCharacter.Team.InputSource;
        base.StartState(currentCharacter);
        GameServices.GameplayTimer.Resume();
        _inputSource.IsAimingEnabled = true;
        _trajectoryRenderer.SetOrigin(currentCharacter.transform, currentCharacter.FeetOffset);
        _trajectoryRenderer.ToggleGravity(true);
        _trajectoryRenderer.SetTrajectoryMultipler(currentCharacter.JumpStrength);
        _inputSource.RequestAction(State);
    }

    private void OnAimStarted(Vector2 initialPosition)
    {
        _uiManager.ShowAimCircles(initialPosition);
        _currentCharacter.PrepareToJump();
    }

    private void OnAimChanged(Vector2 aimVector)
    {
        _trajectoryRenderer.DrawTrajectory(aimVector);
        _uiManager.UpdateAimCircles(aimVector);
        _currentCharacter.ChangeJumpAim(aimVector);
    }

    private void OnAimCancelled()
    {
        _trajectoryRenderer.HideTrajectory();
        _uiManager.HideAimCircles();
        _currentCharacter.CancelJump();
    }

    private void OnImpulseReleased(Vector2 aimDirection)
    {
        _trajectoryRenderer.HideTrajectory();
        _uiManager.HideAimCircles();
        _currentCharacter.Jump(aimDirection);
    }

    protected override void EndState()
    {
        _inputSource.ForceCancelAiming();
        _inputSource.IsAimingEnabled = false;
        GameServices.GameplayTimer.Pause();
        base.EndState();
    }
}
