using System;
using System.Collections.Generic;
using UnityEngine;

public class OfflineTurnStateManager : MonoBehaviour, ITurnStateManager
{
    [SerializeField] private UISoundsDefinition _uiSounds;

    private TurnStateManagerLogic _logic;
    public bool IsInitialized { get; private set; }

    public event Action GameStarted;
    public event Action<Team> GameEnded;

    public void Initialize(IEnumerable<Team> teams)
    {
        var trajectoryRenderer = FindFirstObjectByType<PixelTrajectoryRenderer>();
        var itemPreviewRendererManager = FindFirstObjectByType<ItemPreviewRendererManager>();
        var cameraController = FindFirstObjectByType<CameraController>();
        var uiManager = FindFirstObjectByType<GameplayUIManager>();
        var laserRenderer = FindFirstObjectByType<PixelLaserRenderer>();
        var characterActionManager = new CharacterActionManager(trajectoryRenderer, itemPreviewRendererManager, cameraController, uiManager, laserRenderer, _uiSounds);

        uiManager.CreateTeamHealthbars(teams);

        var characterActionsState = new DoCharacterActionsWithTeamTurnState(characterActionManager);
        var dropItemsState = new DropPackagesTurnState();
        var finishedState = new FinishedTurnState();

        _logic = new TurnStateManagerLogic(teams, characterActionsState, dropItemsState, finishedState);
        _logic.GameEnded += (team) => GameEnded?.Invoke(team);
        _logic.TurnStateEnded += OnTurnStateEnded;
        IsInitialized = true;
    }

    private void OnDestroy()
    {
        _logic.Dispose();
    }

    public void StartGame()
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

    public void ForceEndGame()
    {
        _logic.ForceEndGame();
    }
}
