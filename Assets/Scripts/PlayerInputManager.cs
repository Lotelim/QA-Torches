using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Components")]
    public PlayerCameraControl PlayerCameraControlComponent;
    public PlayerMovement PlayerMovementComponent;


    private bool _canGetInput = true;

    public bool CanGetInput {
        get => _canGetInput;
        set
        {
            _canGetInput = value;
            PlayerCameraControlComponent.CanLook = value;        
            PlayerMovementComponent.CanMove = value;
        }
    }
}
