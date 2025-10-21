using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ParallaxScrollingUIImage : MonoBehaviour
{
    [SerializeField] private PixelUIDefinition _uiDefinition;
    [SerializeField] private float _pixelScrollSpeed = 1f;
    private Image _image;
    private Material _runtimeMaterial;
    private Vector2 _currentOffset = Vector2.zero;
    private const float OverflowThreshold = 1f;


    void Awake()
    {
        _image = GetComponent<Image>();

        _runtimeMaterial = new Material(_image.material);
        _image.material = _runtimeMaterial;
    }

    void Update()
    {
        MoveTexture();
    }

    private void MoveTexture()
    {
        float pixelDelta = _pixelScrollSpeed * Time.deltaTime;
        _currentOffset.y += pixelDelta / _uiDefinition.PixelsPerUnit;

        if (_currentOffset.y >= OverflowThreshold)
        {
            _currentOffset.y -= OverflowThreshold;
        }

        _runtimeMaterial.mainTextureOffset = _currentOffset;
    }
  
}
