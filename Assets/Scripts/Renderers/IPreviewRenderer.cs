using UnityEngine;

public interface IPreviewRenderer
{
    void OnAimStarted(Vector2 initialAimPosition);
    void OnAimChanged(Vector2 aimVector);
    void OnAimCancelled();
    void OnImpulseReleased(Vector2 impulse);
}
