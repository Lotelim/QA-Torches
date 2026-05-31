using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateKeybindHintText : MonoBehaviour
{
    [SerializeField] CanvasGroup panel;
    [SerializeField] TextMeshProUGUI KeyText;
    [SerializeField] TextMeshProUGUI ValueText;

    private KeybindHint currentHint;

    public void SetPanelActive(bool isActive)
    {
        if (panel.alpha == (isActive ? 1f : 0f)) return;
        panel.alpha = isActive ? 1f : 0f;
    }

    public void UpdateKeybindHint(KeybindHint keybindHint)
    {
        if (currentHint == null || currentHint.Value != keybindHint.Value)
        {
            currentHint = keybindHint;
            KeyText.text = currentHint.Key;
            ValueText.text = currentHint.Value;
        }
    }
}
