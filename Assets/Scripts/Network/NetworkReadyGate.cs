using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkReadyGate : NetworkBehaviour
{
    public bool AllClientsReady => _allReady.Value;
    public bool AllClientsAcknowledgedReady => _allAcknowledged.Value;

    private NetworkVariable<bool> _allReady = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> _allAcknowledged = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    private NetworkVariable<int> _cycle = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    // Server-side tracking
    private readonly HashSet<ulong> _readyClients = new();
    private readonly HashSet<ulong> _ackClients = new();

    public void MarkReady()
    {
        if (!IsClient) return;
        ReadyRpc(_cycle.Value);
    }

    public void AcknowledgeReady()
    {
        if (!IsClient) return;
        AckRpc(_cycle.Value);
    }


    [Rpc(SendTo.Server)]
    private void ReadyRpc(int cycle, RpcParams rpcParams = default)
    {
        if (cycle != _cycle.Value)
            return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!_readyClients.Add(clientId))
            return;

        Debug.Log($"{_readyClients.Count} / {NetworkManager.ConnectedClientsIds.Count} clients are ready for phase {_cycle.Value}");
        if (_readyClients.Count == NetworkManager.ConnectedClientsIds.Count)
        {
            _allAcknowledged.Value = false;
            _allReady.Value = true;
        }
    }

    [Rpc(SendTo.Server)]
    private void AckRpc(int cycle, RpcParams rpcParams = default)
    {
        if (cycle != _cycle.Value || !_allReady.Value)
            return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!_ackClients.Add(clientId))
            return;

        Debug.Log($"{_ackClients.Count} / {NetworkManager.ConnectedClientsIds.Count} clients acknowledged ready for phase {_cycle.Value}");
        if (_ackClients.Count == NetworkManager.ConnectedClientsIds.Count)
        {
            AdvanceCycle();
        }
    }

    private void AdvanceCycle()
    {
        _cycle.Value++;
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
