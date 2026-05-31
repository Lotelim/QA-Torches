using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] PlayerInputManager _playerInputManager;

    [Header("UI")]
    [SerializeField] KeybindsHandler _keybindsHandler;
    [SerializeField] UpdateKeybindHintText _keybindHintText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        BindInteract();
    }

    private void BindInteract()
    {
        _playerInputManager.PlayerInteractComponent.OnPlayerInteract += () =>  // on player pickup invoke, add to player inventory
        {
            Interactable latestItem = _playerInputManager.PlayerMouseOverComponent.latestItem;
            if (latestItem is InteractableTorch IT)
            {
                if (!IT.IsLit)
                    IT.LightUp();
            }
        };

        // On player mouse over, activate the keybind hint for grab.
        _playerInputManager.PlayerMouseOverComponent.OnPlayerMouseOver += (Interactable interactable) =>
        {
            if (interactable is InteractableTorch IT)
            {
                if (!IT.IsLit)
                {
                    interactable.OnFocus();
                    ActivateKeybindHint("Interact");
                }
            }
        };

        // On player mouse out, deactivate the keybind hint.
        _playerInputManager.PlayerMouseOverComponent.OnPlayerMouseOut += () =>
        {
            _playerInputManager.PlayerMouseOverComponent.latestItem.OnUnfocus();
            _keybindHintText.SetPanelActive(false);
        };
    }
    public void ActivateKeybindHint(string value)
    {
        _keybindHintText.SetPanelActive(true);
        _keybindHintText.UpdateKeybindHint(_keybindsHandler.GetKeybindHint(value));
    }
}
