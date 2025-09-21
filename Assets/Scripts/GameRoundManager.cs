using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRoundManager : MonoBehaviour
{
    [HideInInspector] public Team CurrentTeam => _teams[_teamIndex];
    [HideInInspector] public TurnState CurrentTurnState => _turnStates[_turnStateIndex];
    [SerializeField] private List<Team> _teams;
    private List<TurnState> _turnStates;
    private int _teamIndex;
    private int _turnStateIndex;

    public event Action<Team> GameEnded;
    private bool IsGameOver => _teams.Count(t => t.IsTeamAlive) <= 1;

    void Awake()
    {
        if (_teams == null || _teams.Count == 0)
        {
            Debug.LogWarning("There are no teams.");
        }
        InitializeTurnStates();
    }

    private void InitializeTurnStates()
    {
        var inputManager = FindFirstObjectByType<InputManager>();
        var trajectoryRenderer = FindFirstObjectByType<TrajectoryRenderer>();
        _turnStates = new List<TurnState>
        {
            new ReadyToMoveTurnState(trajectoryRenderer, inputManager, this),
            new MovingTurnState(this),
            new ReadyToFireTurnState(trajectoryRenderer, inputManager, this),
            new FiringTurnState(this),
            new FinishedTurnState(this)
        };
        foreach (var turnState in _turnStates)
        {
            turnState.TurnStateEnded += OnTurnStateEnded;
        }
    }

    private void Start()
    {
        StartTurnState();
    }

    private void StartTurnState()
    {
        CurrentTurnState.StartState(CurrentTeam.CurrentCharacter);
    }

    private void ChangeTurnState()
    {
        _turnStateIndex = (_turnStateIndex + 1 ) % _turnStates.Count;
    }

    private void OnTurnStateEnded()
    {
        if(IsGameOver)
        {
            EndGame();
        }
        else
        {
            if (CurrentTurnState.State == TurnStateType.Finished)
            {
                OnTurnEnded();
            }
            ChangeTurnState();
            StartTurnState();
        } 
        
    }

    private void OnTurnEnded()
    {
        ChangeCurrentTeam();
        CurrentTeam.SelectNextCharacter();
    }

    private void ChangeCurrentTeam()
    {
        do
        {
            _teamIndex = (_teamIndex + 1) % _teams.Count;
        } while (!_teams[_teamIndex].IsTeamAlive);
    }


    private void EndGame()
    {
        if(_teams.Any(t => t.IsTeamAlive))
        {
            var winnerTeam = _teams.First(t => t.IsTeamAlive);
            GameEnded?.Invoke(winnerTeam);
        }
        else
        {
            GameEnded?.Invoke(null);
        }
    }

}
