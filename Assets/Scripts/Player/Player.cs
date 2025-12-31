public readonly struct Player
{
    public readonly int TeamId;
    public readonly string Name;
    public readonly PlayerType Type;

    public Player(int teamId, string name, PlayerType type)
    {
        TeamId = teamId;
        Name = name;
        Type = type;
    }

}
