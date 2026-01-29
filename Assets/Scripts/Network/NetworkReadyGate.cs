using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkReadyGate : NetworkBehaviour
{
    public bool AllClientsReady => _allReady.Value;
    public bool AllClientsAcknowledgedReady => _allAcknowledged.Value;
    public bool IsGateReady => !_allReady.Value;

    private NetworkVariable<bool> _allReady = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> _allAcknowledged = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    // Server-side tracking
    private readonly HashSet<ulong> _readyClients = new();
    private readonly HashSet<ulong> _ackClients = new();

    public IEnumerator MarkAndAckAndWaitUntilEveryClientIsReadyCoroutine()
    {
        yield return new WaitUntil(() => IsGateReady);
        MarkReady();
        yield return new WaitUntil(() => AllClientsReady);
        AcknowledgeReady();
        yield return new WaitUntil(() => AllClientsAcknowledgedReady);
    }

    public void MarkReady()
    {
        if (!IsClient) return;
        ReadyRpc();
    }

    public void AcknowledgeReady()
    {
        if (!IsClient) return;
        AckRpc();
    }

    public void Reset()
    {
        StopAllCoroutines();
        if (!IsServer || !IsSpawned)
        {
            return;
        }
        _allReady.Value = false;
        _allAcknowledged.Value = false;
        _readyClients.Clear();
        _ackClients.Clear();
    }


    [Rpc(SendTo.Server)]
    private void ReadyRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!_readyClients.Add(clientId))
            return;

        //Debug.Log($"{_readyClients.Count} / {NetworkManager.ConnectedClientsIds.Count} clients are ready");
        if (_readyClients.Count == NetworkManager.ConnectedClientsIds.Count)
        {
            _allAcknowledged.Value = false;
            _allReady.Value = true;
        }
    }

    [Rpc(SendTo.Server)]
    private void AckRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!_allReady.Value)
        {
            Debug.LogWarning($"invalid acking before everyone was ready for : {clientId}");
            return;
        }


        if (!_ackClients.Add(clientId))
            return;

        //Debug.Log($"{_ackClients.Count} / {NetworkManager.ConnectedClientsIds.Count} clients acknowledged ready");
        if (_ackClients.Count == NetworkManager.ConnectedClientsIds.Count)
        {
            AdvanceCycle();
        }
    }

    private void AdvanceCycle()
    {
        _allReady.Value = false;
        _allAcknowledged.Value = true;
        _readyClients.Clear();
        _ackClients.Clear();
    }

    public override void OnNetworkDespawn()
    {
        _readyClients.Clear();
        _ackClients.Clear();
        base.OnNetworkDespawn();
    }
}
