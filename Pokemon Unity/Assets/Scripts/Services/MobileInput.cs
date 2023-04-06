using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;

    public MobileInput() => Instance = this;

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

    bool right = false;
    bool left = false;
    bool down = false;
    bool up = false;

    private void Update()
    {
        bool rightBefore = right;
        right = Input.GetAxis("Horizontal") > .9f;
        if (right)
        {
            if (right != rightBefore)
                Press("Right");
        } else if (!pressed["Right"])
        {
            Release("Right");
        }

        bool leftBefore = left;
        left = Input.GetAxis("Horizontal") < -.9f;
        if (left)
        {
            if (left != leftBefore)
                Press("Left");
        }
        else if (!pressed["Left"])
        {
            Release("Left");
        }

        bool downBefore = down;
        down = Input.GetAxis("Vertical") < -.9f;
        if (down)
        {
            if (down != downBefore)
                Press("Down");
        }
        else if (!pressed["Down"])
        {
            Release("Down");
        }

        bool upBefore = up;
        up = Input.GetAxis("Vertical") > .9f;
        if (up)
        {
            if (up != upBefore)
                Press("Up");
        }
        else if (!pressed["Up"])
        {
            Release("Up");
        }


        //(inputName == "Right" &&  ||
        //        (inputName == "Left" && Input.GetAxis("Horizontal") == -1f) ||
        //        (inputName == "Up" && Input.GetAxis("Vertical") == 1f) ||
        //        (inputName == "Down" && Input.GetAxis("Vertical") == -1f
    }
}
