using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPokemonStatsBattleUI : PlayerPokemonStatsUI
{
    [SerializeField] private StatBar xpBar;

    override public void Refresh() => Refresh(true);
    public void Refresh(bool refreshXP)
    {
        base.Refresh();
        if (refreshXP)
            xpBar.Value = pokemon.XpNormalized;
    }

    virtual public void RefreshXP() => xpBar.Value = pokemon.XpNormalized;
    virtual public void ResetXP() => xpBar.Value = 0;

    virtual public IEnumerator RefreshXPAnimated(float speed)
    {
        if (pokemon.XpNormalized < xpBar.Value)
            xpBar.Value = 0;

        return xpBar.SetValueAnimated(pokemon.XpNormalized, speed);
    }
}
