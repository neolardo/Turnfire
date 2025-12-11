using System;
using UnityEngine;

public class BotGameplayInput : MonoBehaviour, IGameplayInputSource
{
    public bool IsAimingEnabled { get; set; }
    public bool IsOpeningInventoryEnabled { get; set; }

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;
    public event Action ActionSkipped;
    public event Action<ItemUsageContext> SelectedItemUsed;
    public event Action<Item> SelectedItemSwitchRequested;

    public event Action<CharacterActionStateType> InputRequested;

    public void RequestInputForAction(CharacterActionStateType action)
    {
        InputRequested?.Invoke(action);
    }

    public void ForceCancelAiming() { }

    public void ForceCloseInventory() { }

    public void AimAndRelease(Vector2 aimVector)
    {
        AimStarted?.Invoke(new Vector2(-1, -1));
        AimChanged?.Invoke(aimVector);
        ImpulseReleased?.Invoke(aimVector);
    }

    public void SkipAction()
    {
        ActionSkipped?.Invoke();
    }

    public void SetSelectedItem(Item item)
    {
        SelectedItemSwitchRequested?.Invoke(item);
    }

    public void UseSelectedItem(ItemUsageContext context)
    {
        SelectedItemUsed?.Invoke(context);
    }
}
