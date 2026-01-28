using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnStateManagerLogic
{
    private List<Team> _teams;
    private CyclicConditionalEnumerator<Team> _teamEnumerator;
    public Team CurrentTeam => _teamEnumerator.Current;

    private Dictionary<Team, CyclicConditionalEnumerator<Character>> _characterEnumeratorsPerTeamDict;

    private Dictionary<TurnStateType, TurnState> _turnStateDict;
    private TurnStateType _currentTurnStateType;
    private TurnState CurrentTurnState => _turnStateDict[_currentTurnStateType];

    private int _numCharactersActedPerTeamThisRound;
    private int _maxCharactersToActPerTeam;
    public bool IsGameOver => _teams.Count(t => t.IsTeamAlive) <= 1 || _isGameOverForced;
    private bool _isGameOverForced;

    public event Action<Team> GameEnded;
    public event Action TurnStateEnded;
    public event Action<Team> SelectedTeamChanged;

    public TurnStateManagerLogic(IEnumerable<Team> teams, DoCharacterActionsWithTeamTurnState characterActionTurnState, DropPackagesTurnState dropPackagesTurnState, FinishedTurnState finishedTurnState)
    {
        _teams = new List<Team>(teams);
        _teamEnumerator = new CyclicConditionalEnumerator<Team>(_teams);
        _teamEnumerator.Reset();
        _characterEnumeratorsPerTeamDict = new Dictionary<Team, CyclicConditionalEnumerator<Character>>();
        foreach (var team in _teams)
        {
            team.TeamHealthChanged += OnAnyTeamHealthChanged;
            team.TeamLost += OnAnyTeamLost;
            _characterEnumeratorsPerTeamDict[team] = new CyclicConditionalEnumerator<Character>(team.GetAllCharacters());
            _characterEnumeratorsPerTeamDict[team].Reset();
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
        SelectNextTeam();
        StartTurnState();
    }

    public void Resume()
    {
        Debug.Log("Turn resumed");
        UpdateMaxCharactersToActPerRound();
        AdvanceTurnState();
        StartTurnState();
    }

    private void AdvanceTurnState()
    {
        var lastStateType = _currentTurnStateType;
        if (lastStateType == TurnStateType.Finished)
        {
            SelectNextTeam();
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
                SelectNextTeam();
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

    private void SelectNextTeam()
    {
        _teamEnumerator.MoveNext(out var _);
        SelectedTeamChanged?.Invoke(CurrentTeam);
    }

    private void DeselectAllTeams()
    {
        SelectedTeamChanged?.Invoke(null);
    }

    private void StartTurnState()
    {
        CurrentTurnState.StartState(new TurnStateContext(CurrentTeam, _characterEnumeratorsPerTeamDict[CurrentTeam]));
    }


    private void OnTurnStateEnded()
    {
        if (!IsGameOver)
        {
            DeselectAllTeams();
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

    public Character GetCurrentCharacterInTeam(Team team) => _characterEnumeratorsPerTeamDict[team].Current;

    public Team GetTeamById(int teamId)
    {
        if(teamId == Constants.InvalidId)
        {
            return null;
        }
        return _teams.First(t => t.TeamId == teamId);
    }

}
