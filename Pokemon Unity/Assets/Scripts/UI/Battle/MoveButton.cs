using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : SelectableImage
{
    [SerializeField] private ShadowedText textName;
    [SerializeField] private ShadowedText textPP;
    [SerializeField] private Image imageType;
    [SerializeField] private Image imageBackground;
    [SerializeField] private Image imageCover;

    public Move move { get; private set; }

    private void Awake() => Initialize();

    override public void AssignElement(object element)
    {
        Move move = (Move) element;
        this.move = move;
        index = move.index;
        imageCover.enabled = true;
        imageType.enabled = true;
        imageBackground.color = Color.white;
        textName.text = move.data.fullName;
        textPP.text = $"{move.pp}/{move.data.maxPP}";
        imageType.sprite = move.data.pokeType.titleSprite;
        imageCover.color = move.data.pokeType.color;

        base.AssignElement(element);
    }

    override public void AssignNone()
    {
        textName.text = "";
        textPP.text = "";
        imageType.enabled = false;
        imageCover.enabled = false;
        imageBackground.color = Color.grey;

        base.AssignNone();
    }
}
