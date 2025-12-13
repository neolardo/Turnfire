using System.Collections.Generic;
using UnityEngine;

public class BotPositionMemory 
{
    private Queue<StandingPoint> _memory;
    //configs
    private int _memorySize;
    private float _penaltyPerTurn;
    private float _stationaryDistance;

    private StandingPoint _lastPoint;
    private int _penaltyStrength;
    public BotPositionMemory(float penaltyPerTurn, int memorySize, float stationaryDistance)
    {
        _memorySize = memorySize;
        _penaltyPerTurn = penaltyPerTurn;
        _stationaryDistance = stationaryDistance;
        _memory = new Queue<StandingPoint>();
    }

    public float GetStationaryPenalty(StandingPoint point)
    {
        float penalty = 0;
        if (IsStationaryPoint(point))
        {
            penalty = (_penaltyStrength+1) * _penaltyPerTurn;
        }
        return penalty;
    }

    public void Commit(StandingPoint point)
    {
        if (IsStationaryPoint(point))
        {
            _penaltyStrength++;
            Debug.Log($"Bot picked a stationary point. Current penalty is: {_penaltyStrength}");
        }
        else
        {
            _penaltyStrength = Mathf.Max(_penaltyStrength - 1, 0);
        }
        Memorize(point);
    }

    private void Memorize(StandingPoint point)
    {
        _lastPoint = point;
        _memory.Enqueue(point);
        if(_memory.Count > _memorySize)
        {
            _memory.Dequeue();
        }
    }

    private bool IsStationaryPoint(StandingPoint point)
    {
        if (_memory.Count == 0)
        {
            return false;
        }
        var stationaryCenter = GetStationaryCenter();
        return Vector2.Distance(point.WorldPos, stationaryCenter) < _stationaryDistance;
    }

    private Vector2 GetStationaryCenter()
    {
        if(_memory.Count == 0)
        {
            return Vector2.zero;
        }

        var avaragePos = Vector2.zero;
        foreach(var p in _memory)
        {
            avaragePos += p.WorldPos;
        }
        avaragePos /= _memory.Count;

        if (Vector2.Distance(avaragePos, _lastPoint.WorldPos) < _stationaryDistance)
        {
            return avaragePos;
        }
        else
        {
            return _lastPoint.WorldPos;
        }
    }
}
