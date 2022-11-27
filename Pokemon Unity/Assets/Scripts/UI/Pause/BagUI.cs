using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;
using System;

public class BagUI : ScalarSelection
{
    [SerializeField] ShadowedText categoryText;
    [SerializeField] ScalarSelection partySelection;
    [SerializeField] SelectableUIElement partySelectableElement;
    // elements contains SelectableGameobjects while itemSelections contains the corresponding ScrollSelections.
    [SerializeField] InspectorFriendlySerializableDictionary<ItemCategory, ScrollSelection> itemSelections;
    [SerializeField] SelectableGameObject[] itemSelectables;

    private PlayerData playerData;
    private ScalarSelection[] selections;
    private ScalarSelection activeSelection => selections[selectedIndex];
    private ItemListEntryUI choosenItemEntry;
    private int choosenItemViewIndex;

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        partySelection.Open(null, -1, ProcessInput);
        base.Open(callback, forceSelection, 1);
        itemSelections[ItemCategory.Items].Open(ChooseItem);
    }

    public override void Close()
    {
        if (!(choosenItemEntry is null))
        {
            selections[choosenItemViewIndex].DeselectSelection();
            selections[choosenItemViewIndex].Close();
        }
        else
        {
            activeSelection.DeselectSelection();
            activeSelection.Close();
        }

        if (!(choosenItemEntry is null))
        {
            choosenItemEntry.SetPlaceMode(false);
            choosenItemEntry = null;
        }

        partySelection.DeselectSelection();
        partySelection.Close();
        base.Close();
    }

    protected override void SelectElement(int index)
    {
        if (activeSelection != partySelection && choosenItemEntry is null)
            activeSelection.Close();
        base.SelectElement(index, false);
        if (activeSelection == partySelection)
            activeSelection.Open(ChoosePokemon, 0, ProcessInput);
        else
        {
            activeSelection.Open(ChooseItem, 0, ProcessInput);
            categoryText.text = ItemTexts.itemCategoryToTitle[itemSelections.keys[selectedIndex - 1]];
        }
    }

    protected override bool TrySelectPositive()
    {
        if (choosenItemEntry is null)
            return base.TrySelectPositive();
        if (selectedElement == partySelectableElement)
        {
            partySelection.DeselectSelection();
            SelectElement(choosenItemViewIndex);
        }
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
        this.playerData = playerData;
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
        {
            Close();
            return;
        }

        ItemListEntryUI entry = (ItemListEntryUI)selection;
        if (choosenItemEntry is null)
        {
            choosenItemEntry = entry;
            choosenItemViewIndex = selectedIndex;
            choosenItemEntry.SetPlaceMode(true);
        }
        else
        {
            playerData.SwapItems(choosenItemEntry.item, entry.item);
            ((ScrollSelection)activeSelection).AssignItems(playerData.items[itemSelections.keys[choosenItemViewIndex - 1]]);
            choosenItemEntry.SetPlaceMode(false);
            choosenItemEntry = null;
        }
    }

    private void ChoosePokemon(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            Close();
            return;
        }

        PlayerPokemonStatsUI statsUI = (PlayerPokemonStatsUI)selection;
        PokemonManager.Instance.TryUseItemOnPokemon(choosenItemEntry.item, statsUI.pokemon, statsUI.RefreshHPAnimated());
    }
}
