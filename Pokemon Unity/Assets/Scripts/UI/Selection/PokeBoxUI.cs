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

    public PokeBoxUI() => Services.Register(this as IPokeBoxUI);

    private bool BoxIsEmpty() => PlayerData.Instance.pokemonsInBox.Count < 1;

    public override void Open(Action<ISelectableUIElement, bool> callback)
    {
        Open(callback, false, 1);
    }

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        AssignElements();
        partySelection.Open(null, -1, ProcessInput);
        boxSelection.Open(null, -1, ProcessInput);
        base.Open(callback, forceSelection, BoxIsEmpty() ? 0 : 1);
    }

    private void ResetChoosing()
    {
        chosenPartyPokemon = null;
        chosenBoxPokemon = null;
    }

    public override void Close()
    {
        ResetChoosing();
        partySelection.DeselectSelection();
        boxSelection.DeselectSelection();
        partySelection.Close();
        boxSelection.Close();
        base.Close();
    }

    protected override void SelectElement(int index)
    {
        print("SelectElement  " + index);
        activeSelection.DeselectSelection();
        base.SelectElement(index, false);
        if (activeSelection == partySelection)
            activeSelection.Open(ChoosePartyPokemon, 0, ProcessInput);
        else
        {
            activeSelection.Open(ChooseBoxPokemon, 0, ProcessInput);
        }
    }

    private void ChooseBoxPokemon(ISelectableUIElement selection, bool goBack)
    {
        if (!(selection is null))
            chosenBoxPokemon = ((PokemonListEntryUI)selection).pokemon;
        ChoosePokemon(chosenBoxPokemon, goBack);
    }

    private void ChoosePartyPokemon(ISelectableUIElement selection, bool goBack)
    {
        if (!(selection is null))
            chosenPartyPokemon = ((PokemonPartyViewStatsUI)selection).pokemon;
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
                SelectOtherView();
            }
            else if (GlobalDialogBox.Instance.chosenIndex == 1)
            {
                if (currentSelectionIsParty)
                {
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
                    else if (PlayerData.Instance.pokemons.Count < 2)
                    {
                        yield return GlobalDialogBox.Instance.DrawText("Du musst mindestens ein Pokémon im Team behalten!");
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
            yield break;
        }
    }

    protected override bool TrySelectPositive()
    {
        return base.TrySelectPositive();
    }

    protected override bool TrySelectNegative()
    {
        return base.TrySelectNegative();
    }

    private void RefreshViews()
    {
        partySelection.AssignElements(PlayerData.Instance.pokemons.ToArray());
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
