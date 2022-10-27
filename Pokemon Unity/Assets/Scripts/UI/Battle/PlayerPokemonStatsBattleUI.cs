using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPokemonStatsBattleUI : PlayerPokemonStatsUI
{
    [SerializeField] StatBar xpBar;

    override public void Refresh()
    {
        base.Refresh();
        xpBar.Value = pokemon.xpNormalized;
    }
}
