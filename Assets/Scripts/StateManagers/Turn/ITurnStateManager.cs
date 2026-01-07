using System;
using System.Collections.Generic;

public interface ITurnStateManager
{
    bool IsInitialized { get; }

    event Action GameStarted;
    event Action<Team> GameEnded;
    void Initialize(IEnumerable<Team> teams);
    void StartGame();
    void ForceEndGame();
}
