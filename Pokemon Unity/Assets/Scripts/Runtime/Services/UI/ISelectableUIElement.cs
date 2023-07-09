using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableUIElement
{
    public void Initialize(int index);
    public bool IsAssigned();
    public int GetIndex();
    public object GetPayload();
    public ISelectableUIElement GetNeighbour(Direction direction);
    public void Select();
    public void Deselect();
    public void Refresh();
    public void AssignElement(object payload);
    public void AssignNone();
}
