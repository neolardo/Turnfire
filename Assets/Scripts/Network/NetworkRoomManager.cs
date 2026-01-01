using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public static class NetworkRoomManager
{
    public const int MaxPlayerNameLength = 10;

    public static void LeaveRoom()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    public static async Task<(NetworkRoomResult, string)> TryHostRoomAsync(string hostPlayerName)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(hostPlayerName) || hostPlayerName.Length > MaxPlayerNameLength)
            {
                return (NetworkRoomResult.PlayerNameInvalid, null);
            }
            //TODO: use player name

            await UnityServicesBootstrap.InitializeAsync();
            Allocation hostAlloc = await RelayService.Instance.CreateAllocationAsync(Constants.MultiplayerMaxPlayers);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAlloc.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayData = AllocationUtils.ToRelayServerData(hostAlloc, "dtls");
            transport.SetRelayServerData(relayData);

            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started");
            return (NetworkRoomResult.Ok, joinCode);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return (NetworkRoomResult.NetworkError, null);
        }
    }

    public static async Task<NetworkRoomResult> TryJoinRoomAsync(string joinCode, string clientPlayerName)
    {
        try
        {
            //TODO: check name
            await UnityServicesBootstrap.InitializeAsync();
            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayData = AllocationUtils.ToRelayServerData(joinAlloc, "dtls");
            transport.SetRelayServerData(relayData);

            NetworkManager.Singleton.StartClient();
            return NetworkRoomResult.Ok;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Relay error: {ex.Reason} | {ex.Message}");

            if (ex.Reason == RelayExceptionReason.JoinCodeNotFound)
            {
                return NetworkRoomResult.JoinCodeInvalid;
            }
            else
            {
                return NetworkRoomResult.NetworkError;
            }
        }
        catch (Exception ex)
        {
            return NetworkRoomResult.NetworkError;
        }
    }
}
