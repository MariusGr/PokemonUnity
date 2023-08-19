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

    public override void Deselect() => Deselect(true);
    public void Deselect(bool deactivatGameObject)
    {
        base.Deselect();
        gameObject.SetActive(!deactivatGameObject);
    }
}
