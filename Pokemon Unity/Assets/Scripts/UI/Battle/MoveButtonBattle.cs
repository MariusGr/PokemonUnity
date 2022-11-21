using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButtonBattle : MoveButton
{
    [SerializeField] private Image imageCover;

    override public void AssignElement(object element)
    {
        imageCover.enabled = true;
        imageCover.color = move.data.pokeType.color;
        imageBackground.color = Color.white;
        base.AssignElement(element);
    }

    override public void AssignNone()
    {
        imageBackground.color = Color.grey;
        imageCover.enabled = false;
        base.AssignNone();
    }
}
