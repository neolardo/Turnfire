using System.Linq;
using UnityEngine;

public class ReadyToUseItemCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToUseItem;
    private ItemPreviewRendererManager _rendererManager;
    private PixelLaserRenderer _laserRenderer;
    private PixelTrajectoryRenderer _trajectoryRenderer;
    private GameplayUIManager _uiManager;
    private ITeamInputSource _inputSource;

    public ReadyToUseItemCharacterActionState(ItemPreviewRendererManager rendererManager, PixelLaserRenderer laserRenderer, PixelTrajectoryRenderer trajectoryRenderer, GameplayUIManager uiManager, UISoundsDefinition uiSounds) : base(CoroutineRunner.Instance, uiSounds)
    {
        _rendererManager = rendererManager;
        _laserRenderer = laserRenderer;
        _trajectoryRenderer = trajectoryRenderer;
        _uiManager = uiManager;
    }
    protected override void SubscribeToEvents()
    {
        _currentCharacter.SelectedItemChanged += OnSelectedItemChanged;
        _currentCharacter.SelectedItemUsed += EndState;
        _inputSource.SelectedItemUsed += _currentCharacter.UseSelectedItem;
        _inputSource.ItemSelected += OnItemSelected;
        _inputSource.ImpulseReleased += OnImpulseReleased;
        _inputSource.AimStarted += OnAimStarted;
        _inputSource.AimChanged += OnAimChanged;
        _inputSource.AimCancelled += OnAimCancelled;
        _inputSource.ActionSkipped += OnActionSkipped;
    }
    protected override void UnsubscribeFromEvents()
    {
        _currentCharacter.SelectedItemChanged -= OnSelectedItemChanged;
        _currentCharacter.SelectedItemUsed -= EndState;
        _inputSource.SelectedItemUsed -= _currentCharacter.UseSelectedItem;
        _inputSource.ItemSelected -= OnItemSelected;
        _inputSource.ImpulseReleased -= OnImpulseReleased;
        _inputSource.AimStarted -= OnAimStarted;
        _inputSource.AimChanged -= OnAimChanged;
        _inputSource.AimCancelled -= OnAimCancelled;
        _inputSource.ActionSkipped -= OnActionSkipped;
    }

    private void OnItemSelected(ItemInstance item)
    {
        _currentCharacter.TrySelectItem(item);
    }

    private void OnAimStarted(Vector2 initialPosition)
    {
        _uiManager.ShowAimCircles(initialPosition);
        _currentCharacter.StartAiming();
    }

    private void OnAimChanged(Vector2 aimVector)
    {
        _trajectoryRenderer.DrawTrajectory(aimVector);
        _uiManager.UpdateAimCircles(aimVector);
        _currentCharacter.ChangeAim(aimVector);
    }

    private void OnAimCancelled()
    {
        _trajectoryRenderer.HideTrajectory();
        _uiManager.HideAimCircles();
        _currentCharacter.CancelAiming();
    }

    public override void StartState(Character currentCharacter)
    {
        _inputSource = currentCharacter.Team.InputSource;
        base.StartState(currentCharacter);

        if (!_currentCharacter.GetAllItems().Any())
        {
            EndState();
            return;
        }

        GameServices.GameplayTimer.Resume();
        _inputSource.IsOpeningInventoryEnabled = true;
        var selectedItem = _currentCharacter.SelectedItem;
        OnSelectedItemChanged(selectedItem);
        _inputSource.RequestAction(State);
    }

    public void OnSelectedItemChanged(ItemInstance selectedItem)
    {
        _inputSource.IsAimingEnabled = selectedItem != null;
        if (selectedItem == null)
        {
            return;
        }
        var context = new ItemUsageContext(_currentCharacter.ItemTransform.position, Vector2.zero, _currentCharacter, _laserRenderer);
        selectedItem.Behavior.InitializePreview(context, _rendererManager);
    }

    protected override void EndState()
    {
        _inputSource.ForceCancelAiming();
        _inputSource.IsAimingEnabled = false;
        _inputSource.IsOpeningInventoryEnabled = false;
        GameServices.GameplayTimer.Pause();
        base.EndState();
    }

    private void OnImpulseReleased(Vector2 aimVector)
    {
        _trajectoryRenderer.HideTrajectory();
        _uiManager.HideAimCircles();
        _currentCharacter.UseSelectedItem(new ItemUsageContext(_currentCharacter.ItemTransform.position, aimVector, _currentCharacter, _laserRenderer));
    }
}
