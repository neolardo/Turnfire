using System.Collections.Generic;
using UnityEngine;

public interface ILaser
{
    bool IsReady { get; }
    Transform LaserHead { get; }
    bool IsFirstRayRendered { get; }
    bool IsAnimationInProgress { get; }
    void Initialize(int maxBounceCount, float maxDistance);
    void StartLaser(Character owner, Vector2 aimOrigin, Vector2 aimVector);
    IEnumerable<Character> GetHitCharacters();
}
