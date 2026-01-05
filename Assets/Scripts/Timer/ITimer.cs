using System;

public interface ITimer
{
    float CurrentTime { get; }
    bool IsRunning { get; }

    event Action TimerEnded;
    void Initialize(float initialTime);
    void Restart();
    void Pause();
    void Resume();
}
