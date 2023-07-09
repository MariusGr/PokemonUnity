using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListEntryUI : SelectableImage
{
    [SerializeField] protected Image icon;
    [SerializeField] protected ShadowedText nameText;

    public override void AssignNone()
    {
        base.AssignNone();
        gameObject.SetActive(false);
    }
}
