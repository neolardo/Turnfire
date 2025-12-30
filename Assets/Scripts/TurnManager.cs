using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private UISoundsDefinition _uiSounds;
    private List<Team> _teams;
    private List<TurnState> _turnStates;
    private int _turnStateIndex;
    private TurnState CurrentTurnState => _turnStates[_turnStateIndex];
    private bool IsGameOver => _teams.Count(t => t.IsTeamAlive) <= 1 || _isGameOverForced;
    private bool _isGameOverForced;
    public bool IsInitialized { get; private set; }

    public event Action<GameplaySceneSettings> GameStarted;
    public event Action<Team> GameEnded;

    public void Initialize(IEnumerable<Team> teams)
    {
        _teams = new List<Team>(teams);
       
        foreach (var team in _teams)
        {
            team.TeamLost += OnAnyTeamLost;
        }

        var localInput = FindFirstObjectByType<LocalGameplayInput>();
        var trajectoryRenderer = FindFirstObjectByType<PixelTrajectoryRenderer>();
        var itemPreviewRendererManager = FindFirstObjectByType<ItemPreviewRendererManager>();
        var dropManager = FindFirstObjectByType<DropManager>();
        var cameraController = FindFirstObjectByType<CameraController>();
        var uiManager = FindFirstObjectByType<GameplayUIManager>();
        var projectilePool = FindFirstObjectByType<ProjectilePool>();
        var laserRenderer = FindFirstObjectByType<PixelLaserRenderer>();
        var characterActionManager = new CharacterActionManager(this, trajectoryRenderer, itemPreviewRendererManager, cameraController, uiManager, laserRenderer, projectilePool, _uiSounds);
        _turnStates = new List<TurnState>
        {
            new AlternativelyDoCharacterActionsForAllTeamsTurnState(this, characterActionManager, _teams),
            new DropPackagesTurnState(this, dropManager),
            new FinishedTurnState(this),
        };
        foreach (var turnState in _turnStates)
        {
            turnState.StateEnded += OnTurnStateEnded;
        }
        GameStarted += (_) => localInput.OnGameStarted();
        GameEnded += (_) => localInput.OnGameEnded();
        uiManager.CreateTeamHealthbars(_teams);
        IsInitialized = true;
    }

    private void OnDestroy()
    {
        foreach(var turnState in _turnStates)
        {
            turnState.OnDestroy();
        }
    }

    public void StartGame(GameplaySceneSettings gameplaySettings)
    {
        GameStarted?.Invoke(gameplaySettings);
        Debug.Log("Game started");
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

    public void ForceEndGame()
    {
        _isGameOverForced = true;
        EndGame();
    }

    private void EndGame()
    {
        CurrentTurnState.ForceEndState();
        bool isTie = _teams.Count(team => team.IsTeamAlive) != 1;
        if(isTie)
        {
            GameEnded?.Invoke(null);
        }
        else
        {
            GameEnded?.Invoke(_teams.First(t => t.IsTeamAlive));
        }
    }

}
