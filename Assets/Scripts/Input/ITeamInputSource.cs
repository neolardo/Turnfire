using System;
using UnityEngine;

public interface ITeamInputSource
{
    event Action<Vector2> AimStarted;
    event Action<Vector2> AimChanged;
    event Action<Vector2> ImpulseReleased;
    event Action AimCancelled;
    event Action ActionSkipped;
    event Action<ItemUsageContext> SelectedItemUsed;
    event Action<ItemInstance> ItemSelected;

    bool IsAimingEnabled { get; set; }
    bool IsOpeningInventoryEnabled { get; set; }

    void ForceCloseInventory();
    void ForceCancelAiming();
    void RequestAction(CharacterActionStateType action);
}
