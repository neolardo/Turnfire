using System.Collections;
using UnityEngine;

public abstract class ScreenSizeDependantUI : MonoBehaviour
{
    private int _lastScreenW, _lastScreenH;
    private FullScreenMode _lastFullscreenMode;

    protected virtual void OnEnable()
    {
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

    protected abstract void OneFrameAfterOnEnable();
    protected abstract void OneFrameAfterSizeChanged();


    private void SaveScreenSizeInfo()
    {
        _lastScreenW = Screen.width;
        _lastScreenH = Screen.height;
        _lastFullscreenMode = Screen.fullScreenMode;
    }

}
