using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;

public class BagUI : ScalarSelection
{
    [SerializeField] ScalarSelection partySelection;
    [SerializeField] SelectableUIElement partySelectableElement;
    // elements contains SelectableGameobjects while itemSelections contains the corresponding ScrollSelections.
    [SerializeField] InspectorFriendlySerializableDictionary<ItemCategory, ScrollSelection> itemSelections;

    protected override void SelectElement(int index)
    {
        ((ScrollSelection)selectedElement).Close()
        base.SelectElement(index);
        ((ScrollSelection)selectedElement).Open();
    }

    public override void AssignElements()
    {
        elements = new SelectableUIElement[] { };
        base.AssignElements(elements);
        partySelection.AssignElements(PlayerData.Instance.pokemons);
    }
}
