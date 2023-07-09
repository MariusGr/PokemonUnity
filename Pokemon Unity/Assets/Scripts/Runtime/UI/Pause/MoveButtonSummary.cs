using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MoveButtonSummary : MoveButton
{
    [SerializeField] private Image swapSelectionImage;

    public void SelectForSwap() => swapSelectionImage.enabled = true;
    public void DeselectForSwap() => swapSelectionImage.enabled = false;
    private void OnEnable() => DeselectForSwap();

    public override void AssignElement(object element)
    {
        base.AssignElement(element);
        gameObject.SetActive(true);
    }

    public override void AssignNone()
    {
        base.AssignNone();
        gameObject.SetActive(false);
    }
}
