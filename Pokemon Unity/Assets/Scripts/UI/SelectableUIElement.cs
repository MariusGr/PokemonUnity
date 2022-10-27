using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectableUIElement : MonoBehaviour
{
    abstract public void AssignElement(object element);
    abstract public void AssignNone();
}
