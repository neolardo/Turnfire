public readonly struct TurnStateContext
{
    public readonly Team CurrentTeam;
    public readonly CyclicConditionalEnumerator<Character> TeamCharacterEnumerator;

    public TurnStateContext(Team team, CyclicConditionalEnumerator<Character> teamCharacterEnumerator)
    {
        CurrentTeam = team;
        TeamCharacterEnumerator = teamCharacterEnumerator;
    }
}
