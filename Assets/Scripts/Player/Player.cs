public readonly struct Player
{
    public readonly ulong ClientId;
    public readonly int TeamId;
    public readonly string Name;
    public readonly PlayerType Type;

    public Player(ulong clientId, int teamId, string name, PlayerType type) // online
    {
        ClientId = clientId;
        TeamId = teamId;
        Name = name;
        Type = type;
    }

    public Player(int teamId, string name, PlayerType type) // offline
    {
        ClientId = 0;
        TeamId = teamId;
        Name = name;
        Type = type;
    }

}
