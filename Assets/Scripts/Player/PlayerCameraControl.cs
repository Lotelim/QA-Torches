using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraControl : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] InputActionReference lookAction;
    [SerializeField] InputActionReference sprintAction;
    [Header("Components")]
    [SerializeField] GameObject _camera;
    [SerializeField] public Camera cameraComponent;
    [Header("Variables")]
    [SerializeField, Range(1f, 2f)] float sprintFOVMultiplier = 1.15f;
    [SerializeField, Range(1f, 20f)] private float fovSmoothSpeed = 8f;

    private float _pitch = -25f;
    private float _yaw = 90f;
    private float _pitchMin = -80;
    private float _pitchMax = 80f;
    private float sensitivity = 30f;
    private Vector2 _cameraVector;
    private bool _isSprinting;
    private float _baseFOV = 65f;
    public bool CanLook { get; set; } = true;
    public Vector3 CameraLocalPosition { get; private set; }

    private void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", sensitivity);
        _baseFOV = PlayerPrefs.GetFloat("FOV", _baseFOV);
        Cursor.lockState = CursorLockMode.Locked;
        CameraLocalPosition = cameraComponent.transform.localPosition;   
    }

    private void Update()
    {
        if (!CanLook) return;

        _cameraVector = lookAction.action.ReadValue<Vector2>();
        _isSprinting = sprintAction.action.IsPressed();

        if (_cameraVector != Vector2.zero)
        {
            float mouseX = _cameraVector.x * sensitivity * Time.deltaTime;
            float mouseY = _cameraVector.y * sensitivity * Time.deltaTime;
            _yaw += mouseX;
            _pitch -= mouseY;
            _pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);
        }
    }

    private void FixedUpdate()
    {
        _camera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
    }

    private void LateUpdate()
    {
        float target = _isSprinting ? _baseFOV * sprintFOVMultiplier : _baseFOV;
        cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, target, fovSmoothSpeed * Time.deltaTime);
    }

    public void ReturnToOrigin()
    {
        cameraComponent.transform.localPosition = CameraLocalPosition;
    }
}