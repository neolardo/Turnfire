using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIScaler : ScreenSizeDependantUI
{
    [Header("Target aspect ratio (W:H)")]
    [SerializeField] private float targetAspect = 16f / 9f;

    [Header("Maximum height relative to screen height")]
    [SerializeField, Range(0.1f, 1f)] private float maxHeightRatio = 0.9f;

    [Header("Maximum width relative to screen width")]
    [SerializeField, Range(0.1f, 1f)] private float maxWidthRatio = 0.9f;

    private RectTransform _rect;


    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    protected override void OneFrameAfterOnEnable()
    {
        ApplyScaling();
    }

    private void Update()
    {
        if(ScreenSizeChanged())
        { 
            ApplyScaling();
        }
    }


    private void ApplyScaling()
    {
        float screenW = Screen.width;
        float screenH = Screen.height;
        float screenAspect = screenW / screenH;

        float width, height;

        if (screenAspect > targetAspect)
        {
            // Screen is wider than target → clamp by height
            height = screenH * maxHeightRatio;
            width = height * targetAspect;
        }
        else
        {
            // Screen is taller than target → clamp by width
            width = screenW * maxWidthRatio;
            height = width / targetAspect;
        }

        _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        _rect.anchoredPosition = Vector2.zero;
    }
}