using System.Linq;
using UnityEngine;

public class KeybindsHandler : MonoBehaviour
{
    [SerializeField] KeybindHint[] keybinds;

    public KeybindHint GetKeybindHint(string value) 
    {
        return keybinds.FirstOrDefault(x => x.name == value); // returns the first value that matches the keybind value
    }
}
