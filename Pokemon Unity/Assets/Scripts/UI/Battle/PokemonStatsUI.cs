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

    private const float barAnimationSpeed = 0.02f;

    public Pokemon pokemon { get; private set; }

    private bool isPlayingAnimation;

    private void AssignPokemon(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        gameObject.SetActive(true);
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

    override public void Refresh()
    {
        nameText.text = pokemon.Name;
        level.text = pokemon.level.ToString();
        gender.text = pokemon.gender.symbol;

        if (pokemon.statusEffectNonVolatile is null)
            status.enabled = false;
        else
        {
            status.sprite = pokemon.statusEffectNonVolatile.icon;
            status.enabled = true;
        }

        RefreshHP();
    }

    virtual public void RefreshHP() => hpBar.Value = pokemon.hpNormalized;

    public IEnumerator RefreshHPAnimated() => RefreshHPAnimated(barAnimationSpeed);
    public virtual IEnumerator RefreshHPAnimated(float speed)
    {
        yield return hpBar.SetValueAnimated(pokemon.hpNormalized, speed);
    }

    public bool IsPlayingAnimation() => isPlayingAnimation;
}
