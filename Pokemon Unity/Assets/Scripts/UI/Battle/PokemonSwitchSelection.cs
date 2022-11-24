using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSwitchSelection : SelectionGraphWindow
{
    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        base.Open(callback, forceSelection, startSelection);
        DialogBox.Instance.DrawText("Wähle ein Pokemon.", DialogBoxContinueMode.External, lines: 1);
    }

    protected override void ChooseSelectedElement()
    {
        StartCoroutine(SelectActionCoroutine());
    }

    private IEnumerator SelectActionCoroutine()
    {
        Pokemon pokemon = PlayerData.Instance.pokemons[selectedIndex];
        yield return DialogBox.Instance.DrawChoiceBox($"Was tun mit {pokemon.Name}?", new string[] { "Bericht", "Tausch", "Abbrechen" });

        if (DialogBox.Instance.chosenIndex == 0)
        {
            // Summary
            SummarySelection.Instance.Open(selectedIndex);
            yield break;
        }
        if (DialogBox.Instance.chosenIndex == 1)
        {
            // Swap

            yield break;
        }
        if (DialogBox.Instance.chosenIndex == 2)
        {
            // Chancel
            Close();
            yield break;
        }

        DialogBox.Instance.Close();
        base.ChooseSelectedElement();
    }
}
