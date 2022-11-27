using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CollectionExtensions;

public class PlayerPokemonStatsUI : PokemonStatsUI
{
    [SerializeField] protected ShadowedText hp;
    [SerializeField] ShadowedText maxHP;

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void RefreshHP()
    {
        base.RefreshHP();

        hp.text = pokemon.hp.ToString();
        if (!(maxHP is null))
            maxHP.text = pokemon.maxHp.ToString();
    }

    override public IEnumerator RefreshHPAnimated(float speed)
    {
        hpBar.SetValueAnimated(pokemon.hpNormalized, speed);
        yield return AnimateHPTextCoroutine();
    }

    IEnumerator AnimateHPTextCoroutine()
    {
        while(hpBar.IsPlayingAnimation())
        {
            hp.text = ((int)(pokemon.maxHp * hpBar.Value)).ToString();
            yield return new WaitForEndOfFrame();
        }

        RefreshHP();
    }
}
