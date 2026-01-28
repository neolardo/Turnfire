using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class RoomNetworkSession : NetworkBehaviour
{
    public static RoomNetworkSession Instance { get; private set; }

    private const int MaxNameLength = 10;

    private readonly Dictionary<ulong, string> _playerNames = new();

    public int NumPlayers => _playerNames.Count;

    public event Action RegisteredPlayersChanged;

    public override void OnNetworkSpawn()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (IsServer)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        Debug.Log($"{nameof(RoomNetworkSession)} spawned");
    }

    public override void OnDestroy()
    {
        Debug.Log($"{nameof(RoomNetworkSession)} destroyed");
        base.OnDestroy();
        if(NetworkManager.Singleton == null || !IsServer)
        {
            return;
        }
        NetworkManager.Singleton.ConnectionApprovalCallback -= OnConnectionApproval;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var json = Encoding.UTF8.GetString(request.Payload);
        var data = JsonUtility.FromJson<PlayerConnectionData>(json);

        int numPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count+1;

        if (numPlayers >= Constants.MultiplayerMaxPlayers)
        {
            response.Approved = false;
            response.Reason = Constants.RoomIsFullReasonValue;
            Debug.Log("Connection not approved: room is full");
            return;
        }


        if (!TryRegisterPlayer(request.ClientNetworkId, data.PlayerName))
        {
            response.Approved = false;
            response.Reason = Constants.InvalidNameReasonValue;
            Debug.Log("Connection not approved: invalid name");
            return;
        }

        Debug.Log("Connection approved");
        response.Approved = true;
        response.CreatePlayerObject = false; 
    }


    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"client {clientId} removed from room session");
        _playerNames.Remove(clientId);
        RegisteredPlayersChanged?.Invoke();
    }

    private bool IsNameValid(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        if (name.Length > MaxNameLength)
            return false;

        if(_playerNames.ContainsValue(name))
            return false;

        return true;
    }

    public bool TryRegisterPlayer(ulong clientId, string playerName)
    {
        if(!IsServer)
        {
            return false;
        }

        if (!IsNameValid(playerName))
            return false;

        _playerNames[clientId] = playerName;
        RegisteredPlayersChanged?.Invoke();
        Debug.Log($"client {clientId} registered as {playerName} to the room session");
        return true;
    }

    public string GetPlayerName(ulong clientId)
    {
        return _playerNames.TryGetValue(clientId, out var name)
            ? name
            : string.Empty;
    }

    public IEnumerable<ulong> GetAllPlayerClientIds()
    {
        return _playerNames.Keys;
    }

}
