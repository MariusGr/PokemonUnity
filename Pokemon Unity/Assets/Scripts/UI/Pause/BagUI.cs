using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;
using System;

public class BagUI : ScalarSelection
{
    [SerializeField] ScalarSelection partySelection;
    [SerializeField] SelectableUIElement partySelectableElement;
    // elements contains SelectableGameobjects while itemSelections contains the corresponding ScrollSelections.
    [SerializeField] InspectorFriendlySerializableDictionary<ItemCategory, ScrollSelection> itemSelections;
    [SerializeField] SelectableGameObject[] itemSelectables;

    private ScalarSelection[] selections;
    private ScalarSelection activeSelection => selections[selectedIndex];
    private ItemListEntryUI choosenItemEntry;
    private int choosenItemViewIndex;

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        partySelection.Open(null, -1, ProcessInput);
        base.Open(callback, forceSelection, 1);
        itemSelections[ItemCategory.Items].Open();
    }

    protected override void SelectElement(int index)
    {
        if (activeSelection != partySelection)
            activeSelection.Close();
        base.SelectElement(index);
        activeSelection.Open(null, 0, ProcessInput);
    }

    protected override bool TrySelectPositive()
    {
        if (choosenItemEntry is null)
            return base.TrySelectPositive();
        if (selectedElement == partySelectableElement)
            SelectElement(choosenItemViewIndex);
        return false;
    }

    protected override bool TrySelectNegative()
    {
        if (choosenItemEntry is null && selectedIndex > 1)
            return base.TrySelectNegative();
        if (!(choosenItemEntry is null))
            SelectElement(0);
        return false;
    }

    public void AssignElements(PlayerData playerData)
    {
        List<ScalarSelection> selections = new List<ScalarSelection>() { partySelection };
        selections.AddRange(itemSelections.Values);
        this.selections = selections.ToArray();

        List<SelectableUIElement> elementsList = new List<SelectableUIElement>() { partySelectableElement };
        elementsList.AddRange(itemSelectables);
        elements = elementsList.ToArray();
        AssignElements(elements);

        partySelection.AssignElements(playerData.pokemons);
        foreach (KeyValuePair<ItemCategory, ScrollSelection> entry in itemSelections)
            entry.Value.AssignItems(playerData.items[entry.Key]);
    }

    private void ChooseItem(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
            Close();

        choosenItemEntry = (ItemListEntryUI)selection;
        choosenItemViewIndex = selectedIndex;
    }
}
