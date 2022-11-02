using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectionGraphWindow : SelectionWindow
{
    public override void Initialize()
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i] = elements[i];

        base.Initialize();
    }

    override public bool ProcessInput(InputData input)
    {
        if(!base.ProcessInput(input))
            if (input.digitalPad.pressed != Direction.None)
            {
                SelectElement(selectedElement.GetNeighbour(input.digitalPad.pressed));
                return true;
            }

        return false;
    }
}
