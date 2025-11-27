using System.Linq;
using UnityEngine;

public class ReadyToUseItemCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToUseItem;
    private ItemPreviewRendererManager _rendererManager;
    private ProjectilePool _projectileManager;
    private TrajectoryRenderer _trajectoryRenderer;
    private GameplayUIManager _uiManager;
    private IGameplayInputSource _inputSource;

    public ReadyToUseItemCharacterActionState(ItemPreviewRendererManager rendererManager, ProjectilePool projectileManager, TrajectoryRenderer trajectoryRenderer, GameplayUIManager uiManager, MonoBehaviour coroutineManager, UISoundsDefinition uiSounds) : base(coroutineManager, uiSounds)
    {
        _rendererManager = rendererManager;
        _projectileManager = projectileManager;
        _trajectoryRenderer = trajectoryRenderer;
        _uiManager = uiManager;
    }
    protected override void SubscribeToEvents()
    {
        _currentCharacter.SelectedItemChanged += OnSelectedItemChanged;
        _inputSource.ImpulseReleased += OnImpulseReleased;
        _inputSource.AimStarted += OnAimStarted;
        _inputSource.AimChanged += OnAimChanged;
        _inputSource.AimCancelled += OnAimCancelled;
        _inputSource.ActionSkipped += OnActionSkipped;
    }
    protected override void UnsubscribeFromEvents()
    {
        _currentCharacter.SelectedItemChanged -= OnSelectedItemChanged;
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
        var context = new ItemUsageContext(_currentCharacter.transform.position, Vector2.zero, _currentCharacter.ItemTransform, _currentCharacter.Collider, _projectileManager);
        currentCharacter.GetSelectedItem().Behavior.InitializePreview(context, _rendererManager);
        _inputSource.InputRequestedForAction(State);
    }

    public void OnSelectedItemChanged(Item selectedItem)
    {
        if(selectedItem == null)
        {
            return;
        }
        var context = new ItemUsageContext(_currentCharacter.transform.position, Vector2.zero, _currentCharacter.ItemTransform, _currentCharacter.Collider, _projectileManager);
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
        _currentCharacter.UseSelectedItem(new ItemUsageContext(_currentCharacter.transform.position, aimVector, _currentCharacter.ItemTransform, _currentCharacter.Collider,_projectileManager));
        EndState();
    }
}
