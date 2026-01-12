public interface ITeamComposer
{
    bool AllTeamsInitialized { get; }
    void ComposeTeams(Team[] teams);
}
