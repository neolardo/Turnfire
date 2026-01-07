public readonly struct Player
{
    public readonly ulong ClientId;
    public readonly int TeamIndex;
    public readonly string Name;
    public readonly PlayerType Type;

    public Player(ulong clientId, int teamIndex, string name, PlayerType type) // online
    {
        ClientId = clientId;
        TeamIndex = teamIndex;
        Name = name;
        Type = type;
    }

    public Player(int teamIndex, string name, PlayerType type) // offline
    {
        ClientId = 0;
        TeamIndex = teamIndex;
        Name = name;
        Type = type;
    }

}
