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

    protected override bool ProcessInput()
    {
        if(!base.ProcessInput())
        {
            if (Input.GetButtonDown("Right"))
            {
                SelectElement(selectedElement.GetNeighbour(Direction.Right));
                return true;
            }
            if (Input.GetButtonDown("Left"))
            {
                SelectElement(selectedElement.GetNeighbour(Direction.Left));
                return true;
            }
            if (Input.GetButtonDown("Up"))
            {
                SelectElement(selectedElement.GetNeighbour(Direction.Up));
                return true;
            }
            if (Input.GetButtonDown("Down"))
            {
                SelectElement(selectedElement.GetNeighbour(Direction.Down));
                return true;
            }
        }

        return false;
    }
}
