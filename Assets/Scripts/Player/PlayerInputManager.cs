using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Components")]
    public PlayerCameraControl PlayerCameraControlComponent;
    public PlayerInteract PlayerInteractComponent;
    public PlayerMovement PlayerMovementComponent;
    public PlayerMouseOver PlayerMouseOverComponent;

    private bool _canGetInput = true;

    public bool CanGetInput {
        get => _canGetInput;
        set
        {
            _canGetInput = value;
            PlayerCameraControlComponent.CanLook = value;
            PlayerInteractComponent.CanInteract = value;
            PlayerMovementComponent.CanMove = value;
            PlayerMouseOverComponent.CanPoint = value;
        }
    }
    private void Update()
    {
        // start the always running raycast
        if (Time.frameCount % 5 == 0)
        {
            if (PlayerCameraControlComponent != null)
                PlayerMouseOverComponent.RaycastToItem(PlayerCameraControlComponent.cameraComponent);
        }
    }
}
