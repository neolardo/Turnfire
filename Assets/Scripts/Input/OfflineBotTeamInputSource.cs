using System;
using UnityEngine;

public class OfflineBotTeamInputSource : MonoBehaviour, ITeamInputSource
{
    private BotManager _botManager;
    public bool IsAimingEnabled { get; set; }
    public bool IsOpeningInventoryEnabled { get; set; }
    public bool IsActionSkippingEnabled { get; set; }

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;
    public event Action ActionSkipped;
    public event Action<ItemUsageContext> SelectedItemUsed;
    public event Action<int> ItemSelected;

    private void Start()
    {
        _botManager = GetComponent<BotManager>();
        var controller = _botManager.Controller;
        controller.SkipAction += InvokeSkipAction;
        controller.AimAndRelease += InvokeAimAndRelease;
        controller.SwitchSelectedItem += InvokeSwitchSelectedItem;
        controller.UseSelectedItem += InvokeUseSelectedItem;
    }

    private void OnDestroy()
    {
        if (_botManager != null && _botManager.Controller != null)
        {
            var controller = _botManager.Controller;
            controller.SkipAction -= InvokeSkipAction;
            controller.AimAndRelease -= InvokeAimAndRelease;
            controller.SwitchSelectedItem -= InvokeSwitchSelectedItem;
            controller.UseSelectedItem -= InvokeUseSelectedItem;
        }
    }
    public void ForceCancelAiming() { }

    public void ForceCloseInventory() { }

    public void RequestAction(CharacterActionStateType action)
    {
        _botManager.BeginThinkingAndActing(action);
    }

    private void InvokeAimAndRelease(Vector2 aimVector)
    {
        AimStarted?.Invoke(new Vector2(-1, -1));
        AimChanged?.Invoke(aimVector);
        ImpulseReleased?.Invoke(aimVector);
    }

    private void InvokeSkipAction()
    {
        ActionSkipped?.Invoke();
    }

    private void InvokeSwitchSelectedItem(ItemInstance item)
    {
        ItemSelected?.Invoke(item.InstanceId);
    }

    private void InvokeUseSelectedItem(ItemUsageContext context)
    {
        SelectedItemUsed?.Invoke(context);
    }

}
