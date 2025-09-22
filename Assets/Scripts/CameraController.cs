using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _characterCam;
    [SerializeField] private CinemachineCamera _projectileCam;

    public void SetProjectileTarget(Projectile p)
    {

    }

    public void SetCharacterTarget(Character c)
    {
        _characterCam.Follow = c.transform;
    }

}
