using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemTexts
{
    public static readonly Dictionary<ItemCategory, string> itemCategoryToTitle = new Dictionary<ItemCategory, string>()
    {
        { ItemCategory.Food, "Futter" },
        { ItemCategory.Items, "Items" },
        { ItemCategory.KeyItems, "Voll Wichtig" },
        { ItemCategory.Medicine, "Medizin" },
        { ItemCategory.TMs, "TMs & VMs" },
    };
}
