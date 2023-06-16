using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeBoxUI : NestedGallerySelection, IPokeBoxUI
{
    [SerializeField] ScalarSelection partySelection;
    [SerializeField] ScrollSelection boxSelection;
    [SerializeField] SelectableUIElement partySelectableElement;
    [SerializeField] SelectableUIElement boxSelectableElement;

    private Pokemon chosenPartyPokemon;
    private Pokemon chosenBoxPokemon;
    private string notEnoughAlivePokemonsErrorMessage = "Du musst mindestens ein unbesiegtes Pok?on im Team behalten!";

    public PokeBoxUI() => Services.Register(this as IPokeBoxUI);

    private bool BoxIsEmpty() => PlayerData.Instance.pokemonsInBox.Count < 1;

    public override void Open(Action<ISelectableUIElement, bool> callback)
    {
        Open(callback, false, 1);
    }

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        AssignElements();
        partySelection.Open(null, false, -1);
        base.Open(callback, forceSelection, BoxIsEmpty() ? 0 : 1);
        SelectOptimalView();
    }

    private void SelectOptimalView()
    {
        if (!(chosenBoxPokemon is null) || BoxIsEmpty())
            SelectElement(0);
        else
            SelectElement(1);
    }

    private void ResetChoosing()
    {
        chosenPartyPokemon = null;
        chosenBoxPokemon = null;
    }

    private void ResetSelection()
    {
        partySelection.DeselectSelection();
        boxSelection.DeselectSelection();
    }

    public override void Close()
    {
        ResetChoosing();
        ResetSelection();
        partySelection.Close();
        boxSelection.Close();
        base.Close();
    }

    protected override void SelectElement(int index)
    {
        activeSelection.DeselectSelection();
        base.SelectElement(index, false);
        if (activeSelection == partySelection)
            activeSelection.Open(ChoosePartyPokemon, activeSelection.selectedIndex, ProcessInput);
        else
            activeSelection.Open(ChooseBoxPokemon, activeSelection.selectedIndex, ProcessInput);
    }

    private void ChooseBoxPokemon(ISelectableUIElement selection, bool goBack)
    {
        if (!(selection is null))
        {
            Pokemon pokemon = ((PokemonListEntryUI)selection).pokemon;
            if (chosenPartyPokemon is null)
                chosenBoxPokemon = pokemon;
            else if (pokemon.IsFainted && PlayerData.Instance.WouldBeDeafeatetWithoutPokemon(chosenPartyPokemon))
            {
                GlobalDialogBox.Instance.DrawText(notEnoughAlivePokemonsErrorMessage, closeAfterFinish: true);
                return;
            }
            else
            {
                PlayerData.Instance.SwapPartyToBox(chosenPartyPokemon, pokemon);
                ResetSelection();
                ResetChoosing();
                RefreshViews();
                return;
            }
        }
        ChoosePokemon(chosenBoxPokemon, goBack);
    }

    private void ChoosePartyPokemon(ISelectableUIElement selection, bool goBack)
    {
        if (!(selection is null))
        {
            Pokemon pokemon = ((PlayerPokemonStatsUI)selection).pokemon;
            if (chosenBoxPokemon is null)
                chosenPartyPokemon = pokemon;
            else if (chosenBoxPokemon.IsFainted && PlayerData.Instance.WouldBeDeafeatetWithoutPokemon(pokemon))
            {
                GlobalDialogBox.Instance.DrawText(notEnoughAlivePokemonsErrorMessage, closeAfterFinish: true);
                return;
            }
            else
            {
                PlayerData.Instance.SwapPartyToBox(pokemon, chosenBoxPokemon);
                ResetSelection();
                ResetChoosing();
                RefreshViews();
                return;
            }
        }
        ChoosePokemon(chosenPartyPokemon, goBack);
    }

    private void ChoosePokemon(Pokemon pokemon, bool goBack)
    {
        if (goBack)
        {
            GoBack();
            return;
        }

        StartCoroutine(ChoosePokemonCoroutine(pokemon));
    }

    protected override void GoBack()
    {
        if (!(chosenBoxPokemon is null) || !(chosenPartyPokemon is null))
            ResetChoosing();
        else
            base.GoBack();
    }

    private void SelectOtherView() => SelectElement(selectedIndex == 0 ? 1 : 0);

    private IEnumerator ChoosePokemonCoroutine(Pokemon pokemon)
    {
        bool currentSelectionIsParty = activeSelection == partySelection;
        string secondOption = currentSelectionIsParty ? "In die Box" : "In's Team";

        while(true)
        {
            yield return GlobalDialogBox.Instance.DrawChoiceBox(
                $"Was tun mit {pokemon.Name}?", new string[] { "Tausch", secondOption, "Abbrechen"}, chancelIndex: 2);

            if (GlobalDialogBox.Instance.chosenIndex == 0)
            {
                if (BoxIsEmpty())
                {
                    yield return GlobalDialogBox.Instance.DrawText("Die Box ist noch leer!");
                    continue;
                }
                SelectOtherView();
            }
            else if (GlobalDialogBox.Instance.chosenIndex == 1)
            {
                if (currentSelectionIsParty)
                {
                    if (PlayerData.Instance.Pokemons.Count < 2)
                    {
                        yield return GlobalDialogBox.Instance.DrawText("Du musst mindestens ein Pok?on im Team behalten!");
                        continue;
                    }
                    else if (PlayerData.Instance.WouldBeDeafeatetWithoutPokemon(pokemon))
                    {
                        yield return GlobalDialogBox.Instance.DrawText(notEnoughAlivePokemonsErrorMessage);
                        continue;
                    }
                    else
                        ResetChoosing();
                    PlayerData.Instance.MovePokemonFromPartyToBox(pokemon);
                }
                else
                {
                    if (PlayerData.Instance.PartyIsFull())
                    {
                        yield return GlobalDialogBox.Instance.DrawText("Dein Team ist schon voll!");
                        continue;
                    }
                    ResetChoosing();
                    PlayerData.Instance.MovePokemonFromBoxToParty(pokemon);
                }
                RefreshViews();
            }
            else
                ResetChoosing();

            GlobalDialogBox.Instance.Close();
            SelectOptimalView();
            yield break;
        }
    }

    protected override bool TrySelectPositive()
    {
        if (chosenBoxPokemon is null && !BoxIsEmpty())
            return base.TrySelectPositive();
        return false;
    }

    protected override bool TrySelectNegative()
    {
        if (chosenPartyPokemon is null)
            return base.TrySelectNegative();
        return false;
    }

    private void RefreshViews()
    {
        partySelection.AssignElements(PlayerData.Instance.Pokemons.ToArray());
        boxSelection.AssignItems(PlayerData.Instance.pokemonsInBox.ToArray());
    }

    public override void AssignElements()
    {
        elements = new SelectableUIElement[] { partySelectableElement, boxSelectableElement };
        selections = new ScalarSelection[] { partySelection, boxSelection };
        base.AssignElements();
        RefreshViews();
    }
}
