using UnityEngine;

public class PixelUIGrid : MonoBehaviour
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private int _columns = 3;
    [SerializeField] private Vector2Int _cellSizeInPixels = new Vector2Int(32, 32);
    [SerializeField] private Vector2Int _spacingInPixels = new Vector2Int(2, 2);
    [SerializeField] private Vector2Int _paddingInPixels = new Vector2Int(4, 4);

    private PixelUIScaler[] children;


    private void OnEnable()
    {
        CacheChildren();
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

            var anchor = new Vector2(0, 1);
            var pivot = new Vector2(0, 1);

            children[i].SetPositionAndSize(anchor, pivot, position, _cellSizeInPixels);
        }
    }
}
