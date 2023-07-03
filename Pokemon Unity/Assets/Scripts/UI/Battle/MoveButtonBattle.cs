using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButtonBattle : MoveButton
{
    [SerializeField] private Image imageCover;

    override public void AssignElement(object element)
    {
        move = (Move)element;
        imageCover.enabled = true;
        imageCover.color = move.Data.PokeType.Value.Color;
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
