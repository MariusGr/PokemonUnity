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

    virtual public void RefreshXP() => xpBar.Value = pokemon.xpNormalized;
    virtual public void ResetXP() => xpBar.Value = 0;

    virtual public System.Func<bool> RefreshXPAnimated(float speed)
    {
        if (pokemon.xpNormalized < xpBar.Value)
            xpBar.Value = 0;

        xpBar.SetValueAnimated(pokemon.xpNormalized, speed);
        return xpBar.IsPlayingAnimation;
    }
}
