using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [HideInInspector] public Team CurrentTeam;
    [SerializeField] private List<Team> _teams;
    [SerializeField] private TrajectoryRenderer _trajectoryRenderer;
    private int _teamIndex;

    void Awake()
    {
        if (_teams == null || _teams.Count == 0)
        {
            Debug.LogWarning("There are no teams.");
        }
        foreach (var team in _teams)
        {
            team.CharacterChanged += OnCharacterChanged;
        }

        CurrentTeam = _teams[_teamIndex];
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ImpulseReleased += OnImpulseReleased;
    }

    private void OnCharacterChanged(Character c) //TODO: stabilize character movement before?
    {
        _trajectoryRenderer.SetTrajectoryMultipler(c.CharacterData.JumpStrength);
        _trajectoryRenderer.SetStartTransform(c.Transform);
    }

    private void OnImpulseReleased(Vector2 impulse)
    {
        CurrentTeam.OnImpulseReleased(impulse);
    }

    private void NextTurn()
    {
        _teamIndex = (_teamIndex +1) % _teams.Count;
        CurrentTeam = _teams[_teamIndex];
    }
}
