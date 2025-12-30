using System;
using System.Collections.Generic;

[Serializable]
public class GameplaySceneSettings
{
    public MapDefinition Map;
    public List<Player> Players;
    public BotDifficulty BotDifficulty;
    public bool UseTimer;
    public bool IsOnlineGame;
}
