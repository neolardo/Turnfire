using System;
using Unity.Netcode;
using UnityEngine;

public class OnlineBotTeamInputSource : NetworkBehaviour, ITeamInputSource
{
    private BotManager _botManager;
    public bool IsAimingEnabled { get; set; }
    public bool IsOpeningInventoryEnabled { get; set; }

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;
    public event Action ActionSkipped;
    public event Action<ItemUsageContext> SelectedItemUsed;
    public event Action<int> ItemSelected;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _botManager = GetComponent<BotManager>();
        var controller = _botManager.Controller;
        controller.SkipAction += InvokeSkipAction;
        controller.AimAndRelease += InvokeAimAndRelease;
        controller.SwitchSelectedItem += InvokeItemSelected;
        controller.UseSelectedItem += InvokeSelectedItemUsed;
    } 

    public override void OnDestroy()
    {
        if(_botManager != null && _botManager.Controller != null)
        {
            var controller = _botManager.Controller;
            controller.SkipAction -= InvokeSkipAction;
            controller.AimAndRelease -= InvokeAimAndRelease;
            controller.SwitchSelectedItem -= InvokeItemSelected;
            controller.UseSelectedItem -= InvokeSelectedItemUsed;
        }
    }

    public void ForceCancelAiming() { }

    public void ForceCloseInventory() { }

    public void RequestAction(CharacterActionStateType action)
    {
        if(!IsServer)
        {
            return;
        }
        _botManager.BeginThinkingAndActing(action);
    }

    private void InvokeAimAndRelease(Vector2 aimVector)
    {
        if (!IsServer)
        {
            return;
        }
        AimStarted?.Invoke(new Vector2(-1, -1));
        AimChanged?.Invoke(aimVector);
        ImpulseReleased?.Invoke(aimVector);
    }

    private void InvokeSkipAction()
    {
        if (!IsServer)
        {
            return;
        }
        ActionSkipped?.Invoke();
    }

    private void InvokeItemSelected(ItemInstance item)
    {
        if (!IsServer)
        {
            return;
        }
        ItemSelected?.Invoke(item.InstanceId);
    }

    private void InvokeSelectedItemUsed(ItemUsageContext context)
    {
        if (!IsServer)
        {
            return;
        }
        SelectedItemUsed?.Invoke(context);
    }
}
