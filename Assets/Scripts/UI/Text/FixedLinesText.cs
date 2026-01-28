using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FixedLinesText : MonoBehaviour
{
    [SerializeField] private int _lineCount = 3;
    [SerializeField] private float _extraRelativeSpacing = .2f;

    private TextMeshProUGUI _tmp;
    private RectTransform _rect;

    void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
        _rect = GetComponent<RectTransform>();
        _tmp.enableAutoSizing = false;
    }

    void OnEnable()
    {
        ApplyHeight();
    }

    void OnRectTransformDimensionsChange()
    {
        ApplyHeight();
    }

    void ApplyHeight()
    {
        var lineHeight = (_rect.rect.height / _lineCount);
        float size = lineHeight  - (_tmp.lineSpacing) * _lineCount - _extraRelativeSpacing * lineHeight;
        _tmp.fontSize = size;
    }
}
