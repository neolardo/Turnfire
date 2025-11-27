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
    public event Action SelectedItemUsed;
    public event Action<Item> ItemSwitched;

    public event Action<CharacterActionStateType> InputRequested;

    public void InputRequestedForAction(CharacterActionStateType action)
    {
        InputRequested?.Invoke(action);
    }

    public void ForceCancelAiming() { }

    public void ForceCloseInventory() { }

    public void AimAndRelease(Vector2 aimVector)
    {
        AimStarted?.Invoke(aimVector);
        AimChanged?.Invoke(aimVector);
        ImpulseReleased?.Invoke(aimVector);
    }

    public void SkipAction()
    {
        ActionSkipped?.Invoke();
    }

    public void SwitchSelectedItemTo(Item item)
    {
        ItemSwitched?.Invoke(item);
    }

    public void UseSelectedItem()
    {
        SelectedItemUsed?.Invoke();
    }
}
