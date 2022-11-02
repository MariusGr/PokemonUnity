using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;

public abstract class SelectableUIElement : MonoBehaviour
{
    [SerializeField] InspectorFriendlySerializableDictionary<Direction, SelectableUIElement> neighbours;

    protected bool selected;
    public int index { get; private set; }
    public bool assigned { get; private set; } = false;

    public virtual void Initialize(int index) => this.index = index;
    public virtual void Refresh() { }
    public SelectableUIElement GetNeighbour(Direction direction)
        => neighbours.ContainsKey(direction) && neighbours[direction].assigned ? neighbours[direction] : null;
    public virtual void AssignNone() => assigned = false;
    public virtual void AssignElement(object element) => assigned = true;
    public virtual void Select() => selected = true;
    public virtual void Deselect() => selected = false;
}
