using UnityEngine;

public class OfflineTeamComposer : MonoBehaviour, ITeamComposer
{
    public bool AllTeamsInitialized { get; private set; }

    public void ComposeTeams(Team[] teams)
    {
        CreateInputSourcesAndInitializeTeams(teams);
    }

    private void CreateInputSourcesAndInitializeTeams(Team[] teams)
    {
        var botManagerFactory = FindFirstObjectByType<BotManagerFactory>();
        var settings = GameplaySceneSettingsStorage.Current;
        var players = settings.Players;
        foreach (var player in players)
        {
            var team = teams[player.TeamIndex];
            var inputSource = TeamInputSourceFactory.Create(player.Type == PlayerType.Human ? InputSourceType.OfflineHuman : InputSourceType.OfflineBot, team.transform);
            team.Initialize(inputSource, player.TeamIndex, player.Name);

            if (player.Type == PlayerType.Bot)
            {
                botManagerFactory.CreateBotForTeam(team, settings.BotDifficulty);
            }
        }
        AllTeamsInitialized = true;
    }

}
