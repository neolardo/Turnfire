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
    protected bool ScreenSizeChanged()
    {
        if (Screen.width != _lastScreenW || Screen.height != _lastScreenH || Screen.fullScreenMode != _lastFullscreenMode)
        {
            SaveScreenSizeInfo();
            return true;
        }
        return false;
    }

    private IEnumerator WaitForFirstFrameThenUpdateLayout()
    {
        yield return null;
        OneFrameAfterOnEnable();
    }


    protected abstract void OneFrameAfterOnEnable();


    private void SaveScreenSizeInfo()
    {
        _lastScreenW = Screen.width;
        _lastScreenH = Screen.height;
        _lastFullscreenMode = Screen.fullScreenMode;
    }

}
