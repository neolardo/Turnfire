using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterItemRenderer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Item _currentItem;

    [SerializeField] private CharacterAnimatorDefinition _animatorDefinition;

    [Header("Ranged Offsets")]
    [SerializeField] private Vector2 _rangedHighOffset = new Vector2(0.12f, -0.0065f);
    [SerializeField] private Vector2 _rangedMiddleOffset = new Vector2(0.12f, -0.055f);
    [SerializeField] private Vector2 _rangedLowOffset = new Vector2(0.13f, -0.085f);
    [Header("Melee Offsets")]
    [SerializeField] private Vector2 _meleeHighOffset = new Vector2(0.12f, -0.0065f);
    [SerializeField] private Vector2 _meleeMiddleOffset = new Vector2(0.12f, -0.055f);
    [SerializeField] private Vector2 _meleeLowOffset = new Vector2(0.13f, -0.085f);

    [SerializeField] private Vector2[] _highAttackOffsets;
    [SerializeField] private Vector2[] _middleAttackOffsets;
    [SerializeField] private Vector2[] _lowAttackOffsets;

    private float _initialRotationDegrees;
    private bool _isRangedWeapon;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeItem(Item item)
    {
        _currentItem = item;
        if (_currentItem.Definition.ItemType == ItemType.Weapon)
        {
            var weaponDef = item.Definition as WeaponDefinition;
            _initialRotationDegrees = weaponDef.InitialVisualRotationDegrees;
            _isRangedWeapon = weaponDef.IsRanged;
        }
    }

    public void ShowItem()
    {
        if( _currentItem != null )
        {
            _spriteRenderer.sprite = _currentItem.Definition.Sprite;
        }
    }

    public void StartMoveAlongAnimationThenHide(Vector2 aimVector)
    {
        aimVector = aimVector.normalized;
        Vector2[] offsetArray = null;
        if (aimVector.y > Constants.UpwardAimThresholdY)
        {
            offsetArray = _highAttackOffsets;
        }
        else if (aimVector.y >= Constants.DownwardAimThresholdY)
        {
            offsetArray = _middleAttackOffsets;
        }
        else
        {
            offsetArray = _lowAttackOffsets;
        }
        bool turnToLeft = aimVector.x < 0;
        float offsetDegrees = turnToLeft ? -_initialRotationDegrees : _initialRotationDegrees;
        transform.rotation = Quaternion.AngleAxis(aimVector.ToAngleDegrees() + offsetDegrees, Vector3.forward);
        _spriteRenderer.flipY = turnToLeft;
        StartCoroutine(MoveAlongAnimationThenHide(offsetArray));
    }

    private IEnumerator MoveAlongAnimationThenHide(Vector2[] offsets)
    {
        for (int i = 0; i < offsets.Length; i++)
        {
            transform.localPosition = new Vector2(_spriteRenderer.flipY ? -offsets[i].x : offsets[i].x, offsets[i].y);
            yield return new WaitForSeconds(_animatorDefinition.MeleeAttackAnimationFrameDuration);
        }
        HideItem();
    }

    public void HideItemAfterDelay()
    {
        StartCoroutine(WaitForItemUsageDelayThenHideCoroutine());
    }

    private IEnumerator WaitForItemUsageDelayThenHideCoroutine()
    {
        yield return new WaitForSeconds(_animatorDefinition.ItemUsageDelay);
        HideItem();
    }

    public void HideItem()
    {
        _spriteRenderer.sprite = null;
    }

    public void ChangeAim(Vector2 aimVector)
    {
        aimVector = aimVector.normalized;
        ChangeOrientation(aimVector);
        var turnToLeft = aimVector.x < 0;
        float offsetDegrees = turnToLeft ? -_initialRotationDegrees : _initialRotationDegrees;
        transform.rotation = Quaternion.AngleAxis(aimVector.ToAngleDegrees() + offsetDegrees, Vector3.forward);
    }

    private void ChangeOrientation(Vector2 aimVector)
    {
        bool turnToLeft = aimVector.x < 0;
        if (aimVector.y > Constants.UpwardAimThresholdY)
        {
            transform.localPosition = _isRangedWeapon? _rangedHighOffset : _meleeHighOffset;
        }
        else if (aimVector.y >= Constants.DownwardAimThresholdY)
        {
            transform.localPosition = _isRangedWeapon? _rangedMiddleOffset : _meleeMiddleOffset;
        }
        else
        {
            transform.localPosition = _isRangedWeapon? _rangedLowOffset : _meleeLowOffset;
        }
        transform.localPosition = new Vector2(turnToLeft ? -transform.localPosition.x : transform.localPosition.x, transform.localPosition.y);
        _spriteRenderer.flipY = turnToLeft;
    }
}
