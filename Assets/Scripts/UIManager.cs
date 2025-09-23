using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _inventoryPanel;

    private void Awake()
    {
        var gameRoundManager = FindFirstObjectByType<GameRoundManager>();
        gameRoundManager.GameEnded += OnGameOver;
        var inputManager = FindFirstObjectByType<InputManager>();
        inputManager.ToggleInventoryPerformed += ToggleInventory;
    }

    private void ToggleInventory()
    {
        _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);
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
