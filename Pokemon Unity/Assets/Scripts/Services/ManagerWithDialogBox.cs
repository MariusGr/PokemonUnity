using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerWithDialogBox : MonoBehaviour
{
    protected IDialogBox dialogBox;
    protected virtual void Initialize() => dialogBox = Services.Get<IDialogBox>();
}
