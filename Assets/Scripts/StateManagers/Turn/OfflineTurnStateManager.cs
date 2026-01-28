using System;
using System.Collections.Generic;
using UnityEngine;

public class OfflineTurnStateManager : MonoBehaviour, ITurnStateManager
{
    private TurnStateManagerLogic _logic;
    public bool IsInitialized { get; private set; }

    public event Action GameStarted;
    public event Action<Team> GameEnded;
    public event Action<Team> SelectedTeamChanged;

    private void Start()
    {
        GameServices.Register(this);
    }
    public void Initialize(IEnumerable<Team> teams)
    {
        var previewRenderer = FindFirstObjectByType<PreviewRendererManager>();
        var cameraController = FindFirstObjectByType<CameraController>();
        var uiManager = FindFirstObjectByType<GameplayUIManager>();
        var characterActionManager = new CharacterActionManager(previewRenderer, cameraController, uiManager);

        uiManager.CreateTeamHealthbars(teams);

        var characterActionsState = new DoCharacterActionsWithTeamTurnState(characterActionManager);
        var dropItemsState = new DropPackagesTurnState();
        var finishedState = new FinishedTurnState();

        _logic = new TurnStateManagerLogic(teams, characterActionsState, dropItemsState, finishedState);
        _logic.GameEnded += OnGameEnded;
        _logic.TurnStateEnded += OnTurnStateEnded;
        _logic.SelectedTeamChanged += OnSelectedTeamChanged;
        IsInitialized = true;
    }

    private void OnDestroy()
    {
        _logic.Dispose();
    }

    public void StartFirstTurn()
    {
        GameStarted?.Invoke();
        Debug.Log("Game started");
        _logic.Start();
    }

    private void OnTurnStateEnded()
    {
        if (!_logic.IsGameOver)
        {
            _logic.Resume();
        }
    }

    private void OnSelectedTeamChanged(Team team)
    {
        SelectedTeamChanged?.Invoke(team);
    }

    private void OnGameEnded(Team team)
    {
        GameEnded?.Invoke(team);
    }

    public void ForceEndGame()
    {
        _logic.ForceEndGame();
    }
    public Character GetCurrentCharacterInTeam(Team team) => _logic.GetCurrentCharacterInTeam(team);

}
