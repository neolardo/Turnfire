using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkReadyGate))]
public class OnlineTurnStateManager : NetworkBehaviour, ITurnStateManager
{
    private TurnStateManagerLogic _logic;

    private NetworkVariable<bool> _isInitialized = new NetworkVariable<bool>();
    public bool IsInitialized => _isInitialized.Value;

    public event Action GameStarted;
    public event Action<Team> GameEnded;
    public event Action<Team> SelectedTeamChanged;

    private NetworkReadyGate _readyGate;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
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

        _readyGate = GetComponent<NetworkReadyGate>();

        if (IsServer)
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
        StartCoroutine(WaitUntilEveryClientIsReadyThenResumeGame());
    }

    private IEnumerator WaitUntilEveryClientIsReadyThenResumeGame()
    {
        yield return _readyGate.MarkAndAckAndWaitUntilEveryClientIsReadyCoroutine();
        if (!IsServer)
        {
            yield break;
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
