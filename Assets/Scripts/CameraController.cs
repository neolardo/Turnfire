using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _characterCam;
    [SerializeField] private CinemachineCamera _projectileCam;
    [SerializeField] private CinemachineCamera _packageCam;
    [SerializeField] private CinemachineCamera _mapCam;
    [SerializeField] private Transform _mapTransform;
    [SerializeField] private CinemachineBrain _brain;
    [SerializeField] private PixelUIDefinition _UIDefintion;

    public bool IsBlending => _brain.IsBlending;
    private void Awake()
    {
        _brain = Camera.main.GetComponent<CinemachineBrain>();
        _brain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        AlignCamerasToPixelPerfectSize();
    }

    private void AlignCamerasToPixelPerfectSize()
    {
        _characterCam.Lens.OrthographicSize = Mathf.Round(_characterCam.Lens.OrthographicSize * 100f) / 100f;
        _projectileCam.Lens.OrthographicSize = Mathf.Round(_projectileCam.Lens.OrthographicSize * 100f) / 100f;
        _packageCam.Lens.OrthographicSize = Mathf.Round(_packageCam.Lens.OrthographicSize * 100f) / 100f;
    }

    private void Start()
    {
        _mapCam.Prioritize();
        _mapCam.Follow = _mapTransform;
    }
    public void SetProjectileTarget(Projectile p)
    {
        _brain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseIn;
        _projectileCam.Prioritize();
        _projectileCam.Follow = p.transform;
    }

    public void SetCharacterTarget(Character c)
    {
        _brain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
        _characterCam.Prioritize();
        _characterCam.Follow = c.transform;
    }

    public void SetPackageTarget(Package p)
    {
        _brain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseIn;
        _packageCam.Prioritize();
        _packageCam.Follow = p.transform;
    }

}
