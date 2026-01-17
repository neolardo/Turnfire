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
    private bool _forceEndActions;
    private CharacterActionState CurrentCharacterActionState => _characterActionStates[_characterActionIndex];

    public event Action CharacterActionsFinished;

    public CharacterActionManager(PreviewRendererManager previewRenderer, CameraController cameraController, GameplayUIManager uiManager) : base(CoroutineRunner.Instance)
    {
        _cameraController = cameraController;
        _uiManager = uiManager;
        _characterActionStates = new List<CharacterActionState>
        {
            new ReadyToMoveCharacterActionState(previewRenderer),
            new MovingCharacterActionState(),
            new ReadyToUseItemCharacterActionState(previewRenderer),
            new UsingItemCharacterActionState(),
            new FinishedCharacterActionState(),
        };

        foreach (var state in _characterActionStates)
        {
            state.StateEnded += OnCharacterActionStateEnded;
        }
        GameServices.GameplayTimer.TimerEnded += ForceEndActions;
    }
    public void StartActionsWithCharacter(Character character)
    {
        _forceEndActions = false;
        _character = character;
        _cameraController.SetCharacterTarget(_character.transform);
        character.Team.InputSource.ForceCloseInventory();
        _uiManager.LoadCharacterData(_character);
        _characterActionIndex = 0;
        StartCoroutine(WaitForCameraToStopBlendingThenStartFirstAction());
    }

    private IEnumerator WaitForCameraToStopBlendingThenStartFirstAction()
    {
        yield return null;
        yield return new WaitUntil(() => !_cameraController.IsBlending);
        GameServices.GameplayTimer.Restart();
        StartCurrentCharacterActionState();
    }

    public void ForceEndActions()
    {
        _forceEndActions = true;
        CurrentCharacterActionState.ForceEndState();
    }

    private void EndActions()
    {
        GameServices.GameplayTimer.Pause();
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
