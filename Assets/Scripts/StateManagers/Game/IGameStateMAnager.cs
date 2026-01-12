using System;

public interface IGameStateManager
{
    GameStateType CurrentState { get; }

    event Action<GameStateType> StateChanged;
    void TogglePauseResumeGameplay();
    void StartGame();
}
