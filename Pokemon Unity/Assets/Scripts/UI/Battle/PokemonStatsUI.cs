using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CollectionExtensions;

public class PokemonStatsUI : MonoBehaviour
{
    [SerializeField] protected ShadowedText nameText;
    [SerializeField] protected ShadowedText level;
    [SerializeField] protected ShadowedText gender;
    [SerializeField] protected Image status;
    [SerializeField] protected StatBar hpBar;

    protected Pokemon pokemon;

    public void AssignPokemon(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        Refresh();
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

        hpBar.Value = pokemon.hpNormalized;
    }
}
