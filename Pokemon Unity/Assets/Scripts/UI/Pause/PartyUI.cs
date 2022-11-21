using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUI : UIView
{
    PokemonSwitchSelection pokemonSelection;

    public override void Open()
    {
        base.Open();
        DialogBox.Instance.DrawText("Wähle ein Pokemon.", DialogBoxContinueMode.External, lines: 1);
        pokemonSelection.Open((ISelectableUIElement selection, bool goBack) => StartCoroutine(Select(selection.GetIndex(), goBack)));
    }

    private Pokemon GetPokemon(int index) => PlayerData.Instance.pokemons[index];
    
    private IEnumerator Select(int index, bool goBack)
    {
        if (goBack)
        {
            Close();
            yield break;
        }

        Pokemon pokemon = GetPokemon(index);
        yield return DialogBox.Instance.DrawChoiceBox($"Was tun mit {pokemon.Name}?", new string[] { "Bericht", "Tausch", "Abbrechen" });

        if (DialogBox.Instance.chosenIndex == 0)
        {
            // Summary

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
    }
}
