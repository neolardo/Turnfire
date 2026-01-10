using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineTurnStateManager : NetworkBehaviour, ITurnStateManager
{
    [SerializeField] private UISoundsDefinition _uiSounds;

    private TurnStateManagerLogic _logic;

    private NetworkVariable<bool> _isInitialized = new NetworkVariable<bool>();
    public bool IsInitialized => _isInitialized.Value;

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
        _isInitialized.Value = true;
    }

    public override void OnDestroy()
    {
        _logic.Dispose();
    }

    public void StartGame()
    { 
        if (!IsServer)
        {
            return;
        }
        StartGameClientRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void StartGameClientRpc()
    {
        GameStarted?.Invoke();
        Debug.Log("Game started");
        _logic.Start();
    }

    private void OnTurnStateEnded()
    {
        if(!IsServer)
        {
            return;
        }

        if (!_logic.IsGameOver)
        {
            ResumeGameCientRpc();
        }
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void ResumeGameCientRpc()
    {
        _logic.Resume();
    }

    public void ForceEndGame()
    {
        if(!IsServer)
        {
            return;
        }

        _logic.ForceEndGame();
    }
}
