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
    private GameplayUIManager _uiManager;
    private InputManager _inputManager;
    private bool _forceEndActions;
    private CharacterActionState CurrentCharacterActionState => _characterActionStates[_characterActionIndex];

    public event Action CharacterActionsFinished;

    public CharacterActionManager(MonoBehaviour coroutineManager, TrajectoryRenderer trajectoryRenderer, ItemPreviewRendererManager itemPreviewRendererManager, InputManager inputManager, CameraController cameraController, GameplayUIManager uiManager, ProjectilePool projectileManager, UISoundsDefinition uiSounds) : base(coroutineManager)
    {
        _cameraController = cameraController;
        _uiManager = uiManager;
        _inputManager = inputManager;
        _characterActionStates = new List<CharacterActionState>
        {
            new ReadyToMoveCharacterActionState(trajectoryRenderer, inputManager, uiManager, coroutineManager, uiSounds),
            new MovingCharacterActionState(coroutineManager, uiSounds),
            new ReadyToUseItemCharacterActionState(itemPreviewRendererManager, inputManager, projectileManager, trajectoryRenderer, uiManager, coroutineManager, uiSounds),
            new UsingItemCharacterActionState(coroutineManager, uiSounds),
            new FinishedCharacterActionState(coroutineManager, uiSounds),
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
        _inputManager.ForceCloseInventory();
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
        _uiManager.PauseGameplayTimer();
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
