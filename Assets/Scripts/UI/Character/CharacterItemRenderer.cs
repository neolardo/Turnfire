using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterItemRenderer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Item _currentItem;

    [SerializeField] private CharacterAnimatorDefinition _animatorDefinition;
    [SerializeField] private Vector2 _highOffset;
    [SerializeField] private Vector2 _middleOffset;
    [SerializeField] private Vector2 _lowOffset;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeItem(Item item)
    {
        _currentItem = item;
    }

    public void ShowItem()
    {
        if( _currentItem != null )
        {
            _spriteRenderer.sprite = _currentItem.Definition.Sprite;
        }
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
        transform.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.right, aimVector), Vector3.forward);
    }

    private void ChangeOrientation(Vector2 aimVector)
    {
        bool turnToRight = aimVector.x < 0;
        if (aimVector.y > Constants.UpwardAimThresholdY)
        {
            transform.localPosition = _highOffset;
        }
        else if (aimVector.y >= Constants.DownwardAimThresholdY)
        {
            transform.localPosition = _middleOffset;
        }
        else
        {
            transform.localPosition = _lowOffset;
        }
        transform.localPosition = new Vector2(turnToRight ? -transform.localPosition.x : transform.localPosition.x, transform.localPosition.y);
        _spriteRenderer.flipY = turnToRight;
    }
}
