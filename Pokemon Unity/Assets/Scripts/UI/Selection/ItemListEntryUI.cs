using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListEntryUI : SelectableImage
{
    [SerializeField] protected Sprite backgroundDefault;
    [SerializeField] protected Sprite backgroundDefaultSelected;
    [SerializeField] protected Sprite backgroundPlace;
    [SerializeField] protected Sprite backgroundPlaceSelected;
    [SerializeField] protected Image icon;
    [SerializeField] protected ShadowedText nameText;
    [SerializeField] protected ShadowedText speratorText;
    [SerializeField] protected ShadowedText detailsText;

    public override void AssignNone()
    {
        base.AssignNone();
        gameObject.SetActive(false);
    }
}
