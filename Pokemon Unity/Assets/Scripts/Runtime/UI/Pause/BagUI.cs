using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;
using System;

public class BagUI : ItemSelection
{
    [SerializeField] ScalarSelection partySelection;
    [SerializeField] SelectableUIElement partySelectableElement;
    // elements contains SelectableGameobjects while itemSelections contains the corresponding ScrollSelections.
    [SerializeField] InspectorFriendlySerializableDictionary<ItemCategory, BagItemScrollSelection> itemSelections;
    [SerializeField] SelectableGameObject[] itemSelectables;


    private BagItemScrollSelection activeItemSelection
        => choosenItemViewIndex > 0 ? itemSelections.values[choosenItemViewIndex - 1] : itemSelections.values[selectedIndex - 1];

    private int choosenItemViewIndex = -1;
    private bool inBattle = false;

    public override void Open(Action<SelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        AssignElements();
        partySelection.Open(null, -1, ProcessInput);
        base.Open(callback, forceSelection, 1);
        itemSelections[ItemCategory.Items].Open(ChooseItem);
        inBattle = false;
        choosenItemViewIndex = -1;
    }

    public void OpenBattle(Action<SelectableUIElement, bool> callback)
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
            pageTitleText.text = ItemTexts.itemCategoryToTitle[itemSelections.keys[selectedIndex - 1]];
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

    public override void AssignElements()
    {
        List<ScalarSelection> selections = new List<ScalarSelection>() { partySelection };
        selections.AddRange(itemSelections.Values);
        this.selections = selections.ToArray();

        List<SelectableUIElement> elementsList = new List<SelectableUIElement>() { partySelectableElement };
        elementsList.AddRange(itemSelectables);
        elements = elementsList.ToArray();
        base.AssignElements();

        RefreshPartySelection();
        foreach (KeyValuePair<ItemCategory, BagItemScrollSelection> entry in itemSelections)
            entry.Value.AssignItems(PlayerData.Instance.items[entry.Key].ToArray());
    }

    private void RefreshItemSelection()=>
        activeItemSelection.AssignItems(PlayerData.Instance.items[itemSelections.keys[choosenItemViewIndex - 1]].ToArray());
    private void RefreshPartySelection() =>
        partySelection.AssignElements(PlayerData.Instance.pokemons.ToArray());

    private bool HandleGoBack(bool goBack)
    {
        if (goBack)
        {
            if (choosenItemViewIndex > -1)
            {
                // If we are currently in the party selection we need to tab out of it back to the item selection we came from first
                if (activeSelection == partySelection)
                    ReturnToItemSelection();
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

    private void ChooseItem(SelectableUIElement selection, bool goBack)
    {
        if (HandleGoBack(goBack) || !selection.IsAssigned())
            return;

        StartCoroutine(ChooseItemCoroutine(selection));
    }

    private IEnumerator ChooseItemCoroutine(SelectableUIElement selection)
    {
        ItemBagListEntryUI entry = (ItemBagListEntryUI)selection;
        if (inBattle && entry.item.data.usableOnBattleOpponent)
        {
            if (entry.item.data.catchesPokemon)
            {
                if (!BattleManager.Instance.OpponentIsWild())
                {
                    yield return GlobalDialogBox.Instance.DrawText(
                        "Du kannst keine Bälle in einem Trainer-Kampf verwenden!", DialogBoxContinueMode.User, closeAfterFinish: true);
                    yield break;
                }

                yield return GlobalDialogBox.Instance.DrawChoiceBox($"M?chtest du {entry.item.data.fullName} verwenden?");
                if (GlobalDialogBox.Instance.chosenIndex == 1)
                    yield break;

                callback?.Invoke(entry, false);
            }
            
        }
        else if (activeItemSelection.ChooseItemEntry(entry))
            choosenItemViewIndex = selectedIndex;
        else
        {
            RefreshItemSelection();
            choosenItemViewIndex = -1;
        }
    }

    private void ChoosePokemon(SelectableUIElement selection, bool goBack)
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

        RefreshPartySelection();
        RefreshItemSelection();
        if (inBattle && itemUsed)
            callback?.Invoke(activeItemSelection.choosenItemEntry, false);
        activeItemSelection.ResetItemSelection();
        ReturnToItemSelection();
        choosenItemViewIndex = -1;
    }
}
