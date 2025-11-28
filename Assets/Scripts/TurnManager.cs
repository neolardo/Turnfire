using System;
using System.Collections;
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
    private bool IsGameOver => _teams.Count(t => t.IsTeamAlive) <= 1;

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
        var trajectoryRenderer = FindFirstObjectByType<TrajectoryRenderer>();
        var itemPreviewRendererManager = FindFirstObjectByType<ItemPreviewRendererManager>();
        var dropManager = FindFirstObjectByType<DropManager>();
        var cameraController = FindFirstObjectByType<CameraController>();
        var uiManager = FindFirstObjectByType<GameplayUIManager>();
        var projectileManager = FindFirstObjectByType<ProjectilePool>();
        var characterActionManager = new CharacterActionManager(this, trajectoryRenderer, itemPreviewRendererManager, cameraController, uiManager, projectileManager, _uiSounds);
        _turnStates = new List<TurnState>
        {
            new AlternativelyDoCharacterActionsForAllTeamsTurnState(this, characterActionManager, _teams),
            new DropItemsAndEffectsTurnState(this, dropManager),
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

    private void EndGame()
    {
        if (_teams.Any(t => t.IsTeamAlive))
        {
            var winnerTeam = _teams.First(t => t.IsTeamAlive);
            BotEvaluationStatistics.GetData(winnerTeam).HasWon = true;
            foreach( var team in _teams)
            {
                BotEvaluationStatistics.GetData(team).RemainingNormalizedTeamHealth = team.NormalizedTeamHealth;
            }
            GameEnded?.Invoke(winnerTeam);
            StartCoroutine(SaveAndTryRestartSimulation());
        }
        else
        {
            GameEnded?.Invoke(null);
        }
    }

    private IEnumerator SaveAndTryRestartSimulation()
    {
        yield return new WaitForSeconds(0.5f);
        BotEvaluationStatistics.Save();
        BotEvaluationStatistics.TryToRestartSimulation();
    }

}
