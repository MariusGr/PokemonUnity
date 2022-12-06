using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : ItemSelection, IShopUI
{
    public ShopUI() => Services.Register(this as IShopUI);


}
