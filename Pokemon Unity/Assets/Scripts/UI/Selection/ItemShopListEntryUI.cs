using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopListEntryUI : ItemListEntryUI
{
    public ItemData item;

    public override void AssignElement(object payload)
    {
        gameObject.SetActive(true);

        base.AssignElement(payload);

        item = (ItemData)payload;
        speratorText.gameObject.SetActive(false);
        detailsText.text = Money.FormatMoneyToString(item.price);
        icon.sprite = item.icon;
        nameText.text = item.fullName;
    }

    public override void AssignNone()
    {
        base.AssignNone();
        item = null;
    }
}
