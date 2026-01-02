using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public static class RoomNetworkManager
{
    public static void LeaveRoom()
    {
        if (NetworkManager.Singleton != null)
        {
            Debug.Log($"{NetworkManager.Singleton.LocalClientId} left the room intentionally");
            NetworkManager.Singleton.Shutdown();
        }
    }

    public static async Task<(RoomNetworkConnectionResult, string)> TryHostRoomAsync(string hostPlayerName)
    {
        try
        { 
            await UnityServicesBootstrap.InitializeAsync();
            Allocation hostAlloc = await RelayService.Instance.CreateAllocationAsync(Constants.MultiplayerMaxPlayers);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAlloc.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayData = AllocationUtils.ToRelayServerData(hostAlloc, "dtls");
            transport.SetRelayServerData(relayData);

            var payload = JsonUtility.ToJson(new PlayerConnectionData
            {
                PlayerName = hostPlayerName
            });

            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;

            if (!NetworkManager.Singleton.StartHost())
            {
                return (RoomNetworkConnectionResult.NetworkError, null);
            }

            Debug.Log("Host started");
            return (RoomNetworkConnectionResult.Ok, joinCode.ToLower().Replace('l', 'L'));
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return (RoomNetworkConnectionResult.NetworkError, null);
        }
    }

    public static async Task<RoomNetworkConnectionResult> TryJoinRoomAsync(string joinCode, string clientPlayerName)
    {
        try
        {
            await UnityServicesBootstrap.InitializeAsync();
            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode.ToUpper());

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayData = AllocationUtils.ToRelayServerData(joinAlloc, "dtls");
            transport.SetRelayServerData(relayData);

            var payload = JsonUtility.ToJson(new PlayerConnectionData
            {
                PlayerName = clientPlayerName
            });

            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;

            if (!NetworkManager.Singleton.StartClient())
            {
                return RoomNetworkConnectionResult.NetworkError;
            }
            Debug.Log("Client started");

            return RoomNetworkConnectionResult.Ok;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogWarning($"Relay error: {ex.Reason} | {ex.Message}");

            if (ex.Reason == RelayExceptionReason.JoinCodeNotFound || ex.Reason == RelayExceptionReason.InvalidRequest)
            {
                return RoomNetworkConnectionResult.JoinCodeInvalid;
            }
            else
            {
                return RoomNetworkConnectionResult.NetworkError;
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return RoomNetworkConnectionResult.NetworkError;
        }
    }

}
