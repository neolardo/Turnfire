using System.Collections;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    private BotBrain _brain;
    private BotController _controller;
    private BotContextProvider _contextProvider;
    private Team _team;

    private BotContext _currentContext;

    public void Initialize(Team team, BotBrain brain, BotGameplayInput input)
    {
        _contextProvider = FindFirstObjectByType<BotContextProvider>();
        _team = team;
        _brain = brain;
        _brain.GoalDecided += OnGoalDecided;
        _controller = new BotController(input);
        input.InputRequested += OnInputRequested;
    }

    private void OnInputRequested(CharacterActionStateType action)
    {
        StartCoroutine(GetContextAndThink(action));
    }

    private IEnumerator GetContextAndThink(CharacterActionStateType action)
    {
        _currentContext = _contextProvider.CreateContext(_team, action);
        _currentContext.JumpGraph.SetJumpStrength(_currentContext.Self.JumpStrength);
        yield return new WaitUntil(() => _currentContext.JumpGraph.IsReady);
        _brain.BeginThinking(_currentContext);
    }

    private void OnGoalDecided(BotGoal goal)
    {
        _controller.Act(goal, _currentContext);
    }

}
