using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SelectableImage : SelectableUIElement
{
    [SerializeField] protected Image image;
    [SerializeField] protected Sprite spriteBefore;
    [SerializeField] protected Sprite selectedSprite;
    [SerializeField] private Image imageCursor;

    protected Sprite currentSprite => selected ? selectedSprite : spriteBefore;

    public override void Select()
    {
        image.sprite = selectedSprite;
        if (!(imageCursor is null))
            imageCursor.enabled = true;
        base.Select();
    }

    public override void Deselect()
    {
        image.sprite = spriteBefore;
        if (!(imageCursor is null))
            imageCursor.enabled = false;
        base.Deselect();
    }
}
