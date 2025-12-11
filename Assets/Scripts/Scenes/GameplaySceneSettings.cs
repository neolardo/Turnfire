using System;
using System.Collections.Generic;

[Serializable]
public class GameplaySceneSettings
{
    public string SceneName;
    public int NumTeams;
    public int NumBots;
    public BotDifficulty BotDifficulty;
    public bool UseTimer;
    public List<string> PlayerNames;
}
