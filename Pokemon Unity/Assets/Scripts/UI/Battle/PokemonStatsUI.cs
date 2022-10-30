using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CollectionExtensions;

public class PokemonStatsUI : SelectableImage
{
    [SerializeField] protected ShadowedText nameText;
    [SerializeField] protected ShadowedText level;
    [SerializeField] protected ShadowedText gender;
    [SerializeField] protected Image status;
    [SerializeField] protected StatBar hpBar;

    protected Pokemon pokemon;

    private bool isPlayingAnimation;

    public void AssignPokemon(Pokemon pokemon)
    {
        gameObject.SetActive(true);
        this.pokemon = pokemon;
        Refresh();
    }

    public override void AssignElement(object element)
    {
        AssignPokemon((Pokemon)element);
        base.AssignElement(element);
    }

    public override void AssignNone()
    {
        gameObject.SetActive(false);
        base.AssignNone();
    }

    virtual public void Refresh()
    {
        nameText.text = pokemon.Name;
        level.text = pokemon.level.ToString();
        gender.text = pokemon.gender.symbol;

        if (pokemon.status == Status.None)
            status.enabled = false;
        else
        {
            status.sprite = Globals.Instance.statusToSpriteMap[pokemon.status];
            status.enabled = true;
        }

        RefreshHP();
    }

    virtual public void RefreshHP() => hpBar.Value = pokemon.hpNormalized;

    virtual public System.Func<bool> RefreshHPAnimated(float speed)
    {
        hpBar.SetValueAnimated(pokemon.hpNormalized, speed);
        return hpBar.IsPlayingAnimation;
    }

    public bool IsPlayingAnimation() => isPlayingAnimation;
}
