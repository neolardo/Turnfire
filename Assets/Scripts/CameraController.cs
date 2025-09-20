using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _cameraZone;
    Transform _cameraTransform;
    Transform _targetObject;

    void Start()
    {
        _cameraTransform = GetComponent<Camera>().transform;
    }

}
