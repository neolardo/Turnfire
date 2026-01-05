using System;
using System.Collections.Generic;

public interface ITurnStateManager
{
    bool IsInitialized { get; }

    event Action<GameplaySceneSettings> GameStarted;
    event Action<Team> GameEnded;
    void Initialize(IEnumerable<Team> teams);
    void StartGame(GameplaySceneSettings gameplaySettings);
    void ForceEndGame();
}
