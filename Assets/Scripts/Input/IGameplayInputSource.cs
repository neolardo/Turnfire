using System;
using UnityEngine;

public interface IGameplayInputSource
{
    event Action<Vector2> AimStarted;
    event Action<Vector2> AimChanged;
    event Action<Vector2> ImpulseReleased;
    event Action AimCancelled;
    event Action ActionSkipped;
    event Action<ItemUsageContext> SelectedItemUsed;
    event Action<Item> SelectedItemSwitchRequested;

    bool IsAimingEnabled { get; set; }
    bool IsOpeningInventoryEnabled { get; set; }

    void ForceCloseInventory();
    void ForceCancelAiming();
    void RequestInputForAction(CharacterActionStateType action);
}
