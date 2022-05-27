using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CollectionExtensions;

public class PlayerPokemonStatsUI : PokemonStatsUI
{
    [SerializeField] ShadowedText hp;
    [SerializeField] ShadowedText maxHP;
    [SerializeField] StatBar xpBar;

    override public void Refresh()
    {
        base.Refresh();
        
        xpBar.Value = pokemon.xpNormalized;
    }

    public override void RefreshHP()
    {
        base.RefreshHP();

        hp.text = pokemon.hp.ToString();
        maxHP.text = pokemon.data.maxHp.ToString();
    }
}
