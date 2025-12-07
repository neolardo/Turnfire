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


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeItem(Item item)
    {
        _currentItem = item;
        _initialRotationDegrees = (item.Definition as WeaponDefinition).InitialVisualRotationDegrees;
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
        transform.rotation = Quaternion.AngleAxis(aimVector.ToAngleDegrees() + _initialRotationDegrees, Vector3.forward);
        bool turnToLeft = aimVector.x < 0;
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
        ChangeOrientation(aimVector);
        transform.rotation = Quaternion.AngleAxis(aimVector.ToAngleDegrees() + _initialRotationDegrees, Vector3.forward);
    }

    private void ChangeOrientation(Vector2 aimVector)
    {
        bool turnToLeft = aimVector.x < 0;
        if (aimVector.y > Constants.UpwardAimThresholdY)
        {
            transform.localPosition = _rangedHighOffset;
        }
        else if (aimVector.y >= Constants.DownwardAimThresholdY)
        {
            transform.localPosition = _rangedMiddleOffset;
        }
        else
        {
            transform.localPosition = _rangedLowOffset;
        }
        transform.localPosition = new Vector2(turnToLeft ? -transform.localPosition.x : transform.localPosition.x, transform.localPosition.y);
        _spriteRenderer.flipY = turnToLeft;
    }
}
