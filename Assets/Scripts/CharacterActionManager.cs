using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionManager
{
    private List<CharacterActionState> _characterActionStates;
    private int _characterActionIndex;
    private Character _character;
    private CameraController _cameraController;
    private UIManager _uiManager;
    private CharacterActionState CurrentCharacterActionState => _characterActionStates[_characterActionIndex];

    public event Action CharacterActionsFinished;

    public CharacterActionManager(MonoBehaviour coroutineManager, TrajectoryRenderer trajectoryRenderer, InputManager inputManager, CameraController cameraController, UIManager uiManager)
    {
        _cameraController = cameraController;
        _uiManager = uiManager;
        _characterActionStates = new List<CharacterActionState>
        {
            new ReadyToMoveCharacterActionState(trajectoryRenderer, inputManager, coroutineManager),
            new MovingCharacterActionState(coroutineManager),
            new ReadyToFireCharacterActionState(trajectoryRenderer, inputManager, coroutineManager),
            new FiringCharacterActionState(coroutineManager),
            new FinishedCharacterActionState(coroutineManager),
        };

        foreach (var state in _characterActionStates)
        {
            state.StateEnded += OnCharacterActionStateEnded;
        }
    }

    public void StartActionsWithCharacter(Character character)
    {
        _character = character;
        _cameraController.SetCharacterTarget(_character); //TODO?
        _uiManager.LoadCharacterData(_character); //TODO?
        _characterActionIndex = 0;
        StartCurrentCharacterActionState();
    }

    private void EndActions()
    {
        CharacterActionsFinished?.Invoke();
    }

    private void OnCharacterActionStateEnded()
    {
        if (CurrentCharacterActionState.State == CharacterActionStateType.Finished || !_character.IsAlive)
        {
            EndActions();
        }
        else
        {
            ChangeCharacterActionState();
            StartCurrentCharacterActionState();
        }
    }

    private void ChangeCharacterActionState()
    {
        _characterActionIndex = (_characterActionIndex + 1) % _characterActionStates.Count;
    }

    private void StartCurrentCharacterActionState()
    {
        CurrentCharacterActionState.StartState(_character);
    }
}
