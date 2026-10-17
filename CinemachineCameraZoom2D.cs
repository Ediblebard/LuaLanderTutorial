using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraZoom2D : MonoBehaviour
{
    private const float NOMRAL_ORTHOGRAPHIC_SIZE = 10f;

    public static CinemachineCameraZoom2D Instance { get; private set;}

    [SerializeField] private CinemachineCamera cinemachineCamera;
    
    private float targetOrthographicSize = 10f;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        float zoomSpeed = .8f;
        cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);
    }

    public void SetTargetOrthographicSize(float targetOrthographicSize) {
        this.targetOrthographicSize = targetOrthographicSize;
    }

    public void SetNormalOrthographicSize() {
        SetTargetOrthographicSize(NOMRAL_ORTHOGRAPHIC_SIZE);
    }
}
