using System.Collections;
using UnityEngine;

public abstract class ScreenSizeDependantHoverableSelectableContainerUI : HoverableSelectableContainerUI
{
    private int _lastScreenW, _lastScreenH;
    private FullScreenMode _lastFullscreenMode;

    protected override void OnEnable()
    {
        base.OnEnable();
        SaveScreenSizeInfo();
        StartCoroutine(WaitForFirstFrameThenUpdateLayout());
    }
    protected bool CheckScreenSizeChanged()
    {
        if (Screen.width != _lastScreenW || Screen.height != _lastScreenH || Screen.fullScreenMode != _lastFullscreenMode)
        {
            SaveScreenSizeInfo();
            StartCoroutine(WaitForFrameEndAfterSizeChangeThenUpdateLayout());
            return true;
        }
        return false;
    }

    protected virtual void Update()
    {
        CheckScreenSizeChanged();
    }

    private IEnumerator WaitForFirstFrameThenUpdateLayout()
    {
        yield return null;
        OneFrameAfterOnEnable();
    }

    private IEnumerator WaitForFrameEndAfterSizeChangeThenUpdateLayout()
    {
        yield return null;
        OneFrameAfterSizeChanged();
    }

    protected virtual void OneFrameAfterOnEnable()
    {
        if (!_isAnchoredPositionInitialized)
        {
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
            _isAnchoredPositionInitialized = true;
        }
    }

    protected virtual void OneFrameAfterSizeChanged()
    {
        var offset = _uiDefinition.CalculateHoverOffset(_parentCanvasRect.sizeDelta.y);
        _rectTransform.anchoredPosition = _isHovered ? _originalAnchoredPosition + offset : _originalAnchoredPosition;
    }


    private void SaveScreenSizeInfo()
    {
        _lastScreenW = Screen.width;
        _lastScreenH = Screen.height;
        _lastFullscreenMode = Screen.fullScreenMode;
    }

}
