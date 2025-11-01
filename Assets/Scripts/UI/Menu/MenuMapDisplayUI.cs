using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuMapDisplayUI : MonoBehaviour
{
    [SerializeField] private MenuArrowButtonUI _rightButton;
    [SerializeField] private MenuArrowButtonUI _leftButton;
    [SerializeField] private MapDefinition[] _maps;
    [SerializeField] private Image _mapImage;
    private MenuInputManager _inputManager;
    private int _mapIndex;
    private int _teamCount;
    public MapDefinition SelectedMap => _maps[_mapIndex];

    private void Awake()
    {
        if(_maps.Length == 0)
        {
            Debug.LogWarning($"No maps set for the {nameof(MenuMapDisplayUI)}.");
        }
        _inputManager = FindFirstObjectByType<MenuInputManager>();
        _teamCount = _maps[0].Minimaps.Min(mm => mm.NumTeams);
        _rightButton.ArrowPressed += IncrementMapIndex;
        _leftButton.ArrowPressed += DecrementMapIndex;
    }

    private void OnEnable()
    {
        _inputManager.MenuNavigateRightPerformed += _rightButton.Press;
        _inputManager.MenuNavigateLeftPerformed += _leftButton.Press;
    }

    private void OnDisable()
    {
        _inputManager.MenuNavigateRightPerformed -= _rightButton.Press;
        _inputManager.MenuNavigateLeftPerformed -= _leftButton.Press;
    }

    private void Start()
    {
        Refresh();
    }

    private void IncrementMapIndex()
    {
        _mapIndex++;
        Refresh();
    }

    private void DecrementMapIndex()
    {
        _mapIndex--;
        Refresh();
    }

    public void SetTeamCount(int teamCount)
    {
        _teamCount = teamCount;
        Refresh();
    }

    private void Refresh()
    {
        _mapImage.sprite = _maps[_mapIndex].Minimaps.First(mm => mm.NumTeams == _teamCount).Sprite;
        _rightButton.SetIsActive(_mapIndex < _maps.Length-1);
        _leftButton.SetIsActive(_mapIndex > 0);
    }

}
