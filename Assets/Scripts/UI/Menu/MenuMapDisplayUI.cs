using UnityEngine;
using UnityEngine.UI;

public class MenuMapDisplayUI : MonoBehaviour
{
    [SerializeField] private MenuArrowButtonUI _rightButton;
    [SerializeField] private MenuArrowButtonUI _leftButton;
    [SerializeField] private Sprite[] _mapImages;
    [SerializeField] private Image _mapImage;
    private int _mapIndex;
    public int MapIndex => _mapIndex;

    private void Awake()
    {
        if(_mapImages.Length == 0)
        {
            Debug.LogWarning($"No map images set for the {nameof(MenuMapDisplayUI)}.");
        }

        _rightButton.ArrowPressed += IncrementMapIndex;
        _leftButton.ArrowPressed += DecrementMapIndex;
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

    private void Refresh()
    {
        _mapImage.sprite = _mapImages[_mapIndex];
        _rightButton.SetIsActive(_mapIndex < _mapImages.Length-1);
        _leftButton.SetIsActive(_mapIndex > 0);
    }

}
