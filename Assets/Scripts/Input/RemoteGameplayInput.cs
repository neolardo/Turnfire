using System;
using UnityEngine;

public class RemoteGameplayInput : MonoBehaviour, IGameplayInputSource
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

    public void ForceCancelAiming() { }

    public void ForceCloseInventory() { }

    public void Initialize(Team team)
    {
        //TODO
    }

    public void InputRequestedForAction(CharacterActionStateType action) 
    {
        //TODO
    }
}
