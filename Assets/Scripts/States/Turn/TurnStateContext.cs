public readonly struct TurnStateContext
{
    public readonly Team CurrentTeam;

    public TurnStateContext(Team team)
    {
        CurrentTeam = team;
    }
}
