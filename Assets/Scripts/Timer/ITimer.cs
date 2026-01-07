using System;

public interface ITimer
{
    float CurrentTime { get; }
    bool IsRunning { get; }
    bool IsInitialized { get; }

    event Action TimerEnded;

    Func<bool> CanRestart { get; set; }
    Func<bool> CanPause { get; set; }
    Func<bool> CanResume { get; set; }

    void Initialize(float initialTime);
    void Restart();
    void Pause();
    void Resume();
}
