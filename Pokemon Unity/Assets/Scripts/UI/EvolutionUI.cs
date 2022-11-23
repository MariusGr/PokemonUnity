using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionUI : UIView, IEvolutionUI
{
    [SerializeField] private Image pokemonImage;

    private IDialogBox dialogBox;

    public EvolutionUI() => Services.Register(this as IEvolutionUI);

    public override void Open()
    {
        base.Open();
        dialogBox = Services.Get<IDialogBox>();
    }

    public IEnumerator AnimateEvolution(Pokemon before, Pokemon after)
    {
        pokemonImage.sprite = before.data.frontSprite;
        yield return dialogBox.DrawText("Was?", DialogBoxContinueMode.User);
        yield return dialogBox.DrawText($"{before.Name} entwicklet sich!", DialogBoxContinueMode.User);
        pokemonImage.sprite = before.data.frontSprite;
        yield return dialogBox.DrawText($"{before.Name} hat sich zu {after.data.fullName} entwickelt!", DialogBoxContinueMode.User);
    }
}