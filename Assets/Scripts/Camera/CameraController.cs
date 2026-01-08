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

    private VirtualCameraType _primaryCameraType;
    private VirtualCameraType _secondaryCameraType;
    private CinemachineBlendDefinition _primaryCameraBlend;
    private CinemachineBlendDefinition _secondaryCameraBlend;

    private readonly CinemachineBlendDefinition _quickBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 0.3f);
    private CinemachineBlendDefinition _defaultEaseInBlend;
    private CinemachineBlendDefinition _defaultEaseInOutBlend;

    public bool IsBlending => _brain.IsBlending;
    private void Awake()
    {
        _brain = Camera.main.GetComponent<CinemachineBrain>();
        _brain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        AlignCamerasToPixelPerfectSize();
        _defaultEaseInBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseIn, _brain.DefaultBlend.Time);
        _defaultEaseInOutBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, _brain.DefaultBlend.Time);
    }

    private void AlignCamerasToPixelPerfectSize()
    {
        _characterCam.Lens.OrthographicSize = Mathf.Round(_characterCam.Lens.OrthographicSize * 100f) / 100f;
        _projectileCam.Lens.OrthographicSize = Mathf.Round(_projectileCam.Lens.OrthographicSize * 100f) / 100f;
        _packageCam.Lens.OrthographicSize = Mathf.Round(_packageCam.Lens.OrthographicSize * 100f) / 100f;
    }

    private void Start()
    {
        _mapCam.Follow = _mapTransform;
        var inputHandler = FindFirstObjectByType<LocalInputHandler>();
        inputHandler.ShowMapToggled += OnShowMapToggled;
        _secondaryCameraType = VirtualCameraType.Map;
        _secondaryCameraBlend = _brain.DefaultBlend;
        PrioritizeVirtualCameras();
    }

    private void OnShowMapToggled(bool showMap)
    {
        _primaryCameraType = showMap? VirtualCameraType.Map :VirtualCameraType.None;
        _primaryCameraBlend = _quickBlend;
        _secondaryCameraBlend = _quickBlend;
        PrioritizeVirtualCameras();
    }

    private void PrioritizeVirtualCameras()
    {
        if(_primaryCameraType != VirtualCameraType.None)
        {
            _brain.DefaultBlend = _primaryCameraBlend;
            PrioritizeVirtualCamera(_primaryCameraType);
        }
        else
        {
            _brain.DefaultBlend = _secondaryCameraBlend;
            PrioritizeVirtualCamera(_secondaryCameraType);
        }
    }

    private void PrioritizeVirtualCamera(VirtualCameraType type)
    {
        switch (type)
        {
            case VirtualCameraType.Map:
                _mapCam.Prioritize();
                break;
            case VirtualCameraType.Character:
                _characterCam.Prioritize();
                break;
            case VirtualCameraType.Projectile:
                _projectileCam.Prioritize();
                break;
            case VirtualCameraType.Laser:
                _projectileCam.Prioritize();
                break;
            case VirtualCameraType.Package:
                _packageCam.Prioritize();
                break;
        }
    }

    public void SetProjectileTarget(Projectile p)
    {
        _secondaryCameraType = VirtualCameraType.Projectile;
        _secondaryCameraBlend = _defaultEaseInBlend;
        _projectileCam.Follow = p.transform;
        PrioritizeVirtualCameras();
    }
    public void SetLaserTarget(Transform l)
    {
        _secondaryCameraType = VirtualCameraType.Laser;
        _secondaryCameraBlend = _defaultEaseInBlend;
        _projectileCam.Follow = l;
        PrioritizeVirtualCameras();
    }

    public void SetCharacterTarget(Character c)
    {
        _secondaryCameraType = VirtualCameraType.Character;
        _secondaryCameraBlend = _defaultEaseInOutBlend;
        _characterCam.Follow = c.transform;
        PrioritizeVirtualCameras();
    }

    public void SetPackageTarget(IPackage p)
    {
        _secondaryCameraType = VirtualCameraType.Package;
        _secondaryCameraBlend = _defaultEaseInBlend;
        _packageCam.Follow = p.Transform;
        PrioritizeVirtualCameras();
    }



}
