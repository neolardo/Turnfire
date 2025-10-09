using UnityEngine;

public class ReadyToUseItemCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToUseItem;
    private ItemPreviewRendererManager _rendererManager;
    private InputManager _inputManager;
    private ProjectileManager _projectileManager;

    public ReadyToUseItemCharacterActionState(ItemPreviewRendererManager rendererManager, InputManager inputManager, ProjectileManager projectileManager, MonoBehaviour coroutineManager) : base(coroutineManager)
    {
        _rendererManager = rendererManager;
        _projectileManager = projectileManager;
        _inputManager = inputManager;
    }
    protected override void SubscribeToEvents()
    {
        _currentCharacter.SelectedItemChanged += OnSelectedItemChanged;
        _inputManager.ImpulseReleased += OnImpulseReleased;
    }
    protected override void UnsubscribeFromEvents()
    {
        _currentCharacter.SelectedItemChanged -= OnSelectedItemChanged;
        _inputManager.ImpulseReleased -= OnImpulseReleased;
    }

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        _inputManager.IsAimingEnabled = true;
        _inputManager.IsOpeningInventoryEnabled = true;
        var context = new ItemUsageContext(currentCharacter.transform.position, Vector2.zero, currentCharacter.transform, _projectileManager);
        currentCharacter.GetSelectedItem().Behavior.InitializePreview(context, _rendererManager); //TODO: on selected item changed!
    }

    public void OnSelectedItemChanged(Item selectedItem)
    {
        var context = new ItemUsageContext(_currentCharacter.transform.position, Vector2.zero, _currentCharacter.transform, _projectileManager);
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
        _currentCharacter.UseSelectedItem(new ItemUsageContext(_currentCharacter.transform.position, aimVector, _currentCharacter.transform, _projectileManager));
        EndState();
    }
}
