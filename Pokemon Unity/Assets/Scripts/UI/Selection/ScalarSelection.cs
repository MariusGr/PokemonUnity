using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalarSelection : SelectionWindow
{
    public override bool ProcessInput(InputData input)
    {
        if (!base.ProcessInput(input))
        {
            if (input.digitalPad.pressed == Direction.Right)
            {
                SelectElement((selectedIndex + 1) % elements.Length);
                return true;
            }
            if (input.digitalPad.pressed == Direction.Left)
            {
                SelectElement((selectedIndex - 1) % elements.Length);
                return true;
            }
        }
        return false;
    }
}
