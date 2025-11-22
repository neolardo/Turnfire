using System;
using UnityEngine;

public interface IGameplayInputSource
{
    event Action<Vector2> AimStarted;
    event Action<Vector2> AimChanged;
    event Action<Vector2> ImpulseReleased;
    event Action AimCancelled;
    event Action ActionSkipped;
    event Action SelectedItemUsed;
    event Action<Item> ItemSwitched;

    bool IsAimingEnabled { get; set; }
    bool IsOpeningInventoryEnabled { get; set; }

    void Initialize(Team team);
    void ForceCloseInventory();
    void ForceCancelAiming();
    void StartProvidingInputForAction(CharacterActionStateType action);
}
