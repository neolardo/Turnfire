using System;
using System.Collections.Generic;

public interface ITurnStateManager
{
    bool IsInitialized { get; }

    event Action GameStarted;
    event Action<Team> GameEnded;
    event Action<Team> SelectedTeamChanged;
    void Initialize(IEnumerable<Team> teams);
    void StartFirstTurn();
    void ForceEndGame();
    Character GetCurrentCharacterInTeam(Team team);

}
