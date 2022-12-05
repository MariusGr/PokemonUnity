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
    [SerializeField] InspectorFriendlySerializableDictionary<ItemCategory, BagItemScrollSelection> itemSelections;
    [SerializeField] SelectableGameObject[] itemSelectables;

    private ScalarSelection[] selections;
    private ScalarSelection activeSelection => selections[selectedIndex];
    private BagItemScrollSelection activeItemSelection
        => choosenItemViewIndex > 0 ? itemSelections.values[choosenItemViewIndex - 1] : itemSelections.values[selectedIndex - 1];
    private int choosenItemViewIndex = -1;
    private bool inBattle = false;

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        partySelection.Open(null, -1, ProcessInput);
        base.Open(callback, forceSelection, 1);
        itemSelections[ItemCategory.Items].Open(ChooseItem);
        inBattle = false;
        choosenItemViewIndex = -1;
    }

    public void OpenBatlle(Action<ISelectableUIElement, bool> callback)
    {
        Open(callback);
        inBattle = true;
    }

    public override void Close()
    {
        if (activeItemSelection.itemHasBeenChoosen)
        {
            selections[choosenItemViewIndex].DeselectSelection();
            selections[choosenItemViewIndex].Close();
        }
        else
        {
            activeSelection.DeselectSelection();
            activeSelection.Close();
        }

        activeItemSelection.ResetItemSelection();
        partySelection.DeselectSelection();
        partySelection.Close();
        base.Close();
    }

    protected override void SelectElement(int index)
    {
        activeSelection.DeselectSelection();
        if (activeSelection != partySelection && !activeItemSelection.itemHasBeenChoosen)
        {
            activeItemSelection.ResetItemSelection();
            activeSelection.Close();
        }
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
        if (!activeItemSelection.itemHasBeenChoosen)
            return base.TrySelectPositive();
        if (selectedElement == partySelectableElement)
            ReturnToItemSelection();
        return false;
    }

    private void ReturnToItemSelection()
    {
        partySelection.DeselectSelection();
        SelectElement(choosenItemViewIndex);
    }

    protected override bool TrySelectNegative()
    {
        if (!activeItemSelection.itemHasBeenChoosen && selectedIndex > 1)
            return base.TrySelectNegative();
        if (activeItemSelection.itemHasBeenChoosen)
            SelectElement(0);
        return false;
    }

    public void AssignElements()
    {
        List<ScalarSelection> selections = new List<ScalarSelection>() { partySelection };
        selections.AddRange(itemSelections.Values);
        this.selections = selections.ToArray();

        List<SelectableUIElement> elementsList = new List<SelectableUIElement>() { partySelectableElement };
        elementsList.AddRange(itemSelectables);
        elements = elementsList.ToArray();
        AssignElements(elements);

        partySelection.AssignElements(PlayerData.Instance.pokemons);
        foreach (KeyValuePair<ItemCategory, BagItemScrollSelection> entry in itemSelections)
            entry.Value.AssignItems(PlayerData.Instance.items[entry.Key]);
    }

    private void RefreshItemSelection()=>
        activeItemSelection.AssignItems(PlayerData.Instance.items[itemSelections.keys[choosenItemViewIndex - 1]]);

    private bool HandleGoBack(bool goBack)
    {
        if (goBack)
        {
            if (choosenItemViewIndex > -1)
            {
                // Item is chosen and needs to be unchosen
                activeItemSelection.ResetItemSelection();
                choosenItemViewIndex = -1;
                return true;
            } else if (inBattle)
                GoBack();
            else
                Close();
            return true;
        }
        return false;
    }

    private void ChooseItem(ISelectableUIElement selection, bool goBack)
    {
        if (HandleGoBack(goBack) || !selection.IsAssigned())
            return;

        StartCoroutine(ChooseItemCoroutine(selection));
    }

    private IEnumerator ChooseItemCoroutine(ISelectableUIElement selection)
    {
        ItemListEntryUI entry = (ItemListEntryUI)selection;
        if (inBattle && entry.item.data.usableOnBattleOpponent)
        {
            yield return DialogBox.Instance.DrawChoiceBox($"M�chtest du {entry.item.data.fullName} verwenden?");
            if (DialogBox.Instance.chosenIndex == 1)
                yield break;

            callback?.Invoke(entry, false);
        }
        else if (activeItemSelection.ChooseItemEntry(entry))
            choosenItemViewIndex = selectedIndex;
        else
        {
            RefreshItemSelection();
            choosenItemViewIndex = -1;
        }
    }

    private void ChoosePokemon(ISelectableUIElement selection, bool goBack)
    {
        if (!activeItemSelection.itemHasBeenChoosen || HandleGoBack(goBack))
            return;

        StartCoroutine(UseItemOnPokemon((PlayerPokemonStatsUI)selection));
    }

    IEnumerator UseItemOnPokemon(PlayerPokemonStatsUI statsUI)
    {
        bool itemUsed = false;
        yield return PokemonManager.Instance.TryUseItemOnPokemon(
            activeItemSelection.choosenItem, statsUI.pokemon, statsUI.RefreshHPAnimated(), (bool success) => itemUsed = success);

        if (!itemUsed)
            yield break;

        RefreshItemSelection();
        if (inBattle)
            callback?.Invoke(activeItemSelection.choosenItemEntry, false);
        activeItemSelection.ResetItemSelection();
        ReturnToItemSelection();
        choosenItemViewIndex = -1;
    }
}