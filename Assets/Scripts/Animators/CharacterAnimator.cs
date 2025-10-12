using System.Collections;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private CharacterAnimatorDefinition _animatorDefinition;
    [SerializeField] private CharacterDefinition _characterDefinition;
    [SerializeField] private GroundChecker _characterGroundChecker;

    [SerializeField] private SpriteRenderer _baseSpriteRenderer;
    [SerializeField] private SpriteRenderer _clothesSpriteRenderer;
    [SerializeField] private SpriteRenderer _overItemBaseSpriteRenderer;
    [SerializeField] private SpriteRenderer _overItemClothesSpriteRenderer;

    private Color _teamColor;
    private Coroutine _currentAnimationRoutine;
    private CharacterAnimationState _currentAnimationState;

    public void SetTeamColor(Color color) //TODO
    {
        _teamColor = color;
        _clothesSpriteRenderer.color = color;
        _overItemClothesSpriteRenderer.color = color;
    }

    private void Start()
    {
        _characterGroundChecker.IsGroundedChanged += OnCharacterIsGroundedChanged;
        PlayAnimation(CharacterAnimationState.Idle);
    }

    private void OnCharacterIsGroundedChanged(bool isGrounded)
    {
        if(isGrounded)
        {
            PlayLandAnimation();
        }
        else
        {
            PlayJumpAnimation();
        }
    }

    public void ChangeAimFrame(Vector2 aimDirection) //TODO
    {
        StopAnimation();
        _currentAnimationState = CharacterAnimationState.Aim;
        ChangeOrientation(aimDirection.x < 0);
        if (aimDirection.y > Constants.UpwardAimThresholdY)
        {
            SwitchToFrame(_characterDefinition.AimHighFrame);
        }
        else if (aimDirection.y >= Constants.DownwardAimThresholdY)
        {
            SwitchToFrame(_characterDefinition.AimMiddleFrame);
        }
        else
        {
            SwitchToFrame(_characterDefinition.AimLowFrame);
        }
    }

    public void PlayIdleAnimation()
    {
        PlayAnimation(CharacterAnimationState.Idle);
    }

    public void PlayUseItemAnimation()
    {
        PlayAnimation(CharacterAnimationState.Idle, CharacterAnimationState.Idle, _animatorDefinition.ItemUsageDelay);
    }

    public void PlayDeathAnimation()
    {
        PlayAnimation(CharacterAnimationState.Death, CharacterAnimationState.None);
    }

    public void PlayHurtAnimation()
    {
        PlayAnimation(CharacterAnimationState.Hurt);
    }
    private void PlayJumpAnimation()
    {
        PlayAnimation(CharacterAnimationState.Jump, CharacterAnimationState.None);
    }
    private void PlayLandAnimation()
    {
        PlayAnimation(CharacterAnimationState.Land);
    }

    private void ChangeOrientation(bool turnToLeft)
    {
        _baseSpriteRenderer.flipX = turnToLeft;
        _clothesSpriteRenderer.flipX = turnToLeft;
        _overItemBaseSpriteRenderer.flipX = turnToLeft;
        _overItemClothesSpriteRenderer.flipX = turnToLeft;
    }

    private void SwitchToFrame(CharacterFrame frame)
    {
        _baseSpriteRenderer.sprite = frame.Base;
        _clothesSpriteRenderer.sprite = frame.Clothes;
        _overItemBaseSpriteRenderer.sprite = frame.OverItemBase;
        _overItemClothesSpriteRenderer.sprite = frame.OverItemClothes;
    }

    private void PlayAnimation(CharacterAnimationState state, CharacterAnimationState nextState = CharacterAnimationState.Idle, float delay = 0f)
    {
        if(_currentAnimationState == CharacterAnimationState.Death)
        {
            return;
        }

        StopAnimation();

        _currentAnimationState = state;

        _currentAnimationRoutine = StartCoroutine(PlayAnimationCoroutine(GetAnimationFrames(_currentAnimationState), GetFrameDuration(_currentAnimationState), nextState, delay));
    }

    private void StopAnimation()
    {
        if (_currentAnimationRoutine != null)
        {
            StopCoroutine(_currentAnimationRoutine);
            _currentAnimationRoutine = null;
        }
    }


    private CharacterFrame[] GetAnimationFrames(CharacterAnimationState state)
    {
        switch (state)
        {
            case CharacterAnimationState.Idle:
                return _characterDefinition.IdleFrames;
            case CharacterAnimationState.Jump:
                return _characterDefinition.JumpFrames;
            case CharacterAnimationState.Land:
                return _characterDefinition.LandFrames;
            case CharacterAnimationState.Hurt:
                return _characterDefinition.HurtFrames;
            case CharacterAnimationState.Death:
                return _characterDefinition.DeathFrames;
            default:
                throw new System.Exception($"Invalid {nameof(CharacterAnimationState)} when getting {nameof(CharacterFrame)}s"); 

        }
    }

    private float GetFrameDuration(CharacterAnimationState state)
    {
        switch (state)
        {
            case CharacterAnimationState.Idle:
                return _animatorDefinition.IdleAnimationDurationPerFrame;
            case CharacterAnimationState.Jump:
                return _animatorDefinition.FlyAnimationDurationPerFrame;
            case CharacterAnimationState.Land:
                return _animatorDefinition.FlyAnimationDurationPerFrame;
            case CharacterAnimationState.Hurt:
                return _animatorDefinition.HurtAnimationDurationPerFrame;
            case CharacterAnimationState.Death:
                return _animatorDefinition.DeathAnimationDurationPerFrame;
            default:
                throw new System.Exception($"Invalid {nameof(CharacterAnimationState)} when getting {nameof(CharacterFrame)}s");

        }
    }

    private IEnumerator PlayAnimationCoroutine(CharacterFrame[] frames, float frameDuration, CharacterAnimationState nextState, float delay = 0f)
    {
        if(delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
        {
            SwitchToFrame(frames[frameIndex]);
            yield return new WaitForSeconds(frameDuration);
        }

        if(nextState != CharacterAnimationState.None)
        {
            PlayAnimation(nextState);
        }
    }

}
