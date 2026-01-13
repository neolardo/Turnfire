using System.Linq;
using Unity.Netcode;
using UnityEngine;

public static class TeamComposer
{
    public static void Compose(Team team, Player player, BotManagerFactory botManagerFactory)
    {
        var settings = GameplaySceneSettingsStorage.Current;
        bool isOnline = settings.IsOnlineGame;
        var botDifficulty = settings.BotDifficulty;
        ITeamInputSource inputSource;
        if(isOnline)
        {
            inputSource = player.Type == PlayerType.Human ? team.GetComponent<OnlineHumanTeamInputSource>() : team.GetComponent<OnlineBotTeamInputSource>();
            SpawnTeamNetworkObject(team);
        }
        else
        {
            inputSource = player.Type == PlayerType.Human ? team.GetComponent<OfflineHumanTeamInputSource>() : team.GetComponent<OfflineBotTeamInputSource>();
        }
        (inputSource as MonoBehaviour).enabled = true;
        if (player.Type == PlayerType.Bot)
        {
            botManagerFactory.CreateBotForTeam(team, botDifficulty);
        }
        team.Initialize(inputSource, player.TeamIndex, player.Name);
    }

    private static void SpawnTeamNetworkObject(Team team)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        var netObj = team.GetComponent<NetworkObject>();
        if (netObj.IsSpawned)
            return;

        ulong ownerClientId =
            GameplaySceneSettingsStorage.Current.Players
                .First(p => p.TeamIndex == team.TeamId)
                .ClientId;

        netObj.SpawnAsPlayerObject(ownerClientId, true);
    }

}
