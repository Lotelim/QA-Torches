using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] PlayerInputManager _playerInputManager;

    [Header("Player Inventory")]
    [SerializeField] PlayerInventory _playerInventory;

    [Header("Player Vitals and HUD")]
    [SerializeField] PlayerVitalsUI _playerVitalsUI;
    [SerializeField] PlayerHealth _playerHealth;
    [SerializeField] DepleteOxygen _playerOxygen;
    [SerializeField] RawImage crosshair;
    [SerializeField] Radar _radarManager;

    [Header("Respawn Handlers and Misc")]
    [SerializeField] PlayerDeathHandler _playerDeathHandler;
    [SerializeField] CanvasGroup tutorialUI;
    [SerializeField] CanvasGroup hudPanel;

    private Interactable lastItemInteractedWith = null;
    private bool isDead = false;

    private void Start()
    {
        //Application.targetFrameRate = 60;
        Cursor.visible = false;

        BindInventory();
        BindInteract();
        BindReturn();
        BindGeneral();

        PauseGame();
    }

    private void RemoveRadarAfterDelay()
    {
        if (!_playerInventory.IsItemInHotbar("item_radar"))
            _radarManager.HideRadar();
    }

    public void ItemCrafted(CraftingRecipe recipe)
    {
        if (TryCraft(recipe))
        {
            ItemData data = AllItemDatas.Instance.GetItemByName(recipe.ItemResult);
            Item item = Instantiate(data.Prefab).GetComponentInChildren<Item>();

            _playerInventory.PickUpItemRegardlessOfSlot(item);
        }
    }

    private bool TryCraft(CraftingRecipe recipe)
    {
        Item[] itemIndexes = new Item[recipe.ItemsRequired.Length];
        for (int i = 0; i < recipe.ItemsRequired.Length; i++)
        {
            Item thisItem = _playerInventory.GetItem(recipe.ItemsRequired[i]);
            while (itemIndexes.Contains(thisItem) && thisItem != null)
                thisItem = _playerInventory.GetItem(recipe.ItemsRequired[i], _playerInventory.GetItemSlot(recipe.ItemsRequired[i]) + 1);

            itemIndexes[i] = thisItem;
        }
        
        if (itemIndexes.Contains(null)) return false;

        foreach (Item item in itemIndexes)
        {
            _playerInventory.DestroyItem(item);
            Destroy(item.gameObject);
        }

        return true;
    }

    public Item GetItem(string itemName)
    {
        return _playerInventory.GetItem(itemName);
    }

    public Item GetEquippedItem()
    {
        return _playerInventory.SelectedItem.item;
    }

    public void CloseTerminal()
    {
        _playerInputManager.PlayerCameraControlComponent.cameraComponent.enabled = true;
        
        _playerInputManager.CanGetInput = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnterSafeZone()
    {
        _radarManager.HideRadar();
    }

    public void ExitSafeZone()
    {
        if (_playerInventory.IsItemInHotbar("item_radar"))
            _radarManager.ShowRadar();
    }

    private void RemoveItem(Item item)
    {
        item.RootObject.transform.SetParent(null);

        // unsets constrains for better viewing
        item.Rigidbody.freezeRotation = false;
        item.Rigidbody.constraints = RigidbodyConstraints.None;
        item.Collider.enabled = true;
    }


    private void BindInventory()
    {
        _playerInputManager.PlayerInteractComponent.OnPlayerInventory += () =>
        {
            if (!Cursor.visible || _playerInventory.IsInventoryOn())
                _playerInputManager.CanGetInput = !_playerInventory.ShowInventory();

            if (lastItemInteractedWith is Interactable_Storage IS)
            {
                IS.Storage.CloseContainer();
                crosshair.enabled = true;
                _playerInputManager.CanGetInput = true;
            }
        };

        _playerInputManager.PlayerInteractComponent.OnPlayerDrop += () =>
        {
            Item currentItem = _playerInventory.SelectedItem.item;
            if (currentItem == null) return;

            _playerInventory.DropEquippedItem();
            RemoveItem(currentItem);
            if (currentItem.Data.IdentificationName == "item_radar" && !_playerInventory.IsItemInHotbar("item_radar"))
                _radarManager.HideRadar();
            if (currentItem is UsableItem_Hammer hammer)
                hammer.StopAnimationNow();

        };

        _playerInventory.OnItemAdded += CheckForRadar;

        _playerInventory.OnItemRemoved += (item) =>
        {
            if (item != null)
            {
                if (item.Data.IdentificationName == "item_radar")
                    Invoke(nameof(RemoveRadarAfterDelay), 0.1f);
            }
        };
    }

    private void BindInteract()
    {
        _playerInputManager.PlayerInteractComponent.OnPlayerInteract += () =>  // on player pickup invoke, add to player inventory
        {
            Interactable latestItem = _playerInputManager.PlayerMouseOverComponent.latestItem;
            lastItemInteractedWith = latestItem;
            if (latestItem != null)
            {
                if (latestItem is Item item)
                {
                    if (_playerInventory.PickUpItemRegardlessOfSlot(item))
                        CheckForRadar(item);
                }
                else if (latestItem is Interactable_Workbench)
                {
                    _playerInputManager.CanGetInput = false;
                    crosshair.enabled = false;
                    latestItem.Interact();
                }
                else if (latestItem is Interactable_Terminal it)
                {
                    _playerInputManager.PlayerCameraControlComponent.cameraComponent.enabled = false;
                    it.TerminalCamera.enabled = true;
                    _playerInputManager.CanGetInput = false;
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    crosshair.enabled = false;
                    latestItem.Interact();
                }
                else if (latestItem is Interactable_Storage IS)
                {
                    IS.Interact();
                    crosshair.enabled = false;
                    _playerInputManager.CanGetInput = false;
                    _playerInventory.ShowInventory();
                }
                else
                    latestItem.Interact();
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

        _playerInputManager.PlayerInteractComponent.OnPlayerUse += () =>
        {
            Item currentItem = _playerInventory.SelectedItem.item;
            if (currentItem == null) return;
            if (currentItem is UsableItem uItem)
            {
                if (uItem.IsOneTimeUse)
                {
                    RemoveItem(currentItem);
                    _playerInventory.DestroyItem(currentItem);
                }
                uItem.Use();
            }
        };

    }

    private void BindReturn()
    {
        _playerInputManager.PlayerInteractComponent.OnPlayerReturn += () =>
        {
            // hide inventory
            if (isDead) return;

            if (_playerInventory.HideInventory())
            {
                _playerInputManager.CanGetInput = true;
                crosshair.enabled = true;
            }

            if (lastItemInteractedWith == null)
            {
                PauseGame();
            }
            else if (lastItemInteractedWith is Interactable_Storage IS)
            {
                IS.Storage.CloseContainer();
                crosshair.enabled = true;
                _playerInputManager.CanGetInput = true;
            }
            else
            {
                _playerInputManager.CanGetInput = true;
                crosshair.enabled = true;
            }

            lastItemInteractedWith = null;
        };
    }

    private void BindGeneral()
    {
        _playerHealth.OnPlayerOxygenChanged += (float oxygen) =>
        {
            _playerVitalsUI.UpdateOxygenSlider(Mathf.Max(oxygen, 0));
        };

        _playerHealth.OnPlayerHealthChanged += (float newHP) =>
        {
            _playerVitalsUI.UpdateHealthSlider(Mathf.Max(newHP, 0));
        };

        _playerDeathHandler.OnRespawnClicked += () =>
        {
            _playerOxygen.Respawn();
            _playerHealth.Respawn();
            _playerInputManager.CanGetInput = true;
            isDead = false;
        };

        _playerHealth.OnPlayerDeath += () =>
        {
            if (isDead) return;
            StartCoroutine(_playerInventory.HideItem(_playerInventory.SelectedItem.item));
            _playerInputManager.CanGetInput = false;
            _playerDeathHandler.PlayerDied();
            Item[] items = _playerInventory.GetItems()
                .Where(hi => hi.item != null)
                .Select(hi => hi.item)
                .ToArray();
            if (items != null && items.Length > 0)
            {

                StorageInventory newDroppedItemsChest = Instantiate(_playerDeathHandler.DeathDropObject, transform.position + Vector3.down*1.1f, transform.rotation).GetComponentInChildren<StorageInventory>();
                newDroppedItemsChest.InitializeInventory(items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    _playerInventory.DestroyItem(items[i]);
                    newDroppedItemsChest.InventorySlots[i].AddItem(items[i]);
                }
            }

            _playerInventory.ResetItemSelection();
            isDead = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        };

        _playerInputManager.PlayerHotbarInput.OnScroll += _playerInventory.ScrollInput;
        _playerInputManager.PlayerHotbarInput.OnNumDown += _playerInventory.HotbarPressed;
    }

    private void CheckForRadar(Item item)
    {
        if (!_playerOxygen.IsInSafeArea && item.Data.IdentificationName == "item_radar" && _playerInventory.IsItemInHotbar("item_radar"))
            _radarManager.ShowRadar();
    }

    private void PauseGame()
    {
        bool isShown = tutorialUI.alpha == 1;
        tutorialUI.alpha = isShown ? 0 : 1;
        hudPanel.alpha = isShown ? 1 : 0;
        _playerInputManager.CanGetInput = isShown;
        _playerInventory.ToggleHotbarVisibility(isShown);
    }
}
