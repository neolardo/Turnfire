using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBodyAnimator : MonoBehaviour
{
    [SerializeField] private CharacterAnimatorDefinition _animatorDefinition;
    [SerializeField] private GroundChecker _characterGroundChecker;
    [SerializeField] private FlashSpriteAnimator _flashAnimator;

    [Header("Sprite Renderers")]
    [SerializeField] private SpriteRenderer _bodySpriteRenderer;
    [SerializeField] private SpriteRenderer _headSpriteRenderer;
    [SerializeField] private SpriteRenderer _clothesSpriteRenderer;
    [SerializeField] private SpriteRenderer _overItemBodySpriteRenderer;
    [SerializeField] private SpriteRenderer _overItemClothesSpriteRenderer;
    [SerializeField] private SpriteRendererPool _equippedArmorSpriteRendererPool;

    private Dictionary<CharacterAnimationLayer, SpriteRenderer> _baseSpriteRenderers;
    private Dictionary<ArmorDefinition, SpriteRenderer> _equippedArmorSpriteRenderers;

    public IEnumerable<SpriteRenderer> AllRenderers => _baseSpriteRenderers.Values.Concat(_equippedArmorSpriteRenderers.Values);

    private CharacterDefinition _characterDefinition;

    private int _lastFrameIndex;
    private Color _teamColor;
    private Coroutine _currentAnimationRoutine;
    private CharacterAnimationState _currentAnimationState;
    public bool IsFacingLeft => _baseSpriteRenderers[CharacterAnimationLayer.Body].flipX;

    public bool IsPlayingNonIdleAnimation => (_currentAnimationState != CharacterAnimationState.Idle && _currentAnimationState != CharacterAnimationState.None) || _flashAnimator.IsFlashing;

    #region Initialization

    private void Start()
    {
        _baseSpriteRenderers = new Dictionary<CharacterAnimationLayer, SpriteRenderer>();
        _baseSpriteRenderers[CharacterAnimationLayer.Body] = _bodySpriteRenderer;
        _baseSpriteRenderers[CharacterAnimationLayer.Head] = _headSpriteRenderer;
        _baseSpriteRenderers[CharacterAnimationLayer.Clothes] = _clothesSpriteRenderer;
        _baseSpriteRenderers[CharacterAnimationLayer.OverItemBody] = _overItemBodySpriteRenderer;
        _baseSpriteRenderers[CharacterAnimationLayer.OverItemClothes] = _overItemClothesSpriteRenderer;

        _equippedArmorSpriteRenderers = new Dictionary<ArmorDefinition, SpriteRenderer>();

        if (_currentAnimationRoutine != null)
        {
            Debug.LogWarning($"{nameof(CharacterDefinition)} not set for {nameof(CharacterBodyAnimator)}.");
        }
        _characterDefinition.InitializeAnimations();
        _characterGroundChecker.IsGroundedChanged += OnCharacterIsGroundedChanged;
        PlayAnimation(CharacterAnimationState.Idle);
    }

    public void SetCharacterDefinition(CharacterDefinition characterDefinition)
    {
        _characterDefinition = characterDefinition;
    }

    public void SetTeamColor(Color color)
    {
        _teamColor = color;
        _clothesSpriteRenderer.color = color;
        _overItemClothesSpriteRenderer.color = color;
    }

    #endregion

    #region Animations

    public void PlayIdleAnimation()
    {
        PlayAnimation(CharacterAnimationState.Idle);
    }

    public void PlayItemActionAnimation(Vector2 aimVector, Item selectedItem)
    {
        aimVector = aimVector.normalized;
        var nextAnimation = CharacterAnimationState.Idle;
        float preDelay = _animatorDefinition.ItemUsageDelay;
        if (selectedItem.Definition.ItemType == ItemType.Weapon && !(selectedItem.Definition as WeaponDefinition).IsRanged)
        {
            if (aimVector.y > Constants.UpwardAimThresholdY)
            {
                nextAnimation = CharacterAnimationState.MeleeAttackHigh;
            }
            else if (aimVector.y >= Constants.DownwardAimThresholdY)
            {
                nextAnimation = CharacterAnimationState.MeleeAttackMiddle;
            }
            else
            {
                nextAnimation = CharacterAnimationState.MeleeAttackLow;
            }
            preDelay = 0;
        }
        PlayAnimation(nextAnimation, CharacterAnimationState.Idle, preDelay);
    }

    public void PlayDeathAnimation()
    {
        PlayAnimation(CharacterAnimationState.Death, CharacterAnimationState.None);
    }

    public void PlayEquipArmorAnimation(ArmorDefinition armor)
    {
        Debug.Log("Equip animation started");
        _equippedArmorSpriteRenderers[armor] = _equippedArmorSpriteRendererPool.Get();
        _equippedArmorSpriteRenderers[armor].flipX = _baseSpriteRenderers[0].flipX;
        _equippedArmorSpriteRenderers[armor].sprite = armor.Animations[_currentAnimationState][_lastFrameIndex];
        _flashAnimator.Flash(new[] { _equippedArmorSpriteRenderers[armor] }, _animatorDefinition.ItemFlashColor, _animatorDefinition.ItemFlashInSeconds, _animatorDefinition.ItemFlashOutSeconds);
    }

    public void PlayUnequipArmorAnimation(ArmorDefinition armor)
    {
        StartCoroutine(PlayUnequipAnimationThenDestroyRenderer(armor));
    }

    private IEnumerator PlayUnequipAnimationThenDestroyRenderer(ArmorDefinition armor)
    {
        _flashAnimator.Flash(new[] { _equippedArmorSpriteRenderers[armor] }, _animatorDefinition.ItemFlashColor, _animatorDefinition.ItemFlashInSeconds, _animatorDefinition.ItemFlashOutSeconds);
        yield return new WaitUntil(() => !_flashAnimator.IsFlashing);
        _equippedArmorSpriteRendererPool.Release(_equippedArmorSpriteRenderers[armor]);
        _equippedArmorSpriteRenderers.Remove(armor);
    }

    public void PlayGuardAnimation(ArmorDefinition armor)
    {
        _flashAnimator.Flash(new[] { _equippedArmorSpriteRenderers[armor] }, _animatorDefinition.ItemFlashColor, _animatorDefinition.ItemFlashInSeconds, _animatorDefinition.ItemFlashOutSeconds);
        PlayAnimation(CharacterAnimationState.Guard);
    }

    public void PlayHurtAnimation()
    {
        PlayAnimation(CharacterAnimationState.Hurt);
        _flashAnimator.Flash(AllRenderers, _animatorDefinition.HurtFlashColor, _animatorDefinition.HurtAnimationFlashInSeconds, _animatorDefinition.HurtAnimationFlashOutSeconds);
    }
    public void PlayHealAnimation()
    {
        _flashAnimator.Flash(AllRenderers, _animatorDefinition.HealFlashColor, _animatorDefinition.HealAnimationFlashInSeconds, _animatorDefinition.HealAnimationFlashOutSeconds);
    }

    public void PlayAimAnimation(Vector2 aimDirection, bool isRangedWeapon)
    {
        StopAnimation();
        aimDirection = aimDirection.normalized;
        ChangeOrientation(aimDirection.x < 0);
        if (aimDirection.y > Constants.UpwardAimThresholdY)
        {
            _currentAnimationState = isRangedWeapon ? CharacterAnimationState.RangedAimHigh : CharacterAnimationState.MeleeAimHigh;
        }
        else if (aimDirection.y >= Constants.DownwardAimThresholdY)
        {
            _currentAnimationState = isRangedWeapon ? CharacterAnimationState.RangedAimMiddle : CharacterAnimationState.MeleeAimMiddle;
        }
        else
        {
            _currentAnimationState = isRangedWeapon ? CharacterAnimationState.RangedAimLow : CharacterAnimationState.MeleeAimLow;
        }
        SwitchToFrame(0);
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

    #endregion

    #region Play & Stop

    private void PlayAnimation(CharacterAnimationState state, CharacterAnimationState nextState = CharacterAnimationState.Idle, float preDelay = 0f)
    {
        if (_currentAnimationState == CharacterAnimationState.Death)
        {
            return;
        }

        StopAnimation();

        _currentAnimationState = state;

        int frameCount = _characterDefinition.Animations[_currentAnimationState].Length;
        float frameDuration = GetFrameDuration(_currentAnimationState);
        var sfx = GetSFX(_currentAnimationState);
        _currentAnimationRoutine = StartCoroutine(PlayAnimationCoroutine(frameCount, frameDuration, sfx, nextState, preDelay));
    }

    private void StopAnimation()
    {
        if (_currentAnimationRoutine != null)
        {
            StopCoroutine(_currentAnimationRoutine);
            _currentAnimationRoutine = null;
        }
    }

    private IEnumerator PlayAnimationCoroutine(int frameCount, float frameDuration, SFXDefiniton sfx, CharacterAnimationState nextState, float preDelay = 0f)
    {
        if (preDelay > 0f)
        {
            yield return new WaitForSeconds(preDelay);
        }

        if (sfx != null)
        {
            AudioManager.Instance.PlaySFXAt(sfx, transform);
        }

        for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
        {
            SwitchToFrame(frameIndex);
            yield return new WaitForSeconds(frameDuration);
        }

        if (nextState != CharacterAnimationState.None)
        {
            PlayAnimation(nextState);
        }
    }

    #endregion

    #region Frame Switch

    public void ChangeJumpAim(Vector2 aimDirection)
    {
        ChangeOrientation(aimDirection.x < 0);
    }

    private void ChangeOrientation(bool turnToLeft)
    {
        foreach (var sr in _baseSpriteRenderers.Values)
        {
            sr.flipX = turnToLeft;
        }
        foreach (var sr in _equippedArmorSpriteRenderers.Values)
        {
            sr.flipX = turnToLeft;
        }
    }

    private void SwitchToFrame(int frameIndex)
    {
        var characterFrame = _characterDefinition.Animations[_currentAnimationState][frameIndex];
        foreach (var kvp in characterFrame.Sprites)
        {
            _baseSpriteRenderers[kvp.Key].sprite = characterFrame.Sprites[kvp.Key];
        }
        foreach (var kvp in _equippedArmorSpriteRenderers)
        {
            kvp.Value.sprite = kvp.Key.Animations[_currentAnimationState][frameIndex];
        }
        _lastFrameIndex = frameIndex;
    }

    #endregion

    #region SFX

    public void PlayItemAimStartSFX()
    {
        AudioManager.Instance.PlaySFXAt(_characterDefinition.AimStartSFX, transform.position);
    }

    public void PlayItemAimCancelSFX()
    {
        AudioManager.Instance.PlaySFXAt(_characterDefinition.AimCancelSFX, transform.position);
    }

    #endregion

    #region Ground Check

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

    #endregion

    #region Utility

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
            case CharacterAnimationState.Guard:
                return _animatorDefinition.GuardActionAnimationFrameDuration;
            case CharacterAnimationState.Death:
                return _animatorDefinition.DeathAnimationFrameDuration;
            case CharacterAnimationState.BackFromLand:
                return _animatorDefinition.FlyAnimationFrameDuration;
            case CharacterAnimationState.PrepareToJump:
                return _animatorDefinition.FlyAnimationFrameDuration;
            case CharacterAnimationState.MeleeAttackHigh:
                return _animatorDefinition.MeleeAttackAnimationFrameDuration;
            case CharacterAnimationState.MeleeAttackMiddle:
                return _animatorDefinition.MeleeAttackAnimationFrameDuration;
            case CharacterAnimationState.MeleeAttackLow:
                return _animatorDefinition.MeleeAttackAnimationFrameDuration;
            default:
                throw new System.Exception($"Invalid {nameof(CharacterAnimationState)} when getting {nameof(CharacterAnimationFrame)}s");

        }
    }

    private SFXDefiniton GetSFX(CharacterAnimationState state)
    {
        switch (state)
        {
            case CharacterAnimationState.Jump:
                return _characterDefinition.InAirSFX;
            case CharacterAnimationState.Land:
                return _characterDefinition.LandSFX;
            case CharacterAnimationState.Hurt:
                return _characterDefinition.HurtSFX;
            case CharacterAnimationState.Death:
                return _characterDefinition.DeathSFX;
            case CharacterAnimationState.BackFromLand:
                return _characterDefinition.CancelJumpSFX;
            case CharacterAnimationState.PrepareToJump:
                return _characterDefinition.PrepareToJumpSFX;
            case CharacterAnimationState.Guard:
                return _characterDefinition.GuardSFX;
            default:
                return null;
        }
    } 

    #endregion
}
