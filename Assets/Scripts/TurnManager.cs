using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private List<Team> _possibleTeams;
    [SerializeField] private UISoundsDefinition _uiSounds;
    private List<Team> _teams;
    private List<TurnState> _turnStates;
    private int _turnStateIndex;
    private TurnState CurrentTurnState => _turnStates[_turnStateIndex];
    private bool IsGameOver => _teams.Count(t => t.IsTeamAlive) <= 1;

    public event Action<GameplaySceneSettings> GameStarted;
    public event Action<Team> GameEnded;

    void Awake()
    {
        if (_possibleTeams == null || _possibleTeams.Count == 0)
        {
            Debug.LogWarning("There are no teams.");
        }
    }

    private void Start()
    {
        _teams = new List<Team>(_possibleTeams.Take(SceneLoader.Instance.CurrentGameplaySceneSettings.NumTeams));
        for(int i = _teams.Count; i < _possibleTeams.Count; i++)
        {
            _possibleTeams[i].gameObject.SetActive(false);
        }

        foreach (var team in _teams)
        {
            team.TeamLost += OnAnyTeamLost;
        }

        var inputManager = FindFirstObjectByType<GameplayInputManager>();
        var trajectoryRenderer = FindFirstObjectByType<TrajectoryRenderer>();
        var itemPreviewRendererManager = FindFirstObjectByType<ItemPreviewRendererManager>();
        var dropManager = FindFirstObjectByType<DropManager>();
        var cameraController = FindFirstObjectByType<CameraController>();
        var uiManager = FindFirstObjectByType<GameplayUIManager>();
        var projectileManager = FindFirstObjectByType<ProjectilePool>();
        var characterActionManager = new CharacterActionManager(this, trajectoryRenderer, itemPreviewRendererManager, inputManager, cameraController, uiManager, projectileManager, _uiSounds);
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
        GameStarted += (_) => inputManager.OnGameStarted();
        GameEnded += (_) => inputManager.OnGameEnded();
        uiManager.CreateTeamHealthbars(_teams);
    }

    public void StartGame(GameplaySceneSettings gameplaySettings)
    {
        GameStarted?.Invoke(gameplaySettings);
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
