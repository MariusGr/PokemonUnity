using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SelectableImage : SelectableUIElement
{
    [SerializeField] Image image;
    [SerializeField] Sprite selectedSprite;

    private Sprite spriteBefore;

    public override void AssignElement(object element) { }
    public override void AssignNone() { }

    protected void Initialize() => spriteBefore = image.sprite;

    public override void Select()
    {
        image.sprite = selectedSprite;
    }

    public override void Deselect()
    {
        image.sprite = spriteBefore;
    }
}
