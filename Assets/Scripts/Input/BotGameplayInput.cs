using System;
using System.Collections;
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

    private Team _team;
    private BotBrain _brain;
    private BotContextProvider _contextProvider;

    private void Awake()
    {
        _contextProvider = FindFirstObjectByType<BotContextProvider>();
    }
    public void Initialize(Team team)
    {
        _team = team;
        var botBrainFactory = FindFirstObjectByType<BotBrainFactory>();
        _brain = botBrainFactory.CreateBrain(BotDifficulty.Easy, this); //TODO: get from scene loader
    }

    public void ForceCancelAiming() { }

    public void ForceCloseInventory() { }

    public void StartProvidingInputForAction(CharacterActionStateType action)
    {
        StartCoroutine(DelayThenThinkAndAct(action));
    }

    private IEnumerator DelayThenThinkAndAct(CharacterActionStateType action)
    {
        yield return new WaitForSeconds(0.3f);
        var context = _contextProvider.CreateContext(_team, action);
        yield return new WaitUntil(() => context.JumpGraph.IsReady);
        _brain.ThinkAndAct(context);
    }

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
