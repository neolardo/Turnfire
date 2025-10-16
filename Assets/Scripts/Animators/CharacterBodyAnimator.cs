using System.Collections;
using System.Linq;
using UnityEngine;

public class CharacterBodyAnimator : MonoBehaviour
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

    private void Start()
    {
        _characterGroundChecker.IsGroundedChanged += OnCharacterIsGroundedChanged;
        PlayAnimation(CharacterAnimationState.Idle);
    }


    public void SetTeamColor(Color color)
    {
        _teamColor = color;
        _clothesSpriteRenderer.color = color;
        _overItemClothesSpriteRenderer.color = color;
    }


    private void OnCharacterIsGroundedChanged(bool isGrounded)
    {
        if (isGrounded)
        {
            PlayLandAnimation();
        }
        else
        {
            PlayJumpAnimation();
        }
    }

    public void ChangeAimFrame(Vector2 aimDirection)
    {
        StopAnimation();
        _currentAnimationState = CharacterAnimationState.Aim;
        ChangeOrientation(aimDirection.x < 0);
        if (aimDirection.y > Constants.UpwardAimThresholdY)
        {
            SwitchToFrame(_characterDefinition.AimHighAnimationDefinition.Frames.First());
        }
        else if (aimDirection.y >= Constants.DownwardAimThresholdY)
        {
            SwitchToFrame(_characterDefinition.AimMiddleAnimationDefinition.Frames.First());
        }
        else
        {
            SwitchToFrame(_characterDefinition.AimLowAnimationDefinition.Frames.First());
        }
    }

    public void PlayIdleAnimation()
    {
        PlayAnimation(CharacterAnimationState.Idle);
    }

    public void PlayItemActionAnimation()
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

    public void PlayPrepareToJumpAnimation()
    {
        PlayAnimation(CharacterAnimationState.PrepareToJump, CharacterAnimationState.None);
    }

    public void PlayCancelJumpAnimation()
    {
        PlayAnimation(CharacterAnimationState.BackFromLand, CharacterAnimationState.Idle);
    }

    private void PlayJumpAnimation()
    {
        PlayAnimation(CharacterAnimationState.Jump, CharacterAnimationState.None);
    }
    private void PlayLandAnimation()
    {
        PlayAnimation(CharacterAnimationState.Land, CharacterAnimationState.Idle);
    }

    public void ChangeJumpAim(Vector2 aimDirection)
    {
        ChangeOrientation(aimDirection.x < 0);
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
        if (_currentAnimationState == CharacterAnimationState.Death)
        {
            return;
        }

        StopAnimation();

        _currentAnimationState = state;

        _currentAnimationRoutine = StartCoroutine(PlayAnimationCoroutine(GetAnimationDefintion(_currentAnimationState), GetFrameDuration(_currentAnimationState), nextState, delay));
    }

    private void StopAnimation()
    {
        if (_currentAnimationRoutine != null)
        {
            StopCoroutine(_currentAnimationRoutine);
            _currentAnimationRoutine = null;
        }
    }


    private CharacterAnimationDefinition GetAnimationDefintion(CharacterAnimationState state)
    {
        switch (state)
        {
            case CharacterAnimationState.Idle:
                return _characterDefinition.IdleAnimationDefinition;
            case CharacterAnimationState.Jump:
                return _characterDefinition.JumpAnimationDefinition;
            case CharacterAnimationState.Land:
                return _characterDefinition.LandAnimationDefinition;
            case CharacterAnimationState.Hurt:
                return _characterDefinition.HurtAnimationDefinition;
            case CharacterAnimationState.Death:
                return _characterDefinition.DeathAnimationDefinition;
            case CharacterAnimationState.BackFromLand:
                return _characterDefinition.BackFromLandAnimationDefinition;
            case CharacterAnimationState.PrepareToJump:
                return _characterDefinition.PrepareToJumpAnimationDefinition;
            default:
                throw new System.Exception($"Invalid {nameof(CharacterAnimationState)} when getting {nameof(CharacterFrame)}s");

        }
    }

    private float GetFrameDuration(CharacterAnimationState state)
    {
        switch (state)
        {
            case CharacterAnimationState.Idle:
                return _animatorDefinition.IdleAnimationFrameDuration;
            case CharacterAnimationState.Jump:
                return _animatorDefinition.FlyAnimationFrameDuration;
            case CharacterAnimationState.Land:
                return _animatorDefinition.FlyAnimationFrameDuration;
            case CharacterAnimationState.Hurt:
                return _animatorDefinition.HurtAnimationFrameDuration;
            case CharacterAnimationState.Death:
                return _animatorDefinition.DeathAnimationFrameDuration;
            case CharacterAnimationState.BackFromLand:
                return _animatorDefinition.FlyAnimationFrameDuration;
            case CharacterAnimationState.PrepareToJump:
                return _animatorDefinition.FlyAnimationFrameDuration;
            default:
                throw new System.Exception($"Invalid {nameof(CharacterAnimationState)} when getting {nameof(CharacterFrame)}s");

        }
    }

    private IEnumerator PlayAnimationCoroutine(CharacterAnimationDefinition animationDefinition, float frameDuration, CharacterAnimationState nextState, float delay = 0f)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        var frames = animationDefinition.Frames;
        for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
        {
            SwitchToFrame(frames[frameIndex]);
            yield return new WaitForSeconds(frameDuration);
        }

        if (nextState != CharacterAnimationState.None)
        {
            PlayAnimation(nextState);
        }
    }
}
