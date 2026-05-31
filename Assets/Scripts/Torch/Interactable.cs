using System;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public Action OnInteract;
    protected Outline outline;
    public GameObject OutlinedObject;

    private void Awake()
    {
        InitializeOutline();
    }

    protected void InitializeOutline()
    {
        if (OutlinedObject == null)
            OutlinedObject = gameObject;
        outline = OutlinedObject.GetComponent<Outline>();
        if (outline == null)
            outline = OutlinedObject.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = 6;
        outline.OutlineColor = new Color(1, 1, 1);
        outline.enabled = false;
    }

    public virtual void Interact()
    {
        OnInteract?.Invoke();
    }

    public virtual void OnFocus()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public virtual void OnUnfocus()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
