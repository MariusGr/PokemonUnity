using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;

public abstract class SelectableUIElement : MonoBehaviour
{
    [SerializeField] InspectorFriendlySerializableDictionary<Direction, SelectableUIElement> neighbours;

    [HideInInspector] public int index;

    public SelectableUIElement GetNeighbour(Direction direction) => neighbours.ContainsKey(direction) ? neighbours[direction] : null;

    abstract public void AssignElement(object element);
    abstract public void AssignNone();
    abstract public void Select();
    abstract public void Deselect();
}
