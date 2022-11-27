using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListEntryUI : SelectableImage
{
    [SerializeField] private Sprite backgroundPlace;
    [SerializeField] private Sprite backgroundPlaceSelected;
    [SerializeField] private Image icon;
    [SerializeField] private ShadowedText nameText;
    [SerializeField] private ShadowedText xText;
    [SerializeField] private ShadowedText detailsText;

    public Item item { get; private set; }

    private bool place = false;

    protected Sprite GetCurrentBackgroundIdle() => place ? backgroundPlace : spriteBefore;
    protected Sprite GetCurrentBackgroundSelected() => place ? backgroundPlaceSelected : selectedSprite;

    public override void AssignElement(object payload)
    {
        gameObject.SetActive(true);

        base.AssignElement(payload);

        item = (Item)payload;
        xText.gameObject.SetActive(false);
        detailsText.text = item.count.ToString(); ;
        icon.sprite = item.data.icon;
        nameText.text = item.data.fullName;

        if (item.data.stacks)
            xText.gameObject.SetActive(true);
        else
            detailsText.text = item.data.details;
    }

    public override void Refresh()
    {
        base.Refresh();
        SetPlaceMode(place);
    }

    public void SetPlaceMode(bool place)
    {
        this.place = place;
        spriteBefore = GetCurrentBackgroundIdle();
        selectedSprite = GetCurrentBackgroundSelected();
        image.sprite = currentSprite;
    }

    public override void AssignNone()
    {
        base.AssignNone();
        gameObject.SetActive(false);
    }
}
