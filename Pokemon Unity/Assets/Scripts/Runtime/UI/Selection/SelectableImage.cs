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
        if (!(image is null))
            image.sprite = selectedSprite;
        if (!(imageCursor is null))
        {
            imageCursor.gameObject.SetActive(true);
            imageCursor.transform.localPosition = transform.localPosition;
        }
        base.Select();
    }

    public override void Deselect()
    {
        if (!(image is null))
            image.sprite = spriteBefore;
        if (!(imageCursor is null))
            imageCursor.gameObject.SetActive(false);
        base.Deselect();
    }
}
