using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableGameObject : SelectableUIElement
{
    public override void Select()
    {
        base.Select();
        gameObject.SetActive(true);
    }

    public override void Deselect()
    {
        base.Deselect();
        gameObject.SetActive(false);
    }
}
