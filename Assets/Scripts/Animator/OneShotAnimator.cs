using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] 
public class OneShotAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private AnimationDefinition _animation;
    private SFXDefiniton _sfx;

    public bool IsPlaying => _isPlaying;
    private bool _isPlaying;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAnimation(AnimationDefinition animation)
    {
        _animation = animation;
    }

    public void SetSFX(SFXDefiniton sfx)
    {
        _sfx = sfx;
    }


    public void PlayAnimation(float frameDuration, bool hideAfter = true)
    {
        StartCoroutine(AnimateCoroutine(frameDuration, hideAfter));
    }

    private IEnumerator AnimateCoroutine(float frameDuration, bool hideAfter)
    { 
        _isPlaying = true;
        if(_sfx != null)
        {
            AudioManager.Instance.PlaySFXAt(_sfx, transform.position);
        }
        var frames = _animation.Frames;
        for (int i = 0; i < frames.Length; i++)
        {
            _spriteRenderer.sprite = frames[i];
            yield return new WaitForSeconds(frameDuration);
        }
        if(hideAfter)
        {
            _spriteRenderer.sprite = null;
        }
        _isPlaying = false;
    }


}
