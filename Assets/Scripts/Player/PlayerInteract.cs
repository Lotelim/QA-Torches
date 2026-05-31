using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Events")]
    public Action OnPlayerInteract;

    [Header("Input Actions")]
    [SerializeField] InputActionReference interactAction;

    public bool CanInteract { get; set; } = true;

    private void Update()
    {
        if (CanInteract && interactAction.action.WasPressedThisFrame())
            OnPlayerInteract?.Invoke();

    }
}
