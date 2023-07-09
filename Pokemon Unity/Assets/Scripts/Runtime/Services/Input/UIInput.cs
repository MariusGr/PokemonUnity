using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInput : MonoBehaviour
{
    public static UIInput Instance;

    public UIInput() => Instance = this;

    private Dictionary<string, bool> heldDown = new Dictionary<string, bool>();
    private Dictionary<string, bool> pressed = new Dictionary<string, bool>();

    public bool GetButtonHeldDown(string name) => heldDown.ContainsKey(name) && heldDown[name];
    public bool GetButtonPressed(string name) => pressed.ContainsKey(name) && pressed[name];

    public void Press(string name) => StartCoroutine(PressCorotine(name));
    public void Release(string name)
    {
        pressed[name] = false;
        heldDown[name] = false;
    }

    private IEnumerator PressCorotine(string name)
    {
        pressed[name] = true;
        heldDown[name] = true;
        yield return new WaitForEndOfFrame();
        pressed[name] = false;
    }
}
