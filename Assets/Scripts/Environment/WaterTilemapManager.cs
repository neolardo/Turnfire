using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterTilemapManager : MonoBehaviour
{
    [SerializeField] private SFXDefiniton _splashSFX;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(Constants.CharacterTag) || other.CompareTag(Constants.PackageTag) || other.CompareTag(Constants.HitboxTag))
        {
            AudioManager.Instance.PlaySFXAt(_splashSFX, other.transform.position);
        }
    }
}
