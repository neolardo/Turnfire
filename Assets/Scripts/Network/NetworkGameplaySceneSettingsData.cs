using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

public struct NetworkGameplaySceneSettingsData : INetworkSerializable
{
    public bool IsValid;
    public FixedString32Bytes MapName;
    public BotDifficulty BotDifficulty;
    public bool UseTimer;
    public bool IsOnlineGame;

    // NGO requires fixed-size collections
    public int PlayerCount;
    public NetworkPlayerData Player0;
    public NetworkPlayerData Player1;
    public NetworkPlayerData Player2;
    public NetworkPlayerData Player3;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref MapName);
        serializer.SerializeValue(ref BotDifficulty);
        serializer.SerializeValue(ref UseTimer);
        serializer.SerializeValue(ref IsOnlineGame);
        serializer.SerializeValue(ref PlayerCount);

        if (PlayerCount > 0) serializer.SerializeValue(ref Player0);
        if (PlayerCount > 1) serializer.SerializeValue(ref Player1);
        if (PlayerCount > 2) serializer.SerializeValue(ref Player2);
        if (PlayerCount > 3) serializer.SerializeValue(ref Player3);
    }

    public static NetworkGameplaySceneSettingsData ToNetworkData(GameplaySceneSettings settings)
    {
        var data = new NetworkGameplaySceneSettingsData
        {
            IsValid = true,
            MapName = settings.Map.SceneName,
            BotDifficulty = settings.BotDifficulty,
            UseTimer = settings.UseTimer,
            IsOnlineGame = settings.IsOnlineGame,
            PlayerCount = settings.Players.Count
        };

        for (int i = 0; i < settings.Players.Count; i++)
        {
            var p = settings.Players[i];
            var np = new NetworkPlayerData
            {
                ClientId = p.ClientId,
                TeamId = p.TeamId,
                Name = p.Name,
                Type = p.Type
            };

            switch (i)
            {
                case 0: data.Player0 = np; break;
                case 1: data.Player1 = np; break;
                case 2: data.Player2 = np; break;
                case 3: data.Player3 = np; break;
            }
        }

        return data;
    }

    public GameplaySceneSettings ToSceneSettings(MapLocator mapLocator)
    {
        var players = new List<Player>(this.PlayerCount);

        if (this.PlayerCount > 0) players.Add(this.Player0.ToPlayer());
        if (this.PlayerCount > 1) players.Add(this.Player1.ToPlayer());
        if (this.PlayerCount > 2) players.Add(this.Player2.ToPlayer());
        if (this.PlayerCount > 3) players.Add(this.Player3.ToPlayer());

        return new GameplaySceneSettings()
        {
            Map = mapLocator.GetMap(this.MapName.ToString()),
            Players = players,
            BotDifficulty = this.BotDifficulty,
            UseTimer = this.UseTimer,
            IsOnlineGame = this.IsOnlineGame,
        };
    }

  


}
