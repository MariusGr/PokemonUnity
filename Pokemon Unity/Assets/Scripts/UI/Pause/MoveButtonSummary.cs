using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButtonSummary : MoveButton
{
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
