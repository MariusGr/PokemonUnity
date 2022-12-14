using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBagListEntryUI : ItemListEntryUI
{
    [SerializeField] protected Sprite backgroundDefault;
    [SerializeField] protected Sprite backgroundDefaultSelected;
    [SerializeField] protected Sprite backgroundPlace;
    [SerializeField] protected Sprite backgroundPlaceSelected;

    public Item item { get; private set; }

    private bool place = false;

    protected Sprite GetCurrentBackgroundIdle() => place ? backgroundPlace : backgroundDefault;
    protected Sprite GetCurrentBackgroundSelected() => place ? backgroundPlaceSelected : backgroundDefaultSelected;

    public override void AssignElement(object payload)
    {
        gameObject.SetActive(true);

        base.AssignElement(payload);

        item = (Item)payload;
        speratorText.gameObject.SetActive(false);
        detailsText.text = item.Count.ToString(); ;
        icon.sprite = item.data.icon;
        nameText.text = item.data.fullName;

        if (item.data.stacks)
            speratorText.gameObject.SetActive(true);
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
        item = null;
    }
}
