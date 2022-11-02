using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SelectableImage : SelectableUIElement
{
    [SerializeField] protected Image image;
    [SerializeField] protected Sprite selectedSprite;

    protected Sprite spriteBefore;
    protected Sprite currentSprite => selected ? selectedSprite : spriteBefore;

    public override void Initialize(int index)
    {
        spriteBefore = image.sprite;
        base.Initialize(index);
    }

    public override void Select()
    {
        image.sprite = selectedSprite;
        base.Select();
    }

    public override void Deselect()
    {
        image.sprite = spriteBefore;
        base.Deselect();
    }
}
