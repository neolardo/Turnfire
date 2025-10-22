using UnityEngine;

public class PixelUIGrid : ScreenSizeDependantUI
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private int _columns = 3;
    [SerializeField] private Vector2Int _cellSizeInPixels = new Vector2Int(32, 32);
    [SerializeField] private Vector2Int _spacingInPixels = new Vector2Int(2, 2);
    [SerializeField] private Vector2Int _paddingInPixels = new Vector2Int(4, 4);
    [SerializeField] private Vector2 _startAnchor = new Vector2(0,1);
    [SerializeField] private Vector2 _pivot = new Vector2(0, 1);

    private PixelUIScaler[] children;

    protected override void OnEnable()
    {
        base.OnEnable();
        CacheChildren();
    }

    protected override void OneFrameAfterOnEnable()
    {
        UpdateLayout();
    }

    private void CacheChildren()
    {
        if(children != null)
        {
            return;
        }
        // Cache all direct children
        children = new PixelUIScaler[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!child.TryGetComponent(out PixelUIScaler scaler))
            {
                scaler = child.gameObject.AddComponent<PixelUIScaler>();
            }
            scaler.SetUIDefinition(_uiDefinition);
            children[i] = scaler;
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        if (Application.isPlaying)
        {
            UpdateLayout();
        }
    }

    private void UpdateLayout()
    {
        if (_uiDefinition == null || children == null) 
            return;

        for (int i = 0; i < children.Length; i++)
        {
            int row = i / _columns;
            int col = i % _columns;

            Vector2 position = new Vector2(
               _paddingInPixels.x + col * (_cellSizeInPixels.x + _spacingInPixels.x),
               -(_paddingInPixels.y + row * (_cellSizeInPixels.y + _spacingInPixels.y))
           );

           children[i].SetPositionAndSize(_startAnchor, _pivot, position, _cellSizeInPixels);
        }
    }

    protected override void OneFrameAfterSizeChanged()
    {
        UpdateLayout();
    }
}
