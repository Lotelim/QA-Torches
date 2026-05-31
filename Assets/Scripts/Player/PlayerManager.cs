using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] PlayerInputManager _playerInputManager;

    private Interactable lastItemInteractedWith = null;

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
            interactable.OnFocus();
        };

        // On player mouse out, deactivate the keybind hint.
        _playerInputManager.PlayerMouseOverComponent.OnPlayerMouseOut += () =>
        {
            _playerInputManager.PlayerMouseOverComponent.latestItem.OnUnfocus();
        };
    }
}
