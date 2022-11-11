using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPokemonStatsBattleUI : PlayerPokemonStatsUI
{
    [SerializeField] private StatBar xpBar;

    override public void Refresh()
    {
        base.Refresh();
        xpBar.Value = pokemon.xpNormalized;
    }

    virtual public void RefreshXP() => hpBar.Value = pokemon.xpNormalized;

    virtual public System.Func<bool> RefreshXPAnimated(float speed)
    {
        xpBar.SetValueAnimated(pokemon.xpNormalized, speed);
        return xpBar.IsPlayingAnimation;
    }
}
