using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnStateManagerLogic
{
    private List<Team> _teams;
    private CyclicConditionalEnumerator<Team> _teamEnumerator;
    private Team CurrentTeam => _teamEnumerator.Current;

    private Dictionary<TurnStateType, TurnState> _turnStateDict;
    private TurnStateType _currentTurnStateType;
    private TurnState CurrentTurnState => _turnStateDict[_currentTurnStateType];

    private int _numCharactersActedPerTeamThisRound;
    private int _maxCharactersToActPerTeam;
    public bool IsGameOver => _teams.Count(t => t.IsTeamAlive) <= 1 || _isGameOverForced;
    private bool _isGameOverForced;

    public event Action<Team> GameEnded;
    public event Action TurnStateEnded;

    public TurnStateManagerLogic(IEnumerable<Team> teams, DoCharacterActionsWithTeamTurnState characterActionTurnState, DropPackagesTurnState dropPackagesTurnState, FinishedTurnState finishedTurnState)
    {
        _teams = new List<Team>(teams);
        _teamEnumerator = new CyclicConditionalEnumerator<Team>(_teams);
        _teamEnumerator.Reset();
        _teamEnumerator.MoveNext(out var _);
        foreach (var team in _teams)
        {
            team.TeamHealthChanged += OnAnyTeamHealthChanged;
            team.TeamLost += OnAnyTeamLost;
        }
        _turnStateDict = new Dictionary<TurnStateType, TurnState>
        {
            { TurnStateType.DoCharacterActions, characterActionTurnState },
            { TurnStateType.DropItemsAndEffects, dropPackagesTurnState },
            { TurnStateType.Finished, finishedTurnState}
        };

        foreach (var turnState in _turnStateDict.Values)
        {
            turnState.StateEnded += OnTurnStateEnded;
        }
        UpdateMaxCharactersToActPerRound();
    }

    public void Dispose()
    {
        foreach (var turnState in _turnStateDict.Values)
        {
            turnState.OnDestroy();
        }

        foreach (var team in _teams)
        {
            team.TeamHealthChanged -= OnAnyTeamHealthChanged;
            team.TeamLost -= OnAnyTeamLost;
        }
    }

    private void OnAnyTeamHealthChanged(float teamHealth)
    {
        UpdateMaxCharactersToActPerRound();
    }

    private void UpdateMaxCharactersToActPerRound()
    {
        _maxCharactersToActPerTeam = _teams.Where(t => t.IsTeamAlive).Select(t => t.NumAliveCharacters).DefaultIfEmpty(1).Min();
    }

    public void Start()
    {
        StartTurnState();
    }

    public void Resume()
    {
        UpdateMaxCharactersToActPerRound();
        AdvanceTurnState();
        StartTurnState();
    }

    private void AdvanceTurnState()
    {
        var lastStateType = _currentTurnStateType;
        if (lastStateType == TurnStateType.Finished)
        {
            _teamEnumerator.MoveNext(out var _);
            _currentTurnStateType = TurnStateType.DoCharacterActions;
            _numCharactersActedPerTeamThisRound = 0;
        }
        else if (lastStateType == TurnStateType.DoCharacterActions)
        {
            var nextTeam = _teamEnumerator.PeekNext(out var teamEnumerationRestarted);
            if (teamEnumerationRestarted)
            {
                _numCharactersActedPerTeamThisRound++;
            }

            if (_numCharactersActedPerTeamThisRound >= _maxCharactersToActPerTeam)
            {
                _currentTurnStateType = TurnStateType.DropItemsAndEffects;
            }
            else
            {
                _teamEnumerator.MoveNext(out var _);
            }
        }
        else if (lastStateType == TurnStateType.DropItemsAndEffects)
        {
            _currentTurnStateType = TurnStateType.Finished;
        }
        else
        {
            Debug.LogError($"Invalid turn state when changing state: {lastStateType}");
        }
    }


    private void StartTurnState()
    {
        CurrentTurnState.StartState(new TurnStateContext(CurrentTeam));
    }


    private void OnTurnStateEnded()
    {
        if (!IsGameOver)
        {
            TurnStateEnded?.Invoke();
        }
    }

    private void OnAnyTeamLost()
    {
        if (IsGameOver)
        {
            EndGame();
        }
    }

    public void ForceEndGame()
    {
        _isGameOverForced = true;
        EndGame();
    }

    private void EndGame()
    {
        CurrentTurnState.ForceEndState();
        bool isTie = _teams.Count(team => team.IsTeamAlive) != 1;
        if (isTie)
        {
            GameEnded?.Invoke(null);
        }
        else
        {
            GameEnded?.Invoke(_teams.First(t => t.IsTeamAlive));
        }
    }


}
