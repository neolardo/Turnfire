using System.Linq;
using UnityEngine;

public class ReadyToUseItemCharacterActionState : CharacterActionState
{
    public override CharacterActionStateType State => CharacterActionStateType.ReadyToUseItem;
    private PreviewRendererManager _previewRenderer;
    private ITeamInputSource _inputSource;
    public ReadyToUseItemCharacterActionState(PreviewRendererManager previewRenderer) : base(CoroutineRunner.Instance)
    {
        _previewRenderer = previewRenderer;
    }
    protected override void SubscribeToEvents()
    {
        _currentCharacter.SelectedItemChanged += OnSelectedItemChanged;
        _currentCharacter.SelectedItemUsed += EndState;
        _inputSource.SelectedItemUsed += OnSelectedItemUsed;
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
        _inputSource.SelectedItemUsed -= OnSelectedItemUsed;
        _inputSource.ItemSelected -= OnItemSelected;
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

        if (!_currentCharacter.GetAllItems().Any())
        {
            EndState();
            return;
        }

        GameServices.GameplayTimer.Resume();
        _inputSource.IsOpeningInventoryEnabled = true;
        _inputSource.IsActionSkippingEnabled = true;
        InitializePreviewAndAimingForItem(currentCharacter.SelectedItem);
        _inputSource.RequestAction(State);
    }

    private void OnItemSelected(int itemInstanceId)
    {
        _currentCharacter.TrySelectItem(itemInstanceId);
    }
    private void OnSelectedItemUsed(ItemUsageContext context)
    {
        _currentCharacter.UseSelectedItem(context);
    }

    private void OnAimStarted(Vector2 initialPosition)
    {
        _currentCharacter.StartAiming();
    }

    private void OnAimChanged(Vector2 aimVector)
    {
        _currentCharacter.ChangeAim(aimVector);
    }

    private void OnAimCancelled()
    {
        _currentCharacter.CancelAiming();
    }

    private void OnImpulseReleased(Vector2 aimVector)
    {
        _currentCharacter.UseSelectedItem(new ItemUsageContext(_currentCharacter.ItemTransform.position, aimVector, _currentCharacter));
    }

    private void OnSelectedItemChanged(ItemInstance selectedItem)
    {
        InitializePreviewAndAimingForItem(selectedItem);
    }

    private void InitializePreviewAndAimingForItem(ItemInstance selectedItem)
    {
        _inputSource.IsAimingEnabled = selectedItem != null;
        if (selectedItem == null)
        {
            return;
        }
        var context = new ItemUsageContext(_currentCharacter.ItemTransform.position, Vector2.zero, _currentCharacter);
        selectedItem.Behavior.InitializePreview(context, _previewRenderer);
    }

    protected override void EndState()
    {
        _inputSource.ForceCancelAiming();
        _inputSource.IsAimingEnabled = false;
        _inputSource.IsOpeningInventoryEnabled = false;
        _inputSource.IsActionSkippingEnabled = false;
        GameServices.GameplayTimer.Pause();
        base.EndState();
    }

}
