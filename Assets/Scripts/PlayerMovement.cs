using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] InputActionReference moveAction;

    [Header("References")]
    [SerializeField] CharacterController playerCharacterController;
    [SerializeField] GameObject _camera;

    [Header("Variables")]
    [SerializeField, Range(1f, 20f)] float walkSpeed = 5.0f;
    [SerializeField, Range(2f, 40f)] float sprintSpeed = 10.0f;
    [SerializeField, Range(0f, 10f)] float jumpHeight = 3f;

    private Vector2 _movementVector;
    private bool _isSprinting;
    private bool _isJumping;
    private Vector3 velocity;

    public bool CanMove { get; set; } = true;

    private void Update()
    {
        _movementVector = CanMove ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        HandleMovement();
    }

    private void FixedUpdate()
    {
        HandleJump();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        float previousVertical = velocity.y;
        velocity = new Vector3(_movementVector.x, 0.0f, _movementVector.y);
        velocity = new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z).normalized * velocity.z
            + new Vector3(_camera.transform.right.x, 0, _camera.transform.right.z).normalized * velocity.x;
        velocity.y = previousVertical;

        float speed = _isSprinting ? sprintSpeed : walkSpeed;

        playerCharacterController.Move(velocity * speed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (playerCharacterController.isGrounded && _isJumping)
            velocity.y = Mathf.Sqrt(jumpHeight * -1f * Physics.gravity.y);
    }

    private void ApplyGravity()
    {
        if (!playerCharacterController.isGrounded)
            velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        playerCharacterController.Move(velocity * Time.fixedDeltaTime);
    }
}