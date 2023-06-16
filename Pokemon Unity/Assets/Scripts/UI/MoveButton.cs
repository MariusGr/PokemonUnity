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

    public Move move { get; protected set; }

    public override void Refresh()
    {
        if (move is null)
            return;
        textPP.text = $"{move.Pp}/{move.Data.MaxPP}";
        base.Refresh();
    }

    override public void AssignElement(object element)
    {
        Move move = (Move)element;
        this.move = move;
        imageType.enabled = true;
        textName.text = move.Data.Name;
        imageType.sprite = move.Data.PokeType.TitleSprite;

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
