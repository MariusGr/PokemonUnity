using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDialogBox : DialogBox, IDialogBox
{
    static public DialogBox Instance;

    public GlobalDialogBox()
    {
        Instance = this;
        Services.Register(this as IDialogBox);
    }
}
