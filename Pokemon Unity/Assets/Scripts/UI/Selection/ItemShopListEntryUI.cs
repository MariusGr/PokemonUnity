using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopListEntryUI : ItemListEntryUI
{
    [HideInInspector] public ItemData item;

    public override void AssignElement(object payload)
    {
        gameObject.SetActive(true);

        base.AssignElement(payload);

        item = (ItemData)payload;
        speratorText.gameObject.SetActive(false);
        detailsText.text = Money.FormatMoneyToString(item.Price);
        icon.sprite = item.Icon;
        nameText.text = item.Name;
    }

    public override void AssignNone()
    {
        base.AssignNone();
        item = null;
    }
}
