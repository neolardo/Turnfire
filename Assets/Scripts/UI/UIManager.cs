using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _gameplayPausedScreen;
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private TeamHealthbarUIManager _teamHealthbarUIManager;

    private void Awake()
    {
        var turnManager = FindFirstObjectByType<TurnManager>();
        turnManager.GameEnded += OnGameOver;
        var gameStateManager = FindFirstObjectByType<GameStateManager>();
        gameStateManager.StateChanged += OnGameStateChanged;
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ToggleInventoryPerformed += ToggleInventory;//TODO: refactor?
    }

    public void CreateTeamHealthbars(IEnumerable<Team> teams)
    {
        _teamHealthbarUIManager.CreateHealthBars(teams);
    }

    private void ToggleInventory()
    {
        _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);
    }

    private void OnGameStateChanged(GameStateType gameState)
    {
        ShowHidePauseGameplayScreen(gameState == GameStateType.Paused);
    }

    private void ShowHidePauseGameplayScreen(bool show)
    {
        _gameplayPausedScreen.SetActive(show);
    }

    public void LoadCharacterData(Character character)
    {
        _inventoryUI.LoadCharacterData(character);
    }


    public void OnGameOver(Team winnerTeam)
    {
        if(winnerTeam == null)
        {
            _gameOverText.text = "It's a tie!";
        }
        else
        {
            _gameOverText.text = $"{winnerTeam.TeamName} wins!";
        }
        _gameOverScreen.SetActive(true);
    }

}
