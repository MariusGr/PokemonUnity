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
        print(pokemon);
        print(gameObject.name);
        print(pokemon.gender);
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
        level.text = pokemon.Level.ToString();
        gender.text = pokemon.gender.symbol;

        if (pokemon.StatusEffectNonVolatile is null)
            status.enabled = false;
        else
        {
            print(pokemon);
            print(pokemon.StatusEffectNonVolatile);
            print(pokemon.StatusEffectNonVolatile.Data);
            print(((StatusEffectNonVolatileData)pokemon.StatusEffectNonVolatile.Data).icon);
            status.sprite = ((StatusEffectNonVolatileData)pokemon.StatusEffectNonVolatile.Data).icon;
            status.enabled = true;
        }

        RefreshHP();
    }

    virtual public void RefreshHP() => hpBar.Value = pokemon.HpNormalized;

    public IEnumerator RefreshHPAnimated() => RefreshHPAnimated(barAnimationSpeed);
    public virtual IEnumerator RefreshHPAnimated(float speed)
    {
        yield return hpBar.SetValueAnimated(pokemon.HpNormalized, speed);
    }

    public bool IsPlayingAnimation() => isPlayingAnimation;
}
