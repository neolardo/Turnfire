using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuMapDisplayUI : HoverableSelectableContainerUI
{
    [SerializeField] private MenuArrowButtonUI _rightButton;
    [SerializeField] private MenuArrowButtonUI _leftButton;
    [SerializeField] private MapDefinition[] _maps;
    [SerializeField] private Image _mapImage;
    private LocalMenuUIInputSource _inputManager;
    private int _mapIndex;
    private int _teamCount;
    public MapDefinition SelectedMap => _maps[_mapIndex];

    protected override void Awake()
    {
        base.Awake();
        if (_maps.Length == 0)
        {
            Debug.LogWarning($"No maps set for the {nameof(MenuMapDisplayUI)}.");
        }
        _inputManager = FindFirstObjectByType<LocalMenuUIInputSource>();
        _teamCount = _maps[0].Minimaps.Min(mm => mm.NumTeams);
        _rightButton.ArrowPressed += IncrementMapIndex;
        _leftButton.ArrowPressed += DecrementMapIndex;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _inputManager.MenuDecrementValuePerformed += OnDecrementValuePerformed;
        _inputManager.MenuIncrementValuePerformed += OnIncrementValuePerformed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _inputManager.MenuDecrementValuePerformed -= OnDecrementValuePerformed;
        _inputManager.MenuIncrementValuePerformed -= OnIncrementValuePerformed;
    }

    protected override void Start()
    {
        base.Start();
        Refresh();
    }

    private void OnDecrementValuePerformed()
    {
        if (IsSelected)
        {
            _leftButton.Press();
        }
    }
    private void OnIncrementValuePerformed()
    {
        if (IsSelected)
        {
            _rightButton.Press();
        }
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
        _teamCount = Mathf.Clamp(teamCount, 0, SelectedMap.Minimaps.Max(mm => mm.NumTeams));
        Refresh();
    }

    private void Refresh()
    {
        var minimap = SelectedMap.Minimaps.FirstOrDefault(mm => mm.NumTeams == _teamCount);
        _mapImage.sprite = minimap.Sprite == null ? SelectedMap.Minimaps.First().Sprite : minimap.Sprite;
        _rightButton.SetIsActive(_mapIndex < _maps.Length-1);
        _leftButton.SetIsActive(_mapIndex > 0);
    }

}
