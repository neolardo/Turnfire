using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private List<Team> _teams;
    private List<TurnState> _turnStates;
    private int _turnStateIndex;
    private TurnState CurrentTurnState => _turnStates[_turnStateIndex];
    private bool IsGameOver => _teams.Count(t => t.IsTeamAlive) <= 1;

    public event Action<Team> GameEnded;

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
        var itemPreviewRendererManager = FindFirstObjectByType<ItemPreviewRendererManager>();
        var dropZone = FindFirstObjectByType<DropZone>();
        var cameraController = FindFirstObjectByType<CameraController>();
        var uiManager = FindFirstObjectByType<UIManager>();
        var projectileManager = FindFirstObjectByType<ProjectileManager>();
        var characterActionManager = new CharacterActionManager(this, trajectoryRenderer, itemPreviewRendererManager, inputManager, cameraController, uiManager, projectileManager);
        _turnStates = new List<TurnState>
        {
            new DoOneCharacterActionPerTeamTurnState(this, characterActionManager, _teams),
            new DropItemsAndEffectsTurnState(this, dropZone),
            new FinishedTurnState(this),
        };

        foreach (var turnState in _turnStates)
        {
            turnState.StateEnded += OnTurnStateEnded;
        }

        foreach(var team in _teams)
        {
            team.TeamLost += OnAnyTeamLost;
        }
        GameEnded+= (_) => inputManager.OnGameEnded(); //TODO?
    }

    private void Start()
    {
        StartTurnState();
    }

    private void StartTurnState()
    {
        CurrentTurnState.StartState();
    }

    private void ChangeTurnState()
    {
        _turnStateIndex = (_turnStateIndex + 1 ) % _turnStates.Count;
    }

    private void OnTurnStateEnded()
    {
        if (!IsGameOver)
        {
            ChangeTurnState();
            StartTurnState();
        }
    }

    private void OnAnyTeamLost()
    {
        if(IsGameOver)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        if (_teams.Any(t => t.IsTeamAlive))
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
