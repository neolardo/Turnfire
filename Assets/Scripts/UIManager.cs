using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private GameObject _gameOverScreen;

    private void Awake()
    {
        var gameRoundManager = FindFirstObjectByType<GameRoundManager>();
        gameRoundManager.GameEnded += OnGameOver;
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
