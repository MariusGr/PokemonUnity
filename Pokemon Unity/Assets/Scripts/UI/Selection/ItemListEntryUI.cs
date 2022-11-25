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

    public ItemData item { get; private set; }

    private bool place = false;

    protected Sprite GetCurrentBackgroundIdle() => place ? backgroundPlace : spriteBefore;
    protected Sprite GetCurrentBackgroundSelected() => place ? backgroundPlaceSelected : selectedSprite;

    public void AssignItem(ItemData item)
    {
        this.item = item;
        AssignElement(item);
    }

    public void AssignElement(ItemData item, int count)
    {
        xText.gameObject.SetActive(false);
        detailsText.text = count.ToString(); ;
        AssignItem(item);
    }

    public override void AssignElement(object payload)
    {
        gameObject.SetActive(true);
        base.AssignElement(payload);
        icon.sprite = item.icon;
        nameText.text = item.fullName;

        if (!item.stacks)
        {
            xText.gameObject.SetActive(true);
            detailsText.text = item.details;
        }
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
