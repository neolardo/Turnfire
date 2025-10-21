using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _characterCam;
    [SerializeField] private CinemachineCamera _projectileCam;
    [SerializeField] private CinemachineCamera _packageCam;

    public void SetProjectileTarget(Projectile p)
    {
        _projectileCam.Prioritize();
        _projectileCam.Follow = p.transform;
    }

    public void SetCharacterTarget(Character c)
    {
        _characterCam.Prioritize();
        _characterCam.Follow = c.transform;
    }

    public void SetPackageTarget(Package p)
    {
        _packageCam.Prioritize();
        _packageCam.Follow = p.transform;
    }

}
