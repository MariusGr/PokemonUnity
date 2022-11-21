using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : SelectableImage
{
    [SerializeField] private ShadowedText textName;
    [SerializeField] private ShadowedText textPP;
    [SerializeField] private Image imageType;
    [SerializeField] protected Image imageBackground;

    public Move move { get; private set; }

    public override void Refresh()
    {
        textPP.text = $"{move.pp}/{move.data.maxPP}";
        base.Refresh();
    }

    override public void AssignElement(object element)
    {
        Move move = (Move)element;
        this.move = move;
        imageType.enabled = true;
        textName.text = move.data.fullName;
        imageType.sprite = move.data.pokeType.titleSprite;

        Refresh();
        base.AssignElement(element);
    }

    override public void AssignNone()
    {
        textName.text = "";
        textPP.text = "";
        imageType.enabled = false;

        base.AssignNone();
    }
}
