using UnityEngine;

public class ProjectileView
{
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    private CameraController _cameraController;

    public ProjectileView(Transform transform, SpriteRenderer spriteRenderer, CameraController cameraController)
    {
        _transform = transform;
        _spriteRenderer = spriteRenderer;
        _cameraController = cameraController;
    }

    public void Initialize(ProjectileDefinition definition)
    {
        _spriteRenderer.sprite = definition.Sprite;
        _cameraController.SetProjectileTarget(_transform);
    }

    public void Show()
    {
        _transform.gameObject.SetActive(true);
    }
    public void Hide()
    {
        _transform.gameObject.SetActive(false);
    }
}
