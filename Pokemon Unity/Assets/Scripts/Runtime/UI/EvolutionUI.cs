using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionUI : UIView
{
    public static EvolutionUI Instance;

    [SerializeField] private Image pokemonImage;

    public EvolutionUI() => Instance = this;
    public override void Open() => base.Open();

    public IEnumerator AnimateEvolution(Pokemon before, Pokemon after)
    {
        pokemonImage.sprite = before.data.frontSprite;
        yield return DialogBox.Instance.DrawText("Was?", DialogBoxContinueMode.User);
        yield return DialogBox.Instance.DrawText($"{before.Name} entwicklet sich!", DialogBoxContinueMode.User);
        pokemonImage.sprite = before.data.frontSprite;
        yield return DialogBox.Instance.DrawText($"{before.Name} hat sich zu {after.data.fullName} entwickelt!", DialogBoxContinueMode.User);
    }
}
