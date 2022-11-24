using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySelection : SelectionGraphWindow
{
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

    private void CloseSummary(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
            SummarySelection.Instance.Close();
        DrawIntroText();
    }

    private IEnumerator SelectActionCoroutine()
    {
        Pokemon pokemon = PlayerData.Instance.pokemons[selectedIndex];
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
