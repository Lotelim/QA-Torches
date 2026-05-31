using UnityEngine;

public class InteractableTorch : Interactable
{
    [SerializeField] Material litMaterial;
    [SerializeField] Renderer torchHeadRenderer;
    private bool isLit = false;

    public bool IsLit { get { return isLit; } }

    public void LightUp()
    {
        isLit = true;
        torchHeadRenderer.material = litMaterial;
    }
}
