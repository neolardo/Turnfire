using System;

[Serializable]
public class GameplaySceneSettings
{
    public string SceneName;
    public int NumTeams;
    public int NumBots;
    public BotDifficulty BotDifficulty;
    public bool UseTimer;
}
