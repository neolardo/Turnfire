using UnityEngine;

public class ReadyToUseItemCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToUseItem;
    private ItemPreviewRendererManager _rendererManager;
    private InputManager _inputManager;
    private ProjectilePool _projectileManager;
    private TrajectoryRenderer _trajectoryRenderer;

    public ReadyToUseItemCharacterActionState(ItemPreviewRendererManager rendererManager, InputManager inputManager, ProjectilePool projectileManager, TrajectoryRenderer trajectoryRenderer, MonoBehaviour coroutineManager) : base(coroutineManager)
    {
        _rendererManager = rendererManager;
        _projectileManager = projectileManager;
        _inputManager = inputManager;
        _trajectoryRenderer = trajectoryRenderer;
    }
    protected override void SubscribeToEvents()
    {
        _currentCharacter.SelectedItemChanged += OnSelectedItemChanged;
        _inputManager.ImpulseReleased += OnImpulseReleased;
        _inputManager.AimStarted += OnAimStarted;
        _inputManager.AimChanged += OnAimChanged;
        _inputManager.AimCancelled += OnAimCancelled;
    }
    protected override void UnsubscribeFromEvents()
    {
        _currentCharacter.SelectedItemChanged -= OnSelectedItemChanged;
        _inputManager.ImpulseReleased -= OnImpulseReleased;
        _inputManager.AimStarted -= OnAimStarted;
        _inputManager.AimChanged -= OnAimChanged;
        _inputManager.AimCancelled -= OnAimCancelled;
    }


    private void OnAimStarted(Vector2 aimVector)
    {
        _trajectoryRenderer.ShowTrajectory(aimVector);
        _currentCharacter.StartAiming(aimVector);
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
        base.StartState(currentCharacter);
        _inputManager.IsAimingEnabled = true;
        _inputManager.IsOpeningInventoryEnabled = true;
        var context = new ItemUsageContext(_currentCharacter.transform.position, Vector2.zero, _currentCharacter.ItemTransform, _currentCharacter.Collider, _projectileManager);
        currentCharacter.GetSelectedItem().Behavior.InitializePreview(context, _rendererManager);
    }

    public void OnSelectedItemChanged(Item selectedItem)
    {
        var context = new ItemUsageContext(_currentCharacter.transform.position, Vector2.zero, _currentCharacter.ItemTransform, _currentCharacter.Collider, _projectileManager);
        selectedItem.Behavior.InitializePreview(context, _rendererManager);
    }

    protected override void EndState()
    {
        base.EndState();
        _inputManager.IsAimingEnabled = false;
        _inputManager.IsOpeningInventoryEnabled = false;
    }

    private void OnImpulseReleased(Vector2 aimVector)
    {
        _trajectoryRenderer.HideTrajectory();
        _currentCharacter.UseSelectedItem(new ItemUsageContext(_currentCharacter.transform.position, aimVector, _currentCharacter.ItemTransform, _currentCharacter.Collider,_projectileManager));
        EndState();
    }
}
