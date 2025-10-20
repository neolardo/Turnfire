using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] 
public class OneShotAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private AnimationDefinition _animation;

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

    public void PlayAnimation(float frameDuration, bool hideAfter = true)
    {
        StartCoroutine(AnimateCoroutine(frameDuration, hideAfter));
    }

    private IEnumerator AnimateCoroutine(float frameDuration, bool hideAfter)
    { 
        _isPlaying = true;
        if(_animation.SFX != null)
        {
            AudioManager.Instance.PlaySFXAt(_animation.SFX, transform.position);
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
