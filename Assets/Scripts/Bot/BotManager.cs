using System.Collections;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public BotController Controller { get; private set; }

    private BotBrain _brain;
    private BotContextProvider _contextProvider;
    private Team _team;
 
    private BotContext _currentContext;

    private bool _isDestroyed;

    public void Initialize(Team team, BotBrain brain)
    { 
        _contextProvider = FindFirstObjectByType<BotContextProvider>();
        _team = team;
        _brain = brain;
        _brain.GoalDecided += OnGoalDecided;
        Controller = new BotController();
    }

    private void OnDestroy()
    {
        _brain.GoalDecided -= OnGoalDecided;
        StopAllCoroutines();
        _isDestroyed = true;
    }

    public void BeginThinkingAndActing(CharacterActionStateType action)
    {
        StartCoroutine(GetContextAndThink(action));
    }

    private IEnumerator GetContextAndThink(CharacterActionStateType action)
    {
        _currentContext = _contextProvider.CreateContext(_team, action);
        _currentContext.JumpGraph.SetJumpStrength(_currentContext.Self.JumpStrength);
        yield return new WaitUntil(() => _currentContext.JumpGraph.IsReady);
        if(_isDestroyed)
        { 
            yield break;
        }
        _brain.BeginThinking(_currentContext);
    }

    private void OnGoalDecided(BotGoal goal)
    {
        Controller.Act(goal, _currentContext);
    }

}
