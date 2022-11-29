using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableUIElement
{
    public bool IsAssigned();
    public int GetIndex();
    public object GetPayload();
}
