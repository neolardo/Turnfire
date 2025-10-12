using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] 
public class SimpleAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _frames;

    public bool IsPlaying => _isPlaying;
    private bool _isPlaying;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation(float frameDuration, bool hideAfter = true)
    {
        StartCoroutine(AnimateCoroutine(frameDuration, hideAfter));
    }

    private IEnumerator AnimateCoroutine(float frameDuration, bool hideAfter)
    {
        _isPlaying = true;
        for (int i = 0; i < _frames.Length; i++)
        {
            _spriteRenderer.sprite = _frames[i];
            yield return new WaitForSeconds(frameDuration);
        }
        if(hideAfter)
        {
            _spriteRenderer.sprite = null;
        }
        _isPlaying = false;
    }


}
