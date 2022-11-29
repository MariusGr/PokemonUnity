using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CollectionExtensions;

public class SelectableUIElement : MonoBehaviour, ISelectableUIElement
{
    [SerializeField] InspectorFriendlySerializableDictionary<Direction, SelectableUIElement> neighbours;

    private Action<object> onSelect;

    protected bool selected;

    public object payload { get; private set; }
    public int index { get; private set; }
    public bool assigned { get; private set; } = false;

    public bool IsAssigned() => assigned;
    public int GetIndex() => index;
    public object GetPayload() => payload;
    public virtual void Initialize(int index) => this.index = index;
    public virtual void Refresh() { }
    public SelectableUIElement GetNeighbour(Direction direction)
        => neighbours.ContainsKey(direction) && neighbours[direction].assigned ? neighbours[direction] : null;

    public virtual void AssignNone() => assigned = false;

    public virtual void AssignElement(object payload)
    {
        assigned = true;
        this.payload = payload;
    }

    public virtual void AssignOnSelectCallback(Action<object> onSelect) => this.onSelect = onSelect;

    public virtual void Select()
    {
        selected = true;
        onSelect?.Invoke(payload);
    }

    public virtual void Deselect() => selected = false;
}
