using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMouseOver : MonoBehaviour
{
    [Header("Events")]
    public Action<Interactable> OnPlayerMouseOver;
    public Action OnPlayerMouseOut;

    [Header("References")]
    [SerializeField] public float pickupRange = 3f;
    [SerializeField] public LayerMask itemLayer;

    private bool isMouseOver = false;
    // private RaycastHit[] hits = new RaycastHit[1];
    private RaycastHit hit;

    [HideInInspector] public Interactable latestItem = null;

    public bool CanPoint { get; set; } = true;

    public void RaycastToItem(Camera _camera, bool ignoreLatestItem = false)
    {
        if (!CanPoint) return;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        Interactable currentItem = null;
        // checks if the raycast hit an item withhin the item layer and pickup range
        if (Physics.Raycast(ray, out hit, pickupRange, itemLayer, QueryTriggerInteraction.Collide))
        {
            // checks if the item has the Item component
            if (hit.collider.TryGetComponent(out Interactable interactable) && !Physics.Raycast(ray, Vector3.Distance(hit.transform.position, _camera.transform.position), LayerMask.GetMask("Default")))
            {
                currentItem = interactable;
                if (latestItem != currentItem || ignoreLatestItem)
                {
                    OnPlayerMouseOver?.Invoke(currentItem);
                    isMouseOver = true;
                }
            }

            if (latestItem != null)
            {
                if (currentItem != null)
                {
                    if (latestItem.GetEntityId() != currentItem.GetEntityId())
                    {
                        OnPlayerMouseOut?.Invoke();
                        OnPlayerMouseOver?.Invoke(currentItem);
                    }
                }
                else
                {
                    OnPlayerMouseOut?.Invoke();
                    isMouseOver = false;
                }
            }
        }
        else if (isMouseOver && latestItem != null)
        {
            OnPlayerMouseOut?.Invoke();
            isMouseOver = false;
        }

        latestItem = currentItem;
    }
}
