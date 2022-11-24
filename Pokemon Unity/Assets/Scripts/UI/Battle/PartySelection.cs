using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySelection : SelectionGraphWindow
{
    private bool swapping = false;
    private bool battle = false;
    private PokemonPartyViewSwappableStatsUI swapButton;

    private void DrawIntroText() => DialogBox.Instance.DrawText("Wähle ein Pokemon.", DialogBoxContinueMode.External, lines: 1);

    public virtual void Open(bool battle, bool forceSelection) => Open(null, forceSelection, 0, battle);
    public virtual void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, bool battle) => Open(callback, forceSelection, 0, battle);
    public virtual void Open(Action<ISelectableUIElement, bool> callback, int startSelection, bool battle) => Open(callback, false, startSelection, battle);

    public virtual void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection, bool battle)
    {
        this.battle = battle;
        Open(callback, forceSelection, startSelection);
    }

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
                DialogBox.Instance.DrawText($"Mit wem soll {pokemon.Name} getauscht werden?", DialogBoxContinueMode.External);
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
            if (battle)
            {
                yield return DialogBox.Instance.DrawChoiceBox($"Was tun mit {pokemon.Name}?", new string[] { "Wechsel", "Bericht", "Abbrechen" }, chancelIndex: 2);
                
                if (DialogBox.Instance.chosenIndex == 0)
                {
                    // Switch
                    callback.Invoke(selectedElement, false);
                }
                if (DialogBox.Instance.chosenIndex == 1)
                {
                    // Summary
                    DialogBox.Instance.Close();
                    SummarySelection.Instance.Open(CloseSummary, selectedIndex);
                    yield break;
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
