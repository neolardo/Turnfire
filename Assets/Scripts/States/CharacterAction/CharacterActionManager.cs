using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionManager : UnityDriven
{
    private List<CharacterActionState> _characterActionStates;
    private int _characterActionIndex;
    private Character _character;
    private CameraController _cameraController;
    private UIManager _uiManager;
    private bool _forceEndActions;
    private CharacterActionState CurrentCharacterActionState => _characterActionStates[_characterActionIndex];

    public event Action CharacterActionsFinished;

    public CharacterActionManager(MonoBehaviour coroutineManager, TrajectoryRenderer trajectoryRenderer, ItemPreviewRendererManager itemPreviewRendererManager, InputManager inputManager, CameraController cameraController, UIManager uiManager, ProjectilePool projectileManager) : base(coroutineManager)
    {
        _cameraController = cameraController;
        _uiManager = uiManager;
        _characterActionStates = new List<CharacterActionState>
        {
            new ReadyToMoveCharacterActionState(trajectoryRenderer, inputManager, coroutineManager),
            new MovingCharacterActionState(coroutineManager),
            new ReadyToUseItemCharacterActionState(itemPreviewRendererManager, inputManager, projectileManager, trajectoryRenderer, coroutineManager),
            new UsingItemCharacterActionState(coroutineManager),
            new FinishedCharacterActionState(coroutineManager),
        };

        foreach (var state in _characterActionStates)
        {
            state.StateEnded += OnCharacterActionStateEnded;
        }
        _uiManager.GameplayTimerEnded += ForceEndActions;
    }
    public void StartActionsWithCharacter(Character character)
    {
        _forceEndActions = false;
        _character = character;
        _cameraController.SetCharacterTarget(_character);
        _uiManager.LoadCharacterData(_character);
        _characterActionIndex = 0;
        StartCoroutine(WaitForCameraToStopBlendingThenStartFirstAction());
    }

    private IEnumerator WaitForCameraToStopBlendingThenStartFirstAction()
    {
        yield return null;
        yield return new WaitUntil(() => !_cameraController.IsBlending);
        _uiManager.StartGameplayTimer();
        StartCurrentCharacterActionState();
    }

    private void ForceEndActions()
    {
        _forceEndActions = true;
        CurrentCharacterActionState.ForceEndState();
    }

    private void EndActions()
    {
        _uiManager.StopGameplayTimer();
        CharacterActionsFinished?.Invoke();
    }

    private void OnCharacterActionStateEnded()
    {
        if (CurrentCharacterActionState.State == CharacterActionStateType.Finished || !_character.IsAlive || _forceEndActions)
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
