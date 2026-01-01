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

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Instance = this;
        }
        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.ConnectionApprovalCallback -= OnConnectionApproval;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var json = Encoding.UTF8.GetString(request.Payload);
        var data = JsonUtility.FromJson<PlayerConnectionData>(json);

        if (!TryRegisterPlayer(request.ClientNetworkId, data.PlayerName))
        {
            response.Approved = false;
            response.Reason = Constants.InvalidNameReasonValue;
            Debug.Log("Connection not approved");
            return;
        }

        Debug.Log("Connection approved");
        response.Approved = true;
        response.CreatePlayerObject = false; 
    }


    private void OnClientDisconnected(ulong clientId)
    {
        _playerNames.Remove(clientId);
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
        if (!IsNameValid(playerName))
            return false;

        _playerNames[clientId] = playerName;
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
