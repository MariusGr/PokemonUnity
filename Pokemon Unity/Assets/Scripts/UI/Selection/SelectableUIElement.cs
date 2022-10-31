using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;

public abstract class SelectableUIElement : MonoBehaviour
{
    [SerializeField] InspectorFriendlySerializableDictionary<Direction, SelectableUIElement> neighbours;

    [HideInInspector] public int index;
    public bool assigned { get; private set; } = false;

    public SelectableUIElement GetNeighbour(Direction direction)
        => neighbours.ContainsKey(direction) && neighbours[direction].assigned ? neighbours[direction] : null;
    public virtual void AssignNone() => assigned = false;
    public virtual void AssignElement(object element) => assigned = true;
    public abstract void Select();
    public abstract void Deselect();
}
