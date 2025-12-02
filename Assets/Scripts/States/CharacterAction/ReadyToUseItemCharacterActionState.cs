using System.Linq;
using UnityEngine;

public class ReadyToUseItemCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToUseItem;
    private ItemPreviewRendererManager _rendererManager;
    private ProjectilePool _projectilePool;
    private PixelLaserRenderer _laserRenderer;
    private TrajectoryRenderer _trajectoryRenderer;
    private GameplayUIManager _uiManager;
    private IGameplayInputSource _inputSource;

    public ReadyToUseItemCharacterActionState(ItemPreviewRendererManager rendererManager, PixelLaserRenderer laserRenderer, ProjectilePool projectilePool, TrajectoryRenderer trajectoryRenderer, GameplayUIManager uiManager, MonoBehaviour coroutineManager, UISoundsDefinition uiSounds) : base(coroutineManager, uiSounds)
    {
        _rendererManager = rendererManager;
        _projectilePool = projectilePool;
        _laserRenderer = laserRenderer;
        _trajectoryRenderer = trajectoryRenderer;
        _uiManager = uiManager;
    }
    protected override void SubscribeToEvents()
    {
        _currentCharacter.SelectedItemChanged += OnSelectedItemChanged;
        _currentCharacter.SelectedItemUsed += EndState;
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
        _inputSource.ImpulseReleased -= OnImpulseReleased;
        _inputSource.AimStarted -= OnAimStarted;
        _inputSource.AimChanged -= OnAimChanged;
        _inputSource.AimCancelled -= OnAimCancelled;
        _inputSource.ActionSkipped -= OnActionSkipped;
    }


    private void OnAimStarted(Vector2 initialPosition)
    {
        _trajectoryRenderer.ShowTrajectory(initialPosition);
        _currentCharacter.StartAiming();
    }

    private void OnAimChanged(Vector2 aimVector)
    {
        _trajectoryRenderer.DrawTrajectory(aimVector);
        _currentCharacter.ChangeAim(aimVector);
    }

    private void OnAimCancelled()
    {
        _trajectoryRenderer.HideTrajectory();
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

        _uiManager.ResumeGameplayTimer();
        _inputSource.IsAimingEnabled = true;
        _inputSource.IsOpeningInventoryEnabled = true;
        var context = new ItemUsageContext(_currentCharacter.transform.position, Vector2.zero, _currentCharacter, _laserRenderer, _projectilePool);
        currentCharacter.GetSelectedItem().Behavior.InitializePreview(context, _rendererManager);
        _inputSource.InputRequestedForAction(State);
    }

    public void OnSelectedItemChanged(Item selectedItem)
    {
        if(selectedItem == null)
        {
            return;
        }
        var context = new ItemUsageContext(_currentCharacter.transform.position, Vector2.zero, _currentCharacter, _laserRenderer, _projectilePool);
        selectedItem.Behavior.InitializePreview(context, _rendererManager);
    }

    protected override void EndState()
    {
        _inputSource.ForceCancelAiming();
        _inputSource.IsAimingEnabled = false;
        _inputSource.IsOpeningInventoryEnabled = false;
        _uiManager.PauseGameplayTimer();
        base.EndState();
    }

    private void OnImpulseReleased(Vector2 aimVector)
    {
        _trajectoryRenderer.HideTrajectory();
        _currentCharacter.UseSelectedItem(new ItemUsageContext(_currentCharacter.transform.position, aimVector, _currentCharacter, _laserRenderer, _projectilePool));
    }
}
