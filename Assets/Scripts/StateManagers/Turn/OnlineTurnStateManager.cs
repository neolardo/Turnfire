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
    public event Action<Team> SelectedTeamChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameServices.Register(this);
    }

    public void Initialize(IEnumerable<Team> teams)
    {
        var trajectoryRenderer = FindFirstObjectByType<PixelTrajectoryRenderer>();
        var itemPreviewRendererManager = FindFirstObjectByType<ItemPreviewRendererManager>();
        var cameraController = FindFirstObjectByType<CameraController>();
        var uiManager = FindFirstObjectByType<GameplayUIManager>();
        var characterActionManager = new CharacterActionManager(trajectoryRenderer, itemPreviewRendererManager, cameraController, uiManager, _uiSounds);

        uiManager.CreateTeamHealthbars(teams);

        var characterActionsState = new DoCharacterActionsWithTeamTurnState(characterActionManager);
        var dropItemsState = new DropPackagesTurnState();
        var finishedState = new FinishedTurnState();

        _logic = new TurnStateManagerLogic(teams, characterActionsState, dropItemsState, finishedState);
        _logic.GameEnded += OnGameEnded;
        _logic.TurnStateEnded += OnTurnStateEnded;
        _logic.SelectedTeamChanged += OnSelectedTeamChanged;
        if(IsServer)
        {
            _isInitialized.Value = true;
        }
    }

    private void OnGameEnded(Team team)
    {
        if(!IsServer)
        {
            return;
        }
        InvokeGameEndedClientRpc(team == null ? Constants.InvalidId : team.TeamId);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeGameEndedClientRpc(int winnerTeamid)
    {
        GameEnded?.Invoke(_logic.GetTeamById(winnerTeamid));
    }

    public override void OnDestroy()
    {
        _logic.Dispose();
    }

    public void StartFirstTurn()
    { 
        if (!IsServer)
        {
            return;
        }
        StartFirstTurnClientRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void StartFirstTurnClientRpc()
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

    private void OnSelectedTeamChanged(Team team)
    {
        if (!IsServer)
        {
            return;
        }
        InvokeSelectedTeamChangedClientRpc(team == null ? Constants.InvalidId : team.TeamId);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeSelectedTeamChangedClientRpc(int teamId)
    {
        SelectedTeamChanged?.Invoke(_logic.GetTeamById(teamId));
    }

    public void ForceEndGame()
    {
        if(!IsServer)
        {
            return;
        }

        _logic.ForceEndGame();
    }

    public Character GetCurrentCharacterInTeam(Team team) => _logic.GetCurrentCharacterInTeam(team);

}
