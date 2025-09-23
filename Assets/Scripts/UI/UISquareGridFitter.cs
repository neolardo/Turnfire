using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]

public class UISquareGridFitter : MonoBehaviour
{
    [SerializeField] private int _columns = 3;
    [SerializeField] private int _rows = 3;
    [SerializeField] private Vector2 _spacing = new Vector2(5, 5);

    private GridLayoutGroup _grid;
    private RectTransform _rect;

    private void Awake()
    {
        _grid = GetComponent<GridLayoutGroup>();
        _rect = GetComponent<RectTransform>();
        UpdateGrid();
    }

    private void Update()
    {
        // Recalculate only if resolution changes
        if (_rect.hasChanged)
        {
            _rect.hasChanged = false;
            UpdateGrid();
        }
    }

    private void UpdateGrid()
    {
        float parentWidth = _rect.rect.width - _grid.padding.left - _grid.padding.right;
        float parentHeight = _rect.rect.height - _grid.padding.top - _grid.padding.bottom;

        float cellWidth = (parentWidth - (_columns - 1) * _spacing.x) / _columns;
        float cellHeight = (parentHeight - (_rows - 1) * _spacing.y) / _rows;

        float cellSize = Mathf.Min(cellWidth, cellHeight); // ensures squares

        _grid.cellSize = new Vector2(cellSize, cellSize);
        _grid.spacing = _spacing;
    }
}
