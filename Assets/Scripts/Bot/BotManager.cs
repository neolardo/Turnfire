using System.Collections;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    private BotBrain _brain;
    private BotController _controller;
    private BotGameplayInput _input;
    private BotContextProvider _contextProvider;
    private Team _team;
 
    private BotContext _currentContext;

    private bool _isDestroyed;

    public void Initialize(Team team, BotBrain brain, BotGameplayInput input)
    {
        _contextProvider = FindFirstObjectByType<BotContextProvider>();
        _team = team;
        _brain = brain;
        _brain.GoalDecided += OnGoalDecided;
        _input = input;
        _controller = new BotController(_input);
        _input.InputRequested += OnInputRequested;
    }

    private void OnDestroy()
    {
        _brain.Dispose();
        _brain.GoalDecided -= OnGoalDecided;
        _input.InputRequested -= OnInputRequested;
        StopAllCoroutines();
        _isDestroyed = true;
    }

    private void OnInputRequested(CharacterActionStateType action)
    {
        StartCoroutine(GetContextAndThink(action));
    }

    private IEnumerator GetContextAndThink(CharacterActionStateType action)
    {
        _currentContext = _contextProvider.CreateContext(_team, action);
        yield return new WaitUntil(() => _currentContext.JumpGraph.IsReady);
        if(_isDestroyed)
        { 
            yield break;
        }
        _brain.BeginThinking(_currentContext);
    }

    private void OnGoalDecided(BotGoal goal)
    {
        _controller.Act(goal, _currentContext);
    }

}
