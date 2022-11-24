using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySelection : SelectionGraphWindow
{
    private bool swapping = false;
    private PokemonPartyViewSwappableStatsUI swapButton;

    private void DrawIntroText() => DialogBox.Instance.DrawText("Wähle ein Pokemon.", DialogBoxContinueMode.External, lines: 1);

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        base.Open(callback, forceSelection, startSelection);
        DrawIntroText();
    }

    public override void Close()
    {
        DialogBox.Instance.Close();
        base.Close();
    }

    protected override void ChooseSelectedElement()
    {
        StartCoroutine(SelectActionCoroutine());
    }

    protected override void GoBack()
    {
        if (swapping)
            StopSwapping();
        else
            base.GoBack();
    }

    private void CloseSummary(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
            SummarySelection.Instance.Close();
        DrawIntroText();
    }

    private void StopSwapping()
    {
        swapButton?.Refresh(false);
        swapButton = null;
        swapping = false;
        DrawIntroText();
    }

    private IEnumerator SelectActionCoroutine()
    {
        Pokemon pokemon = PlayerData.Instance.pokemons[selectedIndex];

        if (swapping)
        {
            if (swapButton is null)
            {
                swapButton = (PokemonPartyViewSwappableStatsUI)selectedElement;
                swapButton.Refresh(true);
                DialogBox.Instance.DrawText($"Mit wem soll {pokemon.Name} getauscht werden?", DialogBoxContinueMode.External, lines: 1);
            }
            else
            {
                PlayerData.Instance.SwapPokemons(swapButton.pokemon, ((PokemonPartyViewSwappableStatsUI)selectedElement).pokemon);
                AssignElements(PlayerData.Instance.pokemons);
                StopSwapping();
            }
        }
        else
        {
            yield return DialogBox.Instance.DrawChoiceBox($"Was tun mit {pokemon.Name}?", new string[] { "Bericht", "Tausch", "Abbrechen" }, chancelIndex: 2);

            if (DialogBox.Instance.chosenIndex == 0)
            {
                // Summary
                DialogBox.Instance.Close();
                SummarySelection.Instance.Open(CloseSummary, selectedIndex);
                yield break;
            }
            if (DialogBox.Instance.chosenIndex == 1)
            {
                // Swap
                swapping = true;
                DrawIntroText();
                yield break;
            }
            if (DialogBox.Instance.chosenIndex == 2)
            {
                // Chancel
                DrawIntroText();
                yield break;
            }

            base.ChooseSelectedElement();
        }
    }
}
