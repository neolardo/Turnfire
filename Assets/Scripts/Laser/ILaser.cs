using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILaser
{
    bool IsReady { get; }
    Transform LaserHead { get; }
    bool IsFirstRayRendered { get; }
    bool IsBeamAnimationInProgress { get; }

    event Action<ILaser> BeamEnded;

    void Initialize(int maxBounceCount, float maxDistance);
    void StartBeam(Vector2 aimOrigin, Vector2 aimVector, Character owner);
    IEnumerable<Character> GetHitCharacters();
}
