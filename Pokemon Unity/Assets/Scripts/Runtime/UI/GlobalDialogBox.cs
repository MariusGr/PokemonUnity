using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDialogBox : DialogBox
{
    new static public GlobalDialogBox Instance;

    public GlobalDialogBox() => Instance = this;
}
